using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Missions.QuestComp;
using ReconAndDiscovery.Utils;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_Festival : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        public List<Faction> GetAllNonPlayerFriends(Faction faction)
        {
            var list = Find.FactionManager.AllFactionsVisible.ToList();
            if (list.Contains(Faction.OfPlayer))
            {
                list.Remove(Faction.OfPlayer);
            }

            return (from f in list
                where faction.GoodwillWith(f) > 10f
                select f).ToList();
        }

        public int FriendsCount(Faction faction)
        {
            var list = Find.FactionManager.AllFactionsVisible.ToList();
            if (list.Contains(faction))
            {
                list.Remove(faction);
            }

            list = (from f in list
                where faction.GoodwillWith(f) > 10f
                select f).ToList();
            return list.Count;
        }

        private List<Faction> GetFactions(Predicate<Faction> validator)
        {
            var list = Find.FactionManager.AllFactionsVisible.ToList();
            if (list.Contains(Faction.OfPlayer))
            {
                list.Remove(Faction.OfPlayer);
            }

            list = (from f in list
                where validator(f)
                select f).ToList();

            return list;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if ((from existingTradeFestivals in Find.WorldObjects.AllWorldObjects
                    where existingTradeFestivals.def == SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks
                    select existingTradeFestivals).Any())
            {
                return false;
            }

            var friendlyFactions = GetFactions(f => f != Faction.OfPlayer && f.PlayerGoodwill >= 0);
            if (!friendlyFactions.Any())
            {
                return false;
            }

            var hostFaction = friendlyFactions.RandomElement();

            if (!TileFinder.TryFindNewSiteTile(out var tile))
            {
                return false;
            }

            var site = (Site) WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_FestivalMap);
            site.Tile = tile;
            site.doorsAlwaysOpenForPlayerPawns = true;
            
            site.AddPart(GetSitePartUtil.WithDefaultParams(SiteDefOfReconAndDiscovery.RD_Festival, site, tile, hostFaction));
            site.AddPart(GetSitePartUtil.WithDefaultParams(SiteDefOfReconAndDiscovery.RD_AbandonedColony, site, tile, hostFaction, true));

            var timeoutDays = 8;
            site.GetComponent<Festival>().SetupFactions(hostFaction, friendlyFactions);
            site.GetComponent<TimeoutComp>().StartTimeout(timeoutDays * 60000);
            SendStandardLetter(parms, site, hostFaction.Name);
            Find.WorldObjects.Add(site);
            return true;
        }
    }
}