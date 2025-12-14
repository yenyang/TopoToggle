// <copyright file="NetToolSystemGetAvailableSnapMaskPatch.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Game.Prefabs;
using Game.Tools;
using HarmonyLib;
using System;

namespace TopoToggle.Patches
{
    /// <summary>
    /// Patch to hide vanilla toggle for Contour lines.
    /// </summary>
    [HarmonyPatch(typeof(NetToolSystem), nameof(NetToolSystem.GetAvailableSnapMask),
             new Type[] {typeof(NetGeometryData), typeof(PlaceableNetData), typeof(NetToolSystem.Mode), typeof(bool), typeof(bool), typeof(bool), typeof(Snap), typeof(Snap) },
             new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
    public class NetToolSystemGetAvailableSnapMaskPatch
    {
        private static void Postfix(NetGeometryData prefabGeometryData, PlaceableNetData placeableNetData, NetToolSystem.Mode mode, bool editorMode, bool laneContainer, bool underground, ref Snap onMask, ref Snap offMask)
        {
            onMask &= ~Snap.ContourLines;
            offMask &= ~Snap.ContourLines;
        }
    }
}
