using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VanillaIdeologyExpanded_Dryads
{
  [StaticConstructorOnStartup]
  public static class HarmonyPatches
  {
    static HarmonyPatches()
    {
      Harmony harmony = new Harmony("VanillaExpanded.Dryads");
      harmony.PatchAll();
    }
  }

  public class InternalTreeConnection : ThingComp
  {
    public Thing awakeningCocoon;

    public bool enableAwakening;

    private CompTreeConnection cachedTreeComp;

    CompTreeConnection TreeComp
    {
      get
      {
        if (cachedTreeComp == null)
        {
          cachedTreeComp = parent.TryGetComp<CompTreeConnection>();
        }
        return cachedTreeComp;
      }
    }

    public new CompProperties_InternalTreeConnection InternalProps => (CompProperties_InternalTreeConnection)props;

    public bool ShouldEnterAwakeningCocoon(Pawn dryad)
    {
      var dryads = Traverse.Create(TreeComp).Field("dryads").GetValue<List<Pawn>>();
      var tmpDryads = Traverse.Create(TreeComp).Field("tmpDryads").GetValue<List<Pawn>>();

      if (awakeningCocoon == null || awakeningCocoon.Destroyed)
      {
        return false;
      }

      if (dryads.NullOrEmpty() || dryads.Count < 3 || !dryads.Contains(dryad))
      {
        return false;
      }

      tmpDryads.Clear();
      for (int i = 0; i < dryads.Count; i++)
      {
        if (dryads[i].kindDef != PawnKindDefOf.Dryad_Gaumaker && dryads[i].kindDef != null)
        {
          tmpDryads.Add(dryads[i]);
        }
      }

      if (tmpDryads.Count < 3)
      {
        tmpDryads.Clear();
        return false;
      }

      tmpDryads.SortBy((Pawn x) => -x.ageTracker.AgeChronologicalTicks);
      for (int j = 0; j < 3; j++)
      {
        if (tmpDryads[j] == dryad)
        {
          tmpDryads.Clear();
          return true;
        }
      }

      tmpDryads.Clear();
      return false;
    }

    public Pawn GenerateAwakenedDryad(PawnKindDef dryadCaste)
    {
      Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(dryadCaste, TreeComp.ConnectedPawn?.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, Gender.Male, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn));
      foreach (TrainableDef allDef in DefDatabase<TrainableDef>.AllDefs)
      {
        if (pawn.training.CanAssignToTrain(allDef).Accepted)
        {
          pawn.training.SetWantedRecursive(allDef, checkOn: true);
          pawn.training.Train(allDef, TreeComp.ConnectedPawn, complete: true);
          if (allDef == TrainableDefOf.Release)
          {
            pawn.playerSettings.followDrafted = true;
          }
        }
      }
      pawn.connections?.ConnectTo(parent);
      return pawn;
    }

    public override string CompInspectStringExtra()
    {
      string text = base.CompInspectStringExtra();
      if (!text.NullOrEmpty())
      {
        text += "\n";
      }

      if (TreeComp.Mode != null && TreeComp.Mode != GauranlenTreeModeDefOf.Gaumaker && TreeComp.MaxDryads < 3 && enableAwakening)
      {
        text += "VDE_ConnectionStrengthTooWeakForAwakeningCocoon".Translate().Colorize(ColorLibrary.RedReadable);
      }

      return text;
    }


    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
      if (TreeComp.Mode != null && TreeComp.Mode != GauranlenTreeModeDefOf.Gaumaker)
      {
        yield return new Command_Toggle
        {
          defaultLabel = "VDE_EnableAwakening".Translate(),
          defaultDesc = "VDE_EnableAwakeningDesc".Translate(),
          icon = (ContentFinder<Texture2D>.Get(string.Concat("Things/Building/" + TreeComp.Mode.defName + "_Awaken"))),
          isActive = () => enableAwakening,
          toggleAction = delegate
          {
            enableAwakening = !enableAwakening;
          }
        };
      }
    }

  }
}
