using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
    public class JobDriver_PrayAtObject : JobDriver
    {
        private const TargetIndex GateIndex = TargetIndex.A;

        public JobDriver_PrayAtObject()
        {
            rotateToFace = TargetIndex.A;
        }

        private Building Building => (Building) pawn.CurJob.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A, 4, 0);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            var t2 = Toils_General.Wait(6000);
            t2.AddFailCondition(() => Building.GetComp<CompWeatherSat>() == null);
            t2 = t2.WithEffect(EffecterDefOf.Research, TargetIndex.A);
            t2.tickAction = delegate
            {
                var num = 0.0002f;
                num *= 1f + (0.5f * GetActor().story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity")));
                var comp = Building.GetComp<CompWeatherSat>();
                if (comp == null)
                {
                    return;
                }
                
                comp.mana += num;
                if (comp.mana < 0f)
                {
                    comp.mana = 0f;
                }

                if (comp.mana > 100f)
                {
                    comp.mana = 100f;
                }
            };
            yield return t2;
            yield return Toils_Reserve.Release(TargetIndex.A);
        }
    }
}