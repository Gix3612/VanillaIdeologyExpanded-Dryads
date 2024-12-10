using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VanillaIdeologyExpanded_Dryads
{
  public class JobGiver_Clean : ThinkNode_JobGiver
  {
    private readonly int MinTicksSinceThickened = 600;

    public PathEndMode PathEndMode => PathEndMode.Touch;

    public ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Filth);

    public int MaxRegionsToScanBeforeGlobalSearch => 4;

    public IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => pawn.Map.listerFilthInHomeArea.FilthInHomeArea;

    public bool ShouldSkip(Pawn pawn) => pawn.Map.listerFilthInHomeArea.FilthInHomeArea.Count == 0;

    public bool HasJobOnThing(Pawn pawn, Thing t)
    {
      if (!(t is Filth filth) || !filth.Map.areaManager.Home[filth.Position] || !pawn.CanReserve(t, 1, -1, null) || filth.TicksSinceThickened < MinTicksSinceThickened)
      {
        return false;
      }

      return true;
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
      if (ShouldSkip(pawn))
      {
        return null;
      }

      bool predicate(Thing x) => x.def.category == ThingCategory.Filth && HasJobOnThing(pawn, x);
      Thing t = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Filth), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn), 100f, predicate, PotentialWorkThingsGlobal(pawn));
      if (t is null)
      {
        return null;
      }

      Job job = JobMaker.MakeJob(JobDefOf.Clean);
      job.AddQueuedTarget(TargetIndex.A, t);
      int num = 15;
      Map map = t.Map;
      Room room = t.GetRoom();
      for (int i = 0; i < 100; i++)
      {
        IntVec3 c2 = t.Position + GenRadial.RadialPattern[i];
        if (!ShouldClean(c2))
        {
          continue;
        }
        List<Thing> thingList = c2.GetThingList(map);
        for (int j = 0; j < thingList.Count; j++)
        {
          Thing thing = thingList[j];
          if (HasJobOnThing(pawn, thing) && thing != t)
          {
            job.AddQueuedTarget(TargetIndex.A, thing);
          }
        }
        if (job.GetTargetQueue(TargetIndex.A).Count >= num)
        {
          break;
        }
      }

      if (job.targetQueueA != null && job.targetQueueA.Count >= 5)
      {
        job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
      }

      return job;

      bool ShouldClean(IntVec3 c)
      {
        if (!c.InBounds(map))
        {
          return false;
        }

        Room room2 = c.GetRoom(map);
        if (room == room2)
        {
          return true;
        }

        Region region = c.GetDoor(map)?.GetRegion(RegionType.Portal);
        if (region != null && !region.links.NullOrEmpty())
        {
          for (int k = 0; k < region.links.Count; k++)
          {
            RegionLink regionLink = region.links[k];
            for (int l = 0; l < 2; l++)
            {
              if (regionLink.regions[l] != null && regionLink.regions[l] != region && regionLink.regions[l].valid && regionLink.regions[l].Room == room)
              {
                return true;
              }
            }
          }
        }

        return false;
      }

    }
  }
}
