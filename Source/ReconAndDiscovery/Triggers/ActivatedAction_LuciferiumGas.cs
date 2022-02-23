using Verse;

namespace ReconAndDiscovery.Triggers;

public class ActivatedAction_LuciferiumGas : ActivatedAction
{
    protected override void DoAnyFurtherActions(Pawn activatedBy, Map map, Thing trigger)
    {
        foreach (var c in GetEffectArea(activatedBy.Position))
        {
            foreach (var thing in c.GetThingList(map))
            {
                if (thing.def.category != ThingCategory.Pawn ||
                    thing.def.race.intelligence != Intelligence.Humanlike)
                {
                    continue;
                }

                if (thing is not Pawn pawn)
                {
                    continue;
                }

                var hediff = HediffMaker.MakeHediff(HediffDef.Named("LuciferiumHigh"), pawn);
                var hediff2 = HediffMaker.MakeHediff(HediffDef.Named("LuciferiumAddiction"), pawn);
                pawn.health.AddHediff(hediff);
                pawn.health.AddHediff(hediff2);
            }
        }

        base.DoAnyFurtherActions(activatedBy, map, trigger);
    }
}