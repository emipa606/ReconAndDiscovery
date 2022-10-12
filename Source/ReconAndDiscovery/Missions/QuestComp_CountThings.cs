using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class QuestComp_CountThings : WorldObjectComp
{
    private bool active;

    public GameConditionDef gameConditionCaused;

    private ThingDef targetDef;

    public int targetNumber;

    public int ticksHeld;

    public int ticksTarget;

    public int worldTileAffected;

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

            var num = mapParent.Map.listerThings.ThingsOfDef(targetDef).Count;
            if (num > targetNumber)
            {
                if (ticksHeld > ticksTarget)
                {
                    StopQuest();
                }
                else
                {
                    ticksHeld++;
                }
            }
            else
            {
                ticksHeld = 0;
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
        Scribe_Values.Look(ref targetNumber, "targetNumber");
        Scribe_Values.Look(ref ticksHeld, "ticksHeld");
        Scribe_Values.Look(ref ticksTarget, "tickTarget");
    }

    public void StartQuest(ThingDef thingDef)
    {
        targetDef = thingDef;
        active = true;
    }

    private void StopQuest()
    {
        active = false;
        if (parent is not MapParent mapParent || mapParent.Map == null)
        {
            return;
        }

        var num = mapParent.Map.listerThings.ThingsOfDef(targetDef).Count;
        if (num <= targetNumber || ticksHeld <= ticksTarget)
        {
            return;
        }

        if (mapParent.Map.gameConditionManager.ConditionIsActive(gameConditionCaused))
        {
            mapParent.Map.gameConditionManager.ActiveConditions.Remove(
                mapParent.Map.gameConditionManager.GetActiveCondition(gameConditionCaused));
        }

        var settlement = Find.World.worldObjects.SettlementAt(worldTileAffected);
        if (settlement is not { HasMap: true })
        {
            return;
        }

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