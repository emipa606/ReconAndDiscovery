using ExpandedIncidents.Settings;
using Verse;

namespace ReconAndDiscovery;

public class Mod : Verse.Mod
{
    public Mod(ModContentPack content) : base(content)
    {
        RaD_Mod.LogMessage("Init mod please");
    }
}