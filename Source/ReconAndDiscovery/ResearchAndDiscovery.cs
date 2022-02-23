using HarmonyLib;
using Verse;

namespace ReconAndDiscovery
{
    public class Mod: Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            Log.Message("Init mod please");
        }      
    }
}