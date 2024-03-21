using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery;

[StaticConstructorOnStartup]
public class CompTeleporter : ThingComp
{
    private static readonly Texture2D TargeterMouseAttachment =
        ContentFinder<Texture2D>.Get("UI/TeleportMouseAttachment");

    private static readonly Texture2D teleSym = ContentFinder<Texture2D>.Get("UI/TeleportSymbol");

    private float fCharge;

    public bool ReadyToTransport => fCharge >= 1f;

    private string SaveKey => "transCharge";

    public void ResetCharge()
    {
        fCharge = 0f;
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (ReadyToTransport)
        {
            yield return TeleportCommand();
        }
    }

    private Command TeleportCommand()
    {
        return new Command_Action
        {
            defaultLabel = "RD_Teleport".Translate(), //Teleport."
            defaultDesc = "RD_TeleportDesc".Translate(), //"Teleport a person or animal."
            icon = teleSym,
            action = StartChoosingTarget
        };
    }

    public override void CompTick()
    {
        if (parent.GetComp<CompPowerTrader>().PowerOn)
        {
            fCharge += 0.0001f;
            if (fCharge > 1f)
            {
                fCharge = 1f;
            }
        }
        else
        {
            fCharge -= 0.0004f;
            if (fCharge < 0f)
            {
                fCharge = 0f;
            }
        }
    }

    private void StartChoosingTarget()
    {
        CameraJumper.TryJump(CameraJumper.GetWorldTarget(parent));
        Find.WorldSelector.ClearSelection();
        _ = parent.Map.Tile;
        Find.WorldTargeter.BeginTargeting(ChoseWorldTarget, true, TargeterMouseAttachment, true);
    }

    private bool ChoseWorldTarget(GlobalTargetInfo target)
    {
        if (!ReadyToTransport)
        {
            return true;
        }

        if (!target.IsValid)
        {
            Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
            return false;
        }

        if (target.WorldObject is MapParent { HasMap: true } mapParent)
        {
            var myMap = parent.Map;
            var map = mapParent.Map;
            Current.Game.CurrentMap = map;
            var targeter = Find.Targeter;

            var targetParams = new TargetingParameters
            {
                canTargetPawns = true,
                canTargetItems = false,
                canTargetSelf = false,
                canTargetLocations = false,
                canTargetBuildings = false,
                canTargetFires = false
            };
            targeter.BeginTargeting(targetParams, delegate(LocalTargetInfo x)
            {
                if (ReadyToTransport)
                {
                    TryTransport(x.ToGlobalTargetInfo(map));
                }
            }, null, ActionWhenFinished, TargeterMouseAttachment);
            return true;

            void ActionWhenFinished()
            {
                if (Find.Maps.Contains(myMap))
                {
                    Current.Game.CurrentMap = myMap;
                }
            }
        }

        Messages.Message("RD_YouCannotLock".Translate(),
            MessageTypeDefOf.RejectInput); //"You cannot lock onto anything there."
        return false;
    }

    private static void AddAnaphylaxisIfAppropriate(Pawn pawn)
    {
        Rand.PushState();
        Rand.Seed = pawn.thingIDNumber * 724;
        var value = Rand.Value;
        Rand.PopState();
        if (value < 0.12f)
        {
            pawn.health.AddHediff(HediffDef.Named("RD_Anaphylaxis"));
        }
    }

    private void TryTransport(GlobalTargetInfo target)
    {
        var map = parent.Map;
        var position = parent.Position;
        var map2 = target.Map;
        if (target.Thing is not Pawn pawn)
        {
            return;
        }

        var position2 = pawn.Position;
        if (map2.roofGrid.Roofed(position2) && map2.roofGrid.RoofAt(position2) == RoofDefOf.RoofRockThick)
        {
            Messages.Message("RD_TeleporterLannotLockThickRock".Translate(),
                MessageTypeDefOf.RejectInput); //Teleporter cannot lock on through the thick rock overhead!
        }
        else
        {
            FleckMaker.ThrowMetaPuff(position2.ToVector3(), map2);
            FleckMaker.ThrowMetaPuff(position.ToVector3(), map);
            pawn.DeSpawn();
            var fire = (Fire)GenSpawn.Spawn(ThingDefOf.Fire, position2, map2);
            fire.fireSize = 1f;
            GenSpawn.Spawn(pawn, position, map, parent.Rotation);
            AddAnaphylaxisIfAppropriate(pawn);
            fCharge = 0f;
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref fCharge, SaveKey);
    }

    public override string CompInspectStringExtra()
    {
        return "RD_Charge".Translate() + fCharge.ToStringPercent(); //"Charge: "
    }
}