using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_MuffaloMassInsanityWarning : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (parms.target is not Map map)
            {
                return false;
            }

            var psychicPawns = from pawn in map.mapPawns.FreeColonistsSpawned
                where pawn.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity"))
                select pawn;
            if (psychicPawns.Any())
            {
                var pawn = psychicPawns.RandomElement();
                Find.LetterStack.ReceiveLetter("RD_ManhunterDanger".Translate(),
                    "RD_MalevolentPsychicDesc"
                        .Translate(pawn
                            .Named("PAWN")) //"{0} believes that a malevolent psychic energy is massing, and that this peaceful herd of muffalo are on the brink of a mass insanity."
                    , LetterDefOf.ThreatSmall, null);
                
                Log.Message("Letter sent?");
            }
            
            return true;
        }

    }
    
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
            string text2 = "AnimalInsanityMultiple".Translate(new NamedArgument[]
            {
                animalDef.label
            });
            Find.LetterStack.ReceiveLetter(text, text2, LetterDefOf.NegativeEvent);
            if (map == Find.CurrentMap)
            {
                Find.CameraDriver.shaker.DoShake(1f);
            }

            return true;
        }
    }
}