
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VanillaIdeologyExpanded_Dryads
{
    public class DeathActionWorker_AcidExplosion : DeathActionWorker
    {



        public override void PawnDied(Corpse corpse, Lord lord)
        {
           



            GenExplosion.DoExplosion(corpse.Position, corpse.Map, 2.9f, InternalDefOf.VDE_AcidSpit, corpse.InnerPawn, 10, -1, InternalDefOf.VDE_GooPop, null, null, null, ThingDefOf.Filth_Slime, 1f, 1, null,false, null, 0f, 1);
        }


    }
}
