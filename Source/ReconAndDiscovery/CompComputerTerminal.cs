using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class CompComputerTerminal : ThingComp
{
    public ActivatedActionDef actionDef;

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        var list = base.CompFloatMenuOptions(selPawn).ToList();
        if (actionDef != null && parent.GetComp<CompPowerTrader>().PowerOn)
        {
            list.Add(new FloatMenuOption("RD_UseComputer".Translate(),
                delegate { selPawn.jobs.TryTakeOrderedJob(UseComputerJob()); }));
        }

        return list;
    }

    private Job UseComputerJob()
    {
        return new Job(JobDefOfReconAndDiscovery.RD_UseComputer, parent);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Defs.Look(ref actionDef, "actionDef");
    }
}