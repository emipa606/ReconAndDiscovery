using System.Linq;
using ExpandedIncidents.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery;

public class GameCondition_Radiation : GameCondition
{
    private readonly SkyColorSet SkyColours;

    public GameCondition_Radiation()
    {
        var sky = new Color(0.8f, 0.8f, 0.3f);
        var shadow = new Color(0.9f, 0.9f, 1f);
        var overlay = new Color(0.7f, 0.7f, 0.5f);
        SkyColours = new SkyColorSet(sky, shadow, overlay, 9f);
    }

    private void AssignRadiationSickness(Pawn p)
    {
        if (p.Faction != Faction.OfPlayer)
        {
            return;
        }

        Messages.Message(
            "RD_DevelopedRadiationSickness".Translate(p.Named("PAWN")) //"{0} has developed radiation sickness" 
            , MessageTypeDefOf.NegativeEvent, false);
        p.health.AddHediff(HediffDef.Named("RD_RadiationSickness"));
    }

    private void GiveCarcinoma(Pawn p)
    {
        if (!p.RaceProps.IsFlesh)
        {
            return;
        }

        var allParts = p.RaceProps.body.AllParts;
        var partDef = allParts.RandomElement().def;
        if (p.RaceProps.body == BodyDefOf.Human)
        {
            var value = Rand.Value;
            switch (value)
            {
                case < 0.1f:
                case < 0.2f:
                    partDef = DefDatabase<BodyPartDef>.GetNamed("Lung");
                    break;
                case < 0.4f:
                    partDef = BodyPartDefOf.Stomach;
                    break;
                case < 0.6f:
                    partDef = BodyPartDefOf.Liver;
                    break;
                case < 0.8f:
                    partDef = BodyPartDefOf.Brain;
                    break;
            }
        }

        var source = from part in allParts
            where part.def == partDef
            select part;
        if (!source.Any())
        {
            return;
        }

        var bodyPartRecord = source.RandomElement();
        if (!allParts.Contains(bodyPartRecord))
        {
            return;
        }

        if (p.health.hediffSet.PartIsMissing(bodyPartRecord))
        {
            return;
        }

        p.health.AddHediff(HediffDef.Named("Carcinoma"), bodyPartRecord);
        RaD_Mod.LogMessage($"Added carcinoma to {p.Label}, part {bodyPartRecord.def.label}");
    }

    private void Miscarry(Pawn pawn)
    {
        if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant, true) == null)
        {
            return;
        }

        if (pawn.Faction == Faction.OfPlayer)
        {
            Messages.Message(
                "RD_MiscarriedRadiation"
                    .Translate(pawn.Named("PAWN")) //has miscarried due to radiation poisoning.
                , pawn, MessageTypeDefOf.NegativeEvent);
        }
    }

    private bool IsProtectedAt(Map map, IntVec3 c)
    {
        var affectedMaps = AffectedMaps;
        bool result;
        var room = c.GetRoom(map);
        if (room == null)
        {
            return false;
        }

        if (room.PsychologicallyOutdoors)
        {
            return false;
        }

        foreach (var c2 in room.Cells)
        {
            if (!c2.Roofed(map))
            {
                return false;
            }

            if (c2.GetRoof(map) != RoofDefOf.RoofRockThick)
            {
                return false;
            }
        }

        return true;
    }

    public override void GameConditionTick()
    {
        var affectedMaps = AffectedMaps;
        foreach (var map in affectedMaps)
        {
            if (Rand.Chance(0.006666667f))
            {
                var list = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
                if (list.Count != 0 && list.RandomElement() is Plant plant && !IsProtectedAt(map, plant.Position) &&
                    plant.def != ThingDef.Named("Plant_Psychoid"))
                {
                    plant.CropBlighted();
                    if (plant.sown)
                    {
                        Messages.Message(
                            "RD_PlantDiedRatiation"
                                .Translate() //A plant has died due to radiation damage"
                            , MessageTypeDefOf.NegativeEvent);
                    }
                }
            }

            foreach (var pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (IsProtectedAt(map, pawn.Position))
                {
                    continue;
                }

                var toxicModifier = 1 - Mathf.Max(pawn.GetStatValue(StatDefOf.ToxicEnvironmentResistance), 0.001f);
                var chance = 0.14f / 60000f * toxicModifier;
                var chance2 = 0.04f / 60000f * toxicModifier;
                if (Rand.Chance(chance))
                {
                    AssignRadiationSickness(pawn);
                }

                if (Rand.Chance(chance2))
                {
                    GiveCarcinoma(pawn);
                }

                if (Rand.Chance(chance) && pawn.health.hediffSet.HasHediff(HediffDefOf.Pregnant))
                {
                    Miscarry(pawn);
                }
            }
        }
    }

    public override SkyTarget? SkyTarget(Map map)
    {
        return new SkyTarget(0.1f, SkyColours, 1f, 1f);
    }

    public override float SkyTargetLerpFactor(Map map)
    {
        return GameConditionUtility.LerpInOutValue(TicksPassed, TicksLeft, 2500f, 0.25f);
    }
}