﻿using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
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
                bool Predicate(Thing x)
                {
                    return !((Building_CryptosleepCasket) x).HasAnyContents && p.CanReserve(x);
                }

                var building_CryptosleepCasket = (Building_CryptosleepCasket) GenClosest.ClosestThingReachable(
                    p.Position, p.Map, ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell,
                    TraverseParms.For(p), 9999f, Predicate);
                if (building_CryptosleepCasket != null)
                {
                    return building_CryptosleepCasket;
                }
            }

            enumerable = from def in DefDatabase<ThingDef>.AllDefs
                where typeof(Building_CryptosleepCasket).IsAssignableFrom(def.thingClass)
                select def;
            foreach (var singleDef2 in enumerable)
            {
                bool Predicate2(Thing x)
                {
                    return !((Building_CryptosleepCasket) x).HasAnyContents && p.CanReserve(x);
                }

                var building_CryptosleepCasket2 =
                    (Building_CryptosleepCasket) GenClosest.ClosestThingReachable
                    (p.Position, p.Map, ThingRequest.ForDef(singleDef2),
                        PathEndMode.InteractionCell, TraverseParms.For(p),
                        9999f, Predicate2);
                if (building_CryptosleepCasket2 != null)
                {
                    return building_CryptosleepCasket2;
                }
            }

            return null;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job result;
            if (!(t is Corpse corpse))
            {
                result = null;
            }
            else if (corpse.InnerPawn.Faction != Faction.OfPlayer)
            {
                result = null;
            }
            else if (corpse.IsNotFresh())
            {
                result = null;
            }
            else if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
            {
                result = null;
            }
            else
            {
                var building_CryptosleepCasket = FindCasket(pawn);
                if (building_CryptosleepCasket == null)
                {
                    result = null;
                }
                else if (building_CryptosleepCasket.ContainedThing != null)
                {
                    result = null;
                }
                else
                {
                    result = new Job(JobDefOfReconAndDiscovery.RD_TakeToOsirisCasket, t, building_CryptosleepCasket)
                    {
                        count = corpse.stackCount
                    };
                }
            }

            return result;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).Count == 0;
        }
    }
}