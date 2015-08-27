using FullInspector;

public class UpdateFullInspectorRootDirectory : fiSettingsProcessor {
    public void Process() {
        fiSettings.RootDirectory = "Assets/ThirdPlugins/FullInspector2/";
    }
}
