using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class WorkGiver_PrayAtObject : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDef.Named("RD_WeatherSat"));

    public override bool Prioritized => true;

    public override float GetPriority(Pawn pawn, TargetInfo t)
    {
        return t.Thing.GetStatValue(StatDefOf.ResearchSpeedFactor);
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return t is Building && pawn.CanReserve(t, 4, -1, null, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(JobDefOfReconAndDiscovery.RD_PrayAtObject, t);
    }
}