using ColossalFramework;
using ColossalFramework.UI;
using KianCommons;
using NetworkDetective.UI.ControlPanel;
using System;
using System.Diagnostics;
using UnityEngine;

namespace NetworkDetective.Tool {
    using static KianCommons.UI.RenderUtil;

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

        public static NetworkDetectiveTool Create() {
            Log.Info("NetworkDetectiveTool.Create()");
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            var tool = toolModControl.GetComponent<NetworkDetectiveTool>() ?? toolModControl.AddComponent<NetworkDetectiveTool>();
            return tool;
        }

        public static NetworkDetectiveTool Instance {
            get {
                GameObject toolModControl = ToolsModifierControl.toolController?.gameObject;
                return toolModControl?.GetComponent<NetworkDetectiveTool>();
            }
        }

        public InstanceID SelectedInstanceID { get; private set; }

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
            DisplayPlanel.Instance?.Display(InstanceID.Empty);
            Log.Debug("NetworkDetectiveTool.OnEnable");
            button.Focus();
            base.OnEnable();
            button.Focus();
            button.Invalidate();
        }

        protected override void OnDisable() {
            DisplayPlanel.Instance?.Close();
            Log.Debug("NetworkDetectiveTool.OnDisable");
            button?.Unfocus();
            base.OnDisable();
            button?.Unfocus();
            button?.Invalidate();
        }

        protected override void OnToolUpdate() {
            base.OnToolUpdate();
            ToolCursor = HoverValid ? NetUtil.netTool.m_upgradeCursor : null;
        }

        public InstanceID GetHoveredInstanceID() {
            if (HelpersExtensions.ControlIsPressed) {
                return new InstanceID { NetNode = HoveredNodeId };
            } else {
                return new InstanceID { NetSegment = HoveredSegmentId };
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            base.RenderOverlay(cameraInfo);
            if (!HoverValid)return;
            InstanceID instanceID = new InstanceID { NetSegment = HoveredSegmentId };
            DisplayPlanel.Instance.Display(GetHoveredInstanceID());
            DisplayPlanel.Instance.RenderOverlay(cameraInfo);
        }

        protected override void OnPrimaryMouseClicked() {
            if (!HoverValid)
                return;
            Log.Info($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
            if (HelpersExtensions.ControlIsPressed) {
                
            } else {
                // segment selection mode:
                SelectedInstanceID = GetHoveredInstanceID();
            }
        }

        protected override void OnSecondaryMouseClicked() {
            if (SelectedInstanceID.IsEmpty)
                DisableTool();
            else
                SelectedInstanceID = InstanceID.Empty;
        }
    } //end class
}
