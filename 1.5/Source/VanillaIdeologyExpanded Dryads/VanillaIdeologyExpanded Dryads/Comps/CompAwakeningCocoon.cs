using RimWorld;
using Verse;
using Verse.Sound;

namespace VanillaIdeologyExpanded_Dryads
{
  public class CompAwakeningCocoon : CompDryadHolder
  {
    private PawnKindDef dryadKind;

    public bool Full => innerContainer.Count >= 3;

    private InternalTreeConnection cachedInternalTreeComp;

    protected InternalTreeConnection InternalTreeComp
    {
      get
      {
        if (cachedInternalTreeComp == null)
        {
          cachedInternalTreeComp = tree?.TryGetComp<InternalTreeConnection>();
        }
        return cachedInternalTreeComp;
      }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
      if (!respawningAfterLoad)
      {
        innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
      }
    }

    public override void PostDeSpawn(Map map)
    {
      innerContainer.TryDropAll(parent.Position, map, ThingPlaceMode.Near, null, playDropSound: false);
      SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(parent);
    }

    public override void TryAcceptPawn(Pawn p)
    {
      base.TryAcceptPawn(p);
      p.Rotation = Rot4.South;
      dryadKind = base.TreeComp.DryadKind;
      if (Full)
      {
        _ = base.TreeComp.ConnectedPawn;
        tickComplete = Find.TickManager.TicksGame + (int)(60000f * base.Props.daysToComplete);
      }
    }

    protected override void Complete()
    {
      tickComplete = Find.TickManager.TicksGame;
      EffecterDefOf.DryadEmergeFromCocoon.Spawn(parent.Position, parent.Map).Cleanup();

      if (base.TreeComp != null && innerContainer.Count >= 3)
      {

        Pawn pawn2 = InternalTreeComp.GenerateAwakenedDryad(PawnKindDef.Named(string.Concat(dryadKind.defName + "_Awakened")));

        for (int num = innerContainer.Count - 1; num >= 0; num--)
        {
          if (innerContainer[num] is Pawn pawn)
          {
            base.TreeComp.RemoveDryad(pawn);
            pawn.Destroy();
          }
        }
        innerContainer.TryAddOrTransfer(pawn2, 1);
      }

      InternalTreeComp.enableAwakening = false;
      parent.Destroy();
      InternalTreeComp.awakeningCocoon = null;
    }

    public override void CompTick()
    {
      base.CompTick();
      if (!parent.Destroyed && tree != null)
      {
        if (dryadKind != null && dryadKind != base.TreeComp.DryadKind)
        {
          InternalTreeComp.enableAwakening = false;
          parent.Destroy();
          InternalTreeComp.awakeningCocoon = null;
        }
        else if (base.TreeComp.MaxDryads < 3)
        {
          InternalTreeComp.enableAwakening = false;
          parent.Destroy();
          InternalTreeComp.awakeningCocoon = null;
        }
      }
    }

    public override string CompInspectStringExtra()
    {
      string text = base.CompInspectStringExtra();
      if (innerContainer.Count >= 3)
      {
        if (!text.NullOrEmpty())
        {
          text += "\n";
        }
        text += "VDE_AwakeningDryad".Translate(GenLabel.BestKindLabel(dryadKind, Gender.Male, plural: true), NamedArgumentUtility.Named(PawnKindDef.Named(string.Concat(dryadKind.defName + "_Awakened")), "TYPE")).Resolve();
      }
      return text;
    }

    public override void PostExposeData()
    {
      Scribe_Defs.Look(ref dryadKind, "dryadKind");
    }

  }
}
