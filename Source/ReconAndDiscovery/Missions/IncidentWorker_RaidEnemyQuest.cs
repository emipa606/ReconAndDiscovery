﻿using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_RaidEnemyQuest : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result;
            if (!(parms.target is Map map))
            {
                result = false;
            }
            else if (!Find.WorldObjects.AnyMapParentAt(map.Tile))
            {
                result = false;
            }
            else if (!Find.WorldObjects.MapParentAt(map.Tile).HasMap)
            {
                result = false;
            }
            else
            {
                try
                {
                    result = TryExecute(parms);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }
    }
}