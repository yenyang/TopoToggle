// <copyright file="TerrainToolSystemGetAvailableSnapMaskPatch.cs" company="Yenyang's Mods.">
// Copyright (c) Yenyang's Mods. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Game.Tools;
using HarmonyLib;
using System;

namespace TopoToggle.Patches
{
    /// <summary>
    /// Patch to hide vanilla toggle for Contour lines.
    /// </summary>
    [HarmonyPatch(typeof(TerrainToolSystem), nameof(TerrainToolSystem.GetAvailableSnapMask),
             new Type[] { typeof(Snap), typeof(Snap) },
             new ArgumentType[] { ArgumentType.Out, ArgumentType.Out })]
    public class TerrainToolSystemGetAvailableSnapMaskPatch
    {
        private static void Postfix(ref Snap onMask, ref Snap offMask)
        {
            onMask &= ~Snap.ContourLines;
            offMask &= ~Snap.ContourLines;
        }
    }
}
