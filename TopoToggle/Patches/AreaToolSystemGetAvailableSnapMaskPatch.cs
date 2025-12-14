// <copyright file="AreaToolSystemGetAvailableSnapMaskPatch.cs" company="Yenyang's Mods.">
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
    [HarmonyPatch(typeof(AreaToolSystem), nameof(AreaToolSystem.GetAvailableSnapMask),
             new Type[] {typeof(AreaGeometryData), typeof(bool), typeof(Snap), typeof(Snap) },
             new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
    public class AreaToolSystemGetAvailableSnapMaskPatch
    {
        [HarmonyAfter(new string[] {"ExtraDetailingTools.EDT"})]
        private static void Postfix(AreaGeometryData prefabAreaData, bool editorMode, ref Snap onMask, ref Snap offMask)
        {
            onMask &= ~Snap.ContourLines;
            offMask &= ~Snap.ContourLines;
        }
    }
}
