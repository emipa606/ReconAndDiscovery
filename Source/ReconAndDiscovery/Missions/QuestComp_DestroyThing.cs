using ExpandedIncidents.Settings;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class QuestComp_DestroyThing : WorldObjectComp
{
    private bool active;

    public GameConditionDef gameConditionCaused;

    private ThingDef targetDef;

    private Thing thingToDestroy;

    public int worldTileAffected;

    private Thing ThingToDestroy
    {
        get
        {
            if (thingToDestroy != null)
            {
                return thingToDestroy;
            }

            if (targetDef == null)
            {
                return null;
            }

            if (parent is not MapParent { HasMap: true } mapParent)
            {
                return thingToDestroy;
            }

            if (mapParent.Map.listerThings.ThingsOfDef(targetDef).Count > 0)
            {
                thingToDestroy = mapParent.Map.listerThings.ThingsOfDef(targetDef).RandomElement();
            }
            else
            {
                return null;
            }

            return thingToDestroy;
        }
    }

    public bool Active => active;

    public override void CompTick()
    {
        base.CompTick();
        try
        {
            if (!active)
            {
                return;
            }

            if (parent is not MapParent mapParent || mapParent.Map == null)
            {
                return;
            }

            if (ThingToDestroy == null)
            {
                return;
            }

            if (ThingToDestroy.Destroyed)
            {
                StopQuest();
            }
        }
        catch
        {
            StopQuest();
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref active, "active");
        Scribe_Values.Look(ref worldTileAffected, "worldTileAffected");
        Scribe_Defs.Look(ref gameConditionCaused, "gameConditionCaused");
        Scribe_Defs.Look(ref targetDef, "targetDef");
        Scribe_References.Look(ref thingToDestroy, "thingToDestroy");
    }

    public void StartQuest(ThingDef thingDef)
    {
        targetDef = thingDef;
        active = true;
    }

    private void StopQuest()
    {
        active = false;
        if (ThingToDestroy is { Destroyed: false })
        {
            return;
        }

        var settlement = Find.World.worldObjects.SettlementAt(worldTileAffected);
        if (settlement is not { HasMap: true })
        {
            return;
        }

        RaD_Mod.LogMessage($"Found player base named {settlement.TraderName}");
        var gameConditionManager = settlement.Map.gameConditionManager;
        if (gameConditionManager.ConditionIsActive(gameConditionCaused))
        {
            gameConditionManager.ActiveConditions.Remove(
                gameConditionManager.GetActiveCondition(gameConditionCaused));
        }
    }

    public override void PostPostRemove()
    {
        StopQuest();
        base.PostPostRemove();
    }
}