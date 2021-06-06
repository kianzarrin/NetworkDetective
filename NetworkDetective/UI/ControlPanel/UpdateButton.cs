using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using System;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class UpdateButton: UIButton {
        public static UpdateButton Instance { get; private set; }


        public string AtlasName => GetType().FullName + "_rev" + GetType().VersionOf();
        const int SIZE = 40;

        const string ButtonBg = "GoToButtonBg";
        const string ButtonBgPressed = "GoToButtonBgPressed";
        const string ButtonBgHovered = "GoToButtonBgHovered";
        const string Icon = "GoToIcon";

        public override void Awake() {
            base.Awake();
            name = nameof(UpdateButton);
            size = new Vector2(SIZE, SIZE);
            anchor = UIAnchorStyle.CenterVertical | UIAnchorStyle.Right;
            relativePosition = new Vector2(-85, 0);
            Instance = this;
        }

        public override void Start() {
            base.Start();
            Log.Info("UpdateButton.Start() is called.");

            playAudioEvents = true;
            tooltip = "Go to network";

            string[] spriteNames = new string[]
            {
                ButtonBg,
                ButtonBgHovered,
                ButtonBgPressed,
                Icon,
            };

            var atlas = TextureUtil.GetAtlas(AtlasName);
            if (atlas == UIView.GetAView().defaultAtlas) {
                atlas = TextureUtil.CreateTextureAtlas("Close.png", AtlasName, SIZE, SIZE, spriteNames);
            }

            Log.Debug("atlas name is: " + atlas.name);
            this.atlas = atlas;

            hoveredBgSprite = ButtonBgHovered;
            pressedBgSprite = ButtonBgPressed;
            normalBgSprite = focusedBgSprite = disabledBgSprite = ButtonBg;
            normalFgSprite = focusedFgSprite = disabledFgSprite = hoveredFgSprite = pressedFgSprite = Icon;

            Show();
            Unfocus();
            Invalidate();
            Log.Info("UpdateButton created sucessfully.");
        }
    }
}
