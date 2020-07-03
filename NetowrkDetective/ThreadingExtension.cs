namespace NetworkDetective {
    using ICities;
    using KianCommons;
    using NetworkDetective.Tool;
    using System;

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
        } // end method
    }
}
