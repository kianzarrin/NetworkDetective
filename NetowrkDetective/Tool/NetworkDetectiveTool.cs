using ColossalFramework;
using ColossalFramework.UI;
using KianCommons;
using NetworkDetective.UI.ControlPanel;
using System;
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
            ControlPanel.Instance?.Open();
            Log.Debug("NetworkDetectiveTool.OnEnable");
            button.Focus();
            base.OnEnable();
            button.Focus();
            button.Invalidate();
        }

        protected override void OnDisable() {
            ControlPanel.Instance?.Close();
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

        Vector3 _cachedHitPos;


        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            base.RenderOverlay(cameraInfo);
            if (!HoverValid)return;
            Color color = GetToolColor(Input.GetMouseButton(0), false);
            NetTool.RenderOverlay(cameraInfo, ref HoveredSegmentId.ToSegment(), color, color);

            //DrawOverlayCircle(cameraInfo, Color.red, HitPos, 1, true);
        }

        protected override void OnPrimaryMouseClicked() {
            if (!HoverValid)
                return;
            Log.Info($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
        }

        protected override void OnSecondaryMouseClicked() {
            //throw new System.NotImplementedException();
        }
    } //end class
}
