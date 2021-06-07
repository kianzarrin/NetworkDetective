namespace NetworkDetective.UI.ControlPanel {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using ColossalFramework;
    using UnityEngine.UI;
    using ColossalFramework.UI;
    using KianCommons;
    using KianCommons.UI;
    using KianCommons.UI.Helpers;
    using NetworkDetective.Tool;

    internal class ActionDropDown: UIDropDownExt {
        public static ActionDropDown Instance { get; private set; }
        public override void Awake() {
            try {
                Instance = this;
                name = GetType().FullName;
                Log.Called();
                base.Awake();
                items = Enum.GetNames(typeof(NetworkDetectiveTool.ActionModeT));
                selectedIndex = 0;
                tooltip = "Action to perform on click.";
                width = 500;
            } catch(Exception ex) { ex.Log(); }
        }

        public NetworkDetectiveTool.ActionModeT SelectedAction =>
            (NetworkDetectiveTool.ActionModeT)selectedIndex;
    }
}
