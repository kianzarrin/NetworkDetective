using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using NetworkDetective.Util;
using System;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class InterAvtiveButton : UIButton {
        public override void Awake() {
            base.Awake();

            //text = "some text\nnewline";
            //tooltip = "some tooltip";
            // Set the button dimensions.
            width = 500;
            height = 30;

            //autoSize = true;
            textPadding = new RectOffset(10, 10, 5, 5);
            textHorizontalAlignment = UIHorizontalAlignment.Left;
        }
        public override void Start() {
            base.Start();
            //Log.Debug("InterAvtiveButton.Start");

            // Style the button to look like a menu
            atlas = TextureUtil.GetAtlas("Ingame");
            normalBgSprite = disabledBgSprite = focusedBgSprite = "ButtonSmall";
            hoveredBgSprite = "ButtonSmallHovered";
            pressedBgSprite = "ButtonSmallPressed";
            //textColor = Color.white;
            //disabledTextColor = new Color32(7, 7, 7, 255);
            //hoveredTextColor = new Color32(7, 132, 255, 255);
            //focusedTextColor = new Color32(255, 255, 255, 255);
            //pressedTextColor = new Color32(30, 30, 44, 255);

            // Enable button sounds.
            playAudioEvents = true;
        }

        public bool IsHovered => this.m_IsMouseHovering;

        private InstanceID _instanceID;

        public InstanceID InstanceID {
            get => _instanceID;
            set {
                _instanceID = value;
                if(value.Type == InstanceType.NetLane)
                    LaneData = NetUtil.GetLaneData(value.NetLane);
                else LaneData = default;
            }
        }

        public LaneData LaneData { get; private set; } //optional. only valid for lanes.

        public void SetLaneData(LaneData laneData) {
            LaneData = laneData;
            _instanceID = new InstanceID {NetLane = laneData.LaneID};
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
            if (InstanceID.IsEmpty)
                return "Please Hover/Select a network";
#pragma warning disable
             switch(InstanceID.Type) {
                case InstanceType.NetNode:
                    return "node flags: " + FlagsUtil.GetNodeFlags(InstanceID.NetNode);
                case InstanceType.NetSegment:
                    return
                        "segment flags: " + FlagsUtil.GetSegmentFlags(InstanceID.NetSegment) + "\n" +
                        "prefab: " + InstanceID.NetSegment.ToSegment().Info?.name.ToSTR();           
                case InstanceType.NetLane:
                    try {
                        return
                            "lane flags: " + FlagsUtil.GetLaneFlags(LaneData.LaneID) + "\n" +
                            "lane types: " + LaneData.LaneInfo.m_laneType + "\n" +
                            "vehicle types: " + LaneData.LaneInfo.m_vehicleType + "\n" +
                            "direction: " + LaneData.LaneInfo.m_direction + "\n" +
                            "final direction: " + LaneData.LaneInfo.m_finalDirection + "\n" +
                            "start node:" + LaneData.StartNode + "\n";
                    } catch (Exception ex) {
                        return LaneData + "\n" + ex.Message;
                    }
                default:
                    return "Unexpected InstanceID.Type: " + InstanceID.Type;
            };
#pragma warning enable
        }


        protected override void OnClick(UIMouseEventParameter p) {
            base.OnClick(p);
            Log.Debug("InterAvtiveButton.OnClick");
            if (InstanceID.Type != InstanceType.NetLane)
                DisplayPanel.Instance.Display(this.InstanceID);
        }

        protected override void OnMouseEnter(UIMouseEventParameter p) {
            base.OnMouseEnter(p);
            //Log.Debug("InterAvtiveButton.OnMouseEnter");
            DisplayPanel.Instance.UpdateDetails(this);
        }

        protected override void OnMouseLeave(UIMouseEventParameter p) {
            base.OnMouseLeave(p);
            //Log.Debug("InterAvtiveButton.OnMouseLeave");
            DisplayPanel.Instance.UpdateDetails(DisplayPanel.Instance.Title); // default
        }

    }
}
