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
            Log.Called();
            base.Awake();
        }

        public enum ModeT {
            Display,
            GoTo,
        }

        public ModeT Mode;


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
                if (Mode == ModeT.GoTo)
                    return;
                if (!SelectedInstanceID.IsValid()) {
                    DisplayPanel.Instance.Display(GetHoveredInstanceID());
                } else {
                    DisplayPanel.Instance.Display(SelectedInstanceID);
                    RenderUtil.RenderInstanceOverlay(cameraInfo, GetHoveredInstanceID(), Color.white, true);
                }
                DisplayPanel.Instance.RenderOverlay(cameraInfo);
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
            DisplayPanel.Instance.Display(SelectedInstanceID);
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
