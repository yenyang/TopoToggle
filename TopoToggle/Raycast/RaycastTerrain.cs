// <copyright file="RaycastTerrain.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Game.Common;
using Unity.Entities;

namespace TopoToggle.Raycast
{
    internal class RaycastTerrain : RaycastBase
    {
        internal RaycastTerrain(World gameWorld) : base(gameWorld)
        { }

        protected override RaycastInput GetInput()
        {
            RaycastInput result = default;
            result.m_Line = Line;
            result.m_Offset = default;
            result.m_TypeMask = TypeMask.Terrain;
            return result;
        }
    }
}
