using ColossalFramework;
using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using NetworkDetective.UI.ControlPanel;
using NetworkDetective.UI.GoToPanel;
using System;
using UnifiedUI.Helpers;
using UnityEngine;

namespace NetworkDetective.Tool {
    public sealed class NetworkDetectiveTool : KianToolBase{
        public static readonly SavedInputKey ActivationShortcut = new SavedInputKey(
            "ActivationShortcut",
            UI.ModSettings.FILE_NAME,
            SavedInputKey.Encode(KeyCode.D, true, false, false),
            true);

        UIButton button;

        static SimulationManager simMan = SimulationManager.instance;
        static NetManager netMan = NetManager.instance;
        UIComponent uuiButton_;

        protected override void Awake() {
            Log.Called();
            string iconPath = UUIHelpers.GetFullPath<NetworkDetectiveMod>("uui_network_detective.png");
            uuiButton_ = UUIHelpers.RegisterToolButton(
                    name: "Network Detective",
                    groupName: null, // default group
                    tooltip: "Network Detective",
                    tool: this,
                    icon: UUIHelpers.LoadTexture(iconPath),
                    hotkeys: new UUIHotKeys { ActivationKey = ActivationShortcut});
            base.Awake();
        }

        public enum ModeT {
            Display,
            GoTo,
        }

        public ModeT Mode;

        public enum ActionModeT {
            None,
            Update,
            Invert,
            Reverse,
            InvertReverse,
        }

        public ActionModeT ActionMode => ActionDropDown.Instance.SelectedAction;

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
                uuiButton_?.Destroy();
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

        protected override bool DetermineHoveredElements() {
            if (DisplayPanel.Instance != null && DisplayPanel.Instance.containsMouse) {
                // prevent panel flipping.
                return HoveredNodeId != 0 || HoveredSegmentId != 0;
            } else {
                return base.DetermineHoveredElements();
            }
        }

        protected override void OnToolUpdate() {
            base.OnToolUpdate();
            if (Mode == ModeT.Display && HoverValid)
                ToolCursor = NetUtil.netTool.m_upgradeCursor;
            else
                ToolCursor = null;
        }

        public InstanceID GetHoveredInstanceID() {
            if (HoveredNodeId == 0 && HoveredSegmentId == 0)
                return InstanceID.Empty;
            else if (Helpers.ControlIsPressed)
                return new InstanceID { NetNode = HoveredNodeId };
            else
                return new InstanceID { NetSegment = HoveredSegmentId };
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            try {
                base.RenderOverlay(cameraInfo);
                if (enabled && Mode == ModeT.Display) {
                    if (!SelectedInstanceID.IsValid()) {
                        Log.Debug($"SelectedInstanceID:{SelectedInstanceID} is invalid. Display hovered instance.");
                        DisplayPanel.Instance.Display(GetHoveredInstanceID(), select: false);
                    } else {
                        DisplayPanel.Instance.Display(SelectedInstanceID, select: false);
                        RenderUtil.RenderInstanceOverlay(cameraInfo, GetHoveredInstanceID(), Color.white, true);
                    }
                    DisplayPanel.Instance.RenderOverlay(cameraInfo);
                }
            } catch (Exception ex) {
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

                if (ActionMode == ActionModeT.Update) {
                    simMan.AddAction(UpdateInstance);
                }else if (SelectedInstanceID.Type == InstanceType.NetSegment) {
                    if (ActionMode == ActionModeT.Invert) {
                        simMan.AddAction(InvertSegment);
                    } else if (ActionMode == ActionModeT.Reverse) {
                        simMan.AddAction(ReverseSegment);
                    } else if (ActionMode == ActionModeT.InvertReverse) {
                        simMan.AddAction(ReverseInvertSegment);
                    }
                }
            }
        }

        void UpdateInstance() {
            InstanceID instanceID = SelectedInstanceID;
            Log.Called(instanceID);
            switch (instanceID.Type) {
                case InstanceType.NetNode:
                    netMan.UpdateNode(instanceID.NetNode);
                    simMan.m_ThreadingWrapper.QueueMainThread(DisplayPanel.Instance.RefreshAll);
                    break;
                case InstanceType.NetSegment:
                    netMan.UpdateSegment(instanceID.NetSegment);
                    simMan.m_ThreadingWrapper.QueueMainThread(DisplayPanel.Instance.RefreshAll);
                    break;
            }
        }

        void InvertSegment() {
            InstanceID instanceID = SelectedInstanceID;
            Log.Called(instanceID);
            if (instanceID.Type == InstanceType.NetSegment) {
                ref NetSegment segment = ref instanceID.NetSegment.ToSegment();
                segment.m_flags = segment.m_flags.SetFlags(NetSegment.Flags.Invert, !segment.IsInvert());
                netMan.UpdateSegment(instanceID.NetSegment);
                simMan.m_ThreadingWrapper.QueueMainThread(DisplayPanel.Instance.RefreshAll);
            }
        }

        void ReverseSegment() {
            try {
                InstanceID instanceID = SelectedInstanceID;
                Log.Called(instanceID);
                if (instanceID.Type == InstanceType.NetSegment) {
                    ushort segmentID = instanceID.NetSegment;
                    var copy = segmentID.ToSegment();
                    netMan.ReleaseSegment(segmentID, true);
                    segmentID = CreateSegment(
                        startNodeID: copy.m_endNode,
                        endNodeID: copy.m_startNode,
                        startDir: copy.m_endDirection,
                        endDir: copy.m_startDirection,
                        info: copy.Info,
                        invert: copy.IsInvert());
                    simMan.m_ThreadingWrapper.QueueMainThread(() => {
                        SelectedInstanceID = new InstanceID { NetSegment = segmentID };
                        DisplayPanel.Instance.Display(SelectedInstanceID);
                    });
                }
            } catch (Exception ex) { ex.Log(); }
        }

        void ReverseInvertSegment() {
            try {
                InstanceID instanceID = SelectedInstanceID;
                Log.Called(instanceID);
                if (instanceID.Type == InstanceType.NetSegment) {
                    ushort segmentID = instanceID.NetSegment;
                    var copy = segmentID.ToSegment();
                    netMan.ReleaseSegment(segmentID, true);
                    segmentID = CreateSegment(
                        startNodeID: copy.m_endNode,
                        endNodeID: copy.m_startNode,
                        startDir: copy.m_endDirection,
                        endDir: copy.m_startDirection,
                        info: copy.Info,
                        invert: !copy.IsInvert());
                    simMan.m_ThreadingWrapper.QueueMainThread(() => {
                        SelectedInstanceID = new InstanceID { NetSegment = segmentID };
                        DisplayPanel.Instance.Display(SelectedInstanceID);
                    });
                }
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

        protected override void OnSecondaryMouseClicked() {
            if (Mode == ModeT.Display && SelectedInstanceID.IsEmpty) {
                DisableTool();
            } else {
                GoToPanel.Instance.Hide();
                SelectedInstanceID = InstanceID.Empty;
                DisplayPanel.Instance.Display(InstanceID.Empty);
            }
        }
    } //end class
}
