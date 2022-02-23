using Verse;

namespace ReconAndDiscovery;

public class CompProperties_Osiris : CompProperties
{
    public float tickCharge = 0.5f;

    public CompProperties_Osiris()
    {
        compClass = typeof(CompOsiris);
    }
}