using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_Computer : SitePartWorker
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

        var thingDef = ThingDef.Named("RD_QuestComputerTerminal");
        var thing = ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef));
        if (action != null)
        {
            ((Building)thing).GetComp<CompComputerTerminal>().actionDef = action;
        }

        GenSpawn.Spawn(thing, loc, map);
    }
}