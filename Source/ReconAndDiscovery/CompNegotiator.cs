using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery;

[StaticConstructorOnStartup]
public class CompNegotiator : ThingComp
{
    private static readonly Material QuestionMarkMat =
        MaterialPool.MatFrom("UI/Overlays/QuestionMark", ShaderDatabase.MetaOverlay);

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        var list = base.CompFloatMenuOptions(selPawn).ToList();
        var item = new FloatMenuOption("RD_Negotiate".Translate(), delegate
        {
            var job = new Job(JobDefOfReconAndDiscovery.RD_Negotiate)
            {
                targetA = parent,
                playerForced = true
            };
            selPawn.jobs.TryTakeOrderedJob(job);
        });
        list.Add(item);
        return list;
    }

    private static void RenderExclamationPointOverlay(Thing t)
    {
        if (!t.Spawned)
        {
            return;
        }

        var drawPos = t.DrawPos;
        drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28125f;
        if (t is Pawn)
        {
            drawPos.x += t.def.size.x - 1f;
            drawPos.z += t.def.size.z + 0.2f;
        }

        RenderPulsingOverlayQuest(t, QuestionMarkMat, drawPos, MeshPool.plane05);
    }

    private static void RenderPulsingOverlayQuest(Thing thing, Material mat, Vector3 drawPos, Mesh mesh)
    {
        var num = ((float)Math.Sin((Time.realtimeSinceStartup + (397f * (thing.thingIDNumber % 571))) * 4f) + 1f) *
                  0.5f;
        num = 0.3f + (num * 0.7f);
        var material = FadedMaterialPool.FadedVersionOf(mat, num);
        Graphics.DrawMesh(mesh, drawPos, Quaternion.identity, material, 0);
    }

    public override void CompTick()
    {
        RenderExclamationPointOverlay(parent);
    }
}