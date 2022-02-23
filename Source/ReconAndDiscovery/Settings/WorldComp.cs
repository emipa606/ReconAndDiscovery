using RimWorld.Planet;

namespace ExpandedIncidents.Settings;

internal class WorldComp : WorldComponent
{
    public WorldComp(World world) : base(world)
    {
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        RaD_Mod.LogMessage("Settings loaded", true);
        RaD_ModSettings.ChangeDefPost();
    }
}