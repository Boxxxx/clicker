﻿using FullInspector.Internal;
using System;
using UnityEngine;
using FullSerializer.Internal;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Draws the default inspector for the given type.
        /// </summary>
        public class DefaultInspector : tkControl<T, TContext> {
            private readonly Type type_fitkControlPropertyEditor = TypeCache.FindType("FullInspector.Internal.fitkControlPropertyEditor");
            private readonly Type type_IObjectPropertyEditor = TypeCache.FindType("FullInspector.Modules.Common.IObjectPropertyEditor");

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                return (T)fiLateBindings.PropertyEditor.EditSkipUntilNot(new [] {
                    type_fitkControlPropertyEditor, type_IObjectPropertyEditor
                }, typeof(T), typeof(T).Resolve(), rect, GUIContent.none, obj, new fiGraphMetadataChild { Metadata = metadata });
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return fiLateBindings.PropertyEditor.GetElementHeightSkipUntilNot(new[] {
                    type_fitkControlPropertyEditor, type_IObjectPropertyEditor
                }, typeof(T), typeof(T).Resolve(), GUIContent.none, obj, new fiGraphMetadataChild { Metadata = metadata });
            }
        }
    }
}