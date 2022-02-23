using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver
{
    public class SymbolResolver_WireOutline : RimWorld.BaseGen.SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            var chanceToSkipWallBlock = rp.chanceToSkipWallBlock;
            var chance = chanceToSkipWallBlock ?? 0f;
            foreach (var loc in rp.rect.EdgeCells)
            {
                if (Rand.Chance(chance))
                {
                    continue;
                }

                var powerConduit = ThingDefOf.PowerConduit;
                var newThing = ThingMaker.MakeThing(powerConduit);
                GenSpawn.Spawn(newThing, loc, BaseGen.globalSettings.map);
            }
        }
    }
}