
namespace NetworkDetective.UI.ControlPanel {
    using ColossalFramework.UI;
    using KianCommons;
    using KianCommons.UI;
    using NetworkDetective.Tool;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ActionModeT = NetworkDetective.Tool.NetworkDetectiveTool.ActionModeT;

    internal class ActionRadioButton : UIButton {
        public ActionModeT ActionMode;

        internal const string ICON = "fg_icon";

        internal const string BG_NORMAL = "bg_normal";

        internal const string BG_HOVERED = "bg_hovered";

        internal const string BG_PRESSED = "bg_pressed";

        protected internal string Icon => ActionMode.ToString();


        public const int SIZE = 40;

        public bool active_ = false;

        public UIComponent Group => parent;


        public static string AtlasName => "ActionRadioButton_Atals_rev" + typeof(ActionRadioButton).VersionOf();

        public virtual string Name => GetType().Name;

        public bool IsActive {
            get {
                return active_;
            }
            set {
                if (value) {
                    UseActiveSprites();
                } else {
                    UseInactiveSprites();
                }
            }
        }


        public override void Awake() {
            try {
                base.Awake();
                isVisible = true;
                size = new Vector2(SIZE, SIZE);
                canFocus = false;
            } catch (Exception ex) {
                ex.Exception();
            }
        }

        public override void Start() {
            try {
                Log.Called(Icon);
                base.Start();
                atlas = GetOrCreateAtlas();
                name = Name;
                tooltip = Icon;
                disabledFgSprite = pressedFgSprite = hoveredFgSprite = normalFgSprite = Icon;
                pressedBgSprite = BG_PRESSED;
                IsActive = ActionMode == NetworkDetectiveTool.Instance.ActionMode;
            } catch (Exception ex) {
                ex.Exception();
            }
        }

        public static UITextureAtlas GetOrCreateAtlas() {
            try {
                string file = "actions.png";
                var spriteNames = new List<string>();
                spriteNames.Add(BG_NORMAL);
                spriteNames.Add(BG_HOVERED);
                spriteNames.Add(BG_PRESSED);
                foreach (var action in NetworkDetectiveTool.ActionModes) {
                    spriteNames.Add(action.ToString());
                }

                return TextureUtil.GetAtlasOrNull(AtlasName) ?? TextureUtil.CreateTextureAtlas(file, AtlasName, SIZE, SIZE, spriteNames.ToArray());
            } catch (Exception ex) {
                ex.Log();
                throw;
            }
        }
        public void UseActiveSprites() {
            normalBgSprite = BG_PRESSED;
            hoveredBgSprite = BG_PRESSED;
            Invalidate();
            active_ = true;
        }

        public void UseInactiveSprites() {
            normalBgSprite = BG_NORMAL;
            hoveredBgSprite = BG_HOVERED;
            Invalidate();
            active_ = false;
        }

        public virtual void Activate() {
            IsActive = true;
            foreach (var rb in Group.GetComponentsInChildren<ActionRadioButton>()) {
                if (rb != this) {
                    rb.Deactivate();
                }
            }
            NetworkDetectiveTool.Instance.ActionMode = this.ActionMode;
        }

        public virtual void Deactivate() {
            IsActive = false;
        }

        public virtual void Toggle() {
            try {
                Log.Called();
                if (IsActive) {
                    Deactivate();
                } else {
                    Activate();
                }
            } catch (Exception ex) {
                ex.Exception();
            }
        }

        protected override void OnClick(UIMouseEventParameter p) {
            base.OnClick(p);
            Activate();
        }
    }
}
