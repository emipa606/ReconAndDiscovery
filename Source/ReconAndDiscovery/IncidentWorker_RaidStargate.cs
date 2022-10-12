using System.Linq;
using ExpandedIncidents.Settings;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery;

public class IncidentWorker_RaidStargate : IncidentWorker_RaidEnemy
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var source = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("RD_Stargate"));
        if (!source.Any())
        {
            return false;
        }

        var building = source.FirstOrDefault();
        ResolveRaidPoints(parms);
        if (!TryResolveRaidFaction(parms))
        {
            return false;
        }

        ResolveRaidStrategy(parms, PawnGroupKindDefOf.Combat);
        ResolveRaidArriveMode(parms);
        if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
        {
            return false;
        }

        var defaultPawnGroupMakerParms =
            IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms, true);
        var list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
        if (list.Count == 0)
        {
            RaD_Mod.LogError("Got no pawns spawning raid from parms " + parms);
            return false;
        }

        var target = TargetInfo.Invalid;
        foreach (var pawn in list)
        {
            if (building != null)
            {
                var position = building.Position;
                GenSpawn.Spawn(pawn, position, map, parms.spawnRotation);
            }

            target = pawn;
        }

        // TODO: Check if these parameters are correct in this raid
        var unused = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction),
            map, list);

        // not sure what to write here instead, while commented out
        //AvoidGridMaker.RegenerateAvoidGridsFor(parms.faction, map);

        LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons,
            OpportunityType.Critical);
        if (!PlayerKnowledgeDatabase.IsComplete(ConceptDefOf.ShieldBelts))
        {
            foreach (var pawn2 in list)
            {
                if (!pawn2.apparel.WornApparel.Any(ap => ap.def.HasComp(typeof(CompShield))))
                {
                    continue;
                }

                LessonAutoActivator.TeachOpportunity(ConceptDefOf.ShieldBelts,
                    OpportunityType.Critical);
                break;
            }
        }

        SendStandardLetter(parms, target, parms.faction.Name);
        Find.TickManager.slower.SignalForceNormalSpeedShort();
        Find.StoryWatcher.statsRecord.numRaidsEnemy++;
        return true;
    }
}