using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_RaidEnemyQuest : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (parms.target is not Map map)
            {
                return false;
            }

            if (!Find.WorldObjects.AnyMapParentAt(map.Tile))
            {
                return false;
            }

            if (!Find.WorldObjects.MapParentAt(map.Tile).HasMap)
            {
                return false;
            }

            try
            {
                return base.TryExecuteWorker(parms);
            }
            catch
            {
                return false;
            }
        }
    }
}