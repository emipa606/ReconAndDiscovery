using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_Stargate : SitePartWorker
{
    public static readonly List<Pawn> tmpPawnsToSpawn = new List<Pawn>();

    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        var enumerable = from p in map.mapPawns.AllPawnsSpawned
            where p.Faction == Find.FactionManager.RandomNonHostileFaction(true, false, true, TechLevel.Spacer)
            select p;
        foreach (var pawn in enumerable)
        {
            pawn.Destroy();
        }

        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(x => x.Standable(map) && !x.Fogged(map), map,
                out var loc))
        {
            return;
        }

        var newThing = ThingMaker.MakeThing(ThingDef.Named("RD_Stargate"));
        GenSpawn.Spawn(newThing, loc, map);
        foreach (var pawn2 in tmpPawnsToSpawn)
        {
            if (pawn2.Spawned)
            {
                pawn2.DeSpawn();
            }

            GenSpawn.Spawn(pawn2, loc, map);
        }

        tmpPawnsToSpawn.Clear();
    }
}