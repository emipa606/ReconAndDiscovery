using RimWorld.BaseGen;

namespace ReconAndDiscovery.Maps.SymbolResolver
{
    public class SymbolResolver_TriangularRoom : RimWorld.BaseGen.SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;
            MapGenUtility.MakeTriangularRoom(map, rp);
        }
    }
}