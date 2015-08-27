using System;
using FullInspector.Internal;
using UnityEngine;
using tk = FullInspector.tk<System.WeakReference, FullInspector.tkEmptyContext>;

namespace FullInspector.Modules.Common {
    [CustomPropertyEditor(typeof(WeakReference))]
    public class WeakReferencePropertyEditor : tkControlPropertyEditor<WeakReference> {
        protected override object CreateInstance() {
            return new WeakReference(null);
        }

        private static tk.Label Label;

        private static readonly tkControlEditor Editor = new tkControlEditor(
            Label = new tk.Label(string.Empty,
                new tk.ShowIf(o => o != null,
                    new tk.VerticalGroup {
                        new tk.PropertyEditor("IsAlive") {
                            Style = new tk.ReadOnly()
                        },

                        new tk.ShowIf(weakRef => weakRef.IsAlive, new tk.VerticalGroup {
                            new tk.PropertyEditor("TrackResurrection") {
                                Style = new tk.ReadOnly()
                            },

                            new tk.PropertyEditor("Target")
                        })
                    })));

        protected override tkControlEditor GetControlEditor(GUIContent label, WeakReference element, fiGraphMetadata graphMetadata) {
            Label.GUIContent = (fiGUIContent)label;
            return Editor;
        }
    }

}