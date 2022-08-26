using ColossalFramework.UI;
using KianCommons;
using KianCommons.Plugins;
using KianCommons.UI;
using NetworkDetective.Util;
using System;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class InterActiveButton : UIButton {
        public bool IsHovered => this.m_IsMouseHovering;

        private InstanceID _instanceID;

        private RemoveButton RemoveButton;

        public InstanceID InstanceID {
            get => _instanceID;
            set {
                _instanceID = value;
                if (value.Type == InstanceType.NetLane) {
                    LaneData = NetUtil.GetLaneData(value.NetLane);
                    DropRemoveButton();
                } else {
                    LaneData = default;
                    AddRemoveButton();
                }
            }
        }

        public LaneData LaneData { get; private set; } //optional. only valid for lanes.

        public void SetLaneData(LaneData laneData) {
            LaneData = laneData;
            _instanceID = new InstanceID { NetLane = laneData.LaneID };
            DropRemoveButton();
        }

        public override void Awake() {
            base.Awake();
            width = 500;
            height = 30;

            textPadding = new RectOffset(10, 10, 5, 5);
            textHorizontalAlignment = UIHorizontalAlignment.Left;

            atlas = TextureUtil.GetAtlas("Ingame");
            normalBgSprite = disabledBgSprite = focusedBgSprite = "ButtonSmall";
            hoveredBgSprite = "ButtonSmallHovered";
            pressedBgSprite = "ButtonSmallPressed";
        }

        public void AddRemoveButton() {
            RemoveButton = AddUIComponent<RemoveButton>();
            RemoveButton.size = new Vector2(30, 30);
            RemoveButton.relativePosition = new Vector2(width - 30, 0);
            RemoveButton.eventClick += RemoveButton_eventClick;
        }

        public void DropRemoveButton() {
            Destroy(RemoveButton?.gameObject);
        }

        private void RemoveButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
            if (InstanceID.Type == InstanceType.NetNode)
                SimulationManager.instance.AddAction(() => NetManager.instance.ReleaseNode(InstanceID.NetNode));
            else if (InstanceID.Type == InstanceType.NetSegment)
                SimulationManager.instance.AddAction(() => NetManager.instance.ReleaseSegment(InstanceID.NetSegment, keepNodes:true));
            eventParam.Use();
        }

        public virtual void RenderOverlay(RenderManager.CameraInfo cameraInfo, bool alphaBlend = false) {
            if (!InstanceID.IsValid())
                return;
            switch (InstanceID.Type) {
                case InstanceType.NetLane:
                    RenderUtil.RenderLaneOverlay(cameraInfo, LaneData, Color.yellow, alphaBlend);
                    break;
                case InstanceType.NetSegment:
                    RenderUtil.RenderSegmnetOverlay(cameraInfo, InstanceID.NetSegment, Color.cyan, alphaBlend);
                    break;
                case InstanceType.NetNode:
                    RenderUtil.DrawNodeCircle(cameraInfo, Color.blue, InstanceID.NetNode, alphaBlend);
                    break;
                default:
                    Log.Error("Unexpected InstanceID.Type: " + InstanceID.Type);
                    return;
            }
        }

        public string GetDetails() {
            try {
                if (InstanceID.IsEmpty)
                    return "Please Hover/Select a network";
#pragma warning disable
                string ret;
                switch (InstanceID.Type) {
                    case InstanceType.NetNode:
                        ret = "node flags: " + FlagsUtil.GetNodeFlags(InstanceID.NetNode);
                        ushort segmentId;
                        bool startNode;
                        if (AdaptiveRoadsUtil.IsActive && DisplayPanel.Instance.IsSegmentEndHoverd(out segmentId, out startNode)) {
                            ret += "\n" + FlagsUtil.GetSegmentEndFlags(segmentId: segmentId, startNode: startNode);
                        }
                        string sharedTags = FlagsUtil.GetNodeTags(InstanceID.NetNode);
                        string combinedTags = FlagsUtil.GetNodeCombinedTags(InstanceID.NetNode);
                        if (sharedTags != "" || combinedTags != "") {
                            ret += "\nshared tags: " + sharedTags;
                            ret += "\nall tags: " + combinedTags;
                        }
                        return ret;
                    case InstanceType.NetSegment:
                        ret = "segment flags: " + FlagsUtil.GetSegmentFlags(InstanceID.NetSegment);
                        if (AdaptiveRoadsUtil.IsActive && DisplayPanel.Instance.IsSegmentEndHoverd(out segmentId, out startNode)) {
                            ret += "\n" + FlagsUtil.GetSegmentEndFlags(segmentId: segmentId, startNode: startNode);
                        }
                        NetInfo segmentInfo = InstanceID.NetSegment.ToSegment().Info;
                        ret += "\nprefab: " + segmentInfo?.name.ToSTR();

                        var prefabTags = FlagsUtil.ArrayToString(segmentInfo?.m_tags);
                        if(prefabTags != "")
                            ret += "\nprefab tags:" + prefabTags;
                        return ret;
                    case InstanceType.NetLane:
                        try {
                            return
                                "lane flags: " + FlagsUtil.GetLaneFlags(LaneData.LaneID) + "\n" +
                                "lane types: " + LaneData.LaneInfo.m_laneType + "\n" +
                                "vehicle types: " + LaneData.LaneInfo.m_vehicleType + "\n" +
                                "direction: " + LaneData.LaneInfo.m_direction + "\n" +
                                "final direction: " + LaneData.LaneInfo.m_finalDirection + "\n" +
                                "start node:" + LaneData.StartNode + "\n";
                        }
                        catch (Exception ex) {
                            return LaneData + "\n" + ex.Message;
                        }
                    default:
                        return "Unexpected InstanceID.Type: " + InstanceID.Type;
                };
            }catch(Exception ex) {
                return ex.ToString();
            }
#pragma warning enable
        }


        protected override void OnClick(UIMouseEventParameter p) {
            base.OnClick(p);
            Log.Debug("InterActiveButton.OnClick");
            if (InstanceID.Type != InstanceType.NetLane)
                DisplayPanel.Instance.Display(this.InstanceID);
        }

        protected override void OnMouseEnter(UIMouseEventParameter p) {
            base.OnMouseEnter(p);
            //Log.Debug("InterActiveButton.OnMouseEnter");
            DisplayPanel.Instance.UpdateDetails(this);
        }

        protected override void OnMouseLeave(UIMouseEventParameter p) {
            base.OnMouseLeave(p);
            //Log.Debug("InterActiveButton.OnMouseLeave");
            DisplayPanel.Instance.UpdateDetails(DisplayPanel.Instance.Title); // default
        }

    }
}
