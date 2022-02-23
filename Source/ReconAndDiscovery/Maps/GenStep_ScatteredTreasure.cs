using RimWorld;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_ScatteredTreasure : GenStep
{
    public override int SeedPart => 319964010;

    public override void Generate(Map map, GenStepParams parms)
    {
        var num = Mathf.Min(18000f, Mathf.Max(Mathf.Exp(Rand.Gaussian(8f, 0.65f))));
        var value = default(ThingSetMakerParams);
        value.techLevel = TechLevel.Spacer;
        var count = Rand.RangeInclusive(2, 10);
        value.countRange = new IntRange(count, count);
        value.totalMarketValueRange = new FloatRange(num, num);
        value.validator = t => t.defName != "Silver";
        if (num > 10000f)
        {
            value.countRange = new IntRange(1, 1);
        }

        var list = ThingSetMakerDefOf.Reward_ItemsStandard.root.Generate(value);
        foreach (var thing in list)
        {
            if (thing.stackCount > thing.def.stackLimit)
            {
                thing.stackCount = thing.def.stackLimit;
            }

            if (CellFinderLoose.TryGetRandomCellWith(
                    x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount >= 2, map, 1000, out var intVec))
            {
                // TODO: check if it works
                GenSpawn.Spawn(thing, intVec, map, Rot4.Random);
            }
        }
    }
}