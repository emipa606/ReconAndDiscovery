using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery;

public class ITab_Scyther_Social : ITab
{
    public const float Width = 540f;

    public ITab_Scyther_Social()
    {
        size = new Vector2(540f, 510f);
        labelKey = "TabSocial".Translate();
        tutorTag = "RD_Social".Translate();
    }

    public override bool IsVisible => SelPawnForSocialInfo.RaceProps.IsMechanoid &&
                                      SelPawnForSocialInfo.RaceProps.intelligence == Intelligence.Humanlike;

    private Pawn SelPawnForSocialInfo
    {
        get
        {
            if (SelPawn != null)
            {
                return SelPawn;
            }

            if (SelThing is not Corpse corpse)
            {
                throw new InvalidOperationException("Social tab on non-pawn non-corpse " + SelThing);
            }

            return corpse.InnerPawn;
        }
    }

    protected override void FillTab()
    {
        SocialCardUtility.DrawSocialCard(new Rect(0f, 0f, size.x, size.y), SelPawnForSocialInfo);
    }
}