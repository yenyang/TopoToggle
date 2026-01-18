// <copyright file="UndergroundPassExecutePatch.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Game.Rendering;
using HarmonyLib;
using TopoToggle.Extensions;
using TopoToggle.Systems;
using Unity.Entities;

namespace TopoToggle.Patches
{
    /// <summary>
    /// This is another point of injection to intercept contour lines. It's being added for redundancy to ensure proper functioning.
    /// </summary>
    /// 
    [HarmonyPatch(typeof(UndergroundPass), "Execute")]
    public class UndergroundPassExecutePatch
    {
        public static void Prefix(UnityEngine.Rendering.HighDefinition.CustomPassContext ctx)
        {
            UndergroundViewSystem undergroundViewSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<UndergroundViewSystem>();
            TopoToggleUISystem topoToggleUISystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TopoToggleUISystem>();
            if (undergroundViewSystem is null ||
                topoToggleUISystem is null)
            {
                return;
            }

            if (undergroundViewSystem.contourLinesOn != topoToggleUISystem.ForceContourLines &&
                !topoToggleUISystem.IsPlatterPrefabActive())
            {
                undergroundViewSystem.SetMemberValue("contourLinesOn", topoToggleUISystem.ForceContourLines);
            }

            return;
        }
    }
}
