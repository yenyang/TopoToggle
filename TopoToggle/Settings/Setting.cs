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
using TopoToggle.Systems;
using Unity.Entities;
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

        [SettingsUISetter(typeof(Setting), nameof(HidePanelToggled))]
        public bool HidePanel { get; set; }

        [SettingsUIKeyboardBinding(actionName: Mod.kContourKeyboardToggleActionName)]
        [SettingsUIBindingMimic(InputManager.kToolMap, "Toggle Contour Lines")]
        [SettingsUIHidden]
        public ProxyBinding KeyboardToggleContourLines { get; set; }

        public override void SetDefaults()
        {
            throw new System.NotImplementedException();
        }
        public void HidePanelToggled(bool value)
        {
            TopoToggleUISystem uiSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TopoToggleUISystem>();
            uiSystem.UpdatePanelVisibility(value);
        }
    }
}
