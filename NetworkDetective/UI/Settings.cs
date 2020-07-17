using ColossalFramework.UI;
using ICities;
using ColossalFramework;

namespace NetworkDetective.UI {
    using Tool;
    using KianCommons.UI;
    public static class ModSettings {
        public const string FILE_NAME = nameof(NetworkDetective);

        public static readonly SavedBool InLineLaneInfo = new SavedBool(
            "InLineLaneInfo",
            FILE_NAME,
            true,
            true);

        static ModSettings() {
            // Creating setting file - from SamsamTS
            if (GameSettings.FindSettingsFileByName(FILE_NAME) == null) {
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = FILE_NAME } });
            }
        }

        public static void OnSettingsUI(UIHelperBase helper) {
            UIHelper group = helper.AddGroup("Network Detective") as UIHelper;
            UIPanel panel = group.self as UIPanel;

            var keymappings = panel.gameObject.AddComponent<UIKeymappingsPanel>();
            keymappings.AddKeymapping("Activation Shortcut", NetworkDetectiveTool.ActivationShortcut);

            var ShowLaneType = group.AddCheckbox( 
                "Display lane type/vehicle type in parantheses in front of each lane.",
                InLineLaneInfo.value,
                val => InLineLaneInfo.value = val);
        }
    }
}
