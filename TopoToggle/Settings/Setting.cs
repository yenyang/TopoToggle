// <copyright file="Setting.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using Unity.Mathematics;

namespace TopoToggle.Settings
{
    [FileLocation(nameof(TopoToggle))]
    [SettingsUIKeyboardAction(Mod.kContourKeyboardToggleActionName, ActionType.Button, usages: new string[] { Usages.kMenuUsage, "TopoToggle" }, interactions: new string[] { "UIButton" })]
    public class Setting : ModSetting
    {
        public Setting(IMod mod) : base(mod)
        {

        }

        [SettingsUIHidden]
        public bool ForceContourLines { get; set; }

        [SettingsUIHidden]
        public float2 GamePanelPosition { get; set; }

        [SettingsUIHidden]
        public float2 EditorPanelPosition { get; set; }

        public bool HidePanel { get; set; }

        [SettingsUIKeyboardBinding(actionName: Mod.kContourKeyboardToggleActionName)]
        [SettingsUIBindingMimic(InputManager.kToolMap, "Toggle Contour Lines")]
        [SettingsUIHidden]
        public ProxyBinding KeyboardToggleContourLines { get; set; }

        public override void SetDefaults()
        {
            throw new System.NotImplementedException();
        }

    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Topo Toggle" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HidePanel)), "Hide Topo Toggle Panel" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HidePanel)), "Hides the panel if you only want to use the keybind." },
            };
        }

        public void Unload()
        {

        }
    }
}
