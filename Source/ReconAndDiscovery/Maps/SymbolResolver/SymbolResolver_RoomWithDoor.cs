using ReconAndDiscovery.Triggers;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver;

public class SymbolResolver_RoomWithDoor : RimWorld.BaseGen.SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        if (rp.TryGetCustom("hasDoor", out char[] array))
        {
            if (rp.rect.Width < 3 && rp.rect.Height < 3)
            {
                return;
            }

            var resolveParams = rp;
            var i = 0;
            while (i < array.Length)
            {
                var c = array[i];
                switch (c)
                {
                    case 'N':
                        resolveParams.thingRot = Rot4.North;
                        resolveParams.rect = new CellRect(rp.rect.minX + 1, rp.rect.maxZ, rp.rect.Width - 2, 1);
                        goto IL_1AA;
                    case 'S':
                        resolveParams.thingRot = Rot4.South;
                        resolveParams.rect = new CellRect(rp.rect.minX + 1, rp.rect.minZ, rp.rect.Width - 2, 1);
                        goto IL_1AA;
                    case 'E':
                        resolveParams.thingRot = Rot4.East;
                        resolveParams.rect = new CellRect(rp.rect.maxX, rp.rect.minZ + 1, 1, rp.rect.Height - 2);
                        goto IL_1AA;
                    case 'W':
                        resolveParams.thingRot = Rot4.West;
                        resolveParams.rect = new CellRect(rp.rect.minX, rp.rect.minZ + 1, 1, rp.rect.Height - 2);
                        goto IL_1AA;
                }

                IL_1BB:
                i++;
                continue;
                IL_1AA:
                BaseGen.symbolStack.Push("wallDoor", resolveParams);
                goto IL_1BB;
            }
        }

        rp.TryGetCustom("madAnimalChance", out float chance);
        rp.TryGetCustom("luciferiumGasChance", out float chance2);
        rp.TryGetCustom("psionicLandmineChance", out float chance3);
        rp.TryGetCustom("smallGoldChance", out float chance4);
        rp.TryGetCustom("smallSilverChance", out float chance5);
        if (ThingMaker.MakeThing(ThingDefOfReconAndDiscovery.RD_RectActionTrigger) is RectActionTrigger
            rectActionTrigger)
        {
            rectActionTrigger.Rect = rp.rect;
            if (Rand.Chance(chance))
            {
                rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_MadAnimal;
            }
            else if (Rand.Chance(chance2))
            {
                rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_LuciferiumGas;
            }
            else if (Rand.Chance(chance3))
            {
                rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_PsionicLandmine;
            }
            else if (Rand.Chance(chance4))
            {
                rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_SmallGold;
            }
            else if (Rand.Chance(chance5))
            {
                rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_SmallSilver;
            }
            else
            {
                rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_BaseActivatedAction;
            }

            if (rectActionTrigger.actionDef != null)
            {
                var resolveParams2 = rp;
                resolveParams2.SetCustom("trigger", rectActionTrigger);
                BaseGen.symbolStack.Push("placeTrigger", resolveParams2);
            }
        }

        BaseGen.symbolStack.Push("emptyRoom", rp);
    }
}