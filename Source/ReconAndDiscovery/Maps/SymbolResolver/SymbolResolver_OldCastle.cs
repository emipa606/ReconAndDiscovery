using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_OldCastle : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        var thingDef = (from d in DefDatabase<ThingDef>.AllDefs
            where d.IsStuff && d.stuffProps.CanMake(ThingDefOf.Wall) &&
                  d.stuffProps.categories.Contains(StuffCategoryDefOf.Stony) && d != ThingDef.Named("Jade")
            select d).ToList().RandomElementByWeight(x => 3f + (1f / x.BaseMarketValue));
        rp.wallStuff = thingDef;
        rp.floorDef = BaseGenUtility.CorrespondingTerrainDef(thingDef, true);
        var num = Rand.RangeInclusive(30, 40);
        for (var i = 0; i < num; i++)
        {
            var minX = Rand.RangeInclusive(rp.rect.minX, rp.rect.maxX - 1);
            var minZ = Rand.RangeInclusive(rp.rect.minZ, rp.rect.maxZ - 1);
            var resolveParams = rp;
            resolveParams.rect = new CellRect(minX, minZ, Rand.RangeInclusive(1, 3), Rand.RangeInclusive(1, 3));
            BaseGen.symbolStack.Push("pathOfDestruction", resolveParams);
        }

        for (var j = 0; j < Rand.RangeInclusive(2, 5); j++)
        {
            var minX2 = Rand.RangeInclusive(rp.rect.minX + 4, rp.rect.maxX - 10);
            var minZ2 = Rand.RangeInclusive(rp.rect.minZ + 4, rp.rect.maxZ - 10);
            var resolveParams2 = rp;
            resolveParams2.rect = new CellRect(minX2, minZ2, Rand.RangeInclusive(5, 8), Rand.RangeInclusive(5, 8));
            BaseGen.symbolStack.Push("emptyRoom", resolveParams2);
        }

        var resolveParams3 = rp;
        resolveParams3.rect = resolveParams3.rect.ContractedBy(21);
        var resolveParams4 = resolveParams3;
        resolveParams4.rect.Width = (int)(0.25 * resolveParams3.rect.Width);
        resolveParams4.rect.Height = resolveParams4.rect.Width;
        for (var k = 0; k < 4; k++)
        {
            BaseGen.symbolStack.Push("roomWithDoor", resolveParams4);
            resolveParams4.rect = new CellRect(resolveParams4.rect.minX, resolveParams4.rect.maxZ,
                resolveParams4.rect.Width, resolveParams4.rect.Height);
        }

        resolveParams3.SetCustom("hasDoor", new[]
        {
            'N'
        });
        BaseGen.symbolStack.Push("roomWithDoor", resolveParams3);
        var resolveParams5 = rp;
        resolveParams5.rect = rp.rect.ContractedBy(5);
        var resolveParams6 = resolveParams5;
        for (var l = 0; l < 25; l++)
        {
            var width = Rand.RangeInclusive(4, 6);
            var height = Rand.RangeInclusive(4, 7);
            if (resolveParams6.rect.minX > resolveParams5.rect.maxX - 3)
            {
                break;
            }

            resolveParams6.rect = new CellRect(resolveParams6.rect.minX, resolveParams5.rect.minZ, width, height);
            if (Rand.Chance(0.15f))
            {
                resolveParams6.rect.minX = resolveParams6.rect.minX + 3;
            }
            else
            {
                var c = !Rand.Chance(0.1f) ? 'N' : 'W';
                resolveParams6.SetCustom("hasDoor", new[]
                {
                    c
                });
                BaseGen.symbolStack.Push("roomWithDoor", resolveParams6);
                resolveParams6.rect.minX = resolveParams6.rect.maxX;
            }
        }

        resolveParams6 = resolveParams5;
        for (var m = 0; m < 25; m++)
        {
            var width2 = Rand.RangeInclusive(4, 6);
            var num2 = Rand.RangeInclusive(4, 7);
            if (resolveParams6.rect.minX > resolveParams5.rect.maxX - 3)
            {
                break;
            }

            resolveParams6.rect = new CellRect(resolveParams6.rect.minX, resolveParams5.rect.maxZ + 1 - num2,
                width2, num2);
            if (Rand.Chance(0.15f))
            {
                resolveParams6.rect.minX = resolveParams6.rect.minX + 3;
            }
            else
            {
                var c2 = !Rand.Chance(0.1f) ? 'S' : 'W';
                resolveParams6.SetCustom("hasDoor", new[]
                {
                    c2
                });
                BaseGen.symbolStack.Push("roomWithDoor", resolveParams6);
                resolveParams6.rect.minX = resolveParams6.rect.maxX;
            }
        }

        resolveParams6 = resolveParams5;
        resolveParams6.rect.minZ = resolveParams6.rect.minZ + 4;
        for (var n = 0; n < 25; n++)
        {
            var width3 = Rand.RangeInclusive(4, 6);
            var height2 = Rand.RangeInclusive(4, 7);
            if (resolveParams6.rect.minZ > resolveParams5.rect.maxZ - 4)
            {
                break;
            }

            resolveParams6.rect = new CellRect(resolveParams5.rect.minX, resolveParams6.rect.minZ, width3, height2);
            if (Rand.Chance(0.15f))
            {
                resolveParams6.rect.minZ = resolveParams6.rect.minZ + 3;
            }
            else
            {
                var c3 = !Rand.Chance(0.1f) ? 'E' : 'S';
                resolveParams6.SetCustom("hasDoor", new[]
                {
                    c3
                });
                BaseGen.symbolStack.Push("roomWithDoor", resolveParams6);
                resolveParams6.rect.minZ = resolveParams6.rect.maxZ;
            }
        }

        resolveParams6 = resolveParams5;
        resolveParams6.rect.minZ = resolveParams6.rect.minZ + 4;
        for (var num3 = 0; num3 < 25; num3++)
        {
            var num4 = Rand.RangeInclusive(4, 6);
            var height3 = Rand.RangeInclusive(4, 7);
            if (resolveParams6.rect.minZ > resolveParams5.rect.maxZ - 4)
            {
                break;
            }

            resolveParams6.rect = new CellRect(resolveParams5.rect.maxX + 1 - num4, resolveParams6.rect.minZ, num4,
                height3);
            if (Rand.Chance(0.15f))
            {
                resolveParams6.rect.minZ = resolveParams6.rect.minZ + 3;
            }
            else
            {
                var c4 = !Rand.Chance(0.1f) ? 'W' : 'S';
                resolveParams6.SetCustom("hasDoor", new[]
                {
                    c4
                });
                BaseGen.symbolStack.Push("roomWithDoor", resolveParams6);
                resolveParams6.rect.minZ = resolveParams6.rect.maxZ;
            }
        }

        ResolveParams resolveParams10;
        ResolveParams resolveParams9;
        ResolveParams resolveParams8;
        var resolveParams7 = resolveParams8 = resolveParams9 = resolveParams10 = rp;
        var cellRect = new CellRect(rp.rect.maxX, rp.rect.maxZ, 1, 1);
        resolveParams8.rect = cellRect.ExpandedBy(4);
        var cellRect2 = new CellRect(rp.rect.minX, rp.rect.maxZ, 1, 1);
        resolveParams7.rect = cellRect2.ExpandedBy(4);
        var cellRect3 = new CellRect(rp.rect.maxX, rp.rect.minZ, 1, 1);
        resolveParams9.rect = cellRect3.ExpandedBy(4);
        var cellRect4 = new CellRect(rp.rect.minX, rp.rect.minZ, 1, 1);
        resolveParams10.rect = cellRect4.ExpandedBy(4);
        resolveParams8.SetCustom("hasDoor", new[]
        {
            'S',
            'W'
        });
        resolveParams7.SetCustom("hasDoor", new[]
        {
            'S',
            'E'
        });
        resolveParams9.SetCustom("hasDoor", new[]
        {
            'N',
            'W'
        });
        resolveParams10.SetCustom("hasDoor", new[]
        {
            'N',
            'E'
        });
        BaseGen.symbolStack.Push("roomWithDoor", resolveParams8);
        BaseGen.symbolStack.Push("roomWithDoor", resolveParams7);
        BaseGen.symbolStack.Push("roomWithDoor", resolveParams9);
        BaseGen.symbolStack.Push("roomWithDoor", resolveParams10);
        var resolveParams11 = rp;
        resolveParams11.rect = resolveParams11.rect.ContractedBy(3);
        BaseGen.symbolStack.Push("edgeShields", resolveParams11);
        var resolveParams12 = rp;
        BaseGen.symbolStack.Push("doors", resolveParams12);
        BaseGen.symbolStack.Push("edgeWalls", resolveParams12);
        BaseGen.symbolStack.Push("floor", resolveParams12);
    }
}