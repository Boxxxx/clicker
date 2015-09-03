using System;
using System.Reflection;
using FullSerializer;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// A standard tkControlPropertyEditor except with some more appropriate values popualted.
    /// </summary>
    public abstract class tkControlPropertyEditor<TEdited> : tkControlPropertyEditor {
        public override bool CanEdit(Type dataType) {
            return typeof(TEdited).IsAssignableFrom(dataType);
        }

        protected override object CreateInstance() {
            return InspectedType.Get(typeof(TEdited)).CreateInstance();
        }

        protected sealed override tkControlEditor GetControlEditor(GUIContent label, object element, fiGraphMetadata graphMetadata) {
            return GetControlEditor(label, (TEdited)element, graphMetadata);
        }

        protected abstract tkControlEditor GetControlEditor(GUIContent label, TEdited element, fiGraphMetadata graphMetadata);
    }

    /// <summary>
    /// Derive from this class if you wish to write a custom property editor that is rendered
    /// from a tkControl.
    /// </summary>
    /// <remarks>You probably want to derive from tkControlPropertyEditor{TEdited}</remarks>
    public class tkControlPropertyEditor : IPropertyEditor, IPropertyEditorEditAPI {
        public class fiLayoutPropertyEditorMetadata : IGraphMetadataItemNotPersistent {
            [fsIgnore]
            public tkControlEditor Layout;
        }

        protected virtual tkControlEditor GetControlEditor(GUIContent label, object element, fiGraphMetadata graphMetadata) {
            fiLayoutPropertyEditorMetadata metadata;
            if (graphMetadata.TryGetMetadata(out metadata) == false) {
                metadata = graphMetadata.GetMetadata<fiLayoutPropertyEditorMetadata>();
                metadata.Layout = ((tkCustomEditor)element).GetEditor();
            }

            return metadata.Layout;
        }

        public PropertyEditorChain EditorChain {
            get;
            set;
        }

        public virtual bool CanEdit(Type dataType) {
            // TODO: this doesn't need to be overridable; do what the default control does
            throw new NotSupportedException();
        }

        protected virtual object CreateInstance() {
            return null;
        }

        public object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            if (element == null) element = CreateInstance();
            return fiEditorGUI.tkControl(region, element, metadata, GetControlEditor(label, element, metadata));
        }

        public float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            if (element == null) element = CreateInstance();
            return fiEditorGUI.tkControlHeight(element, metadata, GetControlEditor(label, element, metadata));
        }

        public GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return label;
        }

        public object OnSceneGUI(object element) {
            return element;
        }

        public bool DisplaysStandardLabel {
            get { return false; }
        }

        public static IPropertyEditor TryCreate(Type dataType, ICustomAttributeProvider attributes) {
            if (typeof(UnityObject).IsAssignableFrom(dataType) ||
                typeof(tkCustomEditor).IsAssignableFrom(dataType) == false) {

                return null;
            }

            return new tkControlPropertyEditor();
        }
    }
}
