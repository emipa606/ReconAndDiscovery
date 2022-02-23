using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_Osiris : SitePartWorker
{
    public ActivatedActionDef action;

    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                x => x.Standable(map) && x.Fogged(map) && x.GetRoom(map).CellCount <= 30, map, out var loc))
        {
            return;
        }

        var thing = ThingMaker.MakeThing(ThingDef.Named("RD_OsirisCasket"));
        GenSpawn.Spawn(thing, loc, map);
        var osirisCasket = thing as OsirisCasket;
        //var faction = Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Spacer);
        var thing2 = PawnGenerator.GeneratePawn(PawnKindDefOf.AncientSoldier,
            Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer));
        osirisCasket?.TryAcceptThing(thing2);
    }
}