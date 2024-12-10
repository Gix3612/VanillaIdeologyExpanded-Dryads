using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VanillaIdeologyExpanded_Dryads
{
  public class JobDriver_MergeIntoAwakeningCocoon : JobDriver
  {
    private const int WaitTicks = 120;

    private CompAwakeningCocoon AwakeningCocoon => job.targetB.Thing.TryGetComp<CompAwakeningCocoon>();

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
      return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
      this.FailOnDespawnedOrNull(TargetIndex.A);
      this.FailOnDespawnedOrNull(TargetIndex.B);
      this.FailOn(() => AwakeningCocoon.Full);
      yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
      yield return Toils_General.WaitWith(TargetIndex.B, WaitTicks, useProgressBar: true);
      yield return Toils_General.Do(delegate
      {
        AwakeningCocoon.TryAcceptPawn(pawn);
      });
    }

  }
}
