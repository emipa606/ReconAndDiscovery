using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class IncidentWorker_FaultyGenerator : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (parms.target is not Map map)
        {
            return false;
        }

        if ((from wo in Find.WorldObjects.Sites
                where wo != null && wo.parts.Select(x => x.def == SiteDefOfReconAndDiscovery.RD_QuakesQuest).Any()
                select wo).Any())
        {
            return false;
        }

        if (!TileFinder.TryFindNewSiteTile(out var tile))
        {
            return false;
        }

        var site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery
            .RD_AdventureDestroyThing);
        site.Tile = tile;
        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_QuakesQuest,
            SiteDefOfReconAndDiscovery.RD_QuakesQuest.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null)));
        var faultyGenerator = new SitePart(site, SiteDefOfReconAndDiscovery.RD_SitePart_FaultyGenerator,
            SiteDefOfReconAndDiscovery.RD_SitePart_FaultyGenerator.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
        {
            hidden = true
        };
        site.parts.Add(faultyGenerator);
        site.GetComponent<QuestComp_DestroyThing>().StartQuest(ThingDefOf.GeothermalGenerator);
        site.GetComponent<QuestComp_DestroyThing>().gameConditionCaused =
            GameConditionDef.Named("RD_Tremors");
        site.GetComponent<QuestComp_DestroyThing>().worldTileAffected = map.Tile;
        if (Rand.Value < 0.05f)
        {
            var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
            {
                hidden = true
            };
            site.parts.Add(scatteredTreasure);
        }

        if (Rand.Value < 0.1f)
        {
            var scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
            {
                hidden = true
            };
            site.parts.Add(scatteredManhunters);
        }

        if (Rand.Value < 0.1f)
        {
            var mechanoidForces = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MechanoidForces,
                SiteDefOfReconAndDiscovery.RD_MechanoidForces.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
            {
                hidden = true
            };
            site.parts.Add(mechanoidForces);
        }

        if (Rand.Value < 0.05f)
        {
            var enemyRaidOnArrival = new SitePart(site, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival,
                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
            {
                hidden = true
            };
            site.parts.Add(enemyRaidOnArrival);
        }

        var num = 30;
        var gameCondition =
            GameConditionMaker.MakeCondition(GameConditionDef.Named("RD_Tremors"), 60000 * num);
        map.gameConditionManager.RegisterCondition(gameCondition);
        site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
        SendStandardLetter(parms, site);
        Find.WorldObjects.Add(site);
        return true;
    }
}