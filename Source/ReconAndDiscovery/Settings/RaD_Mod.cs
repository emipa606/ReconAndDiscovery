using UnityEngine;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000E RID: 14
    internal class RaD_Mod : Mod
    {
        // Token: 0x0400003D RID: 61
        private static RaD_ModSettings settings;

        // Token: 0x0400003E RID: 62
        private Vector2 scrollPosition = Vector2.zero;

        // Token: 0x06000027 RID: 39 RVA: 0x00003737 File Offset: 0x00001937
        public RaD_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RaD_ModSettings>();
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003758 File Offset: 0x00001958
        public override string SettingsCategory()
        {
            return "Recon And Discovery";
        }

        // Token: 0x06000029 RID: 41 RVA: 0x0000376C File Offset: 0x0000196C
        private void ResetSettings()
        { 
            RaD_ModSettings.RaidEnemyQuestBaseChance = 0.0f;
            RaD_ModSettings.DiscoveredStargateBaseChance = 0.0f;
            RaD_ModSettings.RaidStargateBaseChance = 1.9f;
            RaD_ModSettings.MalevolentAIBaseChance = 16f;
            RaD_ModSettings.RaidTeleporterBaseChance = 20.0f;
            RaD_ModSettings.IncidentTremorsBaseChance = 0.2f;
            RaD_ModSettings.IncidentRadiationBaseChance = 0.2f;

            RaD_ModSettings.CrashedShipBaseChance = 6f;
            RaD_ModSettings.TradeFairBaseChance = 1.5f;
            RaD_ModSettings.AbandonedColonyBaseChance = 1.5f;
            RaD_ModSettings.MuffaloHerdMigrationBaseChance = 1.8f;
            RaD_ModSettings.OsirisQuestBaseChance = 0.3f;
            RaD_ModSettings.WarIdolBaseChance = 10f;
            RaD_ModSettings.QuestQuakesBaseChance = 0.8f;
            RaD_ModSettings.QuestRadiationBaseChance = 0.15f;
            RaD_ModSettings.SeraphitesLabBaseChance = 10f;
            RaD_ModSettings.PeaceTalksBaseChance = 1.5f;

            settings.Write();
            settings.ChangeDef();
        }

        private void CreateNewSettingSlider(string label, ref float value, Listing listingStandard)
        {
            var line = listingStandard.GetRect(Text.LineHeight);
            var leftHalf = line.LeftHalf().Rounded();
            var sliderRect = line.RightHalf().Rounded();
            var labelRect = leftHalf.LeftHalf().Rounded();
            var valueRect = leftHalf.RightHalf().Rounded();
            labelRect.Overlaps(valueRect);
            var rect6 = valueRect.RightHalf().Rounded();
            Widgets.Label(labelRect, label);
            Widgets.Label(rect6, value.ToString());
            value = Widgets.HorizontalSlider(
                new Rect(sliderRect.xMin + sliderRect.height + 10f, sliderRect.y, sliderRect.width - ((sliderRect.height * 2f) + 20f),
                    sliderRect.height), value, 0f, 20f, true);
            listingStandard.Gap(10f);
        }

        // Token: 0x0600002C RID: 44 RVA: 0x000039CC File Offset: 0x00001BCC
        public override void DoSettingsWindowContents(Rect rect)
        {
            settings.ChangeDef();
            var rect2 = new Rect(rect.x, rect.y, rect.width - 30f, rect.height - 10f);
            var listing_Standard = new Listing_Standard();
            Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
            listing_Standard.Begin(rect2);
            listing_Standard.Gap(10f);
            var rect3 = listing_Standard.GetRect(Text.LineHeight);
            var flag = Widgets.ButtonText(rect3, "Reset Settings");
            if (flag)
            {
                ResetSettings();
            }

            listing_Standard.Gap(10f);

            var rect7 = listing_Standard.GetRect(Text.LineHeight);
            Widgets.Label(rect7, "RD_SettingHeader".Translate());
            listing_Standard.Gap(10f);

            CreateNewSettingSlider("Enemy Raid Quest", ref RaD_ModSettings.RaidEnemyQuestBaseChance, listing_Standard);
            CreateNewSettingSlider("Discovered Stargate", ref RaD_ModSettings.DiscoveredStargateBaseChance, listing_Standard);
            CreateNewSettingSlider("Stargate Raid", ref RaD_ModSettings.RaidStargateBaseChance, listing_Standard);
            CreateNewSettingSlider("Malevolent AI", ref RaD_ModSettings.MalevolentAIBaseChance, listing_Standard);
            CreateNewSettingSlider("Teleporter Hack", ref RaD_ModSettings.RaidTeleporterBaseChance, listing_Standard);
            CreateNewSettingSlider("Tremors", ref RaD_ModSettings.IncidentTremorsBaseChance, listing_Standard);
            CreateNewSettingSlider("Radiation", ref RaD_ModSettings.IncidentRadiationBaseChance, listing_Standard);

            CreateNewSettingSlider("Crashed Ship", ref RaD_ModSettings.CrashedShipBaseChance, listing_Standard);
            CreateNewSettingSlider("Trade Fair", ref RaD_ModSettings.TradeFairBaseChance, listing_Standard);
            CreateNewSettingSlider("Abandoned Colony", ref RaD_ModSettings.AbandonedColonyBaseChance, listing_Standard);
            CreateNewSettingSlider("Muffalo Herd Migration Spotted", ref RaD_ModSettings.MuffaloHerdMigrationBaseChance, listing_Standard);
            CreateNewSettingSlider("Osiris Casket Quest", ref RaD_ModSettings.OsirisQuestBaseChance, listing_Standard);
            CreateNewSettingSlider("Abandoned Military Base", ref RaD_ModSettings.WarIdolBaseChance, listing_Standard);
            CreateNewSettingSlider("Quakes From Faulty Generator", ref RaD_ModSettings.QuestQuakesBaseChance, listing_Standard);
            CreateNewSettingSlider("Radiation Quest", ref RaD_ModSettings.QuestRadiationBaseChance, listing_Standard);
            CreateNewSettingSlider("Seraphites Quest", ref RaD_ModSettings.SeraphitesLabBaseChance, listing_Standard);
            CreateNewSettingSlider("Peace Talks", ref RaD_ModSettings.PeaceTalksBaseChance, listing_Standard);
            
            listing_Standard.End();
            Widgets.EndScrollView();
            settings.Write();
            settings.ChangeDef();
        }
    }
}