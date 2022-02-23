using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_InsertFurnishing : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        var stuff = rp.wallStuff ?? ThingDefOf.Steel;
        var thingRot = rp.thingRot;
        var rot = thingRot ?? Rot4.East;
        if (rp.singleThingDef.rotatable)
        {
            if (rp.singleThingDef.MadeFromStuff)
            {
                var thing = ThingMaker.MakeThing(rp.singleThingDef, stuff);
                GenSpawn.Spawn(thing, rp.rect.RandomCell, BaseGen.globalSettings.map, rot);
            }
            else
            {
                var thing2 = ThingMaker.MakeThing(rp.singleThingDef);
                GenSpawn.Spawn(thing2, rp.rect.RandomCell, BaseGen.globalSettings.map, rot);
            }
        }
        else if (rp.singleThingDef.MadeFromStuff)
        {
            var thing3 = ThingMaker.MakeThing(rp.singleThingDef, stuff);
            GenSpawn.Spawn(thing3, rp.rect.RandomCell, BaseGen.globalSettings.map, rot);
        }
        else
        {
            var thing4 = ThingMaker.MakeThing(rp.singleThingDef);
            GenSpawn.Spawn(thing4, rp.rect.RandomCell, BaseGen.globalSettings.map, rot);
        }
    }
}