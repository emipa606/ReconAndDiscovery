using System.Linq;
using ExpandedIncidents.Settings;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions;

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
        if (!psychicPawns.Any())
        {
            return true;
        }

        var randomPawn = psychicPawns.RandomElement();
        Find.LetterStack.ReceiveLetter("RD_ManhunterDanger".Translate(),
            "RD_MalevolentPsychicDesc"
                .Translate(randomPawn
                    .Named("PAWN")) //"{0} believes that a malevolent psychic energy is massing, and that this peaceful herd of muffalo are on the brink of a mass insanity."
            , LetterDefOf.ThreatSmall);

        RaD_Mod.LogMessage("Letter sent?");

        return true;
    }
}