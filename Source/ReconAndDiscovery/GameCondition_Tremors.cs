using ExpandedIncidents.Settings;
using RimWorld;
using Verse;

namespace ReconAndDiscovery;

public class GameCondition_Tremors : GameCondition
{
    private void CollapseRandomRoof()
    {
        foreach (var map in AffectedMaps)
        {
            if (!CellFinderLoose.TryGetRandomCellWith(c => c.Standable(map) && map.roofGrid.Roofed(c), map, 500,
                    out var intVec))
            {
                continue;
            }

            map.roofCollapseBuffer.MarkToCollapse(intVec);
            IntVec3[] array =
            {
                intVec + IntVec3.West,
                intVec + IntVec3.East,
                intVec + IntVec3.South,
                intVec + IntVec3.North
            };
            foreach (var c2 in array)
            {
                if (c2.Standable(map) && map.roofGrid.Roofed(c2))
                {
                    map.roofCollapseBuffer.MarkToCollapse(c2);
                }
            }
        }
    }

    public override void GameConditionTick()
    {
        if (Rand.Chance(8.333333E-05f))
        {
            CollapseRandomRoof();
        }
    }

    public override void Init()
    {
        if (RaD_ModSettings.IncidentTremorsBaseChance == 0)
        {
            base.End();
            return;
        }

        CollapseRandomRoof();
        base.Init();
    }
}