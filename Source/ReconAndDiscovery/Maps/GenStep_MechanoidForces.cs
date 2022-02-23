using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Maps;

public class GenStep_MechanoidForces : GenStep
{
    private FloatRange pointsRange = new FloatRange(450f, 700f);

    public override int SeedPart => 339641510;

    public override void Generate(Map map, GenStepParams parms)
    {
        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                x => x.Standable(map) && !x.Fogged(map) && x.GetRoom(map).CellCount >= 4, map, out var intVec))
        {
            return;
        }

        var num = pointsRange.RandomInRange;
        var list = new List<Pawn>();
        for (var i = 0; i < 50; i++)
        {
            var pawnKindDef = (from kind in DefDatabase<PawnKindDef>.AllDefsListForReading
                where kind.RaceProps.IsMechanoid
                select kind).RandomElementByWeight(kind => 1f / kind.combatPower);
            list.Add(PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfMechanoids));
            num -= pawnKindDef.combatPower;
            if (num <= 0f)
            {
                break;
            }
        }

        var point = default(IntVec3);
        foreach (var newThing in list)
        {
            var intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
            point = intVec2;
            GenSpawn.Spawn(newThing, intVec2, map, Rot4.Random);
        }

        LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_DefendPoint(point), map, list);
    }
}