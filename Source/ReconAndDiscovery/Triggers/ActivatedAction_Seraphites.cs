using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers;

public class ActivatedAction_Seraphites : ActivatedAction
{
    protected override void DisplayDialog(Pawn activatedBy, Map map, Thing trigger)
    {
        var goodChoice = Rand.Value > 0.4f;
        var level = activatedBy.skills.GetSkill(SkillDefOf.Intellectual).Level;
        var diaNode = new DiaNode("");
        var diaOption = new DiaOption("RD_LogOff".Translate(activatedBy.Named("PAWN")))
        {
            resolveTree = true
        };
        var diaOption2 = new DiaOption("OK".Translate(activatedBy.Named("PAWN")))
        {
            resolveTree = true
        };
        diaNode.options.Add(diaOption);
        if (level < 5)
        {
            diaNode.text =
                "RD_FilesPertanentLuciferiumCure"
                    .Translate(activatedBy
                        .Label); //"There seem to be files pertanent to a luciferium cure, but {0} lacks the intellectual skills to access them."
        }
        else
        {
            diaNode.text = def.dialogText;
            if (level >= 12)
            {
                if (goodChoice)
                {
                    diaNode.text +=
                        "RD_ThinksScienceUnderlying".Translate(
                            activatedBy.Named(
                                "PAWN")); //"\n{0} thinks that the science underlying the project is sound, and though the process has been lost, any remaining samples of the cure probably work."
                }
                else
                {
                    diaNode.text +=
                        "RD_ThinksResearchDubious".Translate(
                            activatedBy.Named(
                                "PAWN")); //"\n{0} thinks that the research is dubious, and is concerned that any remaining samples will be dangerous.
                }
            }

            var diaOption3 = new DiaOption("RD_DispenseTrialCure".Translate()); //Dispense the trial cure
            if (goodChoice)
            {
                diaOption3.link = new DiaNode(
                    "RD_GoldenFoilPacket".Translate() //"A small, golden foil packet drops out of a nearby hatch!"
                )
                {
                    options =
                    {
                        diaOption2
                    }
                };
                diaOption3.action = delegate
                {
                    var thing = ThingMaker.MakeThing(ThingDefOfReconAndDiscovery.RD_Seraphites);
                    thing.stackCount = Rand.RangeInclusive(1, 3);
                    GenSpawn.Spawn(thing, activatedBy.Position, map);
                    var list = new List<Thing>();
                    list.AddRange(map.listerThings.ThingsOfDef(ThingDef.Named("RD_QuestComputerTerminal")));
                    foreach (var thing2 in list)
                    {
                        thing2.Destroy(DestroyMode.Deconstruct);
                    }
                };
            }
            else
            {
                diaOption3.action = delegate
                {
                    foreach (var c in GetEffectArea(activatedBy.Position))
                    {
                        foreach (var thing in c.GetThingList(map))
                        {
                            if (!Rand.Chance(0.9f) || thing.def.category != ThingCategory.Pawn ||
                                !((Pawn)thing).RaceProps.Humanlike)
                            {
                                continue;
                            }

                            if (thing is not Pawn pawn)
                            {
                                continue;
                            }

                            var hediff = HediffMaker.MakeHediff(HediffDef.Named("FibrousMechanites"), pawn);
                            pawn.health.AddHediff(hediff);
                        }
                    }

                    var list = new List<Thing>();
                    list.AddRange(map.listerThings.ThingsOfDef(ThingDef.Named("RD_QuestComputerTerminal")));
                    foreach (var thing2 in list)
                    {
                        thing2.Destroy(DestroyMode.Deconstruct);
                    }
                };
                diaOption3.link = new DiaNode(
                    "RD_MechanitesCureCorrupted"
                        .Translate() //"The mechanites in the cure have corrupted their programming and escaped their packaging!"
                )
                {
                    options =
                    {
                        diaOption2
                    }
                };
            }

            diaNode.options.Add(diaOption3);
        }

        Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true));
    }
}