using System;
using System.Collections.Generic;
using System.Reflection;
using FullInspector.Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// This is the base collection property editor with a set of extension points for the other
    /// property editors. All Rotorz-style collection editors use this as the base editor. It
    /// provides automatic support for paging.
    /// </summary>
    /// <typeparam name="TActual">The actual type of the collection, ie, List{T}. This is used for instance creation.</typeparam>
    /// <typeparam name="TCollection">The collection interface, ie, IList{T}, or List{T}. The property editor 
    /// selection logic will choose the most associated editor using this type.</typeparam>
    /// <typeparam name="TItem">The type of items stored inside of the collection, ie, {T} in List{T}.</typeparam>
    /// <typeparam name="TAddItem">The type of item that is added to a collection, ie,
    /// TAddItem = {K} in TActual = Dictionary{K, V} where TItem = KeyValuePair{K, V}</typeparam>
    public abstract class BaseCollectionPropertyEditor<TActual, TCollection, TItem, TAddItem> : PropertyEditor<TCollection>
        where TCollection : ICollection<TItem> {

        /// <summary>
        /// Fetch an adaptor for the collection.
        /// </summary>
        protected abstract IReorderableListAdaptor GetAdaptor(TCollection collection, fiGraphMetadata metadata);

        /// <summary>
        /// Called after an edit cycle is done if the collection needs to be updated from the adaptor.
        /// </summary>
        protected virtual void OnPostEdit(ref TCollection collection, IReorderableListAdaptor adaptor) {
        }

        /// <summary>
        /// An item has been added to the collection.
        /// </summary>
        protected virtual void AddItemToCollection(TAddItem item, ref TCollection collection) {
            if (typeof(TItem).IsAssignableFrom(typeof(TAddItem)) == false) {
                Debug.LogError("Please override AddItemToCollection; " + typeof(TAddItem).CSharpName() +
                               " cannot be converted to " + typeof(TItem).CSharpName());
                return;
            }

            collection.Add((TItem)(object)item);
        }

        /// <summary>
        /// Should the item added to the collection be customized *before* adding it?
        /// </summary>
        protected virtual bool DisplayAddItemPreview {
            get { return true; }
        }

        /// <summary>
        /// Can we reorder elements inside of the collection?
        /// </summary>
        protected virtual bool AllowReordering {
            get { return true; }
        }

        // TODO: make this persistent
        public class PageMetadata : IGraphMetadataItemNotPersistent {
            public int PageStartIndex;
            public int PageEndIndex;
        }

        private static readonly PropertyEditorChain _itemEditor = PropertyEditor.Get(typeof(TItem), null);
        private static readonly PropertyEditorChain _addItemEditor = PropertyEditor.Get(typeof(TAddItem), null);

        private readonly bool _overrideDropdownDisable;
        private readonly ReorderableListFlags _listFlags;

        private TAddItem _addItem;

        private readonly int _pageMinimumCollectionLength;

        private const float AddRegionBorder = 3f;
        private const float AddRegionMargin = (AddRegionBorder * 2) + 1;

        private readonly float PagedRegionButtonHeight = EditorGUIUtility.singleLineHeight * 1.3f;
        private readonly float PagedRegionVerticalMargin = fiLateBindings.EditorGUIUtility.standardVerticalSpacing * 1.3f;

        public BaseCollectionPropertyEditor(Type editedType, ICustomAttributeProvider attributes) {
            _overrideDropdownDisable =
                attributes == null ||
                attributes.IsDefined(typeof(InspectorCollectionShowItemDropdownAttribute), true) == false;

            if (DisplayAddItemPreview) {
                _listFlags |= ReorderableListFlags.HideAddButton;
            }

            if (!AllowReordering) {
                _listFlags |= ReorderableListFlags.DisableReordering;
            }

            if (attributes != null && attributes.IsDefined(typeof(InspectorCollectionRotorzFlagsAttribute), /*inherit:*/true)) {
                var attr = (InspectorCollectionRotorzFlagsAttribute)(attributes.GetCustomAttributes(typeof(InspectorCollectionRotorzFlagsAttribute), /*inherit:*/true)[0]);
                _listFlags |= attr.Flags;
            }

            _pageMinimumCollectionLength = fiSettings.DefaultPageMinimumCollectionLength;
            if (attributes != null && attributes.IsDefined(typeof(InspectorCollectionPagerAttribute), /*inherit:*/true)) {
                var attr = (InspectorCollectionPagerAttribute)(attributes.GetCustomAttributes(typeof(InspectorCollectionPagerAttribute), /*inherit:*/true)[0]);
                _pageMinimumCollectionLength = attr.PageMinimumCollectionLength;
            }
        }

        public override bool CanEdit(Type type) {
            // We *have* to be able to create an instance of the type
            return base.CanEdit(type) && type.IsAbstract == false && type.IsInterface == false;
        }

        public override bool DisplaysStandardLabel {
            get { return true; }
        }

        private static void EnsureNotNull(ref TCollection elements) {
            if (elements == null) {
                elements = (TCollection)InspectedType.Get(typeof(TActual)).CreateInstance();
                GUI.changed = true;
            }
        }

        public void DoEdit(Rect region, GUIContent label, ref TCollection collection, fiGraphMetadata metadata,
            IReorderableListAdaptor adaptor) {

            Rect bodyRect = new Rect(region);
            bodyRect.height -= GetAddRegionHeightWithMargin(metadata);

            Rect addItemButtonRect, addItemItemRect;
            {
                Rect baseAddItemRect = new Rect(region);
                baseAddItemRect.y += bodyRect.height + AddRegionMargin;
                baseAddItemRect.height = GetAddRegionHeight(metadata);

                fiRectUtility.SplitLeftHorizontalExact(baseAddItemRect,
                    /*leftWidth:*/ ReorderableListGUI.defaultAddButtonStyle.fixedWidth,
                    /*margin:*/ 1, out addItemButtonRect, out addItemItemRect);

                // move the margin up
                addItemItemRect.x += AddRegionBorder;
                addItemItemRect.width -= AddRegionBorder * 2;
                addItemButtonRect.y -= AddRegionBorder * 2;
                addItemItemRect.y -= AddRegionBorder;

                if (DisplayAddItemPreview && Event.current.type == EventType.repaint) {
                    Rect container = addItemItemRect;
                    container.x -= AddRegionBorder;
                    container.y -= AddRegionBorder;
                    container.width += AddRegionBorder * 2;
                    container.height += AddRegionBorder * 2;
                    ReorderableListGUI.defaultContainerStyle.Draw(container, false, false, false, false);
                }

                addItemButtonRect.height = ReorderableListGUI.defaultAddButtonStyle.fixedHeight;
            }

            // draw the collection elements
            ReorderableListGUI.ListFieldAbsolute(bodyRect, adaptor, DrawEmpty, _listFlags);

            // draw the next key
            metadata.Enter("NextValue").Metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();

            if (DisplayAddItemPreview) {
                var addButtonStyle = ReorderableListGUI.defaultAddButtonStyleFlipped;
                if (adaptor.Count == 0) addButtonStyle = ReorderableListGUI.defaultAddButtonStyleIndependent;

                if (GUI.Button(addItemButtonRect, "", addButtonStyle)) {
                    AddItemToCollection(_addItem, ref collection);
                    GUI.FocusControl(null);
                    _addItem = default(TAddItem);
                }

                EnsureValidAddItem(ref _addItem);
                fiEditorGUI.PushHierarchyMode(false);
                _addItem = _addItemEditor.FirstEditor.Edit(addItemItemRect, GUIContent.none, _addItem,
                    metadata.Enter("NextValue"));
                fiEditorGUI.PopHierarchyMode();
            }
        }

        private bool ShouldDisplayTitleRegion(GUIContent label) {
            // If we are not displaying an add item preview, then we have an add button
            // above the list which means we always have a title region. Otherwise, we
            // have a title region if we have a title label.
            return
                !DisplayAddItemPreview ||
                !string.IsNullOrEmpty(label.text);
        }

        protected float DoGetElementHeight(GUIContent label, TCollection collection, fiGraphMetadata metadata,
            IReorderableListAdaptor adaptor) {

            // height of the title
            float titleHeight = 0;
            if (ShouldDisplayTitleRegion(label)) {
                titleHeight = EditorGUIUtility.singleLineHeight;
            }

            // height of the actual Rotorz list editor
            float listHeight = 0;
            if (collection.Count > 0) {
                listHeight = ReorderableListGUI.CalculateListFieldHeight(adaptor,
                    ReorderableListFlags.HideAddButton);
            }

            if (DisplayAddItemPreview) {
                listHeight += AddRegionMargin;
            }

            // title + editor + new item editor
            return titleHeight + listHeight + GetAddRegionHeight(metadata);
        }

        private bool ShouldDisplayPageControls(TCollection collection) {
            if (_pageMinimumCollectionLength < 0) return false;
            if (_pageMinimumCollectionLength == 0) return true;

            return collection.Count > _pageMinimumCollectionLength;
        }

        private PageMetadata GetPageMetadata(fiGraphMetadata graphMetadata) {
            PageMetadata metadata;
            if (graphMetadata.TryGetMetadata(out metadata) == false) {
                metadata = graphMetadata.GetMetadata<PageMetadata>();
                metadata.PageEndIndex = _pageMinimumCollectionLength;
            }

            return metadata;
        }

        public override TCollection Edit(Rect region, GUIContent label, TCollection collection, fiGraphMetadata metadata) {
            EnsureNotNull(ref collection);

            var pageMetadata = GetPageMetadata(metadata);

            var unpagedAdaptor = GetAdaptor(collection, metadata);
            var adaptor = unpagedAdaptor;

            // draw the title
            if (ShouldDisplayTitleRegion(label)) {
                Rect titleRect = new Rect(region);
                titleRect.height = EditorGUIUtility.singleLineHeight;

                region.y += titleRect.height;
                region.height -= titleRect.height;

                GUI.Label(titleRect, label);
            }

            if (ShouldDisplayPageControls(collection)) {
                VerifyPageIndices(pageMetadata, collection);

                Rect pagedRect = region;

                pagedRect.y += PagedRegionVerticalMargin;
                pagedRect.height = PagedRegionButtonHeight;

                region.y += PagedRegionButtonHeight + PagedRegionVerticalMargin * 2;
                region.height -= PagedRegionButtonHeight + PagedRegionVerticalMargin * 2;

                const float margin_Empty_StartLabel = 10;
                const float width_StartLabel = 100;
                const float margin_StartLabel_StartIndex = 2;
                const float width_StartIndex = 45;
                const float margin_StartIndex_ThruLabel = 2;
                const float width_ThruLabel = 20;
                const float margin_ThruLabel_EndIndex = 2;
                const float width_EndIndex = 45;
                const float margin_EndIndex_OfCount = 2;
                const float width_OfCountLabel = 500;

                // -----

                const float width_DecButton = 40;
                const float margin_DecButton_IncButton = 1;
                const float width_IncButton = 40;
                const float margin_IncButton_Empty = 50;

                const float regionIndexSelectorWidth =
                    margin_Empty_StartLabel +
                    width_StartLabel + margin_StartLabel_StartIndex +
                    width_StartIndex + margin_StartIndex_ThruLabel +
                    width_ThruLabel + margin_ThruLabel_EndIndex +
                    width_EndIndex + margin_EndIndex_OfCount +
                    width_OfCountLabel;

                const float regionIncDecWidth =
                    width_DecButton + margin_DecButton_IncButton + width_IncButton + margin_IncButton_Empty;


                Rect rect_RegionIndexSelector, rect_RegionIncDec, rectDummy;
                fiRectUtility.SplitHorizontalFlexibleMiddle(pagedRect, regionIndexSelectorWidth, regionIncDecWidth,
                    out rect_RegionIndexSelector, out rectDummy, out rect_RegionIncDec);


                fiRectUtility.CenterRect(rect_RegionIndexSelector, EditorGUIUtility.singleLineHeight,
                    out rect_RegionIndexSelector);

                Rect rect_StartLabel, rect_StartIndex, rect_ThruLabel, rect_EndIndex, rect_OfCountLabel;

                {
                    float startX = rect_RegionIndexSelector.x + margin_Empty_StartLabel;

                    rect_StartLabel = rect_RegionIndexSelector;
                    rect_StartLabel.x = startX;
                    rect_StartLabel.width = width_StartLabel;
                    startX += width_StartLabel + margin_StartLabel_StartIndex;

                    rect_StartIndex = rect_RegionIndexSelector;
                    rect_StartIndex.x = startX;
                    rect_StartIndex.width = width_StartIndex;
                    startX += width_StartIndex + margin_StartIndex_ThruLabel;

                    rect_ThruLabel = rect_RegionIndexSelector;
                    rect_ThruLabel.x = startX;
                    rect_ThruLabel.width = width_ThruLabel;
                    startX += width_ThruLabel + margin_ThruLabel_EndIndex;

                    rect_EndIndex = rect_RegionIndexSelector;
                    rect_EndIndex.x = startX;
                    rect_EndIndex.width = width_EndIndex;
                    startX += width_EndIndex + margin_EndIndex_OfCount;

                    rect_OfCountLabel = rect_RegionIndexSelector;
                    rect_OfCountLabel.x = startX;
                    rect_OfCountLabel.width = width_OfCountLabel;
                }


                Rect rect_IncButton, rect_DecButton;
                {
                    float startX = rect_RegionIncDec.x;

                    rect_DecButton = rect_RegionIncDec;
                    rect_DecButton.x = startX;
                    rect_DecButton.width = width_DecButton;
                    startX += width_IncButton + margin_DecButton_IncButton;

                    rect_IncButton = rect_RegionIncDec;
                    rect_IncButton.x = startX;
                    rect_IncButton.width = width_IncButton;
                }

                if (unpagedAdaptor.Count > 0) {
                    GUI.Label(rect_StartLabel, "Showing indices ");

                    EditorGUI.BeginChangeCheck();
                    int newStartIndex = EditorGUI.IntField(rect_StartIndex, pageMetadata.PageStartIndex);
                    if (EditorGUI.EndChangeCheck()) {
                        EnsureWithinLimits(ref newStartIndex, 0, collection.Count);
                        if (newStartIndex > pageMetadata.PageEndIndex) {
                            ShiftPageForward(pageMetadata, newStartIndex - pageMetadata.PageStartIndex, collection.Count);
                        }
                        else {
                            pageMetadata.PageStartIndex = newStartIndex;
                        }
                    }

                    GUI.Label(rect_ThruLabel, " - ");

                    EditorGUI.BeginChangeCheck();
                    int newEndIndex = EditorGUI.IntField(rect_EndIndex, pageMetadata.PageEndIndex);
                    if (EditorGUI.EndChangeCheck()) {

                        // we could do the auto shifting going backwards, but it is
                        // easy to accidentaly trigger when typing, say, 150, as Unity
                        // will give us an assigned value of 1 which causes a shift
                        // to 1 length making it annoying, so we disable it

                        /*
                        EnsureWithinLimits(ref newEndIndex, 0, collection.Count);
                        if (newEndIndex <= _pageStartIndex) {
                            ShiftPageBackward(_pageEndIndex - newEndIndex);
                        }
                        else {
                            _pageEndIndex = newEndIndex;
                        }*/


                        if (newEndIndex < pageMetadata.PageStartIndex) newEndIndex = pageMetadata.PageStartIndex;
                        EnsureWithinLimits(ref newEndIndex, 0, collection.Count);
                        pageMetadata.PageEndIndex = newEndIndex;
                    }
                    GUI.Label(rect_OfCountLabel, " of " + (collection.Count - 1));
                }
                else {
                    GUI.Label(rect_StartLabel, "Empty collection");
                }





                EditorGUI.BeginDisabledGroup(!(pageMetadata.PageStartIndex > 0));
                if (GUI.Button(rect_DecButton, "<<")) {
                    ShiftPageBackward(pageMetadata, pageMetadata.PageEndIndex - pageMetadata.PageStartIndex + 1);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!(pageMetadata.PageEndIndex < collection.Count - 1));
                if (GUI.Button(rect_IncButton, ">>")) {
                    ShiftPageForward(pageMetadata, pageMetadata.PageEndIndex - pageMetadata.PageStartIndex + 1, collection.Count);
                }
                EditorGUI.EndDisabledGroup();

                adaptor = new PageAdaptor(unpagedAdaptor, pageMetadata.PageStartIndex, pageMetadata.PageEndIndex);
            }

            fiEditorGUI.PushHierarchyMode(false);
            DoEdit(region, label, ref collection, metadata, adaptor);
            fiEditorGUI.PopHierarchyMode();

            OnPostEdit(ref collection, unpagedAdaptor);

            return collection;
        }

        private void ShiftPageBackward(PageMetadata pageMetadata, int delta) {
            if (delta > pageMetadata.PageStartIndex) delta = pageMetadata.PageStartIndex;

            pageMetadata.PageStartIndex -= delta;
            pageMetadata.PageEndIndex -= delta;
        }

        private void ShiftPageForward(PageMetadata pageMetadata, int delta, int collectionCount) {
            if (delta > (collectionCount - pageMetadata.PageEndIndex - 1))
                delta = collectionCount - pageMetadata.PageEndIndex - 1;

            pageMetadata.PageStartIndex += delta;
            pageMetadata.PageEndIndex += delta;
        }

        private static void EnsureWithinLimits(ref int value, int min, int max) {
            if (value < min) value = min;
            if (value >= max) value = max - 1;
        }

        public override float GetElementHeight(GUIContent label, TCollection collection, fiGraphMetadata metadata) {
            EnsureNotNull(ref collection);

            var pageMetadata = GetPageMetadata(metadata);
            float height = 0;

            var adaptor = GetAdaptor(collection, metadata);
            if (ShouldDisplayPageControls(collection)) {
                VerifyPageIndices(pageMetadata, collection);

                adaptor = new PageAdaptor(adaptor, pageMetadata.PageStartIndex, pageMetadata.PageEndIndex);
                height += PagedRegionButtonHeight + PagedRegionVerticalMargin * 2;
            }

            height += DoGetElementHeight(label, collection, metadata, adaptor);

            return height;
        }

        private void VerifyPageIndices(PageMetadata pageMetadata, TCollection collection) {
            if (pageMetadata.PageStartIndex > pageMetadata.PageEndIndex) {
                if (pageMetadata.PageEndIndex > 0) pageMetadata.PageStartIndex = pageMetadata.PageEndIndex - 1;
                else pageMetadata.PageEndIndex = pageMetadata.PageStartIndex + 1;
            }
            EnsureWithinLimits(ref pageMetadata.PageStartIndex, 0, collection.Count);
            EnsureWithinLimits(ref pageMetadata.PageEndIndex, 0, collection.Count);
        }

        private float GetAddRegionHeight(fiGraphMetadata metadata) {
            if (!DisplayAddItemPreview) return 0;

            EnsureValidAddItem(ref _addItem);

            int buttonHeight = (int)ReorderableListGUI.defaultAddButtonStyle.fixedHeight;
            float itemHeight = _addItemEditor.FirstEditor.GetElementHeight(GUIContent.none, _addItem, metadata.Enter("NextValue"));

            return Math.Max(buttonHeight, itemHeight);
        }

        private float GetAddRegionHeightWithMargin(fiGraphMetadata metadata) {
            if (!DisplayAddItemPreview) return 0;

            return AddRegionMargin + GetAddRegionHeight(metadata);
        }

        private void EnsureValidAddItem(ref TAddItem value) {
            if (typeof(TAddItem) == typeof(string) && value == null) {
                value = (TAddItem)(object)"";
            }
        }

        public override GUIContent GetFoldoutHeader(GUIContent label, object element) {
            if (element == null) {
                return label;
            }

            int count = ((TCollection)element).Count;
            String text = label.text + " (";
            if (count == 0) text += "empty";
            else if (count == 1) text += "1 element";
            else text += count + " elements";
            text += ")";

            return new GUIContent(text, label.tooltip);
        }

        private static void DrawEmpty(Rect rect) {
        }

        protected TItem DrawItem(Rect rect, TItem element, fiGraphMetadataChild metadata) {
            if (_overrideDropdownDisable) {
                metadata.Metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();
            }

            return _itemEditor.FirstEditor.Edit(rect, GUIContent.none, element, metadata);
        }

        protected float GetItemHeight(TItem element, fiGraphMetadataChild metadata) {
            float height = _itemEditor.FirstEditor.GetElementHeight(GUIContent.none, element, metadata);
            if (height < EditorGUIUtility.singleLineHeight) {
                return EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
    }

}