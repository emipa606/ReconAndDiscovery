using Verse;

namespace ReconAndDiscovery;

public class CompProperties_Stargate : CompProperties
{
    public float tickCharge = 0.5f;

    public CompProperties_Stargate()
    {
        compClass = typeof(CompStargate);
    }
}