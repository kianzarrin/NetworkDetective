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

        protected override void Awake() {
            //button = NetworkDetectiveButton.CreateButton();
            base.Awake();
        }

        public enum ModeT {
            Display,
            GoTo,
        }

        public ModeT Mode;


        public static NetworkDetectiveTool Create() {
            Log.Info("NetworkDetectiveTool.Create()");
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            var tool = toolModControl.GetComponent<NetworkDetectiveTool>() ?? toolModControl.AddComponent<NetworkDetectiveTool>();
            tool.DisableTool();
            return tool;
        }

        public static NetworkDetectiveTool Instance {
            get {
                GameObject toolModControl = ToolsModifierControl.toolController?.gameObject;
                return toolModControl?.GetComponent<NetworkDetectiveTool>();
            }
        }

        public InstanceID SelectedInstanceID { get; set; }

        public static void Remove() {
            Log.Debug("NetworkDetectiveTool.Remove()");
            var tool = Instance;
            if (tool != null)
                Destroy(tool);
        }

        protected override void OnDestroy() {
            Log.Debug("NetworkDetectiveTool.OnDestroy()\n" + Environment.StackTrace);
            button?.Hide();
            Destroy(button);
            base.OnDestroy();
        }

        //public override void EnableTool() => ToolsModifierControl.SetTool<NetworkDetectiveTool>();

        protected override void OnEnable() {
            DisplayPanel.Display(InstanceID.Empty);
            Log.Debug("NetworkDetectiveTool.OnEnable");
            base.OnEnable();
        }

        protected override void OnDisable() {
            DisplayPanel.Release();
            GoToPanel.Release();
            Log.Debug("NetworkDetectiveTool.OnDisable");
            base.OnDisable();
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
            else if (HelpersExtensions.ControlIsPressed) 
                return new InstanceID { NetNode = HoveredNodeId };
            else 
                return new InstanceID { NetSegment = HoveredSegmentId };
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            try {
                base.RenderOverlay(cameraInfo);
                if (!enabled)
                    return;
                if (Mode == ModeT.GoTo)
                    return;
                if (!SelectedInstanceID.IsValid()) {
                    DisplayPanel.Display(GetHoveredInstanceID());
                } else {
                    DisplayPanel.Display(SelectedInstanceID);
                    RenderUtil.RenderInstanceOverlay(cameraInfo, GetHoveredInstanceID(), Color.white, true);
                }
                DisplayPanel.Instance?.RenderOverlay(cameraInfo);
            } catch(Exception ex) {
                Log.Exception(ex);
            }
        }

        protected override void OnPrimaryMouseClicked() {
            if (Mode == ModeT.GoTo)
                return;
            if (!HoverValid)
                return;
            Log.Info($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
            SelectedInstanceID = GetHoveredInstanceID();
            DisplayPanel.Display(SelectedInstanceID);
        }

        protected override void OnSecondaryMouseClicked() {
            if (Mode == ModeT.Display && SelectedInstanceID.IsEmpty) {
                DisableTool();
            } else {
                GoToPanel.Release();
                SelectedInstanceID = InstanceID.Empty;
                DisplayPanel.Display(InstanceID.Empty);
            }
        }
    } //end class
}
