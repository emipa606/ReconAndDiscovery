using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_HoloDisk : SitePartWorker
{
    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        var enumerable = from p in map.mapPawns.AllPawnsSpawned
            where p.Faction == Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer)
            select p;
        foreach (var pawn in enumerable)
        {
            pawn.Destroy();
        }

        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount <= 30, map, out var loc))
        {
            return;
        }

        var newThing = ThingMaker.MakeThing(ThingDef.Named("RD_HoloDisk"));
        GenSpawn.Spawn(newThing, loc, map);
    }
}