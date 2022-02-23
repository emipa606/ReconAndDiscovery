using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_OldCastle : GenStep_AdventureGenerator
{
    public override void Generate(Map map, GenStepParams parms)
    {
        if (map.TileInfo.WaterCovered)
        {
            return;
        }

        base.Generate(map, parms);
        var num = Rand.RangeInclusive(55, 80);
        var num2 = Rand.RangeInclusive(55, 80);
        var rect = new CellRect(Rand.RangeInclusive(adventureRegion.minX, adventureRegion.maxX - num),
            Rand.RangeInclusive(adventureRegion.minZ, adventureRegion.maxZ - num2), num, num2);
        var resolveParams = baseResolveParams;
        resolveParams.rect = rect;
        BaseGen.globalSettings.map = map;
        BaseGen.symbolStack.Push("oldCastle", resolveParams);
        BaseGen.Generate();
        MapGenUtility.MakeDoors(new ResolveParams
        {
            wallStuff = ThingDefOf.WoodLog
        }, map);
        BaseGen.Generate();
        MapGenUtility.ScatterWeaponsWhere(resolveParams.rect, Rand.RangeInclusive(7, 11), map,
            thing => thing.weaponTags != null && thing.equipmentType == EquipmentType.Primary &&
                     !thing.destroyOnDrop && !thing.weaponTags.Contains("Gun"));
        MapGenUtility.ResolveCustomGenSteps(map, parms);
    }
}