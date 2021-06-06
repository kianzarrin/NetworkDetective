using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using System;
using UnityEngine;

namespace NetworkDetective.UI.ControlPanel {
    public class UpdateToggle: UICheckBox {
        static public UpdateToggle Instance { get; private set; }
        public string AtlasName => GetType().FullName + "_rev" + GetType().VersionOf();
        const int SIZE = 40;

        UIButton uncheckedButton_, checkedButton_;
        const string ButtonBg = "ButtonBg";
        const string ButtonBgPressed = "ButtonBgPressed";
        const string ButtonBgHovered = "ButtonBgHovered";
        const string IconChecked = "IconChecked";
        const string IconUnchecked = "IconUnChecked";

        public override void Awake() {
            try {
                base.Awake();
                name = GetType().Name;
                size = new Vector2(SIZE, SIZE);

                var button1 =

                uncheckedButton_ = AddUIComponent<UIButton>();
                uncheckedButton_.size = size;
                uncheckedButton_.relativePosition = Vector3.zero;

                checkedButton_ = uncheckedButton_.AddUIComponent<UIButton>();
                checkedBoxObject = checkedButton_;
                checkedBoxObject.size = size;
                checkedBoxObject.relativePosition = Vector3.zero;

                eventCheckChanged += OnCheckChanged;
                Instance = this;
            } catch(Exception ex) { ex.Log(); }
        }

        public virtual void OnCheckChanged(UIComponent component, bool value) {
            Invalidate();
        }

        public override void Start() {
            try {
                base.Start();
                Log.Called();

                playAudioEvents = true;
                tooltip = "toggle update on click";

                string[] spriteNames = new string[] {
                    ButtonBg,
                    ButtonBgHovered,
                    ButtonBgPressed,
                    IconChecked,
                    IconUnchecked,
                };

                var atlas = TextureUtil.GetAtlas(AtlasName);
                if (atlas == UIView.GetAView().defaultAtlas) {
                    atlas = TextureUtil.CreateTextureAtlas("update.png", AtlasName, SIZE, SIZE, spriteNames);
                }

                Log.Debug("atlas name is: " + atlas.name);
                uncheckedButton_.atlas = checkedButton_.atlas = atlas;


                uncheckedButton_.hoveredBgSprite = ButtonBgHovered;
                uncheckedButton_.pressedBgSprite = ButtonBgPressed;
                uncheckedButton_.normalBgSprite = uncheckedButton_.focusedBgSprite = uncheckedButton_.disabledBgSprite = ButtonBg;
                uncheckedButton_.normalFgSprite = uncheckedButton_.focusedFgSprite = uncheckedButton_.disabledFgSprite =
                    uncheckedButton_.hoveredFgSprite = uncheckedButton_.pressedFgSprite = IconUnchecked;

                checkedButton_.hoveredBgSprite = ButtonBgHovered;
                checkedButton_.pressedBgSprite = ButtonBgPressed;
                checkedButton_.normalBgSprite = checkedButton_.focusedBgSprite = checkedButton_.disabledBgSprite = ButtonBg;
                checkedButton_.normalFgSprite = checkedButton_.focusedFgSprite = checkedButton_.disabledFgSprite =
                    checkedButton_.hoveredFgSprite = checkedButton_.pressedFgSprite = IconChecked;


                Show();
                Unfocus();
                Invalidate();
                Log.Succeeded();
            } catch(Exception ex) { ex.Log(); }
        }
    }
}
