using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;

namespace ReconAndDiscovery;

public class AdventureDef : IncidentDef
{
    public GameConditionDef adventureGameCondition = null;

    public AdventureDetectionType detectionType = AdventureDetectionType.NONE;
    public EventDistanceType eventDistance = EventDistanceType.PLANETWIDE;

    public bool hasInitiatedDialog = false;

    public bool hasMapEnterDialog = false;

    public bool hasWorldMapDialog = false;

    public string initiatedText = "";

    public float luciferiumGasChance = 0f;

    public float madAnimalChance = 0f;

    public ActivatedActionDef mainAdventureAction = null;

    public string mapEnterText = "";

    public MapGeneratorDef mapGenerator = null;

    public string mapPinPath = "";

    public float psionicLandmineChance = 0f;

    public float smallGoldChance = 0f;

    public float smallSilverChance = 0f;

    public GameConditionDef targetGameCondition = null;

    public string worldLabel = "Adventure";

    public string worldMapText = "";

    public string zoomedMapPinPath = "";
}