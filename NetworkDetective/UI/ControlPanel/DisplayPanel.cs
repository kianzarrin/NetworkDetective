namespace NetworkDetective.UI.ControlPanel {
    using ColossalFramework;
    using ColossalFramework.UI;
    using KianCommons;
    using KianCommons.UI;
    using NetworkDetective.UI.ControlPanel;
    using System.Collections.Generic;
    using UnityEngine;
    using GoToPanel;
    using NetworkDetective.Tool;
    using static System.Environment;


    // TODO node lanes !
    // TODO lane as title. ?
    // TODO why segment flags are always 0.
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
        }

        bool started_ = false;
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
                lblCaption.relativePosition = new Vector3(100, 14, 0);

                //var sprite = dragHandle_.AddUIComponent<UISprite>();
                //sprite.size = new Vector2(40, 40);
                //sprite.relativePosition = new Vector3(5, 2.5f, 0);
                //sprite.atlas = TextureUtil.GetAtlas(PedestrianBridgeButton.ATLAS_NAME);
                //sprite.spriteName = PedestrianBridgeButton.PedestrianBridgeIconPressed;

                var closeBtn = dragHandle_.AddUIComponent<CloseButton>();
                closeBtn.relativePosition = new Vector2(width - 40 , 3f);

                var gotoBtn = dragHandle_.AddUIComponent<GoToButton>();
                gotoBtn.relativePosition = new Vector2(width - 80, 3f);
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
                Details.padding = new RectOffset(5, 5, 5, 5);
                Details.minimumSize = new Vector2(width, 0);
                Details.wordWrap = true;
            }

            isVisible = false;
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

        public void Populate() {
            if (PupulatablePanel != null) {
                PupulatablePanel.Hide();
                Destroy(PupulatablePanel);
            }

            PupulatablePanel = AddPanel(ContainerOfPopulatablePanel);
            InterAvtiveButtons = new List<InterAvtiveButton>(32);

            if (InstanceID.Type == InstanceType.NetSegment) {
                PupulateSegmentMembers(PupulatablePanel, InstanceID.NetSegment);
            }else if(InstanceID.Type == InstanceType.NetNode) {
                PupulateNodeMembers(PupulatablePanel, InstanceID.NetNode);
            }

            RefreshSizeRecursive();
        }

        void PupulateSegmentMembers(UIAutoSizePanel panel,  ushort segmentId) {
            {
                var item = panel.AddUIComponent<InterAvtiveButton>();
                ushort nodeId = segmentId.ToSegment().m_startNode;
                item.InstanceID = new InstanceID { NetNode = nodeId };
                bool isHeadNode = NetUtil.GetHeadNode(segmentId) == nodeId;
                string t = isHeadNode ? "head" : "tail";
                item.text = $"Start node: {nodeId} ({t} node) ";
                item.tooltip = "if the road was a oneway road, cars drive from tail node to head node.";
                InterAvtiveButtons.Add(item);
            }
            {
                var item = panel.AddUIComponent<InterAvtiveButton>();
                ushort nodeId = segmentId.ToSegment().m_endNode;
                item.InstanceID = new InstanceID { NetNode = nodeId };
                bool isHeadNode = NetUtil.GetHeadNode(segmentId) == nodeId;
                string t = isHeadNode ? "head" : "tail";
                item.text = $"End node: {nodeId} ({t} node) ";
                item.tooltip = "if the road was a oneway road, cars drive from tail node to head node.";
                InterAvtiveButtons.Add(item);
            }

            AddSpacePanel(panel, 5);

            foreach(var laneData in NetUtil.IterateSegmentLanes(segmentId)) {
                var item = panel.AddUIComponent<InterAvtiveButton>();
                item.SetLaneData(laneData);
                item.text = $"Lane[{item.LaneData.LaneIndex}]: {item.InstanceID.NetLane}";
                if(laneData.SegmentID == segmentId)
                    item.text += $" ( {item.LaneData.LaneInfo.m_laneType} | {item.LaneData.LaneInfo.m_vehicleType} ) ";
                else {
                    item.text += "error: segmentID mismatch: " + laneData;
                }
                InterAvtiveButtons.Add(item);
            }
        }

        void PupulateNodeMembers(UIAutoSizePanel panel, ushort nodeId) {
            for(int i = 0; i < 8; ++i) {
                ushort segmentId = nodeId.ToNode().GetSegment(i);
                if (segmentId == 0) continue;
                bool startNode = NetUtil.IsStartNode(segmentId: segmentId, nodeId: nodeId);
                var item = panel.AddUIComponent<InterAvtiveButton>();
                InterAvtiveButtons.Add(item);
                item.InstanceID = new InstanceID { NetSegment = segmentId};
                item.text = $"Segment: {segmentId} " + (startNode ? "(start node)" : "end node");
            }

            AddSpacePanel(panel, 3);

            //foreach (uint laneId in NetUtil.IterateNodeLanes(nodeId)) {
            //    var item = panel.AddUIComponent<InterAvtiveButton>();
            //    item.InstanceID = new InstanceID { NetLane = laneId };
            //    item.text = $"Lane: " + laneId;
            //    InterAvtiveButtons.Add(item);
            //}
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
                InstanceType.NetLane => $"Lane[{Title.LaneData.LaneIndex}]: {InstanceID.NetLane}",
                _ => "Unexpected InstanceID.Type: " + InstanceID.Type,
            };
        }

        public void UpdateDetails(InterAvtiveButton item) {
            Details.text = item.GetDetails();
            RefreshSizeRecursive();
        }

        public void Display(InstanceID instanceID) {
            if (!started_)
                return;
            GoToPanel.Instance.Close();
            NetworkDetectiveTool.Instance.Mode = NetworkDetectiveTool.ModeT.Display;
            if (isVisible && InstanceID == instanceID)
                return;
            Log.Debug("DisplayPanel.Display() called ");
            Show();
            InstanceID = instanceID;
            RefreshSizeRecursive();
        }

        public void Close() {
            Log.Debug("DisplayPanel.Close() called");
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

