using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery;

public class GameCondition_TargetedStorm : GameCondition
{
    private const int RainDisableTicksAfterConditionEnds = 30000;

    private static readonly IntRange TicksBetweenStrikes = new IntRange(250, 600);

    private readonly int areaRadius = 5;

    private int nextLightningTicks;

    private Thing target;

    public override void End()
    {
        foreach (var map in AffectedMaps)
        {
            map.weatherDecider.DisableRainFor(30000);
            base.End();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref target, "target");
        Scribe_Values.Look(ref nextLightningTicks, "nextLightningTicks");
    }

    private void FindNewTarget()
    {
        foreach (var map in AffectedMaps)
        {
            var source = from p in map.mapPawns.AllPawnsSpawned
                where p.HostileTo(Faction.OfPlayer)
                select p;
            if (source.Any())
            {
                target = source.RandomElement();
            }
            else
            {
                End();
            }
        }
    }

    public override void GameConditionTick()
    {
        foreach (var map in AffectedMaps)
        {
            if (target is not { Spawned: true })
            {
                FindNewTarget();
                continue;
            }

            if (Find.TickManager.TicksGame <= nextLightningTicks)
            {
                continue;
            }

            var vector = new Vector2(Rand.Gaussian(), Rand.Gaussian());
            vector.Normalize();
            vector *= Rand.Range(0f, areaRadius);
            var intVec = new IntVec3((int)Math.Round(vector.x) + target.Position.x, 0,
                (int)Math.Round(vector.y) + target.Position.z);
            if (!IsGoodLocationForStrike(map, intVec))
            {
                continue;
            }

            map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(map, intVec));
            nextLightningTicks = Find.TickManager.TicksGame + TicksBetweenStrikes.RandomInRange;
        }
    }

    private IEnumerable<IntVec3> GetPotentiallyAffectedCells()
    {
        return GenRadial.RadialCellsAround(target.Position, 5f, true);
    }

    private bool IsGoodLocationForStrike(Map map, IntVec3 loc)
    {
        return loc.InBounds(map) && !loc.Roofed(map) && loc.Standable(map);
    }
}