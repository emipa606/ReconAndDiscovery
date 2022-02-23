using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_CrashedShip : GenStep_AdventureGenerator
{
    public override void Generate(Map map, GenStepParams parms)
    {
        base.Generate(map, parms);
        var rect = new CellRect(Rand.RangeInclusive(adventureRegion.minX, adventureRegion.maxX - 60),
            Rand.RangeInclusive(adventureRegion.minZ + 15, adventureRegion.maxZ - 15), 20, 20);
        rect.ClipInsideMap(map);
        var resolveParams = baseResolveParams;
        resolveParams.rect = rect;
        BaseGen.symbolStack.Push("crashedShip", resolveParams);
        BaseGen.Generate();
        MapGenUtility.MakeDoors(new ResolveParams
        {
            wallStuff = ThingDefOf.Plasteel
        }, map);
        MapGenUtility.ResolveCustomGenSteps(map, parms);
    }
}