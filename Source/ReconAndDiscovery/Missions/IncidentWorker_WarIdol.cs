using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class IncidentWorker_WarIdol : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
    }

    private bool CanFindVisitor(Map map, out Pawn pawn)
    {
        pawn = null;
        var source = from p in map.mapPawns.AllPawnsSpawned
            where p.RaceProps.Humanlike && p.Faction != Faction.OfPlayer
            select p;
        bool result;
        if (!source.Any())
        {
            result = false;
        }
        else
        {
            pawn = source.RandomElement();
            result = true;
        }

        return result;
    }

    private bool CanFindPsychic(Map map, out Pawn pawn)
    {
        pawn = null;
        foreach (var pawn2 in map.mapPawns.FreeColonists)
        {
            if (!pawn2.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity")))
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

        if (!CanFindPsychic(map, out _))
        {
            return false;
        }

        return CanFindVisitor(map, out _);
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

        if (!CanFindPsychic(map, out var pawn2))
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
        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_PsiMachine,
            SiteDefOfReconAndDiscovery.RD_PsiMachine.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));

        var warIdol = new SitePart(site, SiteDefOfReconAndDiscovery.RD_SitePart_WarIdol,
            SiteDefOfReconAndDiscovery.RD_SitePart_WarIdol.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
        {
            hidden = true
        };

        site.parts.Add(warIdol);
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

        if (Rand.Value < 0.3f)
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

        Find.WorldObjects.Add(site);

        SendStandardLetter(parms, site, pawn.Label, pawn2.Label);
        return true;
    }
}