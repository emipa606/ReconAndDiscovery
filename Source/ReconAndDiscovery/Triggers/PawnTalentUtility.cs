using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers;

public static class PawnTalentUtility
{
    public static Pawn FindBestPsychic(IEnumerable<IntVec3> within, Map map)
    {
        Pawn pawn = null;
        foreach (var c in within)
        {
            foreach (var thing in c.GetThingList(map))
            {
                if (thing.def.category != ThingCategory.Pawn ||
                    thing.def.race.intelligence != Intelligence.Humanlike)
                {
                    continue;
                }

                if (thing is not Pawn pawn2)
                {
                    continue;
                }

                var num = pawn2.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity"));
                if (num <= 0)
                {
                    continue;
                }

                if (pawn == null)
                {
                    pawn = pawn2;
                }
                else if (pawn.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity")) < num)
                {
                    pawn = pawn2;
                }
            }
        }

        return pawn;
    }

    public static Pawn FindBest(IEnumerable<IntVec3> within, Map map, SkillDef skill)
    {
        var list = new List<Pawn>();
        foreach (var c in within)
        {
            foreach (var thing in c.GetThingList(map))
            {
                if (thing.def.category == ThingCategory.Pawn &&
                    thing.def.race.intelligence == Intelligence.Humanlike)
                {
                    list.Add(thing as Pawn);
                }
            }
        }

        Pawn result;
        if (list.NullOrEmpty())
        {
            result = null;
        }
        else
        {
            list.SortByDescending(p => p.skills.GetSkill(skill).Level);
            result = list.FirstOrDefault();
        }

        return result;
    }
}