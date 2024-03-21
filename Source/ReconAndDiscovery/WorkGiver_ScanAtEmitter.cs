using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class WorkGiver_ScanAtEmitter : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Corpse);

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
        if (t is not Corpse corpse)
        {
            return null;
        }

        if (corpse.InnerPawn.Faction != Faction.OfPlayer || !corpse.InnerPawn.RaceProps.Humanlike)
        {
            return null;
        }

        if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
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

        return new Job(JobDefOfReconAndDiscovery.RD_ScanAtEmitter, t, holoEmitter)
        {
            count = corpse.stackCount
        };
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).Count == 0;
    }
}