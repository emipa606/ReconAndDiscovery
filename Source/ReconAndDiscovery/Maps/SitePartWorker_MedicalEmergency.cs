using System.Collections.Generic;
using ReconAndDiscovery.Missions;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_MedicalEmergency : SitePartWorker
{
    public FloatRange casualtiesRange = new FloatRange(400f, 1000f);

    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        var faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
        var maxPawns = Find.World.worldObjects.MapParentAt(map.Tile).GetComponent<QuestComp_MedicalEmergency>()
            .maxPawns;
        var list = new List<Pawn>();
        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount >= 2, map, out var baseCenter))
        {
            return;
        }

        {
            for (var i = 0; i < maxPawns; i++)
            {
                if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                        x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount >= 2, map, out var intVec))
                {
                    continue;
                }

                var request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, faction,
                    PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, 20f, false,
                    true, true, false);
                var pawn = PawnGenerator.GeneratePawn(request);
                list.Add(pawn);
                HealthUtility.DamageUntilDowned(pawn);
                var intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 18);
                GenSpawn.Spawn(pawn, intVec2, map, Rot4.Random);
            }

            var lordJob = new LordJob_DefendBase(faction, baseCenter);
            LordMaker.MakeNewLord(faction, lordJob, map, list);
        }
    }
}