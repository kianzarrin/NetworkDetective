using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using System;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class ReverseButton: UIButton {
        public string AtlasName => GetType().FullName + "_rev" + GetType().VersionOf();
        const int SIZE = 40;

        const string ButtonBg = "ButtonBg";
        const string ButtonBgPressed = "ButtonBgPressed";
        const string ButtonBgHovered = "ButtonBgHovered";
        const string Icon = "Icon";

        public override void Awake() {
            base.Awake();
            name = nameof(ReverseButton);
            size = new Vector2(SIZE, SIZE);
        }

        public override void Start() {
            try {
                base.Start();
                Log.Called();

                playAudioEvents = true;
                tooltip = "Reverse segments";

                string[] spriteNames = new string[]
                {
                ButtonBg,
                ButtonBgHovered,
                ButtonBgPressed,
                Icon,
                };

                var atlas = TextureUtil.GetAtlas(AtlasName);
                if (atlas == UIView.GetAView().defaultAtlas) {
                    atlas = TextureUtil.CreateTextureAtlas("Invert.png", AtlasName, SIZE, SIZE, spriteNames);
                }

                Log.Debug("atlas name is: " + atlas.name);
                this.atlas = atlas;

                hoveredBgSprite = ButtonBgHovered;
                pressedBgSprite = ButtonBgPressed;
                normalBgSprite = focusedBgSprite = disabledBgSprite = ButtonBg;
                normalFgSprite = focusedFgSprite = disabledFgSprite = hoveredFgSprite = pressedFgSprite = Icon;

                isVisible=true;
                Unfocus();
                Invalidate();
            }catch(Exception ex) { ex.Log(); }
        }

        protected override void OnClick(UIMouseEventParameter p) {
            base.OnClick(p);
            Tool.NetworkDetectiveTool.Instance.Mode = Tool.NetworkDetectiveTool.ModeT.Reverse;
        }

    }
}
