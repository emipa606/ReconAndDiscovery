using Mlie;
using UnityEngine;
using Verse;

namespace ExpandedIncidents.Settings;

internal class RaD_Mod : Mod
{
    private static RaD_ModSettings settings;
    private static string currentVersion;

    private Vector2 scrollPosition = Vector2.zero;

    public RaD_Mod(ModContentPack content) : base(content)
    {
        settings = GetSettings<RaD_ModSettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Recon And Discovery";
    }

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
        value = Widgets.HorizontalSlider_NewTemp(
            new Rect(sliderRect.xMin + sliderRect.height + 10f, sliderRect.y,
                sliderRect.width - ((sliderRect.height * 2f) + 20f),
                sliderRect.height), value, 0f, 20f, true);
        listingStandard.Gap(10f);
    }

    public override void DoSettingsWindowContents(Rect rect)
    {
        settings.ChangeDef();
        var rect2 = new Rect(rect.x, rect.y, rect.width - 30f, rect.height + 100f);
        var listing_Standard = new Listing_Standard();
        Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
        listing_Standard.Begin(rect2);
        listing_Standard.Gap(10f);
        var rect3 = listing_Standard.GetRect(Text.LineHeight);
        if (Widgets.ButtonText(rect3, "RD_ResetSettings".Translate()))
        {
            ResetSettings();
        }

        listing_Standard.Gap(10f);
        listing_Standard.CheckboxLabeled("RD_VerboseLogging".Translate(), ref RaD_ModSettings.VerboseLogging,
            "RD_VerboseLoggingTooltip".Translate());

        var rect7 = listing_Standard.GetRect(Text.LineHeight);
        Widgets.Label(rect7, "RD_SettingHeader".Translate());
        listing_Standard.Gap(10f);

        CreateNewSettingSlider("RD_RaidEnemyQuestBaseChance".Translate(), ref RaD_ModSettings.RaidEnemyQuestBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_DiscoveredStargateBaseChance".Translate(),
            ref RaD_ModSettings.DiscoveredStargateBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_RaidStargateBaseChance".Translate(), ref RaD_ModSettings.RaidStargateBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_MalevolentAIBaseChance".Translate(), ref RaD_ModSettings.MalevolentAIBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_RaidTeleporterBaseChance".Translate(), ref RaD_ModSettings.RaidTeleporterBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_IncidentTremorsBaseChance".Translate(),
            ref RaD_ModSettings.IncidentTremorsBaseChance, listing_Standard);
        CreateNewSettingSlider("RD_IncidentRadiationBaseChance".Translate(),
            ref RaD_ModSettings.IncidentRadiationBaseChance, listing_Standard);

        CreateNewSettingSlider("RD_CrashedShipBaseChance".Translate(), ref RaD_ModSettings.CrashedShipBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_TradeFairBaseChance".Translate(), ref RaD_ModSettings.TradeFairBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_AbandonedColonyBaseChance".Translate(),
            ref RaD_ModSettings.AbandonedColonyBaseChance, listing_Standard);
        CreateNewSettingSlider("RD_MuffaloHerdMigrationBaseChance".Translate(),
            ref RaD_ModSettings.MuffaloHerdMigrationBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_OsirisQuestBaseChance".Translate(), ref RaD_ModSettings.OsirisQuestBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_WarIdolBaseChance".Translate(), ref RaD_ModSettings.WarIdolBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_QuestQuakesBaseChance".Translate(), ref RaD_ModSettings.QuestQuakesBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_QuestRadiationBaseChance".Translate(), ref RaD_ModSettings.QuestRadiationBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_SeraphitesLabBaseChance".Translate(), ref RaD_ModSettings.SeraphitesLabBaseChance,
            listing_Standard);
        CreateNewSettingSlider("RD_PeaceTalksBaseChance".Translate(), ref RaD_ModSettings.PeaceTalksBaseChance,
            listing_Standard);
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("RD_ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        Widgets.EndScrollView();
        settings.Write();
        settings.ChangeDef();
    }

    public static void LogMessage(string message, bool force = false)
    {
        if (RaD_ModSettings.VerboseLogging || force)
        {
            Log.Message($"[ReconAndDiscovery]: {message}");
        }
    }

    public static void LogError(string message)
    {
        Log.Error($"[ReconAndDiscovery]: {message}");
    }
}