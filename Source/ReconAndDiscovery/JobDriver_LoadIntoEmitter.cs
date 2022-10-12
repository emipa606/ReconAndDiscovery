using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_LoadIntoEmitter : JobDriver
{
    private const TargetIndex CorpseIndex = TargetIndex.A;

    private const TargetIndex GraveIndex = TargetIndex.B;

    public JobDriver_LoadIntoEmitter()
    {
        rotateToFace = TargetIndex.B;
    }

    private Thing Disk => pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    private HoloEmitter Emitter => (HoloEmitter)pawn.CurJob.GetTarget(TargetIndex.B).Thing;

    private Pawn MakeGeniusPawn()
    {
        var request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, Faction.OfPlayer,
            PawnGenerationContext.PlayerStarter, -1, true, false, false, false, false, 0f,
            true, true, true, false);
        var generatePawn = PawnGenerator.GeneratePawn(request);
        var list = new List<Trait>();
        foreach (var trait in generatePawn.story.traits.allTraits)
        {
            if (trait.def == TraitDefOf.Psychopath || trait.def == TraitDefOf.Cannibal ||
                trait.def == TraitDefOf.Pyromaniac)
            {
                list.Add(trait);
            }
        }

        foreach (var item in list)
        {
            generatePawn.story.traits.allTraits.Remove(item);
        }

        var list2 = (from s in generatePawn.skills.skills
            where !s.TotallyDisabled
            select s).ToList();
        var skillRecord = list2.RandomElement();
        skillRecord.Level = 20;
        skillRecord.passion = Passion.Major;
        list2.Remove(skillRecord);
        skillRecord = list2.RandomElement();
        skillRecord.Level = 20;
        skillRecord.passion = Passion.Major;
        return generatePawn;
    }

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
            initAction = delegate
            {
                var simPawn = MakeGeniusPawn();
                Emitter.GetComp<CompHoloEmitter>().SimPawn = simPawn;
                Emitter.GetComp<CompHoloEmitter>().SetUpPawn();
                Disk.Destroy();
            }
        };
        yield return Toils_Reserve.Release(TargetIndex.B);
    }
}