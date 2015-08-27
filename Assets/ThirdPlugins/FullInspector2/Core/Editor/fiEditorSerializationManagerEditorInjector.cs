using UnityEditor;

namespace FullInspector.Internal {
    [InitializeOnLoad]
    public class fiEditorSerializationManagerEditorInjector {
        static fiEditorSerializationManagerEditorInjector() {
            EditorApplication.update += fiEditorSerializationManager.RunDeserializations;
        }
    }
}