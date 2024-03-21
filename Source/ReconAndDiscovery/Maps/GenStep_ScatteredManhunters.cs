using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_ScatteredManhunters : GenStep
{
    private FloatRange pointsRange = new FloatRange(250f, 700f);

    public override int SeedPart => 349640110;

    public override void Generate(Map map, GenStepParams parms)
    {
        var num = pointsRange.RandomInRange;
        var list = new List<Pawn>();
        for (var i = 0; i < 50; i++)
        {
            if (!AggressiveAnimalIncidentUtility.TryFindAggressiveAnimalKind(pointsRange.RandomInRange, map.Tile,
                    out var pawnKindDef))
            {
                return;
            }

            list.Add(PawnGenerator.GeneratePawn(pawnKindDef));
            num -= pawnKindDef.combatPower;
            if (num <= 0f)
            {
                break;
            }
        }

        foreach (var newThing in list)
        {
            if (!CellFinderLoose.TryGetRandomCellWith(
                    x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount >= 4, map, 1000, out var intVec))
            {
                continue;
            }

            var intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
            GenSpawn.Spawn(newThing, intVec2, map, Rot4.Random);
            newThing.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
        }
    }
}