using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using NetworkDetective.Tool;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class CloseButton: UIButton {
        public string AtlasName => GetType().FullName + "_rev" + GetType().VersionOf();
        const int SIZE = 40;


        const string CloseButtonBg = "CloseButtonBg";
        const string CloseButtonBgPressed = "CloseButtonBgPressed";
        const string CloseButtonBgHovered = "CloseButtonBgHovered";
        const string CloseIcon = "CloseIcon";

        public override void Awake() {
            base.Awake();
            name = nameof(CloseButton);
            size = new Vector2(SIZE, SIZE);
        }

        public override void Start() {
            base.Start();
            Log.Info("CloseButton.Start() is called.");

            playAudioEvents = true;
            tooltip = "Close";

            string[] spriteNames = new string[]
            {
                CloseButtonBg,
                CloseButtonBgHovered,
                CloseButtonBgPressed,
                CloseIcon,
            };

            var atlas = TextureUtil.GetAtlas(AtlasName);
            if (atlas == UIView.GetAView().defaultAtlas) {
                atlas = TextureUtil.CreateTextureAtlas("close.png", AtlasName, SIZE, SIZE, spriteNames);
            }

            Log.Debug("atlas name is: " + atlas.name);
            this.atlas = atlas;

            hoveredBgSprite = CloseButtonBgHovered;
            pressedBgSprite = CloseButtonBgPressed;
            normalBgSprite = focusedBgSprite = disabledBgSprite = CloseButtonBg;
            normalFgSprite = focusedFgSprite = disabledFgSprite = hoveredFgSprite = pressedFgSprite = CloseIcon;

            Show();
            Unfocus();
            Invalidate();
            Log.Info("CloseButton created sucessfully.");
        }

        protected override void OnClick(UIMouseEventParameter p) {
            Log.Debug("ON CLICK CALLED");
            base.OnClick(p);
            NetworkDetectiveTool.Instance.DisableTool();
        }
    }
}
