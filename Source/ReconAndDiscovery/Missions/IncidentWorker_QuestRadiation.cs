using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class IncidentWorker_QuestRadiation : IncidentWorker
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

        if ((from worldObjects in Find.WorldObjects.AllWorldObjects
                where worldObjects is Site site1 &&
                      site1.parts.Select(x => x.def == SiteDefOfReconAndDiscovery.RD_QuakesQuest).Any()
                select worldObjects).Any())
        {
            return false;
        }

        if (!TileFinder.TryFindNewSiteTile(out var tile))
        {
            return false;
        }

        var site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery
            .RD_AdventureThingCounter);
        site.Tile = tile;
        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_SiteRadiationQuest,
            SiteDefOfReconAndDiscovery.RD_SiteRadiationQuest.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null)));
        var radioactiveDust = new SitePart(site, SiteDefOfReconAndDiscovery.RD_SitePart_RadioactiveDust,
            SiteDefOfReconAndDiscovery.RD_SitePart_RadioactiveDust.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
        {
            hidden = true
        };
        site.parts.Add(radioactiveDust);
        var component = site.GetComponent<QuestComp_CountThings>();
        component.targetNumber = 200;
        component.ticksTarget = 60000;
        component.ticksHeld = 0;
        component.worldTileAffected = map.Tile;
        component.gameConditionCaused = GameConditionDef.Named("RD_Radiation");
        component.StartQuest(ThingDef.Named("Plant_Psychoid"));
        if (Rand.Value < 0.1f)
        {
            var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
            {
                hidden = true
            };
            site.parts.Add(scatteredTreasure);
        }

        if (Rand.Value < 0.05f)
        {
            var scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
            {
                hidden = true
            };
            site.parts.Add(scatteredManhunters);
        }

        if (Rand.Value < 0.05f)
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

        if (Find.World.tileTemperatures.GetSeasonalTemp(site.Tile) < 10f ||
            Find.World.tileTemperatures.GetSeasonalTemp(site.Tile) > 40f)
        {
            return false;
        }

        var num = 30;
        var gameCondition =
            GameConditionMaker.MakeCondition(GameConditionDef.Named("RD_Radiation"), 60000 * num);
        map.gameConditionManager.RegisterCondition(gameCondition);
        site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
        SendStandardLetter(parms, site);
        Find.WorldObjects.Add(site);

        return true;
    }
}