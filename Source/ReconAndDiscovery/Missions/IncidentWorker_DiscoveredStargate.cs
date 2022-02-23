using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_DiscoveredStargate : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        protected List<SitePart> RandomizeSite(Site site, int tile, Faction faction)
        {
            // var randomChance = Rand.Value;
            var randomChance = .9;
            
            if (randomChance < 0.25)
            {
                return new List<SitePart> {
                    new SitePart(
                        site, 
                        SiteDefOfReconAndDiscovery.RD_AbandonedCastle,
                    SiteDefOfReconAndDiscovery.RD_AbandonedCastle.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    }
                };
            }

            if (randomChance < 0.5)
            {
                return new List<SitePart>
                {
                    new SitePart(site, 
                        SiteDefOfReconAndDiscovery.RD_AbandonedColony,
                        SiteDefOfReconAndDiscovery.RD_AbandonedColony.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    }
                };
            }

            if (randomChance < 0.75)
            {
                return new List<SitePart>
                {
                    new SitePart(site, 
                        SitePartDefOf.PreciousLump,
                        SitePartDefOf.PreciousLump.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    }
                };
            }


            return new List<SitePart> {
                new SitePart(site, 
                    SitePartDefOf.Outpost,
                    SitePartDefOf.Outpost.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                },
                new SitePart(site, 
                    SitePartDefOf.Turrets,
                    SitePartDefOf.Turrets.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                }
            };
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var tile = -1;
            var faction = Find.FactionManager.RandomEnemyFaction();
            for (var i = 0; i < 20; i++)
            {
                tile = TileFinder.RandomSettlementTileFor(faction);
                if (TileFinder.IsValidTileForNewSettlement(tile))
                {
                    break;
                }

                tile = -1;
            }

            if (tile == -1)
            {
                return false;
            }

            var site = (Site) WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
            site.Tile = tile;
            site.SetFaction(faction);
            var starGate = new SitePart(site, SiteDefOfReconAndDiscovery.RD_Stargate,
                SiteDefOfReconAndDiscovery.RD_Stargate.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
            site.AddPart(starGate);

            foreach (var sitePart in RandomizeSite(site, tile, faction))
            {
                site.AddPart(sitePart);
            }

            if (Rand.Value < 0.2f)
            {
                var scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                    SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };

                site.parts.Add(scatteredManhunters);
            }

            if (Rand.Value < 0.85f)
            {
                var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                    SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };

                site.parts.Add(scatteredTreasure);
            }

            site.GetComponent<TimeoutComp>().StartTimeout(10 * 60000);
            SendStandardLetter(parms, site);
            Find.WorldObjects.Add(site);
            return true;
        }
    }
}