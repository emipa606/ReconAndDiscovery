using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers;

public class ActionTrigger : Thing
{
    public ActivatedActionDef actionDef;

    public virtual ICollection<IntVec3> Cells { get; } = new List<IntVec3>();

    private void ActivatedBy(Pawn p)
    {
        if (!actionDef.ActivatedAction.TryAction(p, Map, this))
        {
            return;
        }

        if (!Destroyed)
        {
            Destroy();
        }
    }

    public override void ExposeData()
    {
        Scribe_Defs.Look(ref actionDef, "actionDef");
        base.ExposeData();
    }

    public override void Tick()
    {
        Pawn pawn = null;
        if (!this.IsHashIntervalTick(20))
        {
            return;
        }

        var map = Map;
        foreach (var c in Cells)
        {
            var thingList = c.GetThingList(map);
            foreach (var thing in thingList)
            {
                if (thing.def.category != ThingCategory.Pawn ||
                    thing.def.race.intelligence != Intelligence.Humanlike || thing.Faction != Faction.OfPlayer)
                {
                    continue;
                }

                pawn = thing as Pawn;
                break;
            }

            if (pawn != null)
            {
                break;
            }
        }

        if (pawn != null)
        {
            ActivatedBy(pawn);
        }
    }
}