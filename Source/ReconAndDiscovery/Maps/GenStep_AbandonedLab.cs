using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_AbandonedLab : GenStep_AdventureGenerator
{
    public override void Generate(Map map, GenStepParams parms)
    {
        if (map.TileInfo.WaterCovered)
        {
            return;
        }

        base.Generate(map, parms);
        var rect = new CellRect(Rand.RangeInclusive(adventureRegion.minX, adventureRegion.maxX - 50),
            Rand.RangeInclusive(adventureRegion.minZ, adventureRegion.maxZ - 50), 50, 50);
        rect.ClipInsideMap(map);
        var resolveParams = baseResolveParams;
        resolveParams.rect = rect;
        BaseGen.symbolStack.Push("abandonedLab", resolveParams);
        BaseGen.Generate();
        MapGenUtility.MakeDoors(new ResolveParams
        {
            wallStuff = ThingDefOf.Plasteel
        }, map);
        MapGenUtility.ResolveCustomGenSteps(map, parms);
    }
}