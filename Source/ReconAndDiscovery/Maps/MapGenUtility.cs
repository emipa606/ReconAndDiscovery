using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps;

public static class MapGenUtility
{
    private static readonly List<IntVec3> DoorPosList = new();

    private static List<ThingStuffPair> weapons;

    private static readonly List<GenStepDef> customGenSteps = new();

    public static void AddGenStep(GenStepDef step)
    {
        customGenSteps.Add(step);
    }

    public static void ResolveCustomGenSteps(Map map, GenStepParams parms)
    {
        foreach (var genStepDef in customGenSteps)
        {
            genStepDef.genStep.Generate(map, parms);
        }

        customGenSteps.Clear();
    }

    public static bool TryFindRandomCellWhere(IEnumerable<IntVec3> candidates, Predicate<IntVec3> validator,
        out IntVec3 loc)
    {
        loc = default;
        var source = from v in candidates
            where validator(v)
            select v;
        bool result;
        if (source.Any())
        {
            loc = source.RandomElement();
            result = true;
        }
        else
        {
            result = false;
        }

        return result;
    }

    public static void MakeRoom(Map map, CellRect rect, RoomStructure rs)
    {
        if (rs.wallMaterial == null)
        {
            var faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
            rs.wallMaterial = BaseGenUtility.RandomCheapWallStuff(faction);
        }

        rs.floorMaterial ??= BaseGenUtility.CorrespondingTerrainDef(rs.wallMaterial, true);

        if (rs.floorMaterial == null)
        {
            rs.floorMaterial = BaseGenUtility.RandomBasicFloorDef(Faction.OfMechanoids);
        }

        TileArea(map, rect, rs.floorMaterial, rs.floorChance);
        MakeLongWall(new IntVec3(rect.minX, 1, rect.minZ), map, rect.Width, true, rs.wallS, rs.wallMaterial);
        MakeLongWall(new IntVec3(rect.minX, 1, rect.maxZ), map, rect.Width, true, rs.wallN, rs.wallMaterial);
        MakeLongWall(new IntVec3(rect.minX, 1, rect.minZ), map, rect.Height, false, rs.wallW, rs.wallMaterial);
        MakeLongWall(new IntVec3(rect.maxX, 1, rect.minZ), map, rect.Height, false, rs.wallE, rs.wallMaterial);
        for (var i = 0; i < rs.doorN; i++)
        {
            RandomAddDoor(new IntVec3(rect.minX + 2, 1, rect.maxZ), map, rect.Width - 3, true, rs.wallMaterial);
        }

        for (var j = 0; j < rs.doorS; j++)
        {
            RandomAddDoor(new IntVec3(rect.minX + 2, 1, rect.minZ), map, rect.Width - 3, true, rs.wallMaterial);
        }

        for (var k = 0; k < rs.doorE; k++)
        {
            RandomAddDoor(new IntVec3(rect.minX, 1, rect.minZ + 2), map, rect.Height - 3, false, rs.wallMaterial);
        }

        for (var l = 0; l < rs.doorW; l++)
        {
            RandomAddDoor(new IntVec3(rect.maxX, 1, rect.minZ + 2), map, rect.Height - 3, false, rs.wallMaterial);
        }

        map.MapUpdate();
    }

    private static List<ThingStuffPair> GetWeapons(Predicate<ThingDef> validator)
    {
        var list = new List<ThingStuffPair>();
        if (weapons.NullOrEmpty())
        {
            FillPossibleObjectLists();
        }

        foreach (var item in weapons)
        {
            if (validator(item.thing))
            {
                list.Add(item);
            }
        }

        return list;
    }

    public static void MakeTriangularRoom(Map map, ResolveParams rp)
    {
        if (rp.wallStuff == null)
        {
            var faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);

            rp.wallStuff = BaseGenUtility.RandomCheapWallStuff(faction);
        }

        rp.floorDef ??= BaseGenUtility.CorrespondingTerrainDef(rp.wallStuff, true);

        if (rp.floorDef == null)
        {
            rp.floorDef = BaseGenUtility.RandomBasicFloorDef(Faction.OfMechanoids);
        }

        var resolveParams = rp;
        resolveParams.rect = new CellRect(rp.rect.minX, rp.rect.minZ, 1, rp.rect.Height);
        BaseGen.symbolStack.Push("edgeWalls", resolveParams);
        for (var i = 0; i <= rp.rect.Width; i++)
        {
            var num = rp.rect.minX + i;
            var num2 = (int)Math.Floor(0.5f * i);
            var num3 = (int)Math.Ceiling(0.5f * i);
            for (var j = rp.rect.minZ + num2; j < rp.rect.minZ + rp.rect.Width - num2; j++)
            {
                foreach (var thing in map.thingGrid.ThingsAt(new IntVec3(num, 1, j)))
                {
                    thing.TakeDamage(new DamageInfo(DamageDefOf.Blunt, 10000, -1f));
                }

                TryToSetFloorTile(new IntVec3(num, 1, j), map, rp.floorDef);
                if (j != rp.rect.minZ + num2 && j != rp.rect.minZ + num3 &&
                    j != rp.rect.minZ + rp.rect.Width - (num2 + 1) &&
                    j != rp.rect.minZ + rp.rect.Width - (num3 + 1))
                {
                    continue;
                }

                var resolveParams2 = rp;
                resolveParams2.rect = new CellRect(num, j, 1, 1);
                BaseGen.symbolStack.Push("edgeWalls", resolveParams2);
            }
        }

        map.MapUpdate();
        var roofGrid = BaseGen.globalSettings.map.roofGrid;
        var def = rp.roofDef ?? RoofDefOf.RoofConstructed;
        for (var k = 0; k <= rp.rect.Width; k++)
        {
            var newX = rp.rect.minX + k;
            var num4 = (int)Math.Floor(0.5f * k);
            for (var l = rp.rect.minZ + num4; l < rp.rect.minZ + rp.rect.Width - num4; l++)
            {
                var c = new IntVec3(newX, 1, l);
                if (!roofGrid.Roofed(c))
                {
                    roofGrid.SetRoof(c, def);
                }
            }
        }
    }

    public static void DestroyAllInArea(Map map, CellRect rect)
    {
        for (var i = rect.minX; i <= rect.maxX; i++)
        {
            for (var j = rect.minZ; j <= rect.maxZ; j++)
            {
                var c = new IntVec3(i, 1, j);
                DestroyAllAtLocation(c, map);
                //rect.GetIterator().MoveNext();
                //c = rect.GetIterator().Current;
            }
        }
    }

    private static void TileArea(Map map, CellRect rect, TerrainDef floorMaterial = null, float floorIntegrity = 1f)
    {
        floorMaterial ??= BaseGenUtility.RandomBasicFloorDef(Faction.OfMechanoids);

        for (var i = rect.minX; i <= rect.maxX; i++)
        {
            for (var j = rect.minZ; j <= rect.maxZ; j++)
            {
                var c = new IntVec3(i, 1, j);
                if (Rand.Value <= floorIntegrity)
                {
                    TryToSetFloorTile(c, map, floorMaterial);
                    //rect.GetIterator().MoveNext();
                    //c = rect.GetIterator().Current;
                }
            }
        }
    }

    public static void RoofArea(Map map, CellRect rect, float roofIntegrity = 1f)
    {
        for (var i = rect.minX; i <= rect.maxX; i++)
        {
            for (var j = rect.minZ; j <= rect.maxZ; j++)
            {
                var c = new IntVec3(i, 1, j);
                if (!(Rand.Value <= roofIntegrity))
                {
                    continue;
                }

                if (!map.roofGrid.Roofed(c))
                {
                    map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
                }
            }
        }
    }

    private static void RandomAddDoor(IntVec3 start, Map map, int extent, bool horizontal, ThingDef material = null)
    {
        material ??= BaseGenUtility.RandomCheapWallStuff(
            Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer));

        var num = Rand.RangeInclusive(0, extent);
        if (horizontal)
        {
            start.x += num;
        }
        else
        {
            start.z += num;
        }

        TryMakeDoor(start, map, material);
    }

    private static void FillPossibleObjectLists()
    {
        if (weapons.NullOrEmpty())
        {
            weapons = ThingStuffPair.AllWith(td => td.IsWeapon && !td.weaponTags.NullOrEmpty());
        }
    }

    public static void ScatterWeaponsWhere(CellRect within, int num, Map map, Predicate<ThingDef> validator)
    {
        var source = GetWeapons(validator);
        for (var i = 0; i < num; i++)
        {
            var thingStuffPair = source.RandomElement();
            var thing = ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
            var compQuality = thing.TryGetComp<CompQuality>();
            compQuality?.SetQuality(QualityUtility.GenerateQualityCreatedByPawn(12, false),
                ArtGenerationContext.Outsider);

            if (thing != null &&
                CellFinder.TryFindRandomCellInsideWith(within, loc => loc.Standable(map), out var loc2))
            {
                GenSpawn.Spawn(thing, loc2, map);
            }
        }
    }

    private static void MakeLongWall(IntVec3 start, Map map, int extendDist, bool horizontal, float integrity,
        ThingDef stuffDef)
    {
        var c = start;
        for (var i = 0; i < extendDist; i++)
        {
            if (!c.InBounds(map))
            {
                break;
            }

            if (Rand.Value < integrity)
            {
                TrySetCellAsWall(c, map, stuffDef);
            }

            if (horizontal)
            {
                c.x++;
            }
            else
            {
                c.z++;
            }
        }
    }

    private static void TrySetCellAsWall(IntVec3 c, Map map, ThingDef stuffDef)
    {
        var thingList = c.GetThingList(map);
        foreach (var thing in thingList)
        {
            if (!thing.def.destroyable)
            {
                return;
            }
        }

        for (var j = thingList.Count - 1; j >= 0; j--)
        {
            thingList[j].Destroy();
        }

        map.terrainGrid.SetTerrain(c, BaseGenUtility.CorrespondingTerrainDef(stuffDef, true));
        var newThing = ThingMaker.MakeThing(ThingDefOf.Wall, stuffDef);
        GenSpawn.Spawn(newThing, c, map);
    }

    private static void DestroyAllAtLocation(IntVec3 c, Map map)
    {
        var thingList = c.GetThingList(map);
        foreach (var thing in thingList)
        {
            if (!thing.def.destroyable)
            {
                return;
            }
        }

        for (var j = thingList.Count - 1; j >= 0; j--)
        {
            thingList[j].Destroy();
        }
    }

    private static void TryToSetFloorTile(IntVec3 c, Map map, TerrainDef floorDef)
    {
        var thingList = c.GetThingList(map);
        foreach (var thing in thingList)
        {
            if (!thing.def.destroyable)
            {
                return;
            }
        }

        for (var j = thingList.Count - 1; j >= 0; j--)
        {
            thingList[j].Destroy();
        }

        map.terrainGrid.SetTerrain(c, floorDef);
    }

    private static void TryMakeDoor(IntVec3 c, Map map, ThingDef doorStuff = null)
    {
        doorStuff ??= BaseGenUtility.RandomCheapWallStuff(
            Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer));

        var thingList = c.GetThingList(map);
        foreach (var thing in thingList)
        {
            if (!thing.def.destroyable)
            {
                return;
            }
        }

        for (var j = thingList.Count - 1; j >= 0; j--)
        {
            thingList[j].Destroy();
        }

        var newThing = ThingMaker.MakeThing(ThingDefOf.Door, doorStuff);
        GenSpawn.Spawn(newThing, c, map);
    }

    public static void UnfogFromRandomEdge(Map map)
    {
        map.fogGrid.Refog(new CellRect(0, 0, map.Size.x, map.Size.z));
        //FogAll(map);
        MapGenerator.rootsToUnfog.Clear();
        foreach (var pawn in map.mapPawns.FreeColonists)
        {
            MapGenerator.rootsToUnfog.Add(pawn.Position);
        }

        MapGenerator.rootsToUnfog.Add(CellFinderLoose.RandomCellWith(
            loc => loc.Standable(map) &&
                   (loc.x < 4 || loc.z < 4 || loc.x > map.Size.x - 5 || loc.z > map.Size.z - 5), map));
        foreach (var root in MapGenerator.rootsToUnfog)
        {
            FloodFillerFog.FloodUnfog(root, map);
        }
    }

    //private static void FogAll(Map map)
    //{
    //    var fogGrid = map.fogGrid;
    //    if (fogGrid == null)
    //    {
    //        return;
    //    }

    //    var cellIndices = map.cellIndices;
    //    if (fogGrid.fogGrid == null)
    //    {
    //        fogGrid.fogGrid = new bool[cellIndices.NumGridCells];
    //    }

    //    foreach (var c in map.AllCells)
    //    {
    //        fogGrid.fogGrid[cellIndices.CellToIndex(c)] = true;
    //    }

    //    if (Current.ProgramState == ProgramState.Playing)
    //    {
    //        map.roofGrid.Drawer.SetDirty();
    //    }
    //}

    public static ThingDef RandomExpensiveWallStuff(Faction faction, bool notVeryFlammable = true)
    {
        ThingDef result;
        if (faction != null && faction.def.techLevel.IsNeolithicOrWorse())
        {
            result = ThingDefOf.WoodLog;
        }
        else
        {
            result = (from d in DefDatabase<ThingDef>.AllDefsListForReading
                where d.IsStuff && d.stuffProps.CanMake(ThingDefOf.Wall) &&
                      (!notVeryFlammable || d.BaseFlammability < 0.5f) && d.BaseMarketValue / d.VolumePerUnit > 8f
                select d).RandomElement();
        }

        return result;
    }

    public static void PushDoor(IntVec3 loc)
    {
        DoorPosList.Add(loc);
    }

    public static void MakeDoors(ResolveParams rp, Map map)
    {
        foreach (var intVec in DoorPosList)
        {
            DestroyAllAtLocation(intVec, map);
            var thing = ThingMaker.MakeThing(ThingDefOf.Door, rp.wallStuff);
            if (map.ParentFaction != null)
            {
                thing.SetFaction(map.ParentFaction);
            }
            else
            {
                var faction = Find.FactionManager.RandomEnemyFaction(true, false, true, TechLevel.Spacer);
                thing.SetFaction(faction);
            }

            GenSpawn.Spawn(thing, intVec, map);
        }

        DoorPosList.Clear();
    }
}