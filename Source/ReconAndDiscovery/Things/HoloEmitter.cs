using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReconAndDiscovery.Things;

public class HoloEmitter : Building, IThingHolder
{
    private ThingOwner innerContainer;

    public HoloEmitter()
    {
        innerContainer = new ThingOwner<Thing>(this, false);
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
    }

    public override void TickRare()
    {
        base.TickRare();
        innerContainer ??= new ThingOwner<Thing>(this, false);

        var enumerable = from t in innerContainer
            where t is Corpse
            select t;
        foreach (var thing in enumerable)
        {
            if (thing is not Corpse corpse)
            {
                continue;
            }

            var innerPawn = corpse.InnerPawn;
            if (innerPawn == null)
            {
                break;
            }

            if (!innerPawn.Dead)
            {
                continue;
            }

            CompOsiris.Ressurrect(innerPawn, this);
            var comp = GetComp<CompHoloEmitter>();
            if (comp.SimPawn == innerPawn)
            {
                continue;
            }

            comp.SimPawn = innerPawn;
            comp.MakeValidAllowedZone();
        }
    }

    public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
    {
        if (thing is not Corpse && thing is not Pawn)
        {
            return false;
        }

        if (thing.holdingOwner == null)
        {
            return innerContainer.TryAdd(thing);
        }

        if (thing.holdingOwner.Owner is Map)
        {
            thing.holdingOwner.Remove(thing);
            innerContainer.TryAdd(thing);
            return true;
        }

        thing.holdingOwner.TryTransferToContainer(thing, innerContainer, thing.stackCount);
        return true;
    }
}