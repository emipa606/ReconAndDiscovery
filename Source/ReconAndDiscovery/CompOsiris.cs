using System.Collections.Generic;
using System.Linq;
using ExpandedIncidents.Settings;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

[StaticConstructorOnStartup]
public class CompOsiris : ThingComp
{
    private Building_Casket Casket => parent as Building_Casket;

    private bool ReadyToHeal => Casket.ContainedThing is Pawn || Casket.ContainedThing is Corpse &&
        !(Casket.ContainedThing as Corpse).IsNotFresh() && parent.GetComp<CompPowerTrader>().PowerOn &&
        parent.GetComp<CompRefuelable>().Fuel >= 50f;

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        if (mode == DestroyMode.Deconstruct)
        {
            GenSpawn.Spawn(ThingDef.Named("RD_OsirisAI"), parent.Position, previousMap);
        }
    }

    public static void Ressurrect(Pawn pawn, Thing thing)
    {
        if (thing is HoloEmitter)
        {
            ResurrectionUtility.Resurrect(pawn);
            PawnDiedOrDownedThoughtsUtility.RemoveDiedThoughts(pawn);
            PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn);
            FixPawnRelationships(pawn);
            pawn.health.Reset();
            if (pawn.Corpse is { Spawned: true })
            {
                pawn.Corpse.DeSpawn();
            }

            if (!pawn.Spawned)
            {
                GenSpawn.Spawn(pawn, thing.Position, thing.Map);
            }

            GiveSideEffects(pawn);

            pawn.Corpse?.Destroy();
        }
        else
        {
            ResurrectionUtility.Resurrect(pawn);
            PawnDiedOrDownedThoughtsUtility.RemoveDiedThoughts(pawn);
            FixPawnRelationships(pawn);
            pawn.health.Reset();
            if (pawn.Corpse is { Spawned: true })
            {
                pawn.Corpse.DeSpawn();
            }

            GenSpawn.Spawn(pawn, thing.Position, thing.Map);
            GiveSideEffects(pawn);

            if (thing is Building_Casket building_Casket)
            {
                building_Casket.GetDirectlyHeldThings().Clear();
            }
        }
    }

    public void HealContained()
    {
        if (Casket.ContainedThing == null)
        {
            return;
        }

        Pawn pawn;
        if (Casket.ContainedThing is Corpse corpse)
        {
            pawn = corpse.InnerPawn;
        }
        else
        {
            pawn = Casket.ContainedThing as Pawn;
        }

        if (pawn == null)
        {
            return;
        }

        if (pawn.Dead)
        {
            Ressurrect(pawn, parent);
        }
        else
        {
            pawn.health.Reset();
        }

        if (pawn.RaceProps.Humanlike)
        {
            pawn.ageTracker.AgeBiologicalTicks = 90000000L;
            pawn.needs.mood.thoughts.memories.TryGainMemory(
                Rand.Value < 0.65f
                    ? ThoughtDef.Named("RD_ReturnedFromTheDeadBad")
                    : ThoughtDef.Named("RD_ReturnedFromTheDeadGood"));
        }
        else if (pawn.RaceProps.Animal)
        {
            pawn.ageTracker.AgeBiologicalTicks = (long)(pawn.RaceProps.lifeStageAges[2].minAge * 3600000f);
        }

        pawn.health.AddHediff(HediffDef.Named("LuciferiumAddiction"));
        pawn.health.AddHediff(HediffDef.Named("LuciferiumHigh"));
    }

    private static void FixPawnRelationships(Pawn p)
    {
        foreach (var pawn in PawnsFinder.AllCaravansAndTravelingTransportPods_Alive)
        {
            if (pawn == p)
            {
                continue;
            }

            if (pawn?.needs?.mood?.thoughts?.memories == null)
            {
                continue;
            }

            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDefOf.KnowColonistDied, p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDefOf.PawnWithBadOpinionDied, p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDefOf.PawnWithGoodOpinionDied, p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("BondedAnimalDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MySonDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyDaughterDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyHusbandDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyWifeDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyFianceDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyFianceeDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyLoverDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyBrotherDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MySisterDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyGrandchildDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyFatherDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyMotherDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyNieceDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyNephewDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyHalfSiblingDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyAuntDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyUncleDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyGrandparentDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyCousinDied"), p);
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(
                ThoughtDef.Named("MyKinDied"), p);
        }
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        var list = new List<FloatMenuOption>();
        var floatMenuOption = new FloatMenuOption("RD_ResurrectContained".Translate(), delegate
        {
            var job = new Job(JobDefOfReconAndDiscovery.RD_ActivateOsirisCasket, parent)
            {
                playerForced = true
            };
            selPawn.jobs.TryTakeOrderedJob(job);
        });

        if (ReadyToHeal)
        {
            list.Add(floatMenuOption);
        }

        return list;
    }

    public static bool TryRandomlyMissingColonist(out Pawn pawn)
    {
        var result = false;
        pawn = Find.WorldPawns.AllPawnsDead.Where(x => x.Faction == Faction.OfPlayer && x.Corpse == null)
            .RandomElement();
        if (pawn != null)
        {
            result = true;
        }

        return result;
    }

    private static void GiveSideEffects(Pawn pawn)
    {
        var brain = pawn.health.hediffSet.GetBrain();
        var hediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionSickness, pawn);
        if (!pawn.health.WouldDieAfterAddingHediff(hediff))
        {
            pawn.health.AddHediff(hediff);
        }

        if (Rand.Chance(0.8f) && brain != null)
        {
            var hediff2 = HediffMaker.MakeHediff(HediffDefOf.Dementia, pawn, brain);
            if (!pawn.health.WouldDieAfterAddingHediff(hediff2))
            {
                pawn.health.AddHediff(hediff2);
            }
        }

        if (Rand.Chance(0.8f))
        {
            foreach (var bodyPartRecord in pawn.health.hediffSet.GetNotMissingParts()
                         .Where(x => x.def == BodyPartDefOf.Eye))
            {
                var hediff3 = HediffMaker.MakeHediff(HediffDefOf.Blindness, pawn, bodyPartRecord);
                pawn.health.AddHediff(hediff3);
            }
        }

        if (brain != null && Rand.Chance(0.8f))
        {
            var hediff4 = HediffMaker.MakeHediff(HediffDefOf.ResurrectionPsychosis, pawn, brain);
            if (!pawn.health.WouldDieAfterAddingHediff(hediff4))
            {
                pawn.health.AddHediff(hediff4);
            }
        }

        if (!pawn.Dead)
        {
            return;
        }

        RaD_Mod.LogError("The pawn has died while being resurrected.");
        ResurrectionUtility.Resurrect(pawn);
    }
}