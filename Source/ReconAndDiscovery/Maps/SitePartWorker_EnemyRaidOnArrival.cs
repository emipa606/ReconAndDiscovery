using System;
using System.Linq;
using ReconAndDiscovery.DefOfs;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_EnemyRaidOnArrival : SitePartWorker
{
    public override void PostMapGenerate(Map map)
    {
        var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategories.FactionArrival, map);
        incidentParms.forced = true;
        if (RCellFinder.TryFindRandomPawnEntryCell(out var spawnCenter, map, 0f, false, v => v.Standable(map)))
        {
            incidentParms.spawnCenter = spawnCenter;
        }

        if (!(from f in Find.FactionManager.AllFactions
                where !f.def.hidden && f.HostileTo(Faction.OfPlayer)
                select f).TryRandomElement(out var faction))
        {
            return;
        }

        if (!CellFinder.TryFindRandomEdgeCellWith(c => map.reachability.CanReachColony(c), map,
                CellFinder.EdgeRoadChance_Neutral, out var spawnCenter2))
        {
            return;
        }

        incidentParms.faction = faction;
        incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
        incidentParms.spawnCenter = spawnCenter2;
        incidentParms.points *= 20f;
        incidentParms.points = Math.Max(incidentParms.points, 250f);
        var qi = new QueuedIncident(
            new FiringIncident(ThingDefOfReconAndDiscovery.RD_RaidEnemyQuest, null, incidentParms),
            Find.TickManager.TicksGame + Rand.RangeInclusive(5000, 15000));
        Find.Storyteller.incidentQueue.Add(qi);
    }
}