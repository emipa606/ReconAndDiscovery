﻿using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
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
                bool Predicate(Thing x)
                {
                    return ((HoloEmitter) x).GetComp<CompHoloEmitter>().SimPawn == null && p.CanReserve(x);
                }

                var holoEmitter = (HoloEmitter) GenClosest.ClosestThingReachable(p.Position, p.Map,
                    ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell, TraverseParms.For(p), 9999f,
                    Predicate);
                if (holoEmitter != null)
                {
                    return holoEmitter;
                }
            }

            return null;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job result;
            if (t is Corpse corpse)
            {
                if (corpse.InnerPawn.Faction != Faction.OfPlayer || !corpse.InnerPawn.RaceProps.Humanlike)
                {
                    result = null;
                }
                else if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
                {
                    result = null;
                }
                else
                {
                    var holoEmitter = FindEmitter(pawn);
                    if (holoEmitter == null)
                    {
                        result = null;
                    }
                    else if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn != null)
                    {
                        result = null;
                    }
                    else
                    {
                        result = new Job(JobDefOfReconAndDiscovery.RD_ScanAtEmitter, t, holoEmitter)
                        {
                            count = corpse.stackCount
                        };
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).Count == 0;
        }
    }
}