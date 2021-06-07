using ColossalFramework;
using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using NetworkDetective.UI.ControlPanel;
using NetworkDetective.UI.GoToPanel;
using System;
using UnityEngine;

namespace NetworkDetective.Tool {
    public sealed class NetworkDetectiveTool : KianToolBase {
        public static readonly SavedInputKey ActivationShortcut = new SavedInputKey(
            "ActivationShortcut",
            UI.ModSettings.FILE_NAME,
            SavedInputKey.Encode(KeyCode.D, true, false, false),
            true);

        UIButton button;

        static SimulationManager simMan = SimulationManager.instance;
        static NetManager netMan = NetManager.instance;

        protected override void Awake() {
            Log.Called();
            base.Awake();
        }

        public enum ModeT {
            Display,
            GoTo,
            Reverse,
        }

        public ModeT Mode;

        public enum ReverseModeT {
            Invert,
            Reverse,
            Both,
        }

        public ReverseModeT ReverseMode;

        public static NetworkDetectiveTool Create() {
            try {
                Log.Called();
                GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
                var tool = toolModControl.GetComponent<NetworkDetectiveTool>() ?? toolModControl.AddComponent<NetworkDetectiveTool>();
                tool.DisableTool();
                return tool;
            } catch (Exception ex) {
                ex.Log();
                throw ex;
            }
        }

        public static NetworkDetectiveTool Instance {
            get {
                GameObject toolModControl = ToolsModifierControl.toolController?.gameObject;
                return toolModControl?.GetComponent<NetworkDetectiveTool>();
            }
        }

        public InstanceID SelectedInstanceID { get; set; }

        public static void Remove() {
            try {
                Log.Called();
                var tool = Instance;
                if (tool != null)
                    Destroy(tool);
            } catch (Exception ex) { ex.Log(); }
        }

        protected override void OnDestroy() {
            try {
                Log.Debug("NetworkDetectiveTool.OnDestroy()\n" + Environment.StackTrace);
                button?.Hide();
                Destroy(button);
            } catch (Exception ex) { ex.Log(); }
            base.OnDestroy();
        }

        //public override void EnableTool() => ToolsModifierControl.SetTool<NetworkDetectiveTool>();

        protected override void OnEnable() {
            try {
                base.OnEnable();
                Log.Called();
                DisplayPanel.Instance?.Display(InstanceID.Empty);
                Log.Called();
            } catch (Exception ex) { ex.Log(); }
        }

        protected override void OnDisable() {
            try {
                DisplayPanel.Instance?.Close();
                GoToPanel.Instance.Close();
                Log.Called();
                base.OnDisable();
            } catch (Exception ex) { ex.Log(); }
        }

        protected override void OnToolUpdate() {
            base.OnToolUpdate();
            if (Mode == ModeT.Display && HoverValid)
                ToolCursor = NetUtil.netTool.m_upgradeCursor;
            else
                ToolCursor = null;
        }

        public InstanceID GetHoveredInstanceID() {
            if (!HoverValid)
                return InstanceID.Empty;
            else if (Helpers.ControlIsPressed) 
                return new InstanceID { NetNode = HoveredNodeId };
            else 
                return new InstanceID { NetSegment = HoveredSegmentId };
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            try {
                base.RenderOverlay(cameraInfo);
                if (!enabled)
                    return;
                if (Mode == ModeT.Display) {

                    if (!SelectedInstanceID.IsValid()) {
                        DisplayPanel.Instance.Display(GetHoveredInstanceID());
                    } else {
                        DisplayPanel.Instance.Display(SelectedInstanceID);
                        RenderUtil.RenderInstanceOverlay(cameraInfo, GetHoveredInstanceID(), Color.white, true);
                    }
                    DisplayPanel.Instance.RenderOverlay(cameraInfo);
                } else if(Mode == ModeT.Reverse) {
                    RenderUtil.RenderInstanceOverlay(cameraInfo, GetHoveredInstanceID(), Color.white, true);
                }
            } catch(Exception ex) {
                Log.Exception(ex);
            }
        }

        protected override void OnPrimaryMouseClicked() {
            if (!HoverValid)
                return;
            Log.Info($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");

            if (Mode == ModeT.Display) {
                SelectedInstanceID = GetHoveredInstanceID();
                DisplayPanel.Instance.Display(SelectedInstanceID);
            } else if (Mode == ModeT.Reverse) {
                if (ReverseMode == ReverseModeT.Invert) {
                    simMan.AddAction(() => InvertSegment(HoveredSegmentId));
                } else if (ReverseMode == ReverseModeT.Reverse) {
                    simMan.AddAction(() => ReverseSegment(HoveredSegmentId));
                } else if (ReverseMode == ReverseModeT.Both) {
                    simMan.AddAction(() => {
                        InvertSegment(HoveredSegmentId);
                        ReverseSegment(HoveredSegmentId);
                    });
                }

            }
        }

        static void InvertSegment(ushort segmentID) {
            bool invert = segmentID.ToSegment().IsInvert();
            segmentID.ToSegment().m_flags.SetFlags(NetSegment.Flags.Invert, !invert);
        }

        static void ReverseSegment(ushort segmentID) {
            try {
                var copy = segmentID.ToSegment();
                netMan.ReleaseSegment(segmentID, true);
                segmentID = CreateSegment(
                    startNodeID: copy.m_endNode,
                    endNodeID: copy.m_startNode,
                    startDir: copy.m_endDirection,
                    endDir: copy.m_startDirection,
                    info: copy.Info,
                    invert: copy.IsInvert());
            } catch (Exception ex) { ex.Log(); }
        }

        static ushort CreateSegment(
            ushort startNodeID, ushort endNodeID,
            Vector3 startDir, Vector3 endDir,
            NetInfo info, bool invert) {
            Log.Info($"creating segment for {info.name} between nodes {startNodeID} {endNodeID}");
            var bi = simMan.m_currentBuildIndex;
            bool res = netMan.CreateSegment(
                segment: out ushort segmentID, randomizer: ref simMan.m_randomizer, info: info,
                startNode: startNodeID, endNode: endNodeID, startDirection: startDir, endDirection: endDir,
                buildIndex: bi, modifiedIndex: bi, invert: invert);
            if (!res)
                throw new NetServiceException("Segment creation failed");
            simMan.m_currentBuildIndex++;
            return segmentID;
        }

        static 

        protected override void OnSecondaryMouseClicked() {
            if (Mode == ModeT.Display && SelectedInstanceID.IsEmpty) {
                DisableTool();
            } else {
                GoToPanel.Instance.Hide();
                SelectedInstanceID = InstanceID.Empty;
                DisplayPanel.Instance.Display(InstanceID.Empty);
                if(UpdateToggle.Instance.isChecked)
                    UpdateInstance(SelectedInstanceID);
            }
        }

        static void UpdateInstance(InstanceID instanceID) {
            switch (instanceID.Type) {
                case InstanceType.NetNode:
                    SimulationManager.instance.AddAction(() => NetManager.instance.UpdateNode(instanceID.NetNode));
                    break;
                case InstanceType.NetSegment:
                    SimulationManager.instance.AddAction(() => NetManager.instance.UpdateSegment(instanceID.NetSegment));
                    break;
            }
        }
    } //end class
}
