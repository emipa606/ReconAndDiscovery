using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_TravelThroughStargate : JobDriver
{
    private const TargetIndex StargateIndex = TargetIndex.A;

    public JobDriver_TravelThroughStargate()
    {
        rotateToFace = TargetIndex.A;
    }

    private Building Stargate => (Building)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

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
                var comp = Stargate.GetComp<CompStargate>();
                comp.SendPawnThroughStargate(GetActor());
            }
        };
    }
}