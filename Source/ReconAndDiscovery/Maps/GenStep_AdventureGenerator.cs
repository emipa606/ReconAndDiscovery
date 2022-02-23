using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps;

public class GenStep_AdventureGenerator : GenStep
{
    private readonly Dictionary<string, float> randomRoomEvents = new Dictionary<string, float>();
    protected AdventureWorker adventure = null;

    protected CellRect adventureRegion;

    protected ResolveParams baseResolveParams;

    public override int SeedPart => 149641510;

    public override void Generate(Map map, GenStepParams parms)
    {
        var minX = map.Size.x / 5;
        var width = 3 * map.Size.x / 5;
        var minZ = map.Size.z / 5;
        var height = 3 * map.Size.z / 5;
        adventureRegion = new CellRect(minX, minZ, width, height);
        adventureRegion.ClipInsideMap(map);
        BaseGen.globalSettings.map = map;
        randomRoomEvents.Clear();
        CellFinder.TryFindRandomEdgeCellWith(v => v.Standable(map), map, 0f, out var playerStartSpot);
        MapGenerator.PlayerStartSpot = playerStartSpot;
        baseResolveParams = default;
        foreach (var text in randomRoomEvents.Keys)
        {
            baseResolveParams.SetCustom(text, randomRoomEvents[text]);
        }
    }
}