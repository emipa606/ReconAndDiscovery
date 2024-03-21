using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class WorkGiver_TakeToOsirisCasket : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Corpse);

    private Building_CryptosleepCasket FindCasket(Pawn p)
    {
        var enumerable = from def in DefDatabase<ThingDef>.AllDefs
            where typeof(OsirisCasket).IsAssignableFrom(def.thingClass)
            select def;
        foreach (var singleDef in enumerable)
        {
            var building_CryptosleepCasket = (Building_CryptosleepCasket)GenClosest.ClosestThingReachable(
                p.Position, p.Map, ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell,
                TraverseParms.For(p), 9999f, Predicate);
            if (building_CryptosleepCasket != null)
            {
                return building_CryptosleepCasket;
            }

            continue;

            bool Predicate(Thing x)
            {
                return !((Building_CryptosleepCasket)x).HasAnyContents && p.CanReserve(x);
            }
        }

        enumerable = from def in DefDatabase<ThingDef>.AllDefs
            where typeof(Building_CryptosleepCasket).IsAssignableFrom(def.thingClass)
            select def;
        foreach (var singleDef2 in enumerable)
        {
            var building_CryptosleepCasket2 =
                (Building_CryptosleepCasket)GenClosest.ClosestThingReachable
                (p.Position, p.Map, ThingRequest.ForDef(singleDef2),
                    PathEndMode.InteractionCell, TraverseParms.For(p),
                    9999f, Predicate2);
            if (building_CryptosleepCasket2 != null)
            {
                return building_CryptosleepCasket2;
            }

            continue;

            bool Predicate2(Thing x)
            {
                return !((Building_CryptosleepCasket)x).HasAnyContents && p.CanReserve(x);
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

        if (corpse.InnerPawn.Faction != Faction.OfPlayer)
        {
            return null;
        }

        if (corpse.IsNotFresh())
        {
            return null;
        }

        if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
        {
            return null;
        }

        var building_CryptosleepCasket = FindCasket(pawn);
        if (building_CryptosleepCasket == null)
        {
            return null;
        }

        if (building_CryptosleepCasket.ContainedThing != null)
        {
            return null;
        }

        return new Job(JobDefOfReconAndDiscovery.RD_TakeToOsirisCasket, t, building_CryptosleepCasket)
        {
            count = corpse.stackCount
        };
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).Count == 0;
    }
}