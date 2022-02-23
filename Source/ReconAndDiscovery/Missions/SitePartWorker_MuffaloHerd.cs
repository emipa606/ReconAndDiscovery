using System;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class SitePartWorker_MuffaloHerd : SitePartWorker
    {
        private void QueueFactionArrival(Faction faction, Map map)
        {
            //TODO: check if it works
            var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.FactionArrival, map);
            incidentParms.forced = true;
            if (RCellFinder.TryFindRandomPawnEntryCell(out var spawnCenter, map, 0f, false, v => v.Standable(map)))
            {
                incidentParms.spawnCenter = spawnCenter;
            }

            if (!CellFinder.TryFindRandomEdgeCellWith(c => map.reachability.CanReachColony(c), map,
                CellFinder.EdgeRoadChance_Neutral, out var spawnCenter2))
            {
                return;
            }

            incidentParms.faction = faction;
            incidentParms.spawnCenter = spawnCenter2;
            incidentParms.points *= 4f;
            incidentParms.points = Math.Max(incidentParms.points, 250f);
            var qi = new QueuedIncident(new FiringIncident(IncidentDefOf.TravelerGroup, null, incidentParms),
                Find.TickManager.TicksGame + Rand.RangeInclusive(5000, 15000));
            Find.Storyteller.incidentQueue.Add(qi);
        }

        private void QueueArrivals(Map map)
        {
            var list = (from f in Find.FactionManager.AllFactionsVisible
                where f != Faction.OfPlayer && f.def.techLevel == TechLevel.Neolithic
                select f).ToList();
            if (list.Count == 0)
            {
                return;
            }

            var num = Rand.RangeInclusive(1, list.Count);
            list.Shuffle();
            for (var i = 0; i < num; i++)
            {
                var faction = list[i];
                QueueFactionArrival(faction, map);
            }
        }

        private void MakeMuffalo(Map map)
        {
            var num = Rand.RangeInclusive(80, 150);
            for (var i = 0; i < num; i++)
            {
                var loc = CellFinderLoose.RandomCellWith(c => c.Standable(map) && !c.Fogged(map), map);
                var pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Muffalo"));
                if (Rand.Value < 0.3)
                {
                    pawn.ageTracker.AgeBiologicalTicks = Rand.RangeInclusive(1000, 900000);
                }

                GenSpawn.Spawn(pawn, loc, map);
            }
        }
        
        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);

            MakeMuffalo(map);
            QueueArrivals(map);
            if (!Rand.Chance(0.05f))
            {
                return;
            }

            var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.FactionArrival, map);
            incidentParms.forced = true;
            incidentParms.points = 100f;

            var madMuffaloWarningIncident = new QueuedIncident( 
                new FiringIncident(IncidentDef.Named("RD_MuffaloMassInsanityWarning"), null, incidentParms),
                Find.TickManager.TicksGame + 10
            );
            Find.Storyteller.incidentQueue.Add(madMuffaloWarningIncident);

            var madMuffaloIncident = new QueuedIncident(
                new FiringIncident(IncidentDef.Named("RD_MuffaloMassInsanity"), null, incidentParms),
                Find.TickManager.TicksGame + Rand.RangeInclusive(10000, 45000)
            );
            
            Find.Storyteller.incidentQueue.Add(madMuffaloIncident);
        }
    }
}