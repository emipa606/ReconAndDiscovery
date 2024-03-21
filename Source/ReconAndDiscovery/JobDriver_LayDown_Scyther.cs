using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_LayDown_Scyther : JobDriver_LayDown
{
    public override string GetReport()
    {
        if (asleep)
        {
            return "ReportSleeping".Translate();
        }

        if (GetActor().RaceProps.IsMechanoid)
        {
            return "RD_EnteringRepairCycle".Translate(); //"Entering Repair Cycle"
        }

        return base.GetReport();
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        return GetActor().RaceProps.IsMechanoid ? MechToils() : base.MakeNewToils();
    }

    private IEnumerable<Toil> MechToils()
    {
        var toil = Toils_General.Wait(6000);
        toil.socialMode = RandomSocialMode.Off;
        toil.initAction = delegate
        {
            ticksLeftThisToil = 6000;
            var firstBuilding = GetActor().Position.GetFirstBuilding(GetActor().Map);
            if (firstBuilding is Building_Bed)
            {
                //TODO: check if it works
                JobMaker.MakeJob(JobDefOf.LayDown, firstBuilding);

                //base.GetActor().jobs.curDriver.layingDown = 2;
            }
        };
        toil.tickAction = delegate
        {
            if (!Rand.Chance(0.0004f))
            {
                return;
            }

            var injuriesTendable = GetActor().health.hediffSet.GetHediffsTendable();
            if (!injuriesTendable.Any())
            {
                return;
            }

            var hediff_Injury = injuriesTendable.RandomElement();
            hediff_Injury.Heal(Rand.RangeInclusive(1, 3));
        };
        toil.AddEndCondition(() => !GetActor().health.hediffSet.GetHediffsTendable().Any()
            ? JobCondition.Succeeded
            : JobCondition.Ongoing);
        yield return toil;
    }
}