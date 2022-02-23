using RimWorld.BaseGen;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_PathOfDestruction : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        BaseGen.symbolStack.Push("clear", rp);
    }
}