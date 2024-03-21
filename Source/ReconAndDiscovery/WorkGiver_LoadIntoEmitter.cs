using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class WorkGiver_LoadIntoEmitter : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDef.Named("RD_HoloDisk"));

    private HoloEmitter FindEmitter(Pawn p)
    {
        var enumerable = from def in DefDatabase<ThingDef>.AllDefs
            where typeof(HoloEmitter).IsAssignableFrom(def.thingClass)
            select def;
        foreach (var singleDef in enumerable)
        {
            var holoEmitter = (HoloEmitter)GenClosest.ClosestThingReachable(p.Position, p.Map,
                ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell, TraverseParms.For(p), 9999f,
                Predicate);
            if (holoEmitter != null)
            {
                return holoEmitter;
            }

            continue;

            bool Predicate(Thing x)
            {
                return ((HoloEmitter)x).GetComp<CompHoloEmitter>().SimPawn == null && p.CanReserve(x);
            }
        }

        return null;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t.def.defName != "RD_HoloDisk")
        {
            return null;
        }

        if (!pawn.CanReserveAndReach(t, PathEndMode.Touch, Danger.Deadly, 1, 1))
        {
            return null;
        }

        var holoEmitter = FindEmitter(pawn);
        if (holoEmitter == null)
        {
            return null;
        }

        if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn != null)
        {
            return null;
        }

        return new Job(JobDefOfReconAndDiscovery.RD_LoadIntoEmitter, t, holoEmitter)
        {
            count = 1
        };
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("RD_HoloDisk")).Count == 0;
    }
}