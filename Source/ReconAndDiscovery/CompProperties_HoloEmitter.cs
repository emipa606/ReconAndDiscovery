using Verse;

namespace ReconAndDiscovery;

public class CompProperties_HoloEmitter : CompProperties
{
    public float tickCharge = 0.5f;

    public CompProperties_HoloEmitter()
    {
        compClass = typeof(CompHoloEmitter);
    }
}