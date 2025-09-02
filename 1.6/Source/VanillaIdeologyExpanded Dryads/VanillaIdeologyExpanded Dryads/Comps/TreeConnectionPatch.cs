using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VanillaIdeologyExpanded_Dryads
{
  [StaticConstructorOnStartup, HarmonyPatch(typeof(CompTreeConnection))]
  public class Patch
  {
    [HarmonyPostfix, HarmonyPatch("CompTick")]
    public static void PostFix1(CompTreeConnection __instance, ref List<Pawn> ___dryads)
    {
      if (__instance.Mode == null) { return; }

      InternalTreeConnection internalTreeComp = __instance.ConnectedPawn.connections.ConnectedThings[0].TryGetComp<InternalTreeConnection>();

      if (__instance.Mode != GauranlenTreeModeDefOf.Gaumaker && __instance.Mode != null && ___dryads.Count >= 3 && internalTreeComp.enableAwakening)
      {
        if (internalTreeComp.awakeningCocoon == null && TryGetGaumakerCell(__instance, out var cell))
        {
          internalTreeComp.awakeningCocoon = GenSpawn.Spawn(InternalDefOf.VDE_AwakeningCocoon, cell, __instance.parent.Map);
        }
      }
      else if (internalTreeComp.awakeningCocoon != null && !internalTreeComp.awakeningCocoon.Destroyed)
      {
        internalTreeComp.enableAwakening = false;
        internalTreeComp.awakeningCocoon.Destroy();
        internalTreeComp.awakeningCocoon = null;
      }
    }

    public static bool TryGetGaumakerCell(CompTreeConnection instance, out IntVec3 cell)
    {
      cell = IntVec3.Invalid;
      if (CellFinder.TryFindRandomCellNear(instance.parent.Position, instance.parent.Map, 3, (IntVec3 c) => GauranlenUtility.CocoonAndPodCellValidator(c, instance.parent.Map, ThingDefOf.Plant_PodGauranlen), out cell) || CellFinder.TryFindRandomCellNear(instance.parent.Position, instance.parent.Map, 3, (IntVec3 c) => GauranlenUtility.CocoonAndPodCellValidator(c, instance.parent.Map, ThingDefOf.Plant_TreeGauranlen), out cell))
      {
        return true;
      }

      return false;
    }

  }
}
