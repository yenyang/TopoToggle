using Game.Rendering;
using HarmonyLib;
using TopoToggle.Systems;
using Unity.Entities;

namespace TopoToggle.Patches
{
    /// <summary>
    /// This is another point of entry for controlling contour lines. Put in for redundancy.
    /// </summary>
    [HarmonyPatch(typeof(UndergroundViewSystem), "contourLinesOn")]
    public class ContourLinesOnPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(MethodType.Getter)]
        public static bool GetPrefix(ref bool __result)
        {
            TopoToggleUISystem topoToggleUISystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TopoToggleUISystem>();
            if (!topoToggleUISystem.IsPlatterPrefabActive())
            {
                __result = topoToggleUISystem.ForceContourLines;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(MethodType.Setter)]
        public static void SetPrefix(bool value)
        {
            TopoToggleUISystem topoToggleUISystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TopoToggleUISystem>();
            if (!topoToggleUISystem.IsPlatterPrefabActive())
            {
                value = topoToggleUISystem.ForceContourLines;
            }
        }
    }
}
