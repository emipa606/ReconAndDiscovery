using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
    [StaticConstructorOnStartup]
    public class CompWeatherSat : ThingComp
    {
        public float mana;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            var list = new List<FloatMenuOption>();
            var map = parent.Map;
            var manager = map.gameConditionManager;
            if (mana > 10f)
            {
                list.Add(new FloatMenuOption("RD_EndExtremeWeather".Translate(), // End Extreme Weather (10 mana)
                    delegate
                    {
                        if (map.weatherManager.curWeather == WeatherDefOf.Clear
                            && !manager.ConditionIsActive(GameConditionDefOf.ColdSnap)
                            && !manager.ConditionIsActive(GameConditionDefOf.Flashstorm)
                            && !manager.ConditionIsActive(GameConditionDefOf.HeatWave))
                        {
                            return;
                        }

                        mana -= 10f;
                        map.weatherManager.TransitionTo(WeatherDefOf.Clear);
                        if (manager.ConditionIsActive(GameConditionDefOf.ColdSnap))
                        {
                            manager.ActiveConditions.Remove(manager.GetActiveCondition(GameConditionDefOf.ColdSnap));
                        }

                        if (manager.ConditionIsActive(GameConditionDefOf.Flashstorm))
                        {
                            manager.ActiveConditions.Remove(manager.GetActiveCondition(GameConditionDefOf.Flashstorm));
                        }

                        if (manager.ConditionIsActive(GameConditionDefOf.HeatWave))
                        {
                            manager.ActiveConditions.Remove(manager.GetActiveCondition(GameConditionDefOf.HeatWave));
                        }
                    }
                ));
            }

            if (mana > 15f)
            {
                list.Add(new FloatMenuOption("RD_BringRain".Translate(), delegate //(15mana)
                    {
                        mana -= 15f;
                        map.weatherManager.TransitionTo(WeatherDef.Named("Rain"));
                    }
                ));
            }

            if (mana > 18f)
            {
                list.Add(new FloatMenuOption("RD_BringFog".Translate(), //Bring Fog (18 mana)
                    delegate
                    {
                        mana -= 18f;
                        map.weatherManager.TransitionTo(WeatherDef.Named("Fog"));
                    }
                ));
            }

            if (mana > 40f)
            {
                list.Add(new FloatMenuOption("RD_StrikeOurEnemies".Translate(), delegate //(40mana)
                    {
                        var source = from p in map.mapPawns.AllPawnsSpawned
                            where p.HostileTo(Faction.OfPlayer)
                            select p;
                        
                        if (!source.Any())
                        {
                            return;
                        }
                        
                        mana -= 40f;
                        
                        var gameCondition_TargetedStorm =
                            (GameCondition_TargetedStorm) GameConditionMaker.MakeCondition(
                                GameConditionDef.Named("RD_TargetedStorm"), 12000);
                        map.gameConditionManager.RegisterCondition(gameCondition_TargetedStorm);
                    }
                ));
            }

            return list;
        }

        public override string CompInspectStringExtra()
        {
            return "RD_Mana".Translate() + $": {mana:##.0}";
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref mana, "link");
        }
    }
}