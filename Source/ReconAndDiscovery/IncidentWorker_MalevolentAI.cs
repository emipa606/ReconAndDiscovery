using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery;

public class IncidentWorker_MalevolentAI : IncidentWorker
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (parms.target is not Map map)
        {
            return false;
        }

        var activelyHostilePawns = from p in map.mapPawns.AllPawnsSpawned
            where p.Faction.HostileTo(Faction.OfPlayer)
                  && GenHostility.IsActiveThreatTo(p, Faction.OfPlayer)
            select p;

        if (!activelyHostilePawns.Any())
        {
            return false;
        }

        var pawn = activelyHostilePawns.RandomElement();

        var poweredHackableBuildings = (from b in map.listerBuildings.allBuildingsColonist
            where b.def.building.ai_combatDangerous && b.GetComp<CompPowerTrader>() != null &&
                  b.GetComp<CompPowerTrader>().PowerOn
            select b).ToList();
        if (!poweredHackableBuildings.Any())
        {
            return false;
        }

        foreach (var building in poweredHackableBuildings)
        {
            building.SetFaction(pawn.Faction);
        }

        SendStandardLetter(parms, poweredHackableBuildings.FirstOrDefault(), pawn.Faction.Name);

        return true;
    }
}