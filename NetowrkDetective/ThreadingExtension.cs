
namespace NetworkDetective {
    using System;
    using ICities;
    using UnityEngine;
    using static KianCommons.HelpersExtensions;
    using NetworkDetective.Tool;
    using NetworkDetective.UI;

    public class ThreadingExtension : ThreadingExtensionBase{
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
                var tool = ToolsModifierControl.toolController?.CurrentTool;
                bool flag = tool == null || tool is NetworkDetectiveTool ||
                    tool.GetType() == typeof(DefaultTool) || tool is NetTool || tool is BuildingTool ||
                    tool.GetType().FullName.Contains("Roundabout");
                if (flag && NetworkDetectiveTool.ActivationShortcut.IsKeyUp()) {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(
                        () => NetworkDetectiveTool.Instance.ToggleTool());
                }
            }   

    }
}
