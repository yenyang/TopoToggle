// <copyright file="RaycastSystem.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>


using Colossal.Serialization.Entities;
using Game;
using Game.Common;
using Game.Tools;
using TopoToggle.Raycast;
using Unity.Mathematics;

namespace TopoToggle.Systems
{
    internal partial class TopoToggleRaycastSystem : GameSystemBase
    {
        private ToolRaycastSystem m_ToolRaycastSystem;
        private RaycastTerrain m_RaycastTerrain;

        public float3 TerrainHitPosition
        {
            get
            {
                if (m_RaycastTerrain is not null &&
                    Mod.settings.ShowTerrainHitPosition &&
                    (m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
                {
                    return m_RaycastTerrain.HitPosition;
                }

                return new float3();
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            // System instances.
            m_ToolRaycastSystem = World.GetOrCreateSystemManaged<ToolRaycastSystem>();

            Mod.log.Info($"{nameof(TopoToggleRaycastSystem)}.{nameof(OnCreate)}");
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            if (Mod.settings.ShowTerrainHitPosition &&
                (m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
            {
                m_RaycastTerrain = new RaycastTerrain(World);
            }
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            Enabled = mode.IsGameOrEditor();
        }
    }
}
