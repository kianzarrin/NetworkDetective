using ICities;
using JetBrains.Annotations;
using System;
using KianCommons;
using NetworkDetective.UI.ControlPanel;

namespace NetworkDetective {
    public class NetworkDetectiveMod : IUserMod {
        public static Version ModVersion => typeof(NetworkDetectiveMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Network detective" + VersionString;
        public string Description => "use Ctrl+B to activate. " +
            "gives information about segment, nodes and lanes.";
        
        public void OnEnabled() {
            if (HelpersExtensions.InGame)
                LoadTool.Load();
#if DEBUG
            TestsExperiments.Run();
#endif
        }

        public void OnDisabled() {
            LoadTool.Release();
        }

        public void OnSettingsUI(UIHelperBase helper) {
            UI.ModSettings.OnSettingsUI(helper);
        }
    }

    public static class LoadTool {
        public static void Load() {
            ControlPanel.Create();
            //Tool.NetworkDetectiveTool.Create();
        }
        public static void Release() {
            //Tool.NetworkDetectiveTool.Remove();
            ControlPanel.Release();
        }
    }

    public class LoadingExtention : LoadingExtensionBase {
        public override void OnLevelLoaded(LoadMode mode) {
            switch (mode) {
                case LoadMode.NewAsset:
                case LoadMode.LoadAsset:
                case LoadMode.NewTheme:
                case LoadMode.LoadTheme:
                    return; // unsupported modes.
            }
            LoadTool.Load();
        }

        public override void OnLevelUnloading() {
            LoadTool.Release();
        }
    }



} // end namesapce
