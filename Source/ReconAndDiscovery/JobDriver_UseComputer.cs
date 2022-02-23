using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_UseComputer : JobDriver
{
    private Building Computer => (Building)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    public override string GetReport()
    {
        return "RD_UsingComputer".Translate();
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
        yield return new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant,
            initAction = delegate
            {
                Computer.GetComp<CompComputerTerminal>().actionDef.ActivatedAction
                    .TryAction(GetActor(), GetActor().Map, null);
            }
        };
    }
}