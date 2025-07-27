using System.Collections.Generic;
using ReconAndDiscovery.DefOfs;
using ReconAndDiscovery.Missions;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_PeaceTalksFaction : SitePartWorker
{
    public FloatRange casualtiesRange = new(400f, 1000f);

    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        map.fogGrid.ClearAllFog();
        var mapParent = Find.World.worldObjects.MapParentAt(map.Tile);
        var faction = mapParent.GetComponent<QuestComp_PeaceTalks>().Faction;
        //TODO: check if it works
        var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategories.FactionArrival, map);
        incidentParms.points = Mathf.Max(incidentParms.points, 250f);
        incidentParms.points *= 2f;
        var pawnGroupMakerParms = new PawnGroupMakerParms
        {
            groupKind = PawnGroupKindDefOf.Settlement,
            tile = map.Tile,
            faction = faction,
            points = incidentParms.points,
            inhabitants = true
        };
        var list = new List<Pawn>();
        foreach (var pawn in PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms))
        {
            CellFinder.TryFindRandomCellInsideWith(new CellRect(40, 40, map.Size.x - 80, map.Size.z - 80),
                c => c.Standable(map), out var loc);
            GenSpawn.Spawn(pawn, loc, map);
            list.Add(pawn);
        }

        CellFinder.TryFindRandomCellInsideWith(new CellRect(50, 50, map.Size.x - 100, map.Size.z - 100),
            c => c.Standable(map), out var intVec);
        if (faction.leader != null)
        {
            GenSpawn.Spawn(faction.leader, intVec, map);
            mapParent.GetComponent<QuestComp_PeaceTalks>().Negotiator = faction.leader;
            list.Add(faction.leader);
        }

        LordJob lordJob = new LordJob_DefendBase(faction, intVec, 0);
        LordMaker.MakeNewLord(faction, lordJob, map, list);
    }
}