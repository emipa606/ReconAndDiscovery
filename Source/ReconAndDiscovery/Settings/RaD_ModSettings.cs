using System.Linq;
using ReconAndDiscovery.DefOfs;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000D RID: 13
    internal class RaD_ModSettings : ModSettings
    {
        internal static float RaidEnemyQuestBaseChance;
        internal static float DiscoveredStargateBaseChance;
        internal static float RaidStargateBaseChance = 1.9f;
        internal static float MalevolentAIBaseChance = 16f;
        internal static float RaidTeleporterBaseChance = 20.0f;
        internal static float IncidentTremorsBaseChance = 0.2f;
        internal static float IncidentRadiationBaseChance = 0.2f;
        
        internal static float CrashedShipBaseChance = 6f;
        internal static float TradeFairBaseChance = 1.5f;
        internal static float AbandonedColonyBaseChance = 1.5f;
        internal static float MuffaloHerdMigrationBaseChance = 1.8f;
        internal static float OsirisQuestBaseChance = 0.3f;
        internal static float WarIdolBaseChance = 10f;
        internal static float QuestQuakesBaseChance = 0.8f;
        internal static float QuestRadiationBaseChance = 0.15f;
        internal static float SeraphitesLabBaseChance = 10f;
        internal static float PeaceTalksBaseChance = 1.5f;

        public void ChangeDef()
        {
            ChangeDefPost();
        }

        public static void ChangeDefPost()
        {
            var list = DefDatabase<IncidentDef>.AllDefs.ToList();
            foreach (var incidentDef in list)
            {
                switch (incidentDef.defName)
                {
                    case "RD_RaidEnemyQuest":
                        incidentDef.baseChance = RaidEnemyQuestBaseChance;
                        break;
                    case "RD_DiscoveredStargate":
                        incidentDef.baseChance = DiscoveredStargateBaseChance;
                        break;
                    case "RD_RaidStargate":
                        incidentDef.baseChance = RaidStargateBaseChance;
                        break;
                    case "RD_MalevolentAI":
                        incidentDef.baseChance = MalevolentAIBaseChance;
                        break;
                    case "RD_RaidTeleporter":
                        incidentDef.baseChance = RaidTeleporterBaseChance;
                        break;
                    case "RD_IncidentTremors":
                        incidentDef.baseChance = IncidentTremorsBaseChance;
                        break;
                    case "RD_IncidentRadiation":
                        incidentDef.baseChance = IncidentRadiationBaseChance;
                        break;
                    case "RD_CrashedShip":
                        incidentDef.baseChance = CrashedShipBaseChance;
                        break;
                    case "RD_TradeFair":
                        incidentDef.baseChance = TradeFairBaseChance;
                        break;
                    case "RD_AbandonedColonyAdventure":
                        incidentDef.baseChance = AbandonedColonyBaseChance;
                        break;
                    case "RD_MuffaloHerdMigration":
                        incidentDef.baseChance = MuffaloHerdMigrationBaseChance;
                        break;
                    case "RD_QuestOsiris":
                        incidentDef.baseChance = OsirisQuestBaseChance;
                        break;
                    case "RD_WarIdol":
                        incidentDef.baseChance = WarIdolBaseChance;
                        break;
                    case "RD_QuestQuakes":
                        incidentDef.baseChance = QuestQuakesBaseChance;
                        break;
                    case "RD_QuestRadiation":
                        incidentDef.baseChance = QuestRadiationBaseChance;
                        break;
                    case "RD_SeraphitesLab":
                        incidentDef.baseChance = SeraphitesLabBaseChance;
                        break;
                    case "RD_PeaceTalks":
                        incidentDef.baseChance = PeaceTalksBaseChance;
                        break;
                }
            }
        }

        // Token: 0x06000024 RID: 36 RVA: 0x0000345C File Offset: 0x0000165C
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref RaidEnemyQuestBaseChance, "RaidEnemyQuestBaseChance");
            Scribe_Values.Look(ref DiscoveredStargateBaseChance, "DiscoveredStargateBaseChance");
            Scribe_Values.Look(ref RaidStargateBaseChance, "RaidStargateBaseChance", 1.9f);
            Scribe_Values.Look(ref MalevolentAIBaseChance, "MalevolentAIBaseChance", 16f);
            Scribe_Values.Look(ref RaidTeleporterBaseChance, "RaidTeleporterBaseChance", 20.0f);
            Scribe_Values.Look(ref IncidentTremorsBaseChance, "IncidentTremorsBaseChance", 0.2f);
            Scribe_Values.Look(ref IncidentRadiationBaseChance, "IncidentRadiationBaseChance", 0.2f);
            Scribe_Values.Look(ref CrashedShipBaseChance, "CrashedShipBaseChance", 6f);
            Scribe_Values.Look(ref TradeFairBaseChance, "TradeFairBaseChance", 1.5f);
            Scribe_Values.Look(ref AbandonedColonyBaseChance, "AbandonedColonyBaseChance", 1.5f);
            Scribe_Values.Look(ref MuffaloHerdMigrationBaseChance, "MuffaloHerdMigrationBaseChance", 1.8f);
            Scribe_Values.Look(ref OsirisQuestBaseChance, "OsirisQuestBaseChance", 0.3f);
            Scribe_Values.Look(ref WarIdolBaseChance, "WarIdolBaseChance", 10f);
            Scribe_Values.Look(ref QuestQuakesBaseChance, "QuestQuakesBaseChance", 0.8f);
            Scribe_Values.Look(ref QuestRadiationBaseChance, "QuestRadiationBaseChance", 0.15f);
            Scribe_Values.Look(ref SeraphitesLabBaseChance, "SeraphitesLabBaseChance", 10f);
            Scribe_Values.Look(ref PeaceTalksBaseChance, "PeaceTalksBaseChance", 1.5f);
        }
    }
}