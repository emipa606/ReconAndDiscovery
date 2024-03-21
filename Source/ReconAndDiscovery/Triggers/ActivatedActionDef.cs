using System;
using System.Collections.Generic;
using Verse;

namespace ReconAndDiscovery.Triggers;

public class ActivatedActionDef : Def
{
    public readonly string dialogText = "";

    public readonly int effectRadius = 0;

    public readonly bool hasDialog = false;

    public readonly List<ThingDef> spawnedThings = [];
    private ActivatedAction actionObject;

    private Type activatedActionClass;

    public IntRange numPawnsToSpawn;

    public IntRange numThingsToSpawn;

    public List<PawnKindDef> pawnKinds;

    public IntRange spawnedThingStackSize;

    public ActivatedAction ActivatedAction
    {
        get
        {
            if (actionObject != null)
            {
                return actionObject;
            }

            actionObject = (ActivatedAction)Activator.CreateInstance(activatedActionClass);
            actionObject.def = this;

            return actionObject;
        }
    }
}