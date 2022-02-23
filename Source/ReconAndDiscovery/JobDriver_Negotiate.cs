using System.Collections.Generic;
using ReconAndDiscovery.Missions;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_Negotiate : JobDriver
{
    private Pawn OtherParty => (Pawn)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

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
                Find.WorldObjects.MapParentAt(OtherParty.Map.Tile).GetComponent<QuestComp_PeaceTalks>()
                    .ResolveNegotiations(GetActor(), OtherParty);
            }
        };
    }
}