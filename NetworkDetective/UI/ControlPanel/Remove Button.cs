using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using System;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class RemoveButton: UIButton {
        public string AtlasName => GetType().FullName + "_rev" + GetType().VersionOf();
        const int SIZE = 40;

        const string ButtonBg = "ButtonBg";
        const string ButtonBgPressed = "ButtonBgPressed";
        const string ButtonBgHovered = "ButtonBgHovered";
        const string Icon = "Icon";

        public override void Awake() {
            base.Awake();
            name = nameof(RemoveButton);
            size = new Vector2(SIZE, SIZE);
        }

        public override void Start() {
            try {
                base.Start();
                Log.Info("RemoveButton.Start() is called.");

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

                isVisible=true;
                Unfocus();
                Invalidate();
            }catch(Exception ex) { ex.Log(); }
        }
    }
}
