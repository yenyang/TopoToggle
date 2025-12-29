// <copyright file="TopoToggleUISystem.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

//#define LOG_VANILLA_KEYBINDS

using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Common;
using Game.Input;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI;
using System;
using System.Reflection;
using TopoToggle.Raycast;
using Unity.Entities;
using Unity.Mathematics;
# if  LOG_VANILLA_KEYBINDS
using Game.Input;
using System.Collections.Generic;
#endif

namespace TopoToggle.Systems
{
    /// <summary>
    /// UI System for Topo Toggle to handle bindings between UI and C#.
    /// </summary>
    public partial class TopoToggleUISystem : UISystemBase
    {
        private ValueBinding<bool> m_ForceContourLines;
        private ValueBinding<bool> m_HideTopoTogglePanel;
        private ValueBinding<float2> m_PanelPosition;
        private ValueBinding<bool> m_RecheckPanelPosition;
        private ValueBinding<float> m_TerrainElevation;
        private ValueBinding<bool> m_ShowTerrainHitPosition;
        private WaterSystem m_WaterSystem;
        private bool m_FoundPlater;
        private ComponentType m_PlatterComponent;
        private ToolSystem m_ToolSystem;
        private PrefabSystem m_PrefabSystem;
        private ObjectToolSystem m_ObjectToolSystem;
        private ProxyAction m_ToggleContourKeybind;
        private TopoToggleRaycastSystem m_TopoToggleRaycastSystem;
        private int m_HideTimer = 0;
        private int m_TerrainHitTimer = 30;

        /// <summary>
        /// Gets the value of the force contour lines binding.
        /// </summary>
        public bool ForceContourLines
        {
            get { return m_ForceContourLines.value; }
        }

        /// <summary>
        /// Checks if the active prefab is a platter prefab.
        /// </summary>
        /// <returns>True if platter prefab active.</returns>
        public bool IsPlatterPrefabActive()
        {
            return IsPlatterPrefab(m_ToolSystem.activePrefab);
        }

        /// <summary>
        /// Updates the panel visibility. Usually from settings toggle.
        /// </summary>
        /// <param name="hidePanel">True to hide panel, false to generally show panel.</param>
        public void UpdatePanelVisibility(bool hidePanel)
        {
            m_HideTopoTogglePanel.Update(hidePanel);
        }

        /// <summary>
        /// Updates the show terrain hit position binding. Usually from settings toggle.
        /// </summary>
        /// <param name="showTerrainHitPosition">True to show hit position. false to show TOPO title.</param>
        public void UpdateShowTerrainHitPosition(bool showTerrainHitPosition)
        {
            m_HideTimer = 30;
            m_HideTopoTogglePanel.Update(true);
            m_ShowTerrainHitPosition.Update(showTerrainHitPosition);    
        }

        public override GameMode gameMode => GameMode.GameOrEditor;

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

# if DEBUG && LOG_VANILLA_KEYBINDS
            Mod.log.Debug("Shortcuts Action Map:");
            ProxyActionMap shortcutsMap = InputManager.instance.FindActionMap(InputManager.kShortcutsMap);
            foreach (KeyValuePair<string, ProxyAction> keyValue in shortcutsMap.actions)
            {
                Mod.log.Debug(keyValue.Key);
            }

            Mod.log.Debug("Tool Action Map:");
            ProxyActionMap toolMap = InputManager.instance.FindActionMap(InputManager.kToolMap);
            foreach (KeyValuePair<string, ProxyAction> keyValue in toolMap.actions)
            {
                Mod.log.Debug(keyValue.Key);
            }

            Mod.log.Debug("kEngagementMap Action Map:");
            ProxyActionMap kEngagementMap = InputManager.instance.FindActionMap(InputManager.kEngagementMap);
            foreach (KeyValuePair<string, ProxyAction> keyValue in kEngagementMap.actions)
            {
                Mod.log.Debug(keyValue.Key);
            }

            Mod.log.Debug("kMenuMap Action Map:");
            ProxyActionMap kMenuMap = InputManager.instance.FindActionMap(InputManager.kMenuMap);
            foreach (KeyValuePair<string, ProxyAction> keyValue in kEngagementMap.actions)
            {
                Mod.log.Debug(keyValue.Key);
            }

            Mod.log.Debug("kNavigationMap Action Map:");
            ProxyActionMap kNavigationMap = InputManager.instance.FindActionMap(InputManager.kNavigationMap);
            foreach (KeyValuePair<string, ProxyAction> keyValue in kEngagementMap.actions)
            {
                Mod.log.Debug(keyValue.Key);
            }
#endif

            if (mode.IsGame())
            {
                m_PanelPosition.Update(Mod.settings.GamePanelPosition);
                m_PanelPosition.TriggerUpdate();
            }
            else if (mode.IsEditor())
            {
                m_PanelPosition.Update(Mod.settings.EditorPanelPosition);
                m_PanelPosition.TriggerUpdate();
            }

            m_ToggleContourKeybind.shouldBeEnabled = mode.IsGameOrEditor();
            m_HideTopoTogglePanel.Update(true);
            m_HideTimer = 30;

            // Platter detection for compatibility.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type type = assembly.GetType("Platter.Components.ParcelPlaceholderData");
                if (type != null)
                {
                    Mod.log.Info($"Found {type.FullName} in {type.Assembly.FullName}. ");
                    m_PlatterComponent = ComponentType.ReadOnly(type);
                    m_FoundPlater = true;
                }
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            // System instances.
            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_ObjectToolSystem = World.GetOrCreateSystemManaged<ObjectToolSystem>();
            m_TopoToggleRaycastSystem = World.GetOrCreateSystemManaged<TopoToggleRaycastSystem>();
            m_WaterSystem = World.GetOrCreateSystemManaged<WaterSystem>();

            // Event registration
            m_ToolSystem.EventToolChanged += OnToolChanged;
            m_ToolSystem.EventPrefabChanged += OnPrefabChanged;

            // These establish bindings of values to send to the UI.
            AddBinding(m_ForceContourLines = new ValueBinding<bool>(Mod.ID, "ForceContourLines", Mod.settings.ForceContourLines));
            AddBinding(m_HideTopoTogglePanel = new ValueBinding<bool>(Mod.ID, "HideTopoTogglePanel", Mod.settings.HidePanel));
            AddBinding(m_PanelPosition = new ValueBinding<float2>(Mod.ID, "PanelPosition", Mod.settings.GamePanelPosition));
            AddBinding(m_RecheckPanelPosition = new ValueBinding<bool>(Mod.ID, "RecheckPanelPosition", false));
            AddBinding(m_TerrainElevation = new ValueBinding<float>(Mod.ID, "TerrainElevation", 0));
            AddBinding(m_ShowTerrainHitPosition = new ValueBinding<bool>(Mod.ID, "ShowTerrainElevation", Mod.settings.ShowTerrainElevation));

            // These establish bindings to listen to from the UI.
            AddBinding(new TriggerBinding(Mod.ID, "ToggleContourLines", ToggleContourLines));
            AddBinding(new TriggerBinding<float2>(Mod.ID, "SetPanelPosition", SetPanelPosition));
            AddBinding(new TriggerBinding(Mod.ID, "CheckPanelPosition", () => 
            {
                if (m_HideTimer == -1)
                {
                    m_HideTimer = 30;
                    m_HideTopoTogglePanel.Update(true);
                }
            }));

            m_ToggleContourKeybind = Mod.settings.GetAction(Mod.kContourKeyboardToggleActionName);

            m_ToggleContourKeybind.onInteraction += (_,_) => ToggleContourLines();

            Mod.log.Info($"{nameof(TopoToggleUISystem)}.{nameof(OnCreate)}");
        }

        /// <summary>
        /// This is janky and I don't like it. I have strugged for awhile to figure out how to save and use panel position consistenly and this is the best I came up with.
        /// </summary>
        protected override void OnUpdate()
        {
            if (m_HideTimer > 0)
            {
                m_RecheckPanelPosition.Update(!m_RecheckPanelPosition.value);
                m_HideTimer -= 1;
            }
            else if (m_HideTimer == 0)
            {
                if (Mod.settings.HidePanel ||
                    IsPlatterPrefabActive())
                {
                    m_HideTopoTogglePanel.Update(true);
                }
                else
                {
                    m_HideTopoTogglePanel.Update(false);
                }

                m_HideTimer = -1;
            }

            if (m_HideTimer == -1 &&
               !Mod.settings.HidePanel &&
                Mod.settings.ShowTerrainElevation)
            {
                if (m_TerrainHitTimer <= 0)
                {
                    UpdateTerrainElevation(m_TopoToggleRaycastSystem.TerrainHitPosition);
                    m_TerrainHitTimer = 30;
                }
                else
                {
                    m_TerrainHitTimer -= 1;
                }
            }
        }

        private void ToggleContourLines()
        {
            m_ForceContourLines.Update(!m_ForceContourLines.value);
            Mod.settings.ForceContourLines = m_ForceContourLines.value;
            Mod.settings.ApplyAndSave();
        }

        private void OnToolChanged(ToolBaseSystem toolSystem)
        {  
            if (Mod.settings.HidePanel ||
                IsPlatterPrefabActive())
            {
                m_HideTopoTogglePanel.Update(true);
                return;
            }

            m_HideTopoTogglePanel.Update(false);
        }

        private void OnPrefabChanged(PrefabBase prefab)
        {
            if (Mod.settings.HidePanel ||
                IsPlatterPrefab(prefab))
            {
                m_HideTopoTogglePanel.Update(true);
                return;
            }

            m_HideTopoTogglePanel.Update(false);
        }

        private bool IsPlatterPrefab(PrefabBase prefab)
        {
            if (m_FoundPlater &&
                prefab != null &&
                m_ToolSystem.activeTool == m_ObjectToolSystem &&
                m_PrefabSystem.TryGetEntity(m_ToolSystem.activePrefab, out Entity prefabEntity) &&
                EntityManager.HasComponent(prefabEntity, m_PlatterComponent))
            {
                return true;
            }

            return false;
        }

        private void SetPanelPosition(float2 position)
        {
            m_PanelPosition.Update(position);
            if (m_ToolSystem.actionMode.IsGame())
            {
                Mod.settings.GamePanelPosition = position;
                Mod.settings.ApplyAndSave();
            }
            else if (m_ToolSystem.actionMode.IsEditor())
            {
                Mod.settings.EditorPanelPosition = position;
                Mod.settings.ApplyAndSave();
            }
        }
        private void UpdateTerrainElevation(float3 terrainHitPosition)
        {
            if (terrainHitPosition.y != 0)
            {
                m_TerrainElevation.Update(terrainHitPosition.y - m_WaterSystem.SeaLevel);
            }
            else
            {
                m_TerrainElevation.Update(0);
            }
        }
    }
}
