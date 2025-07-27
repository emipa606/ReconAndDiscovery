using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions;

public class IncidentWorker_MuffaloMassInsanity : IncidentWorker
{
    private static void DriveInsane(Pawn p)
    {
        p.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, true);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (parms.target is not Map map)
        {
            return false;
        }

        var animalDef = PawnKindDef.Named("Muffalo");

        var list = (from pawn in map.mapPawns.AllPawnsSpawned
            where pawn.kindDef == animalDef && Rand.Chance(0.5f)
            select pawn).ToList();

        if (list.Count < 5)
        {
            return false;
        }

        list.Shuffle();
        foreach (var animal in list)
        {
            DriveInsane(animal);
        }

        string text = "LetterLabelAnimalInsanityMultiple".Translate() + ": " + animalDef.LabelCap;
        string text2 = "AnimalInsanityMultiple".Translate([
            animalDef.label
        ]);
        Find.LetterStack.ReceiveLetter(text, text2, LetterDefOf.NegativeEvent);
        if (map == Find.CurrentMap)
        {
            Find.CameraDriver.shaker.DoShake(1f);
        }

        return true;
    }
}