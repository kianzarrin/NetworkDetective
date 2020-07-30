namespace NetworkDetective {
    using ICities;
    using KianCommons;
    using NetworkDetective.Tool;
    using System;
    using System.Diagnostics;
    using System.Security.Authentication;
    using System.Threading;
    using UnityEngine;

    public class ThreadingExtension : ThreadingExtensionBase {
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
            try {
                var tool = ToolsModifierControl.toolController?.CurrentTool;
                bool flag = tool == null || tool is NetworkDetectiveTool ||
                    tool.GetType() == typeof(DefaultTool) || tool is NetTool || tool is BuildingTool;
                if (flag && NetworkDetectiveTool.ActivationShortcut.IsKeyUp()) {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(
                        () => NetworkDetectiveTool.Instance.ToggleTool());
                }
            } catch (Exception e) {
                Log.Error(e.ToString());
            }

            //if (Input.GetKey(KeyCode.F4)) Halt(5);
        } // end method

        public override void OnBeforeSimulationFrame() {
            base.OnBeforeSimulationFrame();
            //if (Input.GetKey(KeyCode.F5)) Halt(5);
        }
        public override void OnBeforeSimulationTick() {
            base.OnBeforeSimulationTick();
            //if (Input.GetKey(KeyCode.F6)) Halt(5);
        }

        [Conditional("DEBUG")]
        public static void Halt(int seconds) {
            var waitUntil = DateTime.Now + new TimeSpan(0, 0, seconds);
            while (DateTime.Now <= waitUntil)
                Log.Debug("waiting!");
        }

    }
}
