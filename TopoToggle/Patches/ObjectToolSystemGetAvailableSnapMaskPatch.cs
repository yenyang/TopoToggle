// <copyright file="ObjectToolSystemGetAvailableSnapMaskPatch.cs" company="Yenyang's Mods.">
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
    [HarmonyPatch(typeof(ObjectToolSystem), nameof(ObjectToolSystem.GetAvailableSnapMask),
             new Type[] {typeof(PlaceableObjectData), typeof(bool), typeof(bool), typeof(bool), typeof(ObjectToolSystem.Mode), typeof(Snap), typeof(Snap) },
             new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
    public class ObjectToolSystemGetAvailableSnapMaskPatch
    {
        private static void Postfix(PlaceableObjectData prefabPlaceableData, bool editorMode, bool isBuilding, bool isAssetStamp, ObjectToolSystem.Mode mode, ref Snap onMask, ref Snap offMask)
        {
            onMask &= ~Snap.ContourLines;
            offMask &= ~Snap.ContourLines;
        }
    }
}
