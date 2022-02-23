using Verse;

namespace ReconAndDiscovery;

public class CompProperties_WeatherSat : CompProperties
{
    public float tickCharge = 0.5f;

    public CompProperties_WeatherSat()
    {
        compClass = typeof(CompWeatherSat);
    }
}