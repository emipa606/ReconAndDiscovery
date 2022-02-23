using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class WorkGiver_PsychicPrayer : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDef.Named("RD_PsionicEmanator"));

    public override bool Prioritized => true;

    public override float GetPriority(Pawn pawn, TargetInfo t)
    {
        return 0f;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing pschicEmanator, bool forced = false)
    {
        if (pschicEmanator is not Building)
        {
            return false;
        }

        if (pawn.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity")))
        {
            return pawn.CanReserveAndReach(pschicEmanator, PathEndMode.Touch, Danger.Some, 1, -1, null, forced);
        }

        JobFailReason.Is("RD_OnlyPsychicCanBroadcast"
            .Translate()); //"Only psychic pawns can broadcast a battle prayer"
        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(JobDefOfReconAndDiscovery.RD_PsychicPrayer, t);
    }
}