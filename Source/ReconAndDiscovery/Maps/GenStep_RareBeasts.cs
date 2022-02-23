using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
    public class GenStep_RareBeasts : GenStep
    {
        public override int SeedPart => 349231510;

        public override void Generate(Map map, GenStepParams parms)
        {
            var pawnKindDef = ThingDefOfReconAndDiscovery.RD_Devillo;
            if (Rand.Chance(0.4f))
            {
                pawnKindDef = ThingDefOfReconAndDiscovery.RD_Nitralope;
            }
            
            var request = new PawnGenerationRequest(pawnKindDef, null,
                PawnGenerationContext.NonPlayer, parms.sitePart.site.Tile, true, true, false, false, false, false,
                1f, false, false,
                false, true, true, true,
                false, false, 0f, 0f, null, 1f, null, null, null,
                null, 0f, 1f, 1f, Gender.Male);

            var request2 = new PawnGenerationRequest(pawnKindDef, null,
                PawnGenerationContext.NonPlayer, parms.sitePart.site.Tile, true, true, false, false, false, false, 1f, false, false,
                false, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null,
                null, 0f, 0f, 1f, Gender.Female);

            if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount >= 4, map, out var intVec))
            {
                return;
            }

            var pawn = PawnGenerator.GeneratePawn(request);
            var pawn2 = PawnGenerator.GeneratePawn(request2);
            var intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
            GenSpawn.Spawn(pawn, intVec2, map, Rot4.Random);
            intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
            GenSpawn.Spawn(pawn2, intVec2, map, Rot4.Random);
        }
    }
}