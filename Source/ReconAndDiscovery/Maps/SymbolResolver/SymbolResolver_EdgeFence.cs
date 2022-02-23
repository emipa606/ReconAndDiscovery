using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver
{
    public class SymbolResolver_EdgeFence : RimWorld.BaseGen.SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;
            var rect = rp.rect;
            if (rp.wallStuff == null)
            {
                rp.wallStuff = BaseGenUtility.RandomCheapWallStuff(Faction.OfPlayer);
            }

            var num = -1;
            foreach (var loc in rect.EdgeCells)
            {
                num++;
                if (num % 3 != 0)
                {
                    continue;
                }

                var wall = ThingDefOf.Wall;
                var newThing = ThingMaker.MakeThing(wall, rp.wallStuff);
                GenSpawn.Spawn(newThing, loc, map);
            }
        }
    }
}