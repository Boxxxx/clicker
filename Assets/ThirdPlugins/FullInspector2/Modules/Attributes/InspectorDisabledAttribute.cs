using System;

namespace FullInspector {
    /// <summary>
    /// Draws the regular property editor but with a disabled GUI. With the current implementation
    /// this is not compatible with other attribute editors.
    /// </summary>
    // TODO: rename to [InspectorReadOnly]
    // TODO: implement this inside of the core so we can support multiple attribute editors
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorDisabledAttribute : Attribute {
    }
}