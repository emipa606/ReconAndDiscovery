using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;

namespace ReconAndDiscovery;

[StaticConstructorOnStartup]
public class CompHoloEmitter : ThingComp
{
    private Pawn pawn;

    public Pawn SimPawn
    {
        get => pawn;
        set => pawn = value;
    }

    private HoloEmitter Emitter => parent as HoloEmitter;

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        if (pawn == null)
        {
            return;
        }

        var value = new DamageInfo(DamageDefOf.Blunt, 1000, -1f);
        SimPawn.Kill(value);
        SimPawn.Corpse.Destroy();
    }

    public override void PostDeSpawn(Map map)
    {
        if (pawn is { Spawned: true })
        {
            pawn.DeSpawn();
        }

        base.PostDeSpawn(map);
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn actingPawn)
    {
        var list = new List<FloatMenuOption>();
        if (pawn != null)
        {
            var formatPawn = new FloatMenuOption("RD_FormatOccupant".Translate(), delegate
            {
                pawn.ideo = null;
                pawn.Destroy();
                pawn = null;
            });

            list.Add(formatPawn);
            return list;
        }

        if (!actingPawn.story.traits.HasTrait(TraitDef.Named("RD_Holographic")))
        {
            return list;
        }

        var transferToEmitter = new FloatMenuOption("RD_TransferToThisEmitter".Translate(), delegate
        {
            foreach (var thing in parent.Map.listerBuildings.AllBuildingsColonistOfDef(
                         ThingDef.Named("RD_HolographicEmitter")))
            {
                if (thing is not HoloEmitter holoEmitter)
                {
                    break;
                }

                if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn != actingPawn)
                {
                    continue;
                }

                holoEmitter.GetComp<CompHoloEmitter>().SimPawn = null;
                pawn = actingPawn;
                parent.Map.areaManager.AllAreas.Remove(pawn.playerSettings.AreaRestriction);
                MakeValidAllowedZone();
                break;
            }
        });

        if (pawn != null)
        {
            return list;
        }

        list.Add(transferToEmitter);
        return list;
    }

    public override void PostExposeData()
    {
        Scribe_References.Look(ref pawn, "simulatedPawn");
    }

    public void SetUpPawn()
    {
        if (pawn.Spawned)
        {
            pawn.DeSpawn();
        }

        pawn.health.Reset();
        pawn.story.traits.GainTrait(new Trait(TraitDef.Named("RD_Holographic")));
        GenSpawn.Spawn(pawn, parent.Position, parent.Map);
        MakeValidAllowedZone();
    }

    private void HoloTickPawn()
    {
        if (pawn == null)
        {
            return;
        }

        if (pawn.Dead)
        {
            if (pawn.Corpse.holdingOwner == Emitter.GetDirectlyHeldThings())
            {
                return;
            }

            Emitter.TryAcceptThing(pawn.Corpse);
            return;
        }

        if (!pawn.story.traits.HasTrait(TraitDef.Named("RD_Holographic")))
        {
            SetUpPawn();
        }

        if (!pawn.Spawned)
        {
            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
        }

        pawn.needs.food.CurLevel = 1f;
        if (!pawn.Position.InHorDistOf(parent.Position, 12f) ||
            !GenSight.LineOfSight(parent.Position, pawn.Position, parent.Map, true))
        {
            pawn.inventory.DropAllNearPawn(pawn.Position);
            pawn.equipment.DropAllEquipment(pawn.Position, false);
            pawn.DeSpawn();
            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
        }

        pawn.health.Reset();
    }

    public void Scan(Corpse c)
    {
        if (Emitter.TryAcceptThing(c))
        {
            c.InnerPawn.story.traits.GainTrait(new Trait(TraitDef.Named("RD_Holographic")));
        }
    }

    public void MakeValidAllowedZone()
    {
        var enumerable = from cell in GenRadial.RadialCellsAround(parent.Position, 18f, true)
            where cell.InHorDistOf(parent.Position, 12f) &&
                  GenSight.LineOfSight(parent.Position, cell, parent.Map, true)
            select cell;
        parent.Map.areaManager.TryMakeNewAllowed(out var area_Allowed);
        foreach (var c in enumerable)
        {
            area_Allowed[parent.Map.cellIndices.CellToIndex(c)] = true;
        }

        area_Allowed.SetLabel("RD_HoloEmitterAreaFor".Translate(pawn.Named("PAWN"))); //"HoloEmitter area for {0}."
        pawn.playerSettings.AreaRestriction = area_Allowed;
    }

    public override void CompTickRare()
    {
        base.CompTickRare();
        if (pawn == null)
        {
            return;
        }

        foreach (var designation in parent.Map.designationManager.AllDesignationsOn(parent))
        {
            if (designation.def != DesignationDefOf.Uninstall)
            {
                continue;
            }

            if (pawn.Spawned)
            {
                pawn.DeSpawn();
            }

            return;
        }

        if (parent.GetComp<CompPowerTrader>().PowerOn)
        {
            HoloTickPawn();
        }
        else if (pawn.Spawned)
        {
            pawn.DeSpawn();
        }
    }
}