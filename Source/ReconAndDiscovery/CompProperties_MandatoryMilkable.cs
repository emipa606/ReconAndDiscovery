using Verse;

namespace ReconAndDiscovery;

public class CompProperties_MandatoryMilkable : CompProperties
{
    public readonly int milkAmount = 1;

    public readonly bool milkFemaleOnly = true;

    public readonly int ticksUntilDanger = 60000;

    public ThingDef milkDef;

    public int milkIntervalDays;

    public CompProperties_MandatoryMilkable()
    {
        compClass = typeof(CompMandatoryMilkable);
    }
}