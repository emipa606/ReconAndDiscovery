using System.Collections.Generic;
using System.Linq;
using ExpandedIncidents.Settings;
using ReconAndDiscovery.Maps;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

[StaticConstructorOnStartup]
public class CompStargate : ThingComp
{
    private static readonly Texture2D teleSym = ContentFinder<Texture2D>.Get("UI/StargateSymbol");

    private Thing link;

    private Site linkedSite;

    private Thing LinkedGate
    {
        get
        {
            Thing result;
            if (link is not { Spawned: true } || link.Destroyed)
            {
                result = null;
            }
            else
            {
                result = link;
            }

            return result;
        }
    }

    private Site LinkedSite
    {
        get
        {
            Site result;
            if (linkedSite == null || linkedSite.Tile == -1 || !Find.WorldObjects.AnyMapParentAt(linkedSite.Tile))
            {
                result = null;
            }
            else
            {
                result = linkedSite;
            }

            return result;
        }
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        if (mode == DestroyMode.Deconstruct)
        {
            GenSpawn.Spawn(ThingDef.Named("RD_ExoticMatter"), parent.Position, previousMap);
        }
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        var list = new List<FloatMenuOption>();
        if (link is { Destroyed: false, Spawned: true } || linkedSite != null)
        {
            list.Add(new FloatMenuOption("RD_TravelToTargetGate".Translate(), delegate
                {
                    var job = new Job(JobDefOfReconAndDiscovery.RD_TravelThroughStargate, parent)
                    {
                        playerForced = true
                    };
                    selPawn.jobs.TryTakeOrderedJob(job);
                }
            ));
        }

        return list;
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        yield return LinkCommand();
    }

    private Command LinkCommand()
    {
        return new Command_Action
        {
            defaultLabel = "RD_LinkGate".Translate(), //"Link a gate."
            defaultDesc = "RD_LinkGateDesc".Translate(), //"Link this gate to another."
            icon = teleSym,
            action = StartChoosingTarget
        };
    }

    private void StartChoosingTarget()
    {
        CameraJumper.TryJump(CameraJumper.GetWorldTarget(parent));
        Find.WorldSelector.ClearSelection();
        _ = parent.Map.Tile;
        Find.WorldTargeter.BeginTargeting(ChoseWorldTarget, true, null, true);
    }

    private bool ChoseWorldTarget(GlobalTargetInfo target)
    {
        if (!target.IsValid)
        {
            Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
            return false;
        }

        if (target.WorldObject is MapParent mapParent)
        {
            if (mapParent.HasMap)
            {
                _ = parent.Map;
                var targetMap = mapParent.Map;
                Current.Game.CurrentMap = targetMap;
                if (targetMap.listerThings.ThingsOfDef(ThingDef.Named("RD_Stargate")).Any())
                {
                    MakeLink(targetMap.listerThings.ThingsOfDef(ThingDef.Named("RD_Stargate"))
                        .FirstOrDefault());
                    return true;
                }

                Messages.Message("RD_StargateNoEvidence".Translate(),
                    MessageTypeDefOf.RejectInput); // "There is no evidence of a stargate there."
                return false;
            }

            if (mapParent is Site site && site.parts.Select(x => x.def)
                    .Contains(SiteDefOfReconAndDiscovery.RD_Stargate))
            {
                MakeLink(site);
                return true;
            }

            Messages.Message("RD_StargateNoEvidence".Translate(), MessageTypeDefOf.RejectInput);
            return false;
        }

        Messages.Message("RD_StargateNoEvidence".Translate(), MessageTypeDefOf.RejectInput);
        return false;
    }

    private void MakeLink(Thing stargate)
    {
        link = null;
        linkedSite = null;
        link = stargate;
        Messages.Message("RD_StargateLinked".Translate(),
            MessageTypeDefOf.PositiveEvent); // "Stargate linked to destination."
    }

    private void MakeLink(Site stargateSite)
    {
        link = null;
        linkedSite = null;
        linkedSite = stargateSite;
        Messages.Message("RD_StargateLinked".Translate(), MessageTypeDefOf.PositiveEvent);
    }

    public override void PostExposeData()
    {
        Scribe_References.Look(ref link, "link");
        Scribe_References.Look(ref linkedSite, "linkedSite");
    }

    private bool CheckSetupGateAndMap(Pawn p)
    {
        RaD_Mod.LogMessage("Checking for link");
        _ = p.Drafted;
        _ = Find.Selector.IsSelected(p);
        if (LinkedGate != null)
        {
            RaD_Mod.LogMessage("Linked already!");
            return true;
        }

        if (LinkedSite == null)
        {
            Messages.Message("RD_StargateNotLinked".Translate(),
                MessageTypeDefOf.RejectInput); //"Stargate is not linked to a destination!"
            return false;
        }

        if (!LinkedSite.HasMap)
        {
            var pawns = new List<Pawn>
            {
                p
            };

            LongEventHandler.QueueLongEvent(delegate
            {
                SitePartWorker_Stargate.tmpPawnsToSpawn.AddRange(pawns);
                GetOrGenerateMapUtility.GetOrGenerateMap(LinkedSite.Tile, null);
            }, "GeneratingMapForNewEncounter", false, null);
            return false;
        }

        IEnumerable<Thing> source = LinkedSite.Map.listerThings.ThingsOfDef(ThingDef.Named("RD_Stargate"));
        if (!source.Any())
        {
            Messages.Message("RD_StargateNotLinked".Translate(), MessageTypeDefOf.RejectInput);
            return false;
        }

        MakeLink(source.FirstOrDefault());
        RaD_Mod.LogMessage("Linked extant, unlinked gate!");
        return true;
    }

    public void SendPawnThroughStargate(Pawn p)
    {
        var drafted = p.Drafted;
        var isSelected = Find.Selector.IsSelected(p);
        RaD_Mod.LogMessage($"Attempting to send {p.Label} through gate");
        if (!CheckSetupGateAndMap(p))
        {
            return;
        }

        if (LinkedGate is not { Spawned: true } || LinkedGate.Destroyed)
        {
            Messages.Message("RD_OtherGateBuried".Translate(),
                MessageTypeDefOf.RejectInput); //"The other gate has been buried! We cannot transit!"
        }
        else
        {
            if (p.Spawned)
            {
                p.DeSpawn();
            }

            GenSpawn.Spawn(p, LinkedGate.Position, LinkedGate.Map);
            if (drafted)
            {
                p.drafter.Drafted = true;
            }

            if (!isSelected)
            {
                return;
            }

            Current.Game.CurrentMap = p.Map;
            Find.CameraDriver.JumpToCurrentMapLoc(LinkedGate.Position);
        }
    }
}