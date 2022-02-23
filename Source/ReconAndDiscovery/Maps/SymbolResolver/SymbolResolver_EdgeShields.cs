using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_EdgeShields : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        var map = BaseGen.globalSettings.map;
        var rect = rp.rect;
        if (rp.wallStuff == null)
        {
            rp.wallStuff = BaseGenUtility.RandomCheapWallStuff(Faction.OfPlayer);
        }

        var num = 1;
        foreach (var loc in rect.EdgeCells)
        {
            var def = ThingDefOf.Wall;
            var newThing = ThingMaker.MakeThing(def, rp.wallStuff);
            if (num % 3 == 0)
            {
                def = ThingDefOf.Sandbags;
                newThing = ThingMaker.MakeThing(def, GenStuff.DefaultStuffFor(def));
            }

            num++;
            GenSpawn.Spawn(newThing, loc, map);
        }
    }
}