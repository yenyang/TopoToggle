// <copyright file="Mod.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Input;
using Game.Modding;
using Game.SceneFlow;
using HarmonyLib;
using System.Reflection;
using TopoToggle.Settings;
using TopoToggle.Systems;
using UnityEngine;

namespace TopoToggle
{
    public class Mod : IMod
    {
        private Harmony m_Harmony;

        /// <summary>
        /// Mod ID for UI communications.
        /// </summary>
        public const string ID = nameof(TopoToggle);

        /// <summary>
        /// Action name for contour toggle.
        /// </summary>
        public const string kContourKeyboardToggleActionName = "ToggleContourLines";

        /// <summary>
        ///  Log file for the whole mod.
        /// </summary>
        public static ILog log = LogManager.GetLogger($"{nameof(TopoToggle)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public static Setting settings;

        /// <summary>
        /// Gets the static reference to the mod instance.
        /// </summary>
        public static Mod Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version of the mod.
        /// </summary>
#if STABLE
        internal string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
#else
        internal string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
#endif


        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));
            Instance = this;

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Initializing settings. . .");
#if VERBOSE
            log.effectivenessLevel = Level.Verbose;
#elif DEBUG
            log.effectivenessLevel = Level.Debug;
#else
            log.effectivenessLevel = Level.Info;
#endif
            settings = new Setting(this);
            settings.RegisterInOptionsUI();
            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Loading en-US localization");
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(settings));

            settings.RegisterKeyBindings();

            AssetDatabase.global.LoadSettings(nameof(TopoToggle), settings, new Setting(this));
            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Finished settings.");

            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Injecting Harmony Patches. . .");
            m_Harmony = new Harmony("Mods_Yenyang_Anarchy");
            m_Harmony.PatchAll();

            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Registering systems. . .");

            updateSystem.UpdateAt<TopoToggleUISystem>(SystemUpdatePhase.UIUpdate);

            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Finished registering systems.");

            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Complete.");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            m_Harmony.UnpatchAll();
            if (settings != null)
            {
                settings.UnregisterInOptionsUI();
                settings = null;
            }
        }
    }
}
