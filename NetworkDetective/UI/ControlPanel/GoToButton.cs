using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using System;
using UnityEngine;
using UIUtils = KianCommons.UI.UIUtils;

namespace NetworkDetective.UI.ControlPanel {
    public class GoToButton: UIButton {
        public static GoToButton Instance { get; private set; }

        public static GoToButton Instace { get; private set; }

        public static string AtlasName = "GoToButtonUI_rev" +
            typeof(GoToButton).Assembly.GetName().Version.Revision;
        const int SIZE = 40;
        const string CONTAINING_PANEL_NAME = "RoadsOptionPanel";
        readonly static Vector2 RELATIVE_POSITION = new Vector2(94, 38);

        const string GoToButtonBg = "GoToButtonBg";
        const string GoToButtonBgPressed = "GoToButtonBgPressed";
        const string GoToButtonBgHovered = "GoToButtonBgHovered";
        const string GotoIcon = "GotoIcon";

        public override void Awake() {
            base.Awake();
            Log.Debug("GoToButton.Awake() is called.");
            name = nameof(GoToButton);
            size = new Vector2(SIZE, SIZE);
            Instace = this;
        }

        public override void Start() {
            base.Start();
            Log.Info("GoToButton.Start() is called.");

            playAudioEvents = true;
            tooltip = "Go to network";

            string[] spriteNames = new string[]
            {
                GoToButtonBg,
                GoToButtonBgHovered,
                GoToButtonBgPressed,
                GotoIcon,
            };

            var atlas = TextureUtil.GetAtlas(AtlasName);
            if (atlas == UIView.GetAView().defaultAtlas) {
                atlas = TextureUtil.CreateTextureAtlas("goto.png", AtlasName, SIZE, SIZE, spriteNames);
            }

            Log.Debug("atlas name is: " + atlas.name);
            this.atlas = atlas;

            hoveredBgSprite = GoToButtonBgHovered;
            pressedBgSprite = GoToButtonBgPressed;
            normalBgSprite = focusedBgSprite = disabledBgSprite = GoToButtonBg;
            normalFgSprite = focusedFgSprite = disabledFgSprite = hoveredFgSprite = pressedFgSprite = GotoIcon;

            Show();
            Unfocus();
            Invalidate();
            Log.Info("GoToButton created sucessfully.");
        }

        protected override void OnClick(UIMouseEventParameter p) {
            Log.Debug("ON CLICK CALLED");
            base.OnClick(p);
            GoToPanel.GoToPanel.Open(0);

        }
    }
}
