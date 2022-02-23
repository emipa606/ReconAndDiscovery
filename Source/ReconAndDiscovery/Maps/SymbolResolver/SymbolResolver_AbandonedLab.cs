using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver
{
    public class SymbolResolver_AbandonedLab : RimWorld.BaseGen.SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            rp.floorDef = TerrainDefOf.Concrete;
            var plasteel = ThingDefOf.Plasteel;
            var resolveParams = rp;
            rp.wallStuff = plasteel;
            var minX = Rand.RangeInclusive(rp.rect.minX + 5, rp.rect.minX + 10);
            var minZ = Rand.RangeInclusive(rp.rect.minZ + 5, rp.rect.minZ + 10);
            var width = Rand.RangeInclusive(25, 35);
            var height = Rand.RangeInclusive(25, 35);
            resolveParams.rect = new CellRect(minX, minZ, width, height);
            resolveParams.floorDef = TerrainDefOf.WoodPlankFloor;
            resolveParams.wallStuff = ThingDefOf.Plasteel;
            var resolveParams2 = resolveParams;
            var resolveParams3 = resolveParams;
            var list = new List<ThingDef>
            {
                ThingDef.Named("DrugLab"),
                ThingDef.Named("SimpleResearchBench"),
                ThingDef.Named("SimpleResearchBench"),
                ThingDef.Named("SimpleResearchBench"),
                ThingDef.Named("HiTechResearchBench")
            };
            resolveParams2.floorDef = TerrainDefOf.MetalTile;
            resolveParams2.wallStuff = ThingDefOf.Plasteel;
            resolveParams2.SetCustom("hasDoor", new[]
            {
                'E'
            });
            for (var i = 0; i < 20; i++)
            {
                if (resolveParams2.rect.minZ > resolveParams.rect.maxZ - 2)
                {
                    break;
                }

                resolveParams2.rect = new CellRect(resolveParams2.rect.minX, resolveParams2.rect.minZ, 7, 6);
                resolveParams3.rect = resolveParams2.rect.ContractedBy(1);
                resolveParams3.rect = new CellRect(resolveParams3.rect.minX + 1, resolveParams3.rect.minZ + 1, 1, 1);
                resolveParams3.thingRot = Rot4.West;
                resolveParams3.singleThingDef = list.RandomElement();
                BaseGen.symbolStack.Push("wireOutline", resolveParams2);
                BaseGen.symbolStack.Push("insertFurnishing", resolveParams3);
                BaseGen.symbolStack.Push("roomWithDoor", resolveParams2);
                resolveParams2.rect.minZ = resolveParams2.rect.maxZ;
            }

            resolveParams2.rect.minX = resolveParams2.rect.maxX + 3;
            var rot = Rot4.North;
            if (Rand.Chance(0.5f))
            {
                resolveParams2.rect.minZ = resolveParams.rect.minZ;
                resolveParams2.SetCustom("hasDoor", new[]
                {
                    'N'
                });
                resolveParams.SetCustom("hasDoor", new[]
                {
                    'E',
                    'N'
                });
                rot = Rot4.South;
            }
            else
            {
                resolveParams2.rect.minZ = resolveParams.rect.maxZ - 6;
                resolveParams2.SetCustom("hasDoor", new[]
                {
                    'S'
                });
                resolveParams.SetCustom("hasDoor", new[]
                {
                    'E',
                    'S'
                });
            }

            for (var j = 0; j < 20; j++)
            {
                if (resolveParams2.rect.minX > resolveParams.rect.maxX - 2)
                {
                    break;
                }

                resolveParams2.rect = new CellRect(resolveParams2.rect.minX, resolveParams2.rect.minZ, 6, 7);
                resolveParams3.rect = resolveParams2.rect.ContractedBy(1);
                resolveParams3.rect = rot == Rot4.South
                    ? new CellRect(resolveParams3.rect.minX + 1, resolveParams3.rect.minZ + 1, 1, 1)
                    : new CellRect(resolveParams3.rect.minX + 1, resolveParams3.rect.maxZ - 1, 1, 1);

                resolveParams3.thingRot = rot;
                resolveParams3.singleThingDef = list.RandomElement();
                BaseGen.symbolStack.Push("wireOutline", resolveParams2);
                BaseGen.symbolStack.Push("insertFurnishing", resolveParams3);
                BaseGen.symbolStack.Push("roomWithDoor", resolveParams2);
                resolveParams2.rect.minX = resolveParams2.rect.maxX;
            }

            var resolveParams4 = rp;
            resolveParams4.singleThingDef = ThingDef.Named("Table2x4c");
            resolveParams4.rect = new CellRect(resolveParams.rect.CenterCell.x, resolveParams.rect.CenterCell.z, 1, 1);
            resolveParams4.thingRot = Rot4.North;
            BaseGen.symbolStack.Push("insertFurnishing", resolveParams4);
            for (var k = 0; k < 4; k++)
            {
                resolveParams4.singleThingDef = ThingDef.Named("DiningChair");
                resolveParams4.rect = new CellRect(resolveParams.rect.CenterCell.x - 1,
                    resolveParams.rect.CenterCell.z + 2 - k, 1, 1);
                resolveParams4.thingRot = Rot4.East;
                BaseGen.symbolStack.Push("insertFurnishing", resolveParams4);
            }
            
            resolveParams.chanceToSkipWallBlock = 0.05f;
            BaseGen.symbolStack.Push("wireOutline", resolveParams);
            BaseGen.symbolStack.Push("roomWithDoor", resolveParams);
            var resolveParams5 = rp;
            // resolveParams5.rect = rp.rect.ContractedBy(2);
            resolveParams5.rect = new CellRect(resolveParams5.rect.minX, resolveParams5.rect.minZ, 6, 5);
            resolveParams5.wallStuff = ThingDef.Named("BlocksLimestone");
            resolveParams5.SetCustom("hasDoor", new[]
            {
                'N'
            });


            var resolveParams6 = resolveParams5;
            resolveParams6.rect = resolveParams6.rect.ContractedBy(1);
            resolveParams6.singleThingDef = ThingDef.Named("ChemfuelPoweredGenerator");
            resolveParams6.thingRot = Rot4.North; 
            resolveParams6.rect = new CellRect(resolveParams5.rect.minX + 1, resolveParams5.rect.minZ + 1, 3, 3);
            BaseGen.symbolStack.Push("insertFurnishing", resolveParams6);
            BaseGen.symbolStack.Push("wireOutline", resolveParams5);
            BaseGen.symbolStack.Push("roomWithDoor", resolveParams5);

            BaseGen.symbolStack.Push("edgeFence", rp);
            BaseGen.symbolStack.Push("floor", rp);
            BaseGen.symbolStack.Push("clear", rp);
        }
    }
}