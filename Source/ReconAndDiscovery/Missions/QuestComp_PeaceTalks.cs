using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions;

public class QuestComp_PeaceTalks : WorldObjectComp
{
    private bool active;

    private int facOriginalRelationship;

    private Pawn negotiator;

    private float relationsImprovement;

    private Faction requestingFaction;

    public Faction Faction => requestingFaction;

    public Pawn Negotiator
    {
        get
        {
            if (negotiator != null)
            {
                return negotiator;
            }

            if (parent is MapParent { Map: { } } mapParent)
            {
                negotiator = (from p in mapParent.Map.mapPawns.AllPawnsSpawned
                    where p.Faction == requestingFaction
                    select p).RandomElement();
            }

            return negotiator;
        }
        set => negotiator = value;
    }

    public bool Active => active;

    public override void CompTick()
    {
        base.CompTick();
        try
        {
            if (!active)
            {
                return;
            }

            if (parent is not MapParent mapParent || mapParent.Map == null)
            {
                return;
            }

            if (Negotiator is not { Spawned: true })
            {
                return;
            }

            if (Negotiator.GetComp<CompNegotiator>() != null)
            {
                return;
            }

            ThingComp thingComp = new CompNegotiator
            {
                parent = Negotiator
            };
            Negotiator.AllComps.Add(thingComp);
        }
        catch
        {
            StopQuest();
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref active, "active");
        Scribe_Values.Look(ref facOriginalRelationship, "facOriginalRelationship");
        Scribe_Values.Look(ref relationsImprovement, "relationsImprovement");
        Scribe_References.Look(ref requestingFaction, "requestingFaction");
        Scribe_References.Look(ref negotiator, "negotiator");
    }

    public void ResolveNegotiations(Pawn playerNegotiator, Pawn otherNegotiator)
    {
        if (Rand.Chance(0.05f))
        {
            var trapDialogue = new DiaNode("RD_NegotiationsTrap".Translate()); //"The negotiations are a trap!"
            var diaOption2 = new DiaOption("OK".Translate())
            {
                resolveTree = true
            };
            trapDialogue.options.Add(diaOption2);
            var window2 = new Dialog_NodeTree(trapDialogue);
            Find.WindowStack.Add(window2);
            requestingFaction.TryAffectGoodwillWith(Faction.OfPlayer, -101);
            facOriginalRelationship = -101;
            return;
        }

        var qualityCategory =
            QualityUtility.GenerateQualityCreatedByPawn(
                playerNegotiator.skills.GetSkill(SkillDefOf.Social).Level, false);
        var num = 0;
        var text = "";
        switch (qualityCategory)
        {
            case QualityCategory.Awful:
                num = -5;
                text = "RD_DiplomacyAwful".Translate(playerNegotiator.Label, requestingFaction.Name,
                    num); //"The flailing diplomatic "strategy" of {0} seemed chiefly to involve wild swings between aggression and panic, peppered liberally with lewd insults involving the negotiator for {1}'s antecedents. Your already strained relations have, understandably, worsened ({2} to relations).";
                break;
            case QualityCategory.Poor:
                num = -1;
                text = "RD_DiplomacyPoor".Translate(playerNegotiator.Label, requestingFaction.Name, num);
                break;
            case QualityCategory.Normal:
            case QualityCategory.Good:
                num = 12;
                text = "RD_DiplomacyNormalGood".Translate(playerNegotiator.Label, requestingFaction.Name, num);
                break;
            case QualityCategory.Excellent:
                num = 23;
                text = "RD_DiplomacyExcellent".Translate(playerNegotiator.Label, requestingFaction.Name, num);
                break;
            case QualityCategory.Masterwork:
            case QualityCategory.Legendary:
                num = 40;
                text = "RD_DiplomacyMasterWorkLegendary".Translate(playerNegotiator.Label,
                    requestingFaction.Name, num);
                break;
        }

        var diaNode = new DiaNode(text);
        var diaOption = new DiaOption("OK".Translate())
        {
            resolveTree = true
        };
        diaNode.options.Add(diaOption);
        var window = new Dialog_NodeTree(diaNode);
        Find.WindowStack.Add(window);

        facOriginalRelationship += num;
        active = false;
        ThingComp comp = Negotiator.GetComp<CompNegotiator>();
        if (comp != null)
        {
            Negotiator.AllComps.Remove(comp);
        }
    }

    public void StartQuest(Faction faction)
    {
        active = true;
        requestingFaction = faction;
        facOriginalRelationship = faction.PlayerGoodwill;
        requestingFaction.TryAffectGoodwillWith(Faction.OfPlayer, 1 - faction.PlayerGoodwill);
    }

    private void StopQuest()
    {
        active = false;
        requestingFaction.TryAffectGoodwillWith(Faction.OfPlayer,
            facOriginalRelationship - requestingFaction.PlayerGoodwill);
        requestingFaction = null;
    }

    public override void PostPostRemove()
    {
        StopQuest();
        base.PostPostRemove();
    }
}