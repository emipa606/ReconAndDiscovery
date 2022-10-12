using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_CrashedShip : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        if (rp.wallStuff == null)
        {
            rp.wallStuff = ThingDefOf.Steel;
        }

        if (rp.floorDef == null)
        {
            rp.floorDef = BaseGenUtility.CorrespondingTerrainDef(rp.wallStuff, true);
        }

        var resolveParams = rp;
        var resolveParams2 = rp;
        var resolveParams3 = rp;
        var resolveParams4 = rp;
        resolveParams.rect = new CellRect(rp.rect.minX, rp.rect.minZ, 1 + (2 * Rand.RangeInclusive(2, 4)),
            1 + (2 * Rand.RangeInclusive(3, 7)));
        var num = 1 + (2 * Rand.RangeInclusive(4, 12));
        var num2 = 1 + (2 * Rand.RangeInclusive(4, 10));
        resolveParams2.rect = new CellRect(resolveParams.rect.maxX,
            resolveParams.rect.minZ - (int)Math.Round((num2 - resolveParams.rect.Height) / 2f), num, num2);
        num = 1 + (2 * Rand.RangeInclusive(4, 8));
        num2 = num;
        resolveParams3.rect = new CellRect(resolveParams2.rect.maxX,
            resolveParams2.rect.minZ - (int)Math.Round((num2 - resolveParams2.rect.Height) / 2f), num, num2);
        if (resolveParams3.rect.Height >= resolveParams2.rect.Height &&
            resolveParams3.rect.Height >= resolveParams.rect.Height)
        {
            resolveParams4.rect = new CellRect(0, resolveParams3.rect.minZ, resolveParams3.rect.minX,
                resolveParams3.rect.Height);
        }
        else if (resolveParams2.rect.Height >= resolveParams.rect.Height)
        {
            resolveParams4.rect = new CellRect(0, resolveParams2.rect.minZ, resolveParams2.rect.minX,
                resolveParams2.rect.Height);
        }
        else
        {
            resolveParams4.rect = new CellRect(0, resolveParams.rect.minZ, resolveParams.rect.minX,
                resolveParams.rect.Height);
        }

        var resolveParams5 = resolveParams;
        resolveParams5.rect = new CellRect(resolveParams.rect.minX - 2, resolveParams.rect.minZ + 1, 1, 1);
        resolveParams5.thingRot = new Rot4(1);
        resolveParams5.singleThingDef = ThingDefOf.Ship_Engine;
        BaseGen.symbolStack.Push("insertFurnishing", resolveParams5);
        resolveParams5.rect.minZ += 2;
        BaseGen.symbolStack.Push("insertFurnishing", resolveParams5);
        resolveParams5.rect.minZ = resolveParams.rect.maxZ;
        BaseGen.symbolStack.Push("insertFurnishing", resolveParams5);
        resolveParams5.rect.minZ -= 2;
        BaseGen.symbolStack.Push("insertFurnishing", resolveParams5);
        BaseGen.symbolStack.Push("batteryRoom", resolveParams);
        resolveParams2.SetCustom("hasDoor", new[]
        {
            'N',
            'S',
            'E',
            'W'
        });
        BaseGen.symbolStack.Push("interior_diningRoom", resolveParams2);
        BaseGen.symbolStack.Push("roomWithDoor", resolveParams2);
        BaseGen.symbolStack.Push("triangularRoom", resolveParams3);
        BaseGen.symbolStack.Push("pathOfDestruction", resolveParams4);
    }
}