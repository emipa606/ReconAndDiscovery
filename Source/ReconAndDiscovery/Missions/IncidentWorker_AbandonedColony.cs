using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class IncidentWorker_AbandonedColony : IncidentWorker
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (parms.target is not Caravan caravan)
        {
            return false;
        }

        var site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
        TileFinder.TryFindPassableTileWithTraversalDistance(caravan.Tile, 1, 2, out var tile,
            t => !Find.WorldObjects.AnyMapParentAt(t));
        site.Tile = tile;
        var faction = Find.FactionManager.RandomEnemyFaction(true, false, true, TechLevel.Spacer);
        site.SetFaction(faction);

        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedColony,
            SiteDefOfReconAndDiscovery.RD_AbandonedColony.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));

        var holoDisk = new SitePart(site, SiteDefOfReconAndDiscovery.RD_HoloDisk,
            SiteDefOfReconAndDiscovery.RD_HoloDisk.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
        {
            hidden = true
        };
        site.parts.Add(holoDisk);
        if (Rand.Value < 0.3f)
        {
            var scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(scatteredManhunters);
        }

        if (Rand.Value < 0.1f)
        {
            var mechanoidForces = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MechanoidForces,
                SiteDefOfReconAndDiscovery.RD_MechanoidForces.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(mechanoidForces);
        }

        if (Rand.Value < 0.05f)
        {
            var stargate = new SitePart(site, SiteDefOfReconAndDiscovery.RD_Stargate,
                SiteDefOfReconAndDiscovery.RD_Stargate.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };

            site.parts.Add(stargate);
        }

        Find.WorldObjects.Add(site);
        SendStandardLetter(parms, caravan);

        return true;
    }
}