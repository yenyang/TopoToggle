// <copyright file="LocaleEN.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Colossal;
namespace TopoToggle.Settings
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Settings m_Setting;
        public LocaleEN(Settings setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Topo Toggle" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Settings.HidePanel)), "Hide Topo Toggle Panel" },
                { m_Setting.GetOptionDescLocaleID(nameof(Settings.HidePanel)), "Hides the panel if you only want to use the keybind." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Settings.Version)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Settings.Version)), $"Version number for the Topo Toggle mod installed."  },

                { m_Setting.GetOptionLabelLocaleID(nameof(Settings.ShowTerrainElevation)), "Show Terrain Elevation of Cursor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Settings.ShowTerrainElevation)), $"Replace the TOPO title in the panel with the Elevation (E) relative to Sea Level of the terrain surface at the cursor."  },

                { m_Setting.GetOptionLabelLocaleID(nameof(Settings.GameToggleOption)), "Game Toggle Option" },
                { m_Setting.GetOptionDescLocaleID(nameof(Settings.GameToggleOption)), "You can use the Floating Panel, or dock the toggle to multiple in-game options." },

                { m_Setting.GetEnumValueLocaleID(Settings.GameToggleOptions.FloatingPanel), "Floating Panel" },
                { m_Setting.GetEnumValueLocaleID(Settings.GameToggleOptions.TopRight), "Top Right" },
                { m_Setting.GetEnumValueLocaleID(Settings.GameToggleOptions.TopLeft), "Top Left" },
                { m_Setting.GetEnumValueLocaleID(Settings.GameToggleOptions.BottomRight), "Bottom Right" },
                { m_Setting.GetEnumValueLocaleID(Settings.GameToggleOptions.UniversalModButton), "Universal Mod Button" },

                { TextLabel("ElevationAbbreviation"), "E" },
            };
        }

        public void Unload()
        {

        }

        private string TextLabel(string key)
        {
            return $"{Mod.ID}.Text_Label_[{key}]";
        }
    }
}
