using RimWorld;
using Verse;

namespace ReconAndDiscovery;

public class CompMandatoryMilkable : CompHasGatherableBodyResource
{
    private long ticksOverFull;

    public bool Overfull => ticksOverFull * 2L >= Props.ticksUntilDanger;

    protected override bool Active
    {
        get
        {
            bool result;
            if (!base.Active)
            {
                result = false;
            }
            else
            {
                var pawn = parent as Pawn;
                result = (!Props.milkFemaleOnly || pawn == null || pawn.gender == Gender.Female) &&
                         (pawn == null || pawn.ageTracker.CurLifeStage.milkable);
            }

            return result;
        }
    }

    private CompProperties_MandatoryMilkable Props => (CompProperties_MandatoryMilkable)props;

    protected override int GatherResourcesIntervalDays => Props.milkIntervalDays;

    protected override int ResourceAmount => Props.milkAmount;

    protected override ThingDef ResourceDef => Props.milkDef;

    protected override string SaveKey => "milkFullness";

    public override void CompTick()
    {
        base.CompTick();
        if (Fullness >= 1f)
        {
            ticksOverFull += 1L;
        }
        else
        {
            ticksOverFull = 0L;
        }

        if (ticksOverFull <= Props.ticksUntilDanger)
        {
            return;
        }

        if (!Rand.Chance(2.5E-05f))
        {
            return;
        }

        var value = new DamageInfo(DamageDefOf.Bomb, 100, -1f);
        parent.Kill(value);
    }

    public override string CompInspectStringExtra()
    {
        string result;
        if (!Active)
        {
            result = null;
        }
        else
        {
            string text = "MilkFullness".Translate() + ": " + Fullness.ToStringPercent();
            if (ticksOverFull > 0.33 * Props.ticksUntilDanger)
            {
                text += "\n" + "RD_Overfull".Translate(); //"Overfull!";
            }
            else if (ticksOverFull > 0.67f * Props.ticksUntilDanger)
            {
                text += "\n" + "RD_DangrouslyOverfull".Translate(); //Dangrously Overfull!
            }

            result = text;
        }

        return result;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref ticksOverFull, "ticksOverFull");
    }
}