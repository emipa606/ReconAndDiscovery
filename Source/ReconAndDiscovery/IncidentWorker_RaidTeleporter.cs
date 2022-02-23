using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery
{
    public class IncidentWorker_RaidTeleporter : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map) parms.target;
            var activeMechanoids = from p in map.mapPawns.AllPawnsSpawned
                where p.Faction == Faction.OfMechanoids && !p.Downed
                select p;
            
            if (!activeMechanoids.Any())
            {
                return false;
            }

            var activeTeleporters =
                from b in map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("RD_Teleporter"))
                where b.GetComp<CompPowerTrader>().PowerOn && b.GetComp<CompTeleporter>().ReadyToTransport
                select b;
            
            if (!activeTeleporters.Any())
            {
                return false;
            }

            var list = new List<Pawn>();
            var mechanoidLord = activeMechanoids.RandomElement().GetLord();
            foreach (var building in activeTeleporters)
            {
                var pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Mech_Scyther"), Faction.OfMechanoids);
                GenSpawn.Spawn(pawn, building.Position, building.Map);
                building.GetComp<CompTeleporter>().ResetCharge();
                list.Add(pawn);
                mechanoidLord.AddPawn(pawn);
            }

            SendStandardLetter(parms, list.FirstOrDefault());
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;

            return true;
        }
    }
}