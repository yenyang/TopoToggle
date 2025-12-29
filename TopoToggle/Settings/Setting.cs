// <copyright file="Setting.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using Game.Tools;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using TopoToggle.Systems;
using Unity.Entities;
using Unity.Mathematics;

namespace TopoToggle.Settings
{
    [FileLocation(nameof(TopoToggle))]
    [SettingsUIKeyboardAction(Mod.kContourKeyboardToggleActionName, ActionType.Button, usages: new string[] { Usages.kMenuUsage, "TopoToggle" }, interactions: new string[] { "UIButton" })]
    [SettingsUIGroupOrder(General, About)]
    public class Setting : ModSetting
    {

        /// <summary>
        /// This is for general settings.
        /// </summary>
        public const string General = "General";

        /// <summary>
        /// This is for about section of settings.
        /// </summary>
        public const string About = "About";


        public Setting(IMod mod) : base(mod)
        {

        }

        [SettingsUIHidden]
        public bool ForceContourLines { get; set; }

        [SettingsUIHidden]
        public float2 GamePanelPosition { get; set; }

        [SettingsUIHidden]
        public float2 EditorPanelPosition { get; set; }

        [SettingsUISection(General, General)]
        [SettingsUISetter(typeof(Setting), nameof(HidePanelToggled))]
        public bool HidePanel { get; set; }

        [SettingsUISection(General, General)]
        [SettingsUISetter(typeof(Setting), nameof(ShowTerrainHitPositionToggled))]
        public bool ShowTerrainHitPosition { get; set; }

        /// <summary>
        /// Gets a value indicating the version.
        /// </summary>
        [SettingsUISection(General, About)]
        public string Version => Mod.Instance.Version;

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

        public void ShowTerrainHitPositionToggled(bool value)
        {
            TopoToggleUISystem uiSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TopoToggleUISystem>();
            if (value)
            {
                uiSystem.Enabled = true;
            }

            uiSystem.UpdateShowTerrainHitPosition(value);
        }
    }
}
