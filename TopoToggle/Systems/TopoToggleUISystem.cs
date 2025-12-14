// <copyright file="TopoToggleUISystem.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Prefabs;
using Game.Tools;
using Game.UI;
using System;
using System.Reflection;
using Unity.Entities;

namespace TopoToggle.Systems
{
    /// <summary>
    /// UI System for Topo Toggle to handle bindings between UI and C#.
    /// </summary>
    public partial class TopoToggleUISystem : UISystemBase
    {
        private ValueBinding<bool> m_ForceContourLines;
        private ValueBinding<bool> m_HideTopoTogglePanel;
        private bool m_FoundPlater;
        private ComponentType m_PlatterComponent;
        private ToolSystem m_ToolSystem;
        private PrefabSystem m_PrefabSystem;
        private ObjectToolSystem m_ObjectToolSystem;

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

        protected override void OnCreate()
        {
            base.OnCreate();

            // System instances.
            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_ObjectToolSystem = World.GetOrCreateSystemManaged<ObjectToolSystem>();

            // Event registration
            m_ToolSystem.EventToolChanged += OnToolChanged;
            m_ToolSystem.EventPrefabChanged += OnPrefabChanged;

            // These establish bindings of values to send to the UI.
            AddBinding(m_ForceContourLines = new ValueBinding<bool>(Mod.ID, "ForceContourLines", Mod.settings.ForceContourLines));
            AddBinding(m_HideTopoTogglePanel = new ValueBinding<bool>(Mod.ID, "HideTopoTogglePanel", Mod.settings.HidePanel));

            // These establish bindings to listen to from the UI.
            AddBinding(new TriggerBinding(Mod.ID, "ToggleContourLines", ToggleContourLines));

            Mod.log.Info($"{nameof(TopoToggleUISystem)}.{nameof(OnCreate)}");
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

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
    }
}
