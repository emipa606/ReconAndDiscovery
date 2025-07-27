using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_WallDoor : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        if (rp.rect is { Width: > 1, Height: > 1 })
        {
            return;
        }

        rp.wallStuff ??= BaseGenUtility.RandomCheapWallStuff(Faction.OfPlayer);

        var randomCell = rp.rect.RandomCell;
        TryPlaceDoor(randomCell);
    }

    private void TryPlaceDoor(IntVec3 loc)
    {
        MapGenUtility.PushDoor(loc);
    }

    private bool IsOutdoorsAt(IntVec3 c)
    {
        var map = BaseGen.globalSettings.map;
        return c.GetRegion(map) != null && c.GetRegion(map).Room.PsychologicallyOutdoors;
    }
}