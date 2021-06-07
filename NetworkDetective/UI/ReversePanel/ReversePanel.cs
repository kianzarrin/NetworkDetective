namespace NetworkDetective.UI.ReversePanel {
    using ColossalFramework;
    using ColossalFramework.UI;
    using KianCommons;
    using KianCommons.UI;
    using NetworkDetective.Tool;
    using NetworkDetective.UI.ControlPanel;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ReversePanel : UIAutoSizePanel {
        #region Instanciation
        public static ReversePanel Instance { get; private set; }

        public static ReversePanel Create() {
            var uiView = UIView.GetAView();
            ReversePanel panel = uiView.AddUIComponent(typeof(ReversePanel)) as ReversePanel;
            return panel;
        }

        public static void Release() {
            Destroy(Instance);
        }

        #endregion Instanciation

        UICheckBoxExt InvertButton;
        UICheckBoxExt ReverseButton;
        UICheckBoxExt BothButton;

        private uint _ID;
        public uint ID {
            set {
                _ID = value;
                RefreshButtons();
            }
            get => _ID;
        }


        public override void Awake() {
            base.Awake();
            Instance = this;
        }


        bool started_ = false;
        public override void Start() {
            base.Start();
            Log.Debug("ReversePanel started");

            width = 150;
            name = "ReversePanel";
            backgroundSprite = "MenuPanel2";
            absolutePosition = new Vector3(DisplayPanel.SavedX, DisplayPanel.SavedY);
 
            {
                var dragHandle_ = AddUIComponent<UIDragHandle>();
                dragHandle_.width = width;
                dragHandle_.height = 42;
                dragHandle_.relativePosition = Vector3.zero;
                dragHandle_.target = parent;

                var lblCaption = dragHandle_.AddUIComponent<UILabel>();
                lblCaption.text = "Go To";
                lblCaption.relativePosition = new Vector3(14, 14, 0);

                var backBtn = dragHandle_.AddUIComponent<BackButton>();
                backBtn.relativePosition = new Vector2(width - 40, 3f);
            }

            AddSpacePanel(this, 10);

            {
                var panel = AddPanel();
                var inGameAtlas = TextureUtil.GetAtlas("Ingame");
                InvertButton = panel.AddUIComponent<UICheckBoxExt>();
                InvertButton.Label = "Invert";
                InvertButton.eventCheckChanged += (UIComponent component, bool value) => {
                    if (value) {
                        NetworkDetectiveTool.Instance.ReverseMode = NetworkDetectiveTool.ReverseModeT.Invert;
                        ReverseButton.isChecked = BothButton.isChecked = false;
                    }
                };
                ReverseButton = panel.AddUIComponent<UICheckBoxExt>();
                ReverseButton.Label= "Reverse";
                ReverseButton.eventCheckChanged += (UIComponent component, bool value) => {
                    if (value) {
                        NetworkDetectiveTool.Instance.ReverseMode = NetworkDetectiveTool.ReverseModeT.Reverse;
                        BothButton.isChecked = InvertButton.isChecked = false;
                    }
                };
                BothButton = panel.AddUIComponent<UICheckBoxExt>();
                BothButton.Label = "Both";
                BothButton.eventCheckChanged += (UIComponent component, bool value) => {
                    if (value) {
                        NetworkDetectiveTool.Instance.ReverseMode = NetworkDetectiveTool.ReverseModeT.Both;
                        ReverseButton.isChecked = InvertButton.isChecked = false;
                    }
                };
            }

            AddSpacePanel(this, 10);

            {
                var panel = AddPanel();
                var backButton = panel.AddUIComponent<UIButtonExt>();
                backButton.text = "back";
                backButton.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) => {
                    DisplayPanel.Instance.Display(NetworkDetectiveTool.Instance.SelectedInstanceID);
                };
            }

            isVisible = false;
            RefreshSizeRecursive();
            Invalidate();
            started_ = true;
        }

        UIAutoSizePanel AddPanel() => AddPanel(this);

        static UIAutoSizePanel AddPanel(UIPanel panel) {
            Assertion.AssertNotNull(panel, "panel");
            int pad_horizontal = 0;
            int pad_vertical = 0;
            UIAutoSizePanel newPanel = panel.AddUIComponent<UIAutoSizePanel>();
            Assertion.AssertNotNull(newPanel, "newPanel");
            newPanel.width = panel.width - pad_horizontal * 2;
            newPanel.autoLayoutPadding =
                new RectOffset(pad_horizontal, pad_horizontal, pad_vertical, pad_vertical);
            return newPanel;
        }

        static UIPanel AddSpacePanel(UIPanel panel, int space) {
            panel = panel.AddUIComponent<UIPanel>();
            panel.width = panel.width;
            panel.height = space;
            return panel;
        }

        public void Open() {
            if (!started_)
                return;
            Log.Debug("ReversePanel.Display() called");
            NetworkDetectiveTool.Instance.Mode = NetworkDetectiveTool.ModeT.Reverse;
            NetworkDetectiveTool.Instance.ReverseMode = default;
            DisplayPanel.Instance.Close();
            Show();
            RefreshSizeRecursive();
            RefreshButtons();
        }

        public void Close() {
            //Log.Debug("ReversePanel.Close() called");
            Hide();
        }

        protected override void OnPositionChanged() {
            base.OnPositionChanged();
            Log.Debug("OnPositionChanged called");

            Vector2 resolution = GetUIView().GetScreenResolution();

            absolutePosition = new Vector2(
                Mathf.Clamp(absolutePosition.x, 0, resolution.x - width),
                Mathf.Clamp(absolutePosition.y, 0, resolution.y - height));

            DisplayPanel.SavedX.value = absolutePosition.x;
            DisplayPanel.SavedY.value = absolutePosition.y;
            Log.Debug("absolutePosition: " + absolutePosition);
        }

        public static void GoToInstance(InstanceID instanceID) {
            Vector3 pos = instanceID.Type switch{
                InstanceType.NetNode => instanceID.NetNode.ToNode().m_position,
                InstanceType.NetSegment => instanceID.NetSegment.ToSegment().m_middlePosition,
                _ => throw new NotImplementedException("instanceID.Type:"+ instanceID.Type),
            };
            pos.y = Camera.main.transform.position.y;
            ToolsModifierControl.cameraController.SetTarget(instanceID, pos, true);
        }

        void RefreshButtons() {
            if (!started_)
                return;
            BothButton.isEnabled = NetUtil.IsLaneValid(ID);
            InvertButton.isEnabled = NetUtil.IsNodeValid((ushort)ID);
            ReverseButton.isEnabled = NetUtil.IsSegmentValid((ushort)ID);
            Invalidate();
        }
    }
}

