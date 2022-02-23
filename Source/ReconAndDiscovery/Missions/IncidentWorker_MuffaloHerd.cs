using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class IncidentWorker_MuffaloHerd : IncidentWorker
{
    private static readonly IntRange TimeoutDaysRange = new IntRange(7, 12);

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var num = TimeoutDaysRange.RandomInRange;
        int num2;
        string text;
        if (parms.target is Map)
        {
            if (!TileFinder.TryFindNewSiteTile(out num2))
            {
                return false;
            }

            text = "RD_LargeMuffaloMigration".Translate()

//A large muffalo migration is due to pass near here in
                   + " {0} " + "Days".Translate(); //days!
        }
        else
        {
            var list = (from c in Find.World.worldObjects.Caravans
                where c.Faction == Faction.OfPlayer
                select c).ToList();
            if (list.Count == 0)
            {
                return false;
            }

            var caravan = list.RandomElement();
            num -= 3;
            TileFinder.TryFindPassableTileWithTraversalDistance(caravan.Tile, 1, 2, out num2,
                t => !Find.WorldObjects.AnyMapParentAt(t));
            if (num2 == 0 || num2 == -1)
            {
                return false;
            }

            text = "RD_YourCaravanSpottedMuffaloMigration"
                .Translate(); //"Your caravan has spotted a huge muffalo migration!";
        }

        if (num2 is 0 or -1)
        {
            return false;
        }

        var site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
        site.Tile = num2;

        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_MuffaloMigration,
            SiteDefOfReconAndDiscovery.RD_MuffaloMigration.Worker.GenerateDefaultParams(
                StorytellerUtility.DefaultSiteThreatPointsNow(), num2, null)));
        if (Rand.Value < 0.5f)
        {
            var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), num2, null))
            {
                hidden = true
            };
            site.parts.Add(scatteredTreasure);
        }

        Find.LetterStack.ReceiveLetter("RD_MuffaloMigration".Translate(), text, LetterDefOf.PositiveEvent,
            site);
        site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
        Find.WorldObjects.Add(site);

        return true;
    }
}