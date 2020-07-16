namespace NetworkDetective.UI.GoToPanel {
    using ColossalFramework;
    using ColossalFramework.UI;
    using KianCommons;
    using KianCommons.UI;
    using NetworkDetective.Tool;
    using NetworkDetective.UI.ControlPanel;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class GoToPanel : UIAutoSizePanel {
        #region Instanciation
        public static GoToPanel Instance { get; private set; }

        public static GoToPanel Create() {
            var uiView = UIView.GetAView();
            GoToPanel panel = uiView.AddUIComponent(typeof(GoToPanel)) as GoToPanel;
            return panel;
        }

        public static void Release() {
            Destroy(Instance);
        }

        UIButton NodeButton;
        UIButton SegmentButton;
        UIButton LaneButton;

        private uint _ID;
        public uint ID {
            set {
                _ID = value;
                RefreshButtons();
            }
            get => _ID;
        }

        #endregion Instanciation

        public override void Awake() {
            base.Awake();
            Instance = this;
            isVisible = true;
        }

        public override void Start() {
            base.Start();
            Log.Debug("GoToPanel started");

            width = 150;
            name = "GoToPanel";
            backgroundSprite = "MenuPanel2";
            absolutePosition = new Vector3(DisplayPanel.SavedX, DisplayPanel.SavedY);
 
            {
                var dragHandle_ = AddUIComponent<UIDragHandle>();
                dragHandle_.width = width;
                dragHandle_.height = 42;
                dragHandle_.relativePosition = Vector3.zero;
                dragHandle_.target = parent;

                var lblCaption = dragHandle_.AddUIComponent<UILabel>();
                lblCaption.text = "Network Detective";
                lblCaption.relativePosition = new Vector3(100, 14, 0);

                var gotoBtn = dragHandle_.AddUIComponent<GoToButton>();
                gotoBtn.relativePosition = new Vector2(width - 40 - 40, 2.5f);
            }

            AddSpacePanel(this, 10);

            {
                var panel = AddPanel();
                panel.AddUIComponent<IDTextField>();
            }

            AddSpacePanel(this, 10);

            {
                var panel = AddPanel();
                NodeButton = panel.AddUIComponent<UIButtonExt>();
                NodeButton.text = "Node";
                NodeButton.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) => {
                    NetworkDetectiveTool.Instance.SelectedInstanceID = new InstanceID { NetNode = (ushort)ID };
                    DisplayPanel.Instance.Display(NetworkDetectiveTool.Instance.SelectedInstanceID);
                };
                SegmentButton = panel.AddUIComponent<UIButtonExt>();
                SegmentButton.text = "Segment";
                SegmentButton.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) => {
                    NetworkDetectiveTool.Instance.SelectedInstanceID = new InstanceID { NetSegment = (ushort)ID };
                    DisplayPanel.Instance.Display(NetworkDetectiveTool.Instance.SelectedInstanceID);
                };
                LaneButton = panel.AddUIComponent<UIButtonExt>();
                LaneButton.text = "Lane";
                LaneButton.eventClicked += (UIComponent component, UIMouseEventParameter eventParam) => {
                    NetworkDetectiveTool.Instance.SelectedInstanceID = new InstanceID { NetLane = (ushort)ID };
                    DisplayPanel.Instance.Display(NetworkDetectiveTool.Instance.SelectedInstanceID);
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
        }

        UIAutoSizePanel AddPanel() => AddPanel(this);

        static UIAutoSizePanel AddPanel(UIPanel panel) {
            HelpersExtensions.AssertNotNull(panel, "panel");
            int pad_horizontal = 0;
            int pad_vertical = 0;
            UIAutoSizePanel newPanel = panel.AddUIComponent<UIAutoSizePanel>();
            HelpersExtensions.AssertNotNull(newPanel, "newPanel");
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

        public void Open(uint id) {
            Log.Debug("GoToPanel.Display() called");
            NetworkDetectiveTool.Instance.Mode = NetworkDetectiveTool.ModeT.GoTo;
            DisplayPanel.Instance.Close();
            Show();
            ID = id;
            RefreshSizeRecursive();
            RefreshButtons();
        }

        public void Close() {
            Log.Debug("GoToPanel.Close() called");
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

        void RefreshButtons() {
            LaneButton.isEnabled = NetUtil.IsLaneValid(ID);
            NodeButton.isEnabled = NetUtil.IsNodeValid((ushort)ID);
            SegmentButton.isEnabled = NetUtil.IsSegmentValid((ushort)ID);
            Invalidate();
        }
    }
}

