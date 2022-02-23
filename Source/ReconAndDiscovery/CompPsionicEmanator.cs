using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery;

[StaticConstructorOnStartup]
public class CompPsionicEmanator : ThingComp
{
    public void DoPsychicShockwave()
    {
        var enumerable = from pawn in parent.Map.mapPawns.AllPawnsSpawned
            where pawn.HostileTo(parent.Faction)
            select pawn;
        foreach (var pawn2 in enumerable)
        {
            if (!pawn2.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity")))
            {
                continue;
            }

            if (pawn2.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity")))
            {
                if (!pawn2.health.hediffSet.HasHediff(HediffDefOf.PsychicShock))
                {
                    pawn2.health.AddHediff(HediffDefOf.PsychicShock);
                }
            }
            else if (!pawn2.health.hediffSet.HasHediff(HediffDef.Named("RD_PsychicAttack")))
            {
                pawn2.health.AddHediff(HediffDef.Named("RD_PsychicAttack"));
            }
        }
    }

    public override string CompInspectStringExtra()
    {
        return "RD_AwaitingPrayersSacrifices".Translate(); //"Awaiting prayers and sacrifices."
    }

    public void DoBattlePrayer()
    {
        IEnumerable<Pawn> freeColonistsSpawned = parent.Map.mapPawns.FreeColonistsSpawned;
        foreach (var pawn in freeColonistsSpawned)
        {
            if (!pawn.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity")))
            {
                continue;
            }

            if (pawn.health.hediffSet.HasHediff(HediffDef.Named("RD_BattlePrayer")))
            {
                var other = HediffMaker.MakeHediff(HediffDef.Named("RD_BattlePrayer"), pawn);
                pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RD_BattlePrayer"))
                    .TryGetComp<HediffComp_Disappears>().CompPostMerged(other);
            }
            else
            {
                pawn.health.AddHediff(HediffDef.Named("RD_BattlePrayer"));
            }
        }
    }
}