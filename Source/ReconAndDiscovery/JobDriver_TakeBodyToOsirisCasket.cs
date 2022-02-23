using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_TakeBodyToOsirisCasket : JobDriver
{
    private const TargetIndex CorpseIndex = TargetIndex.A;

    private const TargetIndex GraveIndex = TargetIndex.B;

    public JobDriver_TakeBodyToOsirisCasket()
    {
        rotateToFace = TargetIndex.B;
    }

    private Corpse Corpse => (Corpse)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    private Building_CryptosleepCasket Casket =>
        (Building_CryptosleepCasket)pawn.CurJob.GetTarget(TargetIndex.B).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Reserve.Reserve(TargetIndex.B);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
        yield return Toils_Haul.StartCarryThing(TargetIndex.A);
        yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
        yield return Toils_Haul.DepositHauledThingInContainer(TargetIndex.B, TargetIndex.A);
        yield return Toils_Reserve.Release(TargetIndex.A);
        yield return Toils_Reserve.Release(TargetIndex.B);
    }
}