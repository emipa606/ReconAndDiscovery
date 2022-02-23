using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;

namespace ReconAndDiscovery.Missions.QuestComp
{
    public class Festival: WorldObjectComp
    {
        public Faction HostFaction;
        
        public List<Faction> AttendingFactions;

        public void SetupFactions(Faction hostFaction, List<Faction> attendingFactions)
        {
            HostFaction = hostFaction;
            AttendingFactions = attendingFactions.FindAll(faction => faction != hostFaction);
        }
    }
}