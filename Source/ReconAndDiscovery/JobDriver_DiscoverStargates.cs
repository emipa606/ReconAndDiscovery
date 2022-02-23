using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class JobDriver_DiscoverStargates : JobDriver
{
    private const TargetIndex GateIndex = TargetIndex.A;

    public JobDriver_DiscoverStargates()
    {
        rotateToFace = TargetIndex.A;
    }

    private Building Stargate => (Building)pawn.CurJob.GetTarget(TargetIndex.A).Thing;

    private void TriggerFindStargate()
    {
        var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.World);
        incidentParms.forced = true;
        var qi = new QueuedIncident(
            new FiringIncident(ThingDefOfReconAndDiscovery.RD_DiscoveredStargate, null, incidentParms),
            Find.TickManager.TicksGame + 100);
        Find.Storyteller.incidentQueue.Add(qi);
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        var t2 = Toils_General.Wait(1000);
        t2 = t2.WithEffect(EffecterDefOf.Research, TargetIndex.A);
        var fResearchScore = GetActor().GetStatValue(StatDefOf.ResearchSpeed);
        t2.tickAction = delegate
        {
            if (!(180000f * Rand.Value < fResearchScore))
            {
                return;
            }

            TriggerFindStargate();
            EndJobWith(JobCondition.Succeeded);
        };
        yield return t2;
        yield return Toils_Reserve.Release(TargetIndex.A);
    }
}