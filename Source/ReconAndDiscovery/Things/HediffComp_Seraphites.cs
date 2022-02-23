using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Things
{
    public class HediffComp_Seraphites : HediffComp
    {
        private bool hasBeenRemoved = false;

        public HediffCompProperties_Seraphites Props => (HediffCompProperties_Seraphites) props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (hasBeenRemoved) return;
            
            FixLuciferumHediffs();
        }

        private void FixLuciferumHediffs()
        {
            // Only fix one type of hediff per tick, starting with the addiction and then fixing the high
            var hediff = Pawn.health.hediffSet.hediffs.Find(hediff => hediff.def == HediffDef.Named("LuciferiumAddiction"))
                ?? Pawn.health.hediffSet.hediffs.Find(hediff => hediff.def == HediffDef.Named("LuciferiumHigh"));

            if (hediff == null)
            {
                hasBeenRemoved = true;
                return;
            }
            
            Pawn.health.RemoveHediff(hediff);
        }
    }
}