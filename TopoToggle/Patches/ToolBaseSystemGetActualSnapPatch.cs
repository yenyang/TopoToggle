// <copyright file="ToolBaseSystemGetActualSnapPatch.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Game.Tools;
using HarmonyLib;
using TopoToggle.Systems;
using Unity.Entities;

namespace TopoToggle.Patches
{
    /// <summary>
    /// Patch to control Contour line "Snapping".
    /// </summary>
    [HarmonyPatch(typeof(ToolBaseSystem))]
    [HarmonyPatch(nameof(ToolBaseSystem.GetActualSnap))]
    [HarmonyPatch(new[] { typeof(Snap), typeof(Snap), typeof(Snap) })]
    public class ToolBaseSystemGetActualSnapPatch
    {
        private static void Postfix(ToolBaseSystem __instance, ref Snap __result)
        {
            TopoToggleUISystem uiSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TopoToggleUISystem>();
            if (uiSystem.IsPlatterPrefabActive())
            {
                return;
            }

            if (uiSystem.ForceContourLines)
            {
                __result |= Snap.ContourLines;
            } else
            {
                __result &= ~Snap.ContourLines;
            }
        }
    }
}
