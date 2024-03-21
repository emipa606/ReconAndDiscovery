using System.Collections.Generic;
using ReconAndDiscovery.DefOfs;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_ActivateOsirisCasket : JobDriver
{
    private const TargetIndex CasketIndex = TargetIndex.A;

    public JobDriver_ActivateOsirisCasket()
    {
        rotateToFace = TargetIndex.A;
    }

    private OsirisCasket Casket => (OsirisCasket)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
        var tFuel = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        tFuel.AddFailCondition(() => Casket.GetComp<CompRefuelable>().Fuel < 50f);
        tFuel.AddFailCondition(() => !Casket.GetComp<CompPowerTrader>().PowerOn);
        tFuel.initAction = delegate { Casket.GetComp<CompRefuelable>().ConsumeFuel(25f); };
        yield return tFuel;
        var t2 = Toils_General.Wait(6000);
        t2.AddFailCondition(() => !Casket.GetComp<CompPowerTrader>().PowerOn);
        t2.initAction = delegate { Casket.Map.weatherManager.TransitionTo(WeatherDef.Named("RainyThunderstorm")); };
        t2 = t2.WithProgressBar(TargetIndex.A, () => (6000f - ticksLeftThisToil) / 6000f);
        yield return t2;
        yield return new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant,
            initAction = delegate
            {
                GenExplosion.DoExplosion(Casket.Position, Casket.Map, 50f, DamageDefOf.EMP, Casket,
                    -1, -1f, Sounds.EnergyShield_Broken);
                Casket.GetComp<CompOsiris>().HealContained();
                Casket.Map.weatherManager.TransitionTo(WeatherDef.Named("Rain"));
                var incidentParms =
                    StorytellerUtility.DefaultParmsNow(IncidentCategories.FactionArrival, Casket.Map);
                incidentParms.forced = true;
                incidentParms.target = Casket.Map;
                var qi = new QueuedIncident(
                    new FiringIncident(IncidentDef.Named("ShortCircuit"), null, incidentParms),
                    Find.TickManager.TicksGame + 1);
                Find.Storyteller.incidentQueue.Add(qi);
            }
        };
        yield return Toils_Reserve.Release(TargetIndex.A);
    }
}