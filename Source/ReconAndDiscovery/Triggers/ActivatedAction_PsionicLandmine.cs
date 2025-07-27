using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers;

public class ActivatedAction_PsionicLandmine : ActivatedAction
{
    private Pawn bestPsychic;

    protected override void DisplayDialog(Pawn activatedBy, Map map, Thing trigger)
    {
        bestPsychic = PawnTalentUtility.FindBestPsychic(GetEffectArea(activatedBy.Position), map);
        var diaNode =
            new DiaNode("RD_PsionicLandmine"
                .Translate()); //A psionic landmine in the room generates a short-range psychic shock!"); //"A psionic landmine in the room generates a short-range psychic shock!"
        if (bestPsychic != null)
        {
            if (bestPsychic.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity")) == 2)
            {
                diaNode.text +=
                    "RD_FortunatelyDissipateAttack".Translate(
                        bestPsychic.Named(
                            "PAWN")); //" Fortunately, {0} was able to dissipate the attack with their psychic capabilities.
            }
            else
            {
                diaNode.text +=
                    "RD_FortunatelyDissipateAttackShock"
                        .Translate(bestPsychic
                            .Named("PAWN")); //" {0} was able to channel the attack, preventing harm to others, but is now in psychic shock!
            }
        }

        var diaOption = new DiaOption("OK".Translate())
        {
            resolveTree = true
        };
        diaNode.options.Add(diaOption);
        Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true));
    }

    protected override void DoAnyFurtherActions(Pawn activatedBy, Map map, Thing trigger)
    {
        bestPsychic = PawnTalentUtility.FindBestPsychic(GetEffectArea(activatedBy.Position), map);
        if (bestPsychic != null)
        {
            if (bestPsychic.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity")) != 2)
            {
                var hediff = HediffMaker.MakeHediff(HediffDefOf.PsychicShock, bestPsychic);
                bestPsychic.health.AddHediff(hediff);
            }
        }
        else
        {
            var list = new List<Pawn>();
            foreach (var c in GetEffectArea(activatedBy.Position))
            {
                foreach (var thing in c.GetThingList(map))
                {
                    if (thing.def.category != ThingCategory.Pawn ||
                        thing.def.race.intelligence != Intelligence.Humanlike)
                    {
                        continue;
                    }

                    var pawn = thing as Pawn;
                    var num = pawn?.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity"));
                    if (num >= 0)
                    {
                        list.Add(pawn);
                    }
                }
            }

            foreach (var pawn2 in list)
            {
                var value = Rand.Value;
                if (value < 0.25)
                {
                    pawn2.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, true);
                }
                else if (value < 0.85)
                {
                    var hediff2 = HediffMaker.MakeHediff(HediffDefOf.PsychicShock, pawn2);
                    pawn2.health.AddHediff(hediff2);
                }
            }
        }

        if (trigger is not ActionTrigger actionTrigger)
        {
            return;
        }

        IEnumerable<IntVec3> cells = actionTrigger.Cells;
        var center = cells.RandomElement();
        //TODO: check if it works
        GenExplosion.DoExplosion(center, map, 2f, DamageDefOf.Flame, null, -1, -1f, null, null, null, null,
            ThingDefOf.ChunkSlagSteel, 0.4f, 1, null, null, 0, true);
    }
}