using RimWorld;
using RimWorld.Planet;

namespace ReconAndDiscovery.Utils;

public class GetSitePartUtil
{
    public static SitePart WithDefaultParams(SitePartDef sitePartDef, Site site, int tile, Faction faction,
        bool hidden = false)
    {
        return new SitePart(
            site,
            sitePartDef,
            sitePartDef.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)
        )
        {
            hidden = hidden
        };
    }
}