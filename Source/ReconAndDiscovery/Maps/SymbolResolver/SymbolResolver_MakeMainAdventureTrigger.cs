using System.Linq;
using ExpandedIncidents.Settings;
using ReconAndDiscovery.Triggers;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_MakeMainAdventureTrigger : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        var map = BaseGen.globalSettings.map;
        if (!rp.TryGetCustom("mainAdventureAction", out ActivatedActionDef actionDef))
        {
            return;
        }

        ActionTrigger actionTrigger = null;
        var source = from t in map.listerThings.AllThings
            where t is ActionTrigger
            select t;
        if (!source.Any())
        {
            var allRooms = map.regionGrid.allRooms;
            if (allRooms.Count == 0)
            {
                RaD_Mod.LogError("Could not find contained room for adventure trigger!");
            }
            else
            {
                var room = allRooms.RandomElementByWeight(r => 1f / r.GetStat(RoomStatDefOf.Space));
                actionTrigger = new ActionTrigger();
                foreach (var item in room.Cells)
                {
                    actionTrigger.Cells.Add(item);
                }

                var loc = actionTrigger.Cells.RandomElement();
                GenSpawn.Spawn(actionTrigger, loc, map);
            }
        }
        else
        {
            actionTrigger = source.RandomElement() as ActionTrigger;
        }

        if (actionTrigger != null)
        {
            actionTrigger.actionDef = actionDef;
        }
    }
}