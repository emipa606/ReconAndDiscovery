using RimWorld;
using RimWorld.BaseGen;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_OldMilitaryBase : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        var resolveParams = rp;
        resolveParams.rect = rp.rect.ContractedBy(1);
        resolveParams.wallStuff = ThingDefOf.BlocksGranite;
        resolveParams.SetCustom("minRoomDimension", 6);
        BaseGen.symbolStack.Push("nestedRoomMaze", resolveParams);
        BaseGen.symbolStack.Push("edgeWalls", resolveParams);
        rp.wallStuff = ThingDefOf.Steel;
        BaseGen.symbolStack.Push("edgeWalls", rp);
        BaseGen.symbolStack.Push("floor", rp);
        BaseGen.symbolStack.Push("clear", rp);
    }
}