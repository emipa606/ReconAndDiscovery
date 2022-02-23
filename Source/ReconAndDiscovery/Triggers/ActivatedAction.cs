using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers;

public class ActivatedAction
{
    public ActivatedActionDef def;

    public virtual bool TryAction(Pawn activatedBy, Map map, Thing trigger)
    {
        if (def.numThingsToSpawn.max > 0 && def.spawnedThings != null && def.spawnedThings.Count > 0)
        {
            SpawnThings();
        }

        if (def.numPawnsToSpawn.max > 0 && def.pawnKinds != null && def.pawnKinds.Count > 0)
        {
            SpawnPawns();
        }

        DoAnyFurtherActions(activatedBy, map, trigger);
        if (def.hasDialog)
        {
            DisplayDialog(activatedBy, map, trigger);
        }

        return true;
    }

    protected virtual void DisplayDialog(Pawn activatedBy, Map map, Thing trigger)
    {
        var diaNode = new DiaNode(def.dialogText);
        var diaOption = new DiaOption("OK".Translate())
        {
            resolveTree = true
        };
        diaNode.options.Add(diaOption);
        Find.WindowStack.Add(new Dialog_NodeTree(diaNode));
    }

    protected IEnumerable<IntVec3> GetEffectArea(IntVec3 centre)
    {
        return GenRadial.RadialCellsAround(centre, def.effectRadius, true);
    }

    protected virtual void DoAnyFurtherActions(Pawn activatedBy, Map map, Thing trigger)
    {
    }

    private void SpawnThings()
    {
        var randomInRange = def.numThingsToSpawn.RandomInRange;
        for (var i = 0; i < randomInRange; i++)
        {
            var thingDef = def.spawnedThings.RandomElement();
            var thing = ThingMaker.MakeThing(thingDef);
            thing.stackCount = Math.Min(thingDef.stackLimit, def.spawnedThingStackSize.RandomInRange);
        }
    }

    private void SpawnPawns()
    {
        var randomInRange = def.numPawnsToSpawn.RandomInRange;
        var pawnKindDef = def.pawnKinds.RandomElement();
        for (var i = 0; i < randomInRange; i++)
        {
            Faction faction = null;
            if (!pawnKindDef.RaceProps.Animal)
            {
                faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
            }

            var request = new PawnGenerationRequest(pawnKindDef, faction, PawnGenerationContext.NonPlayer, -1,
                false, false, false, false, true, false, 1f, false, true, true, false);
            var pawn = PawnGenerator.GeneratePawn(request);
            if (!pawn.RaceProps.Humanlike)
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, true);
            }
            else if (pawn.Faction == null || !pawn.Faction.HostileTo(Faction.OfPlayer))
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, true);
            }
        }
    }
}