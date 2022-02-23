using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery;

public class RoomStructure
{
    public readonly int doorE = 0;

    public readonly int doorN = 0;

    public readonly int doorS = 0;

    public readonly int doorW = 0;

    public readonly float floorChance = 1f;

    public TerrainDef floorMaterial = BaseGenUtility.RandomBasicFloorDef(Faction.OfMechanoids);

    public float roofChance = 1f;

    public float wallE = 1f;

    public ThingDef wallMaterial =
        BaseGenUtility.RandomCheapWallStuff(
            Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer));

    public float wallN = 1f;

    public float wallS = 1f;

    public float wallW = 1f;

    public void damage()
    {
        var value = Rand.Value;
        if (value < 0.25)
        {
            wallN = 0f;
        }
        else if (value < 0.5)
        {
            wallS = 0f;
        }
        else if (value < 0.75)
        {
            wallS = 0f;
        }
        else
        {
            wallS = 0f;
        }
    }

    public void delapidate()
    {
        var num = 0.2f + (0.6f * Rand.Value);
        wallN = num - 0.2f + (0.4f * Rand.Value);
        wallS = num - 0.2f + (0.4f * Rand.Value);
        wallE = num - 0.2f + (0.4f * Rand.Value);
        wallW = num - 0.2f + (0.4f * Rand.Value);
    }
}