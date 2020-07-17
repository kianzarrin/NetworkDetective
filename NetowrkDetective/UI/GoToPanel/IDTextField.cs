namespace NetworkDetective.UI.GoToPanel {
    using ColossalFramework.UI;
    using System.Globalization;
    using UnityEngine;
    using KianCommons.UI;

    public class IDTextField : UITextField {
        public static IDTextField Instance { get; private set; }

        public override void Awake() {
            base.Awake();
            Instance = this;
            atlas = TextureUtil.GetAtlas("Ingame");
            size = new Vector2(50, 20);
            padding = new RectOffset(4, 4, 3, 3);
            builtinKeyNavigation = true;
            isInteractive = true;
            readOnly = false;
            horizontalAlignment = UIHorizontalAlignment.Center;
            selectionSprite = "EmptySprite";
            selectionBackgroundColor = new Color32(0, 172, 234, 255);
            normalBgSprite = "TextFieldPanelHovered";
            disabledBgSprite = "TextFieldPanelHovered";
            textColor = new Color32(0, 0, 0, 255);
            disabledTextColor = new Color32(80, 80, 80, 128);
            color = new Color32(255, 255, 255, 255);
            textScale = 0.9f;
            useDropShadow = true;
            text = "0";
            name = nameof(IDTextField);
            tooltip = "enter id to go to";
        }

        public override void Start() {
            base.Start();
        }

        public bool TryGetValue(out uint value) {
            return uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out value);
        }

        public uint Value {
            set => text = value.ToString();
            get => uint.Parse(text, CultureInfo.InvariantCulture.NumberFormat);
        }

        private string _prevText = "0";

        protected override void OnTextChanged() {
            base.OnTextChanged();

            if (TryGetValue(out _)) {
                _prevText = text;
                GoToPanel.Instance.ID = Value;
            } else {
                text = _prevText;
                Unfocus();
            }
        }
    }
}
