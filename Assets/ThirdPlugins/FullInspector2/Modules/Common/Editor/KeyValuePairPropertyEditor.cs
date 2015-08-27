using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FullInspector.Internal {
    [CustomPropertyEditor(typeof(KeyValuePair<,>))]
    public class KeyValuePairPropertyEditor<TKey, TValue> : PropertyEditor<KeyValuePair<TKey, TValue>> {
        private readonly PropertyEditorChain _keyEditor = PropertyEditor.Get(typeof(TKey), null);
        private readonly PropertyEditorChain _valueEditor = PropertyEditor.Get(typeof(TValue), null);

        /// <summary>
        /// Splits the given rect into two rects that are divided horizontally.
        /// </summary>
        /// <param name="rect">The rect to split</param>
        /// <param name="percentage">The horizontal percentage that the rects are split at</param>
        /// <param name="margin">How much space that should be between the two rects</param>
        /// <param name="left">The output left-hand side rect</param>
        /// <param name="right">The output right-hand side rect</param>
        private static void SplitRect(Rect rect, float percentage, float margin, out Rect left, out Rect right) {
            left = new Rect(rect);
            left.width *= .3f;

            right = new Rect(rect);
            right.x += left.width + margin;
            right.width -= left.width + margin;
        }


        public override KeyValuePair<TKey, TValue> Edit(Rect region, GUIContent label, KeyValuePair<TKey, TValue> element, fiGraphMetadata metadata) {
            Rect keyRect, valueRect;
            SplitRect(region, /*percentage:*/ .3f, /*margin:*/ 5, out keyRect, out valueRect);

            keyRect.height = _keyEditor.FirstEditor.GetElementHeight(label, element.Key, metadata.Enter("Key"));
            valueRect.height = _valueEditor.FirstEditor.GetElementHeight(GUIContent.none, element.Value, metadata.Enter("Value"));


            var newKey = _keyEditor.FirstEditor.Edit(keyRect, label, element.Key, metadata.Enter("Key"));
            var newValue = _valueEditor.FirstEditor.Edit(valueRect, GUIContent.none, element.Value, metadata.Enter("Value"));

            return new KeyValuePair<TKey, TValue>(newKey, newValue);
        }

        public override float GetElementHeight(GUIContent label, KeyValuePair<TKey, TValue> element, fiGraphMetadata metadata) {
            float keyHeight = _keyEditor.FirstEditor.GetElementHeight(label, element.Key, metadata.Enter("Key"));
            float valueHeight = _valueEditor.FirstEditor.GetElementHeight(GUIContent.none, element.Value, metadata.Enter("Value"));

            return Math.Max(keyHeight, valueHeight);
        }
    }
}