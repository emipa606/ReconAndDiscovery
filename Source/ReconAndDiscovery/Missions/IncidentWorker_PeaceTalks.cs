using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_PeaceTalks : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        private bool TryFindFaction(out Faction faction, Predicate<Faction> validator)
        {
            faction = null;
            var list = Find.FactionManager.AllFactionsVisible.ToList();
            if (list.Contains(Faction.OfPlayer))
            {
                list.Remove(Faction.OfPlayer);
            }

            list = (from f in list
                where validator(f)
                select f).ToList();

            if (!list.Any())
            {
                return false;
            }

            faction = list.RandomElement();
            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if ((from existingPeaceTalks in Find.WorldObjects.AllWorldObjects
                where existingPeaceTalks.def == SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks
                select existingPeaceTalks).Any())
            {
                return false;
            }

            if (!TryFindFaction(out var faction,
                    f => f != Faction.OfPlayer && f.PlayerGoodwill < 0f && f.def.CanEverBeNonHostile &&
                         f.def.humanlikeFaction))
            {
                return false;
            }
            
            if (!TileFinder.TryFindNewSiteTile(out var tile))
            {
                return false;
            }

            var site = (Site) WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks);
            site.Tile = tile;
            site.doorsAlwaysOpenForPlayerPawns = true;
            site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_PeaceTalks,
                SiteDefOfReconAndDiscovery.RD_PeaceTalks.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));

            var peaceTalksFaction = new SitePart(site, SiteDefOfReconAndDiscovery.RD_PeaceTalksFaction,
                SiteDefOfReconAndDiscovery.RD_PeaceTalksFaction.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(peaceTalksFaction);
            site.GetComponent<QuestComp_PeaceTalks>().StartQuest(faction);
            var timeoutDays = 5;
            site.GetComponent<TimeoutComp>().StartTimeout(timeoutDays * 60000);
            SendStandardLetter(parms, site, faction.leader.Label, faction.Name, timeoutDays.ToString());
            Find.WorldObjects.Add(site);

            return true;
        }
    }
}