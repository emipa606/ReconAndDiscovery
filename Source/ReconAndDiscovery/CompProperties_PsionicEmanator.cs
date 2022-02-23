using Verse;

namespace ReconAndDiscovery;

public class CompProperties_PsionicEmanator : CompProperties
{
    public float tickCharge = 0.5f;

    public CompProperties_PsionicEmanator()
    {
        compClass = typeof(CompPsionicEmanator);
    }
}