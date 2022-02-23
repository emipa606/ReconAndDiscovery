using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_SacrificeAtAltar : JobDriver
{
    private const TargetIndex AnimalIndex = TargetIndex.A;

    private const TargetIndex AltarIndex = TargetIndex.B;

    public JobDriver_SacrificeAtAltar()
    {
        rotateToFace = TargetIndex.B;
    }

    private Pawn Animal => (Pawn)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    private Building Altar => (Building)pawn.CurJob.GetTarget(TargetIndex.B).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, errorOnFailed: errorOnFailed) &&
               pawn.Reserve(job.targetB, job, errorOnFailed: errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Reserve.Reserve(TargetIndex.B);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
        yield return Toils_Haul.StartCarryThing(TargetIndex.A);
        yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
        yield return Toils_General.Wait(1500)
            .WithProgressBar(TargetIndex.B, () => 1f - (1f * ticksLeftThisToil / 1500f));
        yield return new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant,
            initAction = delegate
            {
                var value = new DamageInfo(DamageDefOf.ExecutionCut, 500, -1f, -1f, GetActor());
                Animal.Kill(value);
                Altar.GetComp<CompPsionicEmanator>().DoPsychicShockwave();
            }
        };
        yield return Toils_Reserve.Release(TargetIndex.B);
    }
}