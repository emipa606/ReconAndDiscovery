using System.Collections.Generic;
using System.Linq;
using ExpandedIncidents.Settings;
using ReconAndDiscovery.Missions.QuestComp;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Missions;

public class SitePartWorker_Festival : SitePartWorker
{
    private List<Faction> Factions;
    private Faction hostFaction;

    private void IncrementAllGoodwills()
    {
        foreach (var faction in Factions)
        {
            if (faction.PlayerGoodwill >= 0f)
            {
                faction.TryAffectGoodwillWith(Faction.OfPlayer, 5);
            }
        }
    }

    private List<Pawn> SpawnPawns(IncidentParms parms)
    {
        var map = parms.target as Map;
        RaD_Mod.LogMessage($"Spawning pawns for {parms.faction.Name}");
        var defaultPawnGroupMakerParms =
            IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Trader, parms);
        var list = PawnGroupMakerUtility
            .GeneratePawns(defaultPawnGroupMakerParms, false).ToList();

        foreach (var newThing in list)
        {
            var loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 5);
            GenSpawn.Spawn(newThing, loc, map);
        }

        return list;
    }

    private void MakeTradeCaravan(Faction faction, IntVec3 spot, Map map)
    {
        var incidentParms = Find.Storyteller.storytellerComps[0]
            .GenerateParms(IncidentCategoryDefOf.OrbitalVisitor, map);
        incidentParms.points = Mathf.Min(800f, incidentParms.points);
        incidentParms.spawnCenter = spot;
        incidentParms.faction = faction;
        var list = SpawnPawns(incidentParms);
        if (list.Count == 0)
        {
            return;
        }

        foreach (var pawn in list)
        {
            if (pawn.needs?.food != null)
            {
                pawn.needs.food.CurLevel = pawn.needs.food.MaxLevel;
            }
        }

        foreach (var pawn in list)
        {
            if (pawn.TraderKind == null)
            {
                continue;
            }

            incidentParms.traderKind = pawn.TraderKind;
            break;
        }

        var lordJob = new LordJob_TradeWithColony(incidentParms.faction, spot);
        LordMaker.MakeNewLord(incidentParms.faction, lordJob, map, list);
    }

    private void MakeTradeCaravans(Map map)
    {
        if (!RCellFinder.TryFindRandomSpotJustOutsideColony(
                CellFinderLoose.RandomCellWith(c => c.Standable(map), map), map, out var spot))
        {
            return;
        }

        foreach (var faction in Factions)
        {
            MakeTradeCaravan(faction, spot, map);
        }

        MakeTradeCaravan(hostFaction, spot, map);
    }

    private void MakePartyGroups(Map map)
    {
        var list = (from t in map.listerThings.AllThings
            where t.def.IsTable
            select t).ToList();
        if (list.Count == 0)
        {
            return;
        }

        var thing = list.RandomElement();
        var unused = thing.Position;
        // Seems like somethings missing here
    }

    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        hostFaction = Find.WorldObjects.MapParentAt(map.Tile).GetComponent<Festival>().HostFaction;
        Factions = Find.WorldObjects.MapParentAt(map.Tile).GetComponent<Festival>().AttendingFactions;

        MakeTradeCaravans(map);
        MakePartyGroups(map);
        IncrementAllGoodwills();
    }
}