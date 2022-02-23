using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_PsionicEmanator : SitePartWorker
{
    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount <= 30, map, out var loc))
        {
            return;
        }

        var newThing = ThingMaker.MakeThing(ThingDef.Named("RD_PsionicEmanator"));
        GenSpawn.Spawn(newThing, loc, map);
    }
}