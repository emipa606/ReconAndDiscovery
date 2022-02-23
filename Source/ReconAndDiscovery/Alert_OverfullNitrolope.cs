using RimWorld;
using Verse;

namespace ReconAndDiscovery;

public class Alert_OverfullNitrolope : Alert
{
    public Alert_OverfullNitrolope()
    {
        defaultLabel = "RD_ExplosiveRisk".Translate(); // Explosive risk!
        defaultPriority = AlertPriority.High;
    }

    public override TaggedString GetExplanation()
    {
        string result;
        if (OverfullNitralope() == null)
        {
            result = string.Empty;
        }
        else
        {
            result = "RD_AlertExplanation"
                .Translate(); //A nitralope has become dangerously bloated. You must relieve the pressure by milking it, or it may explode!
        }

        return result;
    }

    public override AlertReport GetReport()
    {
        return OverfullNitralope() != null;
    }

    private Pawn OverfullNitralope()
    {
        var maps = Find.Maps;
        foreach (var map in maps)
        {
            foreach (var pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.Faction != Faction.OfPlayer || pawn.kindDef != ThingDefOfReconAndDiscovery.RD_Nitralope)
                {
                    continue;
                }

                if (pawn.GetComp<CompMandatoryMilkable>().Overfull)
                {
                    return pawn;
                }
            }
        }

        return null;
    }
}