using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_Generator : SitePartWorker
{
    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(x => x.Standable(map) && !x.Fogged(map), map,
                out var loc))
        {
            return;
        }

        var newThing = ThingMaker.MakeThing(ThingDefOf.GeothermalGenerator);
        GenSpawn.Spawn(newThing, loc, map);
    }
}