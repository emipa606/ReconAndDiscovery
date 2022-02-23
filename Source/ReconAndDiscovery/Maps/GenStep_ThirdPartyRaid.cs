using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_ThirdPartyRaid : GenStep
{
    public override int SeedPart => 341541510;

    public override void Generate(Map map, GenStepParams parms)
    {
        if (!map.TileInfo.WaterCovered)
        {
            BaseGen.Generate();
        }
    }

    private void SetAllStructuresToFaction(Faction f, Map m)
    {
        var enumerable = from thing in m.listerThings.AllThings
            where thing.def.IsDoor
            select thing;
        foreach (var thing2 in enumerable)
        {
            thing2.SetFaction(f);
        }

        BaseGen.Generate();
    }
}