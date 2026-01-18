// <copyright file="Mod.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

// #define EXPORT_EN_US

using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using HarmonyLib;
using System.Reflection;
using TopoToggle.Settings;
using TopoToggle.Systems;
using Colossal.Localization;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;


#if DEBUG && EXPORT_EN_US
using Colossal;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
#endif

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

        public static Settings.Settings settings;

        /// <summary>
        /// Gets the static reference to the mod instance.
        /// </summary>
        public static Mod Instance
        {
            get;
            private set;
        }

#if DEBUG && EXPORT_EN_US
        private static string GetThisFilePath([CallerFilePath] string path = null)
        {
            return path;
        }
#endif

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
            settings = new Settings.Settings(this);
            settings.RegisterInOptionsUI();
            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Loading en-US localization");
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(settings));
            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Loading other languages");
            LoadNonEnglishLocalizations();
#if DEBUG && EXPORT_EN_US
            GenerateLanguageFile();
#endif
            settings.RegisterKeyBindings();

            AssetDatabase.global.LoadSettings(nameof(TopoToggle), settings, new Settings.Settings(this));
            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Finished settings.");

            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Injecting Harmony Patches. . .");
            m_Harmony = new Harmony("Mods_Yenyang_Anarchy");
            m_Harmony.PatchAll();

            log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Registering systems. . .");

            updateSystem.UpdateAt<TopoToggleUISystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<TopoToggleRaycastSystem>(SystemUpdatePhase.Raycast);

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

#if DEBUG && EXPORT_EN_US
        private void GenerateLanguageFile()
        {
            log.Info($"[{ID}] Exporting localization");
            var localeDict = new LocaleEN(settings).ReadEntries(new List<IDictionaryEntryError>(), new Dictionary<string, int>()).ToDictionary(pair => pair.Key, pair => pair.Value);
            var str = JsonConvert.SerializeObject(localeDict, Formatting.Indented);
            try
            {
                var path = GetThisFilePath();
                var directory = Path.GetDirectoryName(path);

                var exportPath = $@"{directory}\UI\src\mods\lang\en-US.json";
                File.WriteAllText(exportPath, str);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
#endif


        private void LoadNonEnglishLocalizations()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = thisAssembly.GetManifestResourceNames();

            try
            {
                log.Debug($"Reading localizations");

                foreach (string localeID in GameManager.instance.localizationManager.GetSupportedLocales())
                {
                    string resourceName = $"{thisAssembly.GetName().Name}.l10n.{localeID}.json";
                    if (resourceNames.Contains(resourceName))
                    {
                        log.Debug($"Found localization file {resourceName}");
                        try
                        {
                            log.Debug($"Reading embedded translation file {resourceName}");

                            // Read embedded file.
                            using StreamReader reader = new(thisAssembly.GetManifestResourceStream(resourceName));
                            {
                                string entireFile = reader.ReadToEnd();
                                Colossal.Json.Variant varient = Colossal.Json.JSON.Load(entireFile);
                                Dictionary<string, string> translations = varient.Make<Dictionary<string, string>>();
                                GameManager.instance.localizationManager.AddSource(localeID, new MemorySource(translations));
                            }
                        }
                        catch (Exception e)
                        {
                            // Don't let a single failure stop us.
                            log.Error(e, $"Exception reading localization from embedded file {resourceName}");
                        }
                    }
                    else
                    {
                        log.Debug($"Did not find localization file {resourceName}");
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Exception reading embedded settings localization files");
            }
        }
    }
}
