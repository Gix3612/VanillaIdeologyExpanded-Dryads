using RimWorld;
using Verse;
using Verse.AI;

namespace VanillaIdeologyExpanded_Dryads
{
  public class JobGiver_MergeIntoAwakeningCocoon : ThinkNode_JobGiver
  {
    protected override Job TryGiveJob(Pawn pawn)
    {
      if (!ModsConfig.IdeologyActive || pawn.connections == null || pawn.connections.ConnectedThings.NullOrEmpty())
      {
        return null;
      }

      foreach (Thing connectedThing in pawn.connections.ConnectedThings)
      {
        CompTreeConnection compTreeConnection = connectedThing.TryGetComp<CompTreeConnection>();
        InternalTreeConnection internalTreeComp = connectedThing.TryGetComp<InternalTreeConnection>();

        if (compTreeConnection != null && internalTreeComp != null && internalTreeComp.ShouldEnterAwakeningCocoon(pawn) && pawn.CanReach(internalTreeComp.awakeningCocoon, PathEndMode.Touch, Danger.Deadly))
        {
          return JobMaker.MakeJob(InternalDefOf.VDE_MergeIntoAwakeningCocoon, connectedThing, internalTreeComp.awakeningCocoon);
        }
      }

      return null;
    }

  }
}
