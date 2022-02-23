using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

public class WorkGiver_Sacrifice : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest =>
        ThingRequest.ForDef(ThingDef.Named("RD_PsionicEmanator"));

    public override bool Prioritized => true;

    public override float GetPriority(Pawn pawn, TargetInfo t)
    {
        return 0f;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing psionicEmanator, bool forced = false)
    {
        if (psionicEmanator is not Building building_PsionicEmanator)
        {
            return false;
        }

        if (!pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Slaughter)
                .Any(possibleVictims => pawn.CanReserve(possibleVictims.target, 1, 1, null, forced))
           )
        {
            return false;
        }

        if (pawn.story == null || !pawn.WorkTagIsDisabled(WorkTags.Social))
        {
            return pawn.CanReserve(building_PsionicEmanator, 1, -1, null, forced);
        }

        JobFailReason.Is("IsIncapableOfViolenceShort".Translate());
        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var psionicEmanator = t as Building;
        if (!pawn.CanReserveAndReach(psionicEmanator, PathEndMode.ClosestTouch, Danger.Some))
        {
            return null;
        }

        LocalTargetInfo victim = null;
        foreach (var possibleVictim in pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Slaughter))
        {
            if (!pawn.CanReserve(possibleVictim.target, 1, 1, null, forced))
            {
                continue;
            }

            victim = possibleVictim.target;
            break;
        }

        if (victim == null)
        {
            return null;
        }

        return new Job(JobDefOfReconAndDiscovery.RD_SacrificeAtAltar, victim, psionicEmanator)
        {
            count = 1
        };
    }
}