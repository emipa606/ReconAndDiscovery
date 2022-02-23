using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps;

public class SitePartWorker_RadioactiveDust : SitePartWorker
{
    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        var list = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant).ToList();
        foreach (var thing in list)
        {
            thing.Destroy();
        }

        var gameCondition = GameConditionMaker.MakeCondition(GameConditionDef.Named("RD_Radiation"), 3000000);
        map.gameConditionManager.RegisterCondition(gameCondition);
    }
}