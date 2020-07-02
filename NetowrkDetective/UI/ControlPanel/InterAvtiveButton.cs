using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class InterAvtiveButton : UIButton {
        public override void Awake() {
            base.Awake();

            //text = "some text\nnewline";
            //tooltip = "some tooltip";
            // Set the button dimensions.
            width = 100;
            height = 40;
            // autoSize = true;
            textPadding = new RectOffset(10, 10, 5, 5);
        }
        public override void Start() {
            base.Start();
            Log.Debug("AvtiveLabel.Start");

            // Style the button to look like a menu
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
                if (value.Type == InstanceType.NetLane)
                    LaneData = NetUtil.GetLaneData(value.NetLane);
                else
                    LaneData = default;
            }
        }

        public LaneData LaneData { get; private set; } //optional. only valid for lanes.


        public virtual void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            switch (InstanceID.Type) {
                case InstanceType.NetLane:
                    RenderUtil.RenderLaneOverlay(cameraInfo, LaneData, Color.blue, false);
                    break;
                case InstanceType.NetSegment:
                    RenderUtil.RenderSegmnetOverlay(cameraInfo, InstanceID.NetSegment, Color.blue, false);
                    break;
                case InstanceType.NetNode:
                    RenderUtil.DrawNodeCircle(cameraInfo, Color.blue, InstanceID.NetNode, false);
                    break;
                default:
                    Log.Error("Unexpected InstanceID.Type: "+ InstanceID.Type);
                    return;
            }
        }

        protected override void OnClick(UIMouseEventParameter p) {
            base.OnClick(p);
            Log.Debug("AvtiveLabel.OnClick");
        }

        protected override void OnMouseEnter(UIMouseEventParameter p) {
            base.OnMouseEnter(p);
            Log.Debug("AvtiveLabel.OnMouseEnter");
            ControlPanel.Instance.DisplayDetails(InstanceID);
        }

        protected override void OnMouseLeave(UIMouseEventParameter p) {
            base.OnMouseLeave(p);
            Log.Debug("AvtiveLabel.OnMouseLeave");
            ControlPanel.Instance.DisplayDetails(null); // default
        }

    }
}
