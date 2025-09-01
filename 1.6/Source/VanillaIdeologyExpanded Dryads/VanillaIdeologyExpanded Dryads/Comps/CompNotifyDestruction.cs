
using Verse;
using VEF.AnimalBehaviours;

namespace VanillaIdeologyExpanded_Dryads
{
    class CompNotifyDestruction : ThingComp
    {


        public CompProperties_NotifyDestruction Props
        {
            get
            {
                return (CompProperties_NotifyDestruction)this.props;
            }
        }

      

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            NotifyDestructionToComps(map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            NotifyDestructionToComps(previousMap);
        }

        public void NotifyDestructionToComps(Map map)
        {
            foreach (Pawn p in map.mapPawns.SpawnedColonyAnimals)
            {
                CompBuildPeriodically comp = p.TryGetComp<CompBuildPeriodically>();
                if (comp != null)
                {
                    comp.NotifyBuildingDestroyed(this.parent);
                }
            }

        }
    }
}
