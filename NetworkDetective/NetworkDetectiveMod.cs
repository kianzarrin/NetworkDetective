using ICities;
using JetBrains.Annotations;
using System;
using KianCommons;
using NetworkDetective.UI.ControlPanel;
using ColossalFramework.Plugins;
using System.Linq;
using System.Reflection;
using System.IO;
using NetworkDetective.UI.GoToPanel;

namespace NetworkDetective {
    public class NetworkDetectiveMod : IUserMod {
        public static Version ModVersion => typeof(NetworkDetectiveMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Network detective" + VersionString;
        public string Description => "use Ctrl+D to activate. " +
            "gives information about segment, node and lane instances.";
        
        public void OnEnabled() {
            KianCommons.UI.TextureUtil.EmbededResources = false;
            try {
                if (HelpersExtensions.currentMode != AppMode.ThemeEditor)
                    LoadTool.Load(); // hot reload
            } catch { // we are in intro screen.
            } 
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
            try {
                Log.Called();
                DisplayPanel.Create();
                GoToPanel.Create();
                Tool.NetworkDetectiveTool.Create();
                ToolsModifierControl.SetTool<DefaultTool>(); // disable tool.
            } catch(Exception ex) { ex.Log(); }
        }
        public static void Release() {
            try {
                Log.Called();
                Tool.NetworkDetectiveTool.Remove();
                GoToPanel.Release();
                DisplayPanel.Release();
            } catch (Exception ex) { ex.Log(); }
        }
    }

    public class LoadingExtention : LoadingExtensionBase {
        public override void OnLevelLoaded(LoadMode mode) {
            switch (mode) {
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
