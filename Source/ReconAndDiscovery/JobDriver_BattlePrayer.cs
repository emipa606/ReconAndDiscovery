using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_BattlePrayer : JobDriver
{
    private const TargetIndex GateIndex = TargetIndex.A;

    public JobDriver_BattlePrayer()
    {
        rotateToFace = TargetIndex.A;
    }

    private Building Building => (Building)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Reserve.Reserve(TargetIndex.A, 4, 0);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        var t2 = Toils_General.Wait(10000);
        t2.tickAction = delegate
        {
            if (ticksLeftThisToil % 100 == 50)
            {
                Building.GetComp<CompPsionicEmanator>().DoBattlePrayer();
            }
        };
        yield return t2;
        yield return Toils_Reserve.Release(TargetIndex.A);
    }
}