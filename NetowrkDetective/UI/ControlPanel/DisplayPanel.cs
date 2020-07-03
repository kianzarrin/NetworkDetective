namespace NetworkDetective.UI.ControlPanel {
    using ColossalFramework;
    using ColossalFramework.UI;
    using KianCommons;
    using KianCommons.UI;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection.Emit;
    using UnityEngine;
    using UnityEngine.UI;

    public class DisplayPanel : UIAutoSizePanel {
        public static readonly SavedFloat SavedX = new SavedFloat(
            "PanelX", ModSettings.FILE_NAME, 87, true);
        public static readonly SavedFloat SavedY = new SavedFloat(
            "PanelY", ModSettings.FILE_NAME, 58, true);

        #region Instanciation
        public static DisplayPanel Instance { get; private set; }

        public InstanceID InstanceID {
            get => Title.InstanceID;
            set {
                Title.InstanceID = value;
                UpdateTitle();
                Populate();
                UpdateDetails(Title);
            }
        }

        public List<InterAvtiveButton> InterAvtiveButtons;
        public UIAutoSizePanel ContainerOfPopulatablePanel;
        public UIAutoSizePanel PupulatablePanel;
        public UILabel Details;
        public InterAvtiveButton Title;

        public static DisplayPanel Create() {
            var uiView = UIView.GetAView();
            DisplayPanel panel = uiView.AddUIComponent(typeof(DisplayPanel)) as DisplayPanel;
            return panel;
        }

        public static void Release() {
            Destroy(Instance);
        }

        #endregion Instanciation

        public override void Awake() {
            base.Awake();
            Instance = this;
            isVisible = true;
        }

        public override void Start() {
            base.Start();
            Log.Debug("ControlPanel started");

            width = 500;
            name = "ControlPanel";
            backgroundSprite = "MenuPanel2";
            absolutePosition = new Vector3(SavedX, SavedY);


            {
                var dragHandle_ = AddUIComponent<UIDragHandle>();
                dragHandle_.width = width;
                dragHandle_.height = 42;
                dragHandle_.relativePosition = Vector3.zero;
                dragHandle_.target = parent;

                var lblCaption = dragHandle_.AddUIComponent<UILabel>();
                lblCaption.text = "Network Detective";
                lblCaption.relativePosition = new Vector3(65, 14, 0);

                //var sprite = dragHandle_.AddUIComponent<UISprite>();
                //sprite.size = new Vector2(40, 40);
                //sprite.relativePosition = new Vector3(5, 2.5f, 0);
                //sprite.atlas = TextureUtil.GetAtlas(PedestrianBridgeButton.ATLAS_NAME);
                //sprite.spriteName = PedestrianBridgeButton.PedestrianBridgeIconPressed;
            }

            AddSpacePanel(this, 10);

            {
                var panel = AddPanel();
                Title = panel.AddUIComponent<InterAvtiveButton>();
            }

            AddSpacePanel(this, 5);
            ContainerOfPopulatablePanel = AddPanel();
            AddSpacePanel(this, 10);

            {
                var panel = AddPanel();
                Details = panel.AddUIComponent<UILabel>();
            }

            Hide();
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

        public void Populate() {
            if (PupulatablePanel != null) {
                PupulatablePanel.Hide();
                Destroy(PupulatablePanel);
            }

            PupulatablePanel = AddPanel(ContainerOfPopulatablePanel);
            InterAvtiveButtons = new List<InterAvtiveButton>(32);
            if (InstanceID.Type == InstanceType.NetSegment) {
                PupulateSegmentMembers(PupulatablePanel, InstanceID.NetSegment);
            }
            RefreshSizeRecursive();
        }

        void PupulateSegmentMembers(UIAutoSizePanel panel,  ushort segmentId) {
            {
                var item = panel.AddUIComponent<InterAvtiveButton>();
                item.InstanceID = new InstanceID { NetNode = segmentId.ToSegment().m_startNode };
                item.text = "StartNode: " + item.InstanceID.NetNode;
                InterAvtiveButtons.Add(item);
            }
            {
                var item = panel.AddUIComponent<InterAvtiveButton>();
                item.InstanceID = new InstanceID { NetNode = segmentId.ToSegment().m_endNode };
                item.text = "EndNode: " + item.InstanceID.NetNode;
                InterAvtiveButtons.Add(item);
            }

            AddSpacePanel(panel, 3);

            foreach(var laneData in NetUtil.IterateLanes(segmentId)) {
                var item = panel.AddUIComponent<InterAvtiveButton>();
                item.InstanceID = new InstanceID { NetLane = laneData.LaneID };
                item.text = $"Lane[{laneData.LaneIndex}]: " + item.InstanceID.NetLane;
                InterAvtiveButtons.Add(item);
            }
        }

        public virtual void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            foreach(var item in InterAvtiveButtons) {
                if (item.IsHovered) {
                    item.RenderOverlay(cameraInfo,false);
                } 
            }
            Title.RenderOverlay(cameraInfo,true);
        }

        public void UpdateTitle() {
            if (InstanceID.IsEmpty) {
                Title.text = "0";
                return;
            }
            Title.text = InstanceID.Type switch
            {
                InstanceType.NetNode => "Node: " + InstanceID.NetNode,
                InstanceType.NetSegment => "Segment: " + InstanceID.NetSegment,
                InstanceType.NetLane => $"Lane[{Title.LaneData.LaneIndex}]: " + InstanceID.NetLane,
                _ => "Unexpected InstanceID.Type: " + InstanceID.Type,
            };
        }

        public void UpdateDetails(InterAvtiveButton item) {
            Details.text = item.GetDetails();
            RefreshSizeRecursive();
        }

        public void Display(InstanceID instanceID) {
            if (isVisible && InstanceID == instanceID)
                return;
            Show();
            InstanceID = instanceID;
            RefreshSizeRecursive();
        }

        public void Close() {
            Hide();
        }

        protected override void OnPositionChanged() {
            base.OnPositionChanged();
            Log.Debug("OnPositionChanged called");

            Vector2 resolution = GetUIView().GetScreenResolution();

            absolutePosition = new Vector2(
                Mathf.Clamp(absolutePosition.x, 0, resolution.x - width),
                Mathf.Clamp(absolutePosition.y, 0, resolution.y - height));

            SavedX.value = absolutePosition.x;
            SavedY.value = absolutePosition.y;
            Log.Debug("absolutePosition: " + absolutePosition);
        }
    }
}

