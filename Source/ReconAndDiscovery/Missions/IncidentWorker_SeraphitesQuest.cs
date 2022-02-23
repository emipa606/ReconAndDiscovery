using System.Linq;
using ReconAndDiscovery.Maps;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class IncidentWorker_SeraphitesQuest : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
    }

    private bool CanFindVisitor(Map map, out Pawn pawn)
    {
        pawn = null;
        var source = from p in map.mapPawns.AllPawnsSpawned
            where p.RaceProps.Humanlike && !p.Faction.HostileTo(Faction.OfPlayer) && p.Faction != Faction.OfPlayer
            select p;
        if (!source.Any())
        {
            return false;
        }

        pawn = source.RandomElement();

        return true;
    }

    private bool CanFindLuciferiumAddict(Map map, out Pawn pawn)
    {
        pawn = null;
        foreach (var pawn2 in map.mapPawns.FreeColonists)
        {
            if (!AddictionUtility.IsAddicted(pawn2, ThingDefOfReconAndDiscovery.Luciferium))
            {
                continue;
            }

            pawn = pawn2;
            return true;
        }

        return false;
    }

    private bool GetHasGoodStoryConditions(Map map)
    {
        if (map == null)
        {
            return false;
        }

        if (!CanFindLuciferiumAddict(map, out _))
        {
            return false;
        }

        if (CanFindVisitor(map, out _))
        {
            return true;
        }

        return false;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = parms.target as Map;
        if (!GetHasGoodStoryConditions(map))
        {
            return false;
        }

        if (!CanFindVisitor(map, out var pawn))
        {
            return false;
        }

        if (!CanFindLuciferiumAddict(map, out var pawn2))
        {
            return false;
        }

        if (!TileFinder.TryFindNewSiteTile(out var tile))
        {
            return false;
        }

        var site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
        site.Tile = tile;
        var faction = Faction.OfInsects;
        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_SeraphitesQuest,
            SiteDefOfReconAndDiscovery.RD_SeraphitesQuest.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));
        var sitePart_Computer = new SitePart(site, SiteDefOfReconAndDiscovery.RD_SitePart_Computer,
            SiteDefOfReconAndDiscovery.RD_SitePart_Computer.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
        {
            hidden = true
        };
        site.parts.Add(sitePart_Computer);
        foreach (var sitePartDef in site.parts.Select(x => x.def))
        {
            if (sitePartDef.Worker is SitePartWorker_Computer computer)
            {
                computer.action =
                    ActionDefOfReconAndDiscovery.RD_ActionSeraphites;
            }
        }

        if (Rand.Value < 0.15f)
        {
            var scatteredManhunters = new SitePart(site,
                SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(scatteredManhunters);
        }

        if (Rand.Value < 0.3f)
        {
            var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(scatteredTreasure);
        }

        if (Rand.Value < 0.1f)
        {
            var enemyRaidOnArrival = new SitePart(site,
                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival,
                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(enemyRaidOnArrival);
        }

        if (Rand.Value < 0.1f)
        {
            var mechanoidForces = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MechanoidForces,
                SiteDefOfReconAndDiscovery.RD_MechanoidForces.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(mechanoidForces);
        }

        SendStandardLetter(parms, site, pawn.Label, pawn2.Label);
        Find.WorldObjects.Add(site);

        return true;
    }
}