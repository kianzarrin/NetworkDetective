using ColossalFramework.UI;
using ICities;
using ColossalFramework;
using KianCommons;

namespace NetworkDetective.UI {
    using Tool;
    using KianCommons.UI;
    public static class ModSettings {
        public const string FILE_NAME = nameof(NetworkDetective);

        //public static readonly SavedBool InLineLaneInfo = new SavedBool(
        //    "InLineLaneInfo",
        //    FILE_NAME,
        //    true,
        //    true);

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

            //var ShowLaneType = group.AddCheckbox( 
            //    "Display lane type/vehicle type in parantheses in front of each lane.",
            //    InLineLaneInfo.value,
            //    val => InLineLaneInfo.value = val);

            helper.AddButton(
                "Update All Roads (slower)",
                () => SimulationManager.instance.AddAction(UpdateAllRoads));
            helper.AddButton(
                "Recalculate all nodes (faster)",
                () => SimulationManager.instance.AddAction(RecalculateAllNodes));
        }

        static void UpdateAllRoads() {
            for (ushort segmentID = 0; segmentID < NetManager.MAX_SEGMENT_COUNT; ++segmentID) {
                if (NetUtil.IsSegmentValid(segmentID) && segmentID.ToSegment().Info?.m_netAI is RoadBaseAI) {
                    NetManager.instance.UpdateSegment(segmentID);
                }
            }
        }
        static void RefreshAllNetworkRenderers() {
            for (ushort segmentID = 0; segmentID < NetManager.MAX_SEGMENT_COUNT; ++segmentID) 
                if (NetUtil.IsSegmentValid(segmentID)) 
                    NetManager.instance.UpdateSegmentRenderer(segmentID,true);
            for (ushort nodeID = 0; nodeID < NetManager.MAX_NODE_COUNT; ++nodeID)
                if (NetUtil.IsNodeValid(nodeID))
                    NetManager.instance.UpdateNodeRenderer(nodeID, true);
        }

        static void RecalculateAllNodes () {
            for (ushort nodeID = 0; nodeID < NetManager.MAX_NODE_COUNT; ++nodeID)
                if (NetUtil.IsNodeValid(nodeID))
                    nodeID.ToNode().CalculateNode(nodeID);
            RefreshAllNetworkRenderers();
        }
    }
}
