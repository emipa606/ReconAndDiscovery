using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;

namespace ReconAndDiscovery.Missions.QuestComp;

public class Festival : WorldObjectComp
{
    public List<Faction> AttendingFactions;
    public Faction HostFaction;

    public void SetupFactions(Faction hostFaction, List<Faction> attendingFactions)
    {
        HostFaction = hostFaction;
        AttendingFactions = attendingFactions.FindAll(faction => faction != hostFaction);
    }
}