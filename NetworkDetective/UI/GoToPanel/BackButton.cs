using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using NetworkDetective.Tool;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class BackButton: UIButton {
        public string AtlasName => GetType().FullName + "_rev" + GetType().VersionOf();
        const int SIZE = 40;

        const string BackButtonBg = "BackButtonBg";
        const string BackButtonBgPressed = "BackButtonBgPressed";
        const string BackButtonBgHovered = "BackButtonBgHovered";
        const string BackIcon = "BackIcon";

        public override void Awake() {
            base.Awake();
            Log.Debug("BackButton.Awake() is called.");
            name = nameof(BackButton);
            size = new Vector2(SIZE, SIZE);
        }

        public override void Start() {
            base.Start();
            Log.Info("BackButton.Start() is called.");

            playAudioEvents = true;
            tooltip = "Back";

            string[] spriteNames = new string[]
            {
                BackButtonBg,
                BackButtonBgHovered,
                BackButtonBgPressed,
                BackIcon,
            };

            var atlas = TextureUtil.GetAtlas(AtlasName);
            if (atlas == UIView.GetAView().defaultAtlas) {
                atlas = TextureUtil.CreateTextureAtlas("back.png", AtlasName, SIZE, SIZE, spriteNames);
            }

            this.atlas = atlas;

            hoveredBgSprite = BackButtonBgHovered;
            pressedBgSprite = BackButtonBgPressed;
            normalBgSprite = focusedBgSprite = disabledBgSprite = BackButtonBg;
            normalFgSprite = focusedFgSprite = disabledFgSprite = hoveredFgSprite = pressedFgSprite = BackIcon;

            Show();
            Unfocus();
            Invalidate();
            Log.Info("BackButton created sucessfully.");
        }

        protected override void OnClick(UIMouseEventParameter p) {
            Log.Debug("ON CLICK CALLED");
            base.OnClick(p);
            DisplayPanel.Instance.Display(NetworkDetectiveTool.Instance.SelectedInstanceID);
        }
    }
}
