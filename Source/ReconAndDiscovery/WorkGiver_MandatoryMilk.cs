using RimWorld;
using Verse;

namespace ReconAndDiscovery;

public class WorkGiver_MandatoryMilk : WorkGiver_GatherAnimalBodyResources
{
    protected override JobDef JobDef => JobDefOfReconAndDiscovery.RD_MandatoryMilk;

    protected override CompHasGatherableBodyResource GetComp(Pawn animal)
    {
        return animal.TryGetComp<CompMandatoryMilkable>();
    }
}