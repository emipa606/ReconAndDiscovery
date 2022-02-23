using RimWorld;
using Verse;

namespace ReconAndDiscovery;

public class OsirisCasket : Building_CryptosleepCasket
{
    public Corpse Corpse
    {
        get
        {
            foreach (var thing in innerContainer)
            {
                if (thing is Corpse corpse)
                {
                    return corpse;
                }
            }

            return null;
        }
    }
}