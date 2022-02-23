using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_MilitaryBase : GenStep_AdventureGenerator
{
    public override void Generate(Map map, GenStepParams parms)
    {
        if (map.TileInfo.WaterCovered)
        {
            return;
        }

        base.Generate(map, parms);
        var rect = new CellRect(Rand.RangeInclusive(adventureRegion.minX, adventureRegion.maxX - 60),
            Rand.RangeInclusive(adventureRegion.minZ, adventureRegion.maxZ - 60), 60, 60);
        rect.ClipInsideMap(map);
        var resolveParams = baseResolveParams;
        resolveParams.rect = rect;
        BaseGen.symbolStack.Push("oldMilitaryBase", resolveParams);
        BaseGen.Generate();
        MapGenUtility.MakeDoors(new ResolveParams
        {
            wallStuff = ThingDefOf.Steel
        }, map);
        MapGenUtility.UnfogFromRandomEdge(map);
        MapGenUtility.ResolveCustomGenSteps(map, parms);
        var num = Rand.RangeInclusive(1, 2);
        MapGenUtility.ScatterWeaponsWhere(resolveParams.rect, num, map,
            thing => thing.IsRangedWeapon && !thing.destroyOnDrop && thing.weaponTags != null &&
                     thing.weaponTags.Contains("Gun") && (thing.weaponTags.Contains("GunHeavy") ||
                                                          !thing.weaponTags.Contains("AdvancedGun")));
    }
}