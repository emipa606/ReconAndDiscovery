using Verse;

namespace ReconAndDiscovery;

public class CompProperties_Teleporter : CompProperties
{
    public float tickCharge = 0.005f;

    public CompProperties_Teleporter()
    {
        compClass = typeof(CompTeleporter);
    }
}