using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Missions
{
    public class QuestComp_MedicalEmergency : WorldObjectComp, IThingHolder
    {
        private static readonly List<Thing> tmpRewards = new List<Thing>();

        public Faction DownedFaction;

        private bool active;

        private List<Pawn> injured = new List<Pawn>();

        public int maxPawns;

        private float relationsImprovement;

        private Faction requestingFaction;

        private ThingOwner rewards;

        public QuestComp_MedicalEmergency()
        {
            rewards = new ThingOwner<Thing>(this);
        }

        public bool Active => active;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return rewards;
        }

        private bool CheckAllStanding()
        {
            foreach (var pawn in injured)
            {
                if (pawn.Dead)
                {
                    continue;
                }

                if (pawn.Downed)
                {
                    return false;
                }

                if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
                {
                    return false;
                }
            }

            return true;
        }

        public override string CompInspectStringExtra()
        {
            return null;
        }

        private void CalculateQuestOutcome()
        {
            var num = (from p in injured
                where !p.Dead && p.RaceProps.Humanlike
                select p).Count();
            var giveTech = false;
            if (num - maxPawns == 0)
            {
                if (Rand.Value < 0.1 * num)
                {
                    giveTech = true;
                }
            }

            var num2 = (from p in injured
                where !p.Dead && p.RaceProps.Humanlike && p.Faction != Faction.OfPlayer
                select p).Count();
            var newFaction = num2 > 4 && Rand.Value < 0.6f;
            if (num > 0)
            {
                GiveRewardsAndSendLetter(giveTech, newFaction);
            }

            StopQuest();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!active)
            {
                return;
            }

            if (parent is not MapParent mapParent || mapParent.Map == null)
            {
                return;
            }

            if (DownedFaction == null)
            {
                DownedFaction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
            }

            if (injured.NullOrEmpty())
            {
                injured = (from p in mapParent.Map.mapPawns.AllPawns
                    where p.Faction == DownedFaction && p.RaceProps.Humanlike
                    select p).ToList();
                Log.Message($"Found {injured.Count} injured pawns from the {DownedFaction.Name}");
            }
            else
            {
                // Log.Message($"Active with {injured.Count} in list and max of {maxPawns}.");
                foreach (var pawn in injured)
                {
                    if (pawn.Dead || pawn.Downed || pawn.GetLord() != null)
                    {
                        continue;
                    }

                    var lordJob = new LordJob_DefendBase(DownedFaction, pawn.Position);
                    var list = new List<Pawn> {pawn};
                    LordMaker.MakeNewLord(DownedFaction, lordJob, mapParent.Map, list);
                }

                if (CheckAllStanding())
                {
                    CalculateQuestOutcome();
                }
            }
        }

        private ThingDef RandomHiTechReward()
        {
            var value = Rand.Value;
            
            return value switch
            {
                <= 0.25f => ThingDef.Named("RD_HolographicEmitter"),
                <= 0.50f => ThingDef.Named("RD_Teleporter"),
                <= 0.75f => ThingDef.Named("RD_GattlingLaser"),
                _ => ThingDef.Named("Gun_ChargeRifle")
            };
        }

        private void CloseMapImmediate()
        {
            if (parent is not MapParent mapParent)
            {
                return;
            }

            if (Dialog_FormCaravan.AllSendablePawns(mapParent.Map, true).Any(x =>
                    x.IsColonist || x.IsPrisonerOfColony || x.Faction == Faction.OfPlayer ||
                    x.HostFaction == Faction.OfPlayer))
            {

                foreach (var pawn in mapParent.Map.mapPawns.AllPawnsSpawned)
                {
                    if (!pawn.RaceProps.Humanlike)
                    {
                        continue;
                    }
                
                    if (pawn.GetLord() is not { } lord)
                    {
                        continue;
                    }
                
                    lord.Notify_PawnLost(pawn, PawnLostCondition.ExitedMap);
                    pawn.ClearMind();
                }

                Messages.Message("MessageYouHaveToReformCaravanNow".Translate(),
                    new GlobalTargetInfo(mapParent.Tile), MessageTypeDefOf.NeutralEvent);
                Current.Game.CurrentMap = mapParent.Map;
                Find.WindowStack.Add(new Dialog_FormCaravan(mapParent.Map, true, delegate
                {
                    if (mapParent.HasMap)
                    {
                        Find.WorldObjects.Remove(mapParent);
                    }
                }));

                return;
            }
            
            var list2 = new List<Pawn>();
            list2.AddRange(from x in mapParent.Map.mapPawns.AllPawns
                where x.IsColonist || x.IsPrisonerOfColony || x.Faction == Faction.OfPlayer ||
                      x.HostFaction == Faction.OfPlayer
                select x);
            if (list2.Any())
            {
                if (list2.Any(x => CaravanUtility.IsOwner(x, Faction.OfPlayer)))
                {
                    //TODO: check if it works
                    CaravanExitMapUtility.ExitMapAndCreateCaravan(list2, Faction.OfPlayer,
                        mapParent.Tile, mapParent.Tile, mapParent.Tile, false);
                    Messages.Message("MessageReformedCaravan".Translate(),
                        MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var pawn in list2)
                    {
                        stringBuilder.AppendLine("    " + pawn.LabelCap);
                    }

                    Find.LetterStack.ReceiveLetter("RD_LetterLabelPawnsLostDueToMapCountdown".Translate(),
                        "RD_LetterPawnsLostDueToMapCountdown".Translate(new NamedArgument[]
                        {
                            stringBuilder.ToString().TrimEndNewlines()
                        }), LetterDefOf.ThreatSmall, new GlobalTargetInfo(mapParent.Tile));
                }

                list2.Clear();
            }

            Find.WorldObjects.Remove(mapParent);
        }

        private void GiveRewardsAndSendLetter(bool giveTech, bool newFaction)
        {
            string text = "RD_GratefulSurvivors".Translate();
            string
                text2 = "RD_GratefulSurvivorsDesc"
                    .Translate(); //"The survivors of the crash are very thankful for your help, and have send some supplies as a gesture of gratitude.";
            if (giveTech)
            {
                ThingDef.Named("Gun_ChargeRifle");
                var thingDef = RandomHiTechReward();
                ThingDef stuff = null;
                if (thingDef.MadeFromStuff)
                {
                    stuff = GenStuff.DefaultStuffFor(thingDef);
                }

                rewards.TryAdd(ThingMaker.MakeThing(thingDef, stuff));
                text2 = "RD_GratefulSurvivorsAmazedDesc"
                    .Translate(); //"The survivors of the crash are amazed by your rapid and professional emergency medical response, thanks to which no-one died. In gratitude, they have included a special system removed form the wreck.";
            }

            Find.LetterStack.ReceiveLetter(text, text2, LetterDefOf.PositiveEvent);
            var map = Find.AnyPlayerHomeMap ?? ((MapParent) parent).Map;
            tmpRewards.AddRange(rewards);
            rewards.Clear();
            var dropCenter = DropCellFinder.TradeDropSpot(map);
            DropPodUtility.DropThingsNear(dropCenter, map, tmpRewards);
            tmpRewards.Clear();
            if (!newFaction)
            {
                return;
            }

            CloseMapImmediate();
            GenerateNewFaction();
        }

        private void GenerateNewFaction()
        {
            if (parent is not MapParent mapParent || mapParent.Map == null)
            {
                return;
            }

            var faction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(FactionDefOf.OutlanderCivil));
            mapParent.Map.pawnDestinationReservationManager.GetPawnDestinationSetFor(faction);
            Find.FactionManager.Add(faction);

            Log.Message($"Created faction {faction.Name}");
            var settlement = (Settlement) WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            settlement.SetFaction(faction);
            settlement.Tile = parent.Tile;
            settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement);
            Find.WorldObjects.Add(settlement);
            faction.leader = null;
            foreach (var pawn in injured)
            {
                if (pawn.Dead || pawn.Faction == Faction.OfPlayer)
                {
                    continue;
                }

                pawn.SetFactionDirect(faction);
                if (faction.leader == null)
                {
                    faction.leader = pawn;
                }
            }

            faction.TryAffectGoodwillWith(Faction.OfPlayer, 100);
            string text3 = "RD_NewFaction".Translate(); //New Faction!
            string
                text4 = "RD_NewFactionDesc".Translate() +
                        faction.Name; //"The survivors of the crash have decided to make a life for themselves here, and have founded a new faction"
            Find.LetterStack.ReceiveLetter(text3, text4, LetterDefOf.PositiveEvent);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref injured, true, "injured", LookMode.Reference);
            Scribe_Values.Look(ref active, "active");
            Scribe_Values.Look(ref maxPawns, "maxPawns");
            Scribe_Values.Look(ref relationsImprovement, "relationsImprovement");
            Scribe_References.Look(ref requestingFaction, "requestingFaction");
            Scribe_References.Look(ref DownedFaction, "DownedFaction");
            Scribe_Deep.Look(ref rewards, "rewards", this);
        }

        public override void PostPostRemove()
        {
            base.PostPostRemove();
            rewards.ClearAndDestroyContents();
            GenerateNewFaction();
        }

        public void StartQuest(List<Thing> things)
        {
            DownedFaction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
            active = true;
            rewards.ClearAndDestroyContents();
            rewards.TryAddRangeOrTransfer(things);
        }

        private void StopQuest()
        {
            active = false;
            requestingFaction = null;
            rewards.ClearAndDestroyContents();
        }
    }
}