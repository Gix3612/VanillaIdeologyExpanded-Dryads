using System;
using RimWorld;
using Verse;

namespace VanillaIdeologyExpanded_Dryads
{
  public class HediffComp_PeriodicWounds : HediffComp
  {
    public int checkDownCounter = 0;

    private readonly Random rand = new Random();

    public HediffCompProperties_PeriodicWounds Props => (HediffCompProperties_PeriodicWounds)this.props;

    public override void CompPostTick(ref float severityAdjustment)
    {
      checkDownCounter++;

      if (parent.Severity > Props.severityThirdStage)
      {
        if (checkDownCounter > Props.mtbWoundsThirdStage)
        {
          if (rand.NextDouble() < Props.chanceCutThirdStage)
          {
            base.Pawn.TakeDamage(new DamageInfo(DamageDefOf.Cut, 2, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown));
          }
          checkDownCounter = 0;
        }
      }
      else if (parent.Severity > Props.severitySecondStage)
      {
        if (checkDownCounter > Props.mtbWoundsSecondStage)
        {
          if (rand.NextDouble() < Props.chanceCutSecondStage)
          {
            base.Pawn.TakeDamage(new DamageInfo(DamageDefOf.Cut, 1, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown));
          }
          checkDownCounter = 0;
        }
      }
    }

  }
}
