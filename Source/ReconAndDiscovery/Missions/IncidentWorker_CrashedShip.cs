using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_CrashedShip : IncidentWorker
    {
        private static readonly IntRange TimeoutDaysRange = new(6, 10);

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        private List<Thing> GenerateRewards()
        {
            var value = default(ThingSetMakerParams);
            value.techLevel = TechLevel.Spacer;
            value.countRange = new IntRange(1, 1);
            value.totalMarketValueRange = new FloatRange(500f, 3000f);
            return ThingSetMakerDefOf.Reward_ItemsStandard.root.Generate(value);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (parms.target is not Map map)
            {
                return false;
            }

            if (!TileFinder.TryFindNewSiteTile(out var tile))
            {
                return false;
            }

            var spottedShip = false;
            var pawnsWatchingTheSky = from p in map.mapPawns.FreeColonistsSpawned
                where p.CurJob.def == JobDefOfReconAndDiscovery.Skygaze ||
                      p.CurJob.def == JobDefOfReconAndDiscovery.UseTelescope
                select p;
            Pawn pawn = null;
            if (map.listerBuildings.ColonistsHaveBuilding(ThingDef.Named("CommsConsole")) && Rand.Chance(0.5f))
            {
                spottedShip = true;
            }

            if (pawnsWatchingTheSky.Any())
            {
                spottedShip = true;
                pawn = pawnsWatchingTheSky.RandomElement();
            }

            if (!spottedShip)
            {
                return false;
            }

            var isAMedicalEmergency = Rand.Value < 0.4f;

            var site = (Site) WorldObjectMaker.MakeWorldObject(isAMedicalEmergency
                ? SiteDefOfReconAndDiscovery.RD_AdventureMedical
                : SiteDefOfReconAndDiscovery.RD_Adventure);
            site.Tile = tile;
            var faction = Find.FactionManager.RandomEnemyFaction(true, false, true, TechLevel.Spacer);
            site.SetFaction(faction);
            site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_CrashedShip,
                SiteDefOfReconAndDiscovery.RD_CrashedShip.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));
            
            if (isAMedicalEmergency)
            {
                var medicalEmergency = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MedicalEmergency,
                    SiteDefOfReconAndDiscovery.RD_MedicalEmergency.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };
                site.parts.Add(medicalEmergency);
                var component = site.GetComponent<QuestComp_MedicalEmergency>();
                component.parent = site;
                component.Initialize(new WorldObjectCompProperties_MedicalEmergency());
                component.maxPawns = Rand.RangeInclusive(3, 12);
                var rewards = GenerateRewards();
                component.StartQuest(rewards);
            }
            else if (!Rand.Chance(0.75f))
            {
                var rareBeasts = new SitePart(site, SiteDefOfReconAndDiscovery.RD_RareBeasts,
                    SiteDefOfReconAndDiscovery.RD_RareBeasts.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };

                site.parts.Add(rareBeasts);
            }

            if (Rand.Value < 0.85f)
            {
                var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                    SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };

                site.parts.Add(scatteredTreasure);
            }

            if (Rand.Value < 0.1f)
            {
                var scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                    SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };
                site.parts.Add(scatteredManhunters);
            }

            if (Rand.Value < 0.1f)
            {
                var mechanoidForces = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MechanoidForces,
                    SiteDefOfReconAndDiscovery.RD_MechanoidForces.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };
                site.parts.Add(mechanoidForces);
            }

            if (Rand.Value < 0.5f)
            {
                var enemyRaidOnArrival = new SitePart(site, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival,
                    SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };
                site.parts.Add(enemyRaidOnArrival);
            }

            var randomInRange = TimeoutDaysRange.RandomInRange;
            site.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
            Find.WorldObjects.Add(site);
            if (pawn == null)
            {
                SendStandardLetter(parms, site);
            }
            else
            {
                Find.LetterStack.ReceiveLetter("RD_ShootingStar".Translate(),
                    "SawFallFromSky".Translate(
                        pawn.Named("PAWN")) //just saw something fall from the sky near here!
                    , LetterDefOf.PositiveEvent, site);
            }

            return true;
        }
    }
}