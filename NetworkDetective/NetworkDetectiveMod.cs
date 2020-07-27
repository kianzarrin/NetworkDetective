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
    public static class Test {
        public static float Factorial(int n) {
            if (n > 1) {
                return Factorial(n - 1) * n;
            } else {
                Log.Debug(Environment.StackTrace);
                throw new Exception("test kian exception");
                return 1;
            }
        }
    }

    public class NetworkDetectiveMod : IUserMod {
        public static Version ModVersion => typeof(NetworkDetectiveMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Network detective" + VersionString;
        public string Description => "use Ctrl+D to activate. " +
            "gives information about segment, node and lane instances.";
        
        public void OnEnabled() {
            //Test.Factorial(4); // TODO delete
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
            DisplayPanel.Create();
            GoToPanel.Create();
            Tool.NetworkDetectiveTool.Create();
            ToolsModifierControl.SetTool<DefaultTool>(); // disable tool.
        }
        public static void Release() {
            Tool.NetworkDetectiveTool.Remove();
            GoToPanel.Release();
            DisplayPanel.Release();
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
