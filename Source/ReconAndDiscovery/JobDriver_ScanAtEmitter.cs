using System.Collections.Generic;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_ScanAtEmitter : JobDriver
{
    private const TargetIndex CorpseIndex = TargetIndex.A;

    private const TargetIndex GraveIndex = TargetIndex.B;

    public JobDriver_ScanAtEmitter()
    {
        rotateToFace = TargetIndex.B;
    }

    private Corpse Corpse => (Corpse)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    private HoloEmitter Emitter => (HoloEmitter)pawn.CurJob.GetTarget(TargetIndex.B).Thing;

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
        var t2 = Toils_General.Wait(1000);
        t2.AddFailCondition(() => !Emitter.GetComp<CompPowerTrader>().PowerOn);
        t2 = t2.WithProgressBar(TargetIndex.A, () => (1000f - ticksLeftThisToil) / 1000f);
        yield return t2;
        yield return new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant,
            initAction = delegate { Emitter.GetComp<CompHoloEmitter>().Scan(Corpse); }
        };
        yield return Toils_Reserve.Release(TargetIndex.B);
    }
}