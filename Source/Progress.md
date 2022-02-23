## Progress

#### Working with no changes

 * Site part
   * Abandoned Ship
   * Osiris Casket
 * Quest
   * Osiris Casket Quest
   * Seraphites Quest
   * Quakes Quest
   * Radiation Quest
 * Map Incident
   * RD_IncidentRadiation
   * RD_IncidentTremors
   * RD_MalevolantAI
   * RD_QuestRadiation
   * RD_RaidStargate
   * RD_WarIdol
   * RD_SeraphitesLab
 * World Incident
   * RD_DiscoveredStargate
 * Symbol Resolvers
   * SymbolResolver_OldCastle
   * SymbolResolver_EdgeShields
   * SymbolResolver_WireOutline
   * SymbolResolver_RoomWithDoor
 * Items
   * Teleporters
   * Osiris Casket
   * Portable Generators
   * War Idols
 * Animals
   * Nitralopes
   * Devillo

#### Tested and Fixed

 * [World Object Generation](#World Object Generation)
 * Map Incident
   * [RD_RaidEnemyQuest](#RD_RaidEnemyQuest)
   * [RD_RaidTeleporter](#RD_RaidTeleporter)
   * [RD_MuffaloMassInsanity](#RD_MuffaloMassInsanity)
 * Site Part
   * [RD_Nanites](#RD_Nanites)
   * [RD_AbandonedLab](#RD_AbandonedLab)
   * [RD_AbandonedColony](#RD_AbandonedColony)
 * Quests
   * [Crashed Ship](#RD_CrashedShip)
   * [RD_PeaceTalks](#RD_PeaceTalks)
   * [RD_TradeFair](#RD_TradeFair)
   * [RD_MuffaloHerdMigration](#RD_MuffaloHerdMigration)
 * Buildings
   * [Stargate](#Stargate)
   * [Holo Emitter](#Holo Emitter)
 * Item
   * [Seraphites](#Seraphites)
   * [War Idol Sacrifice](#War Idol Sacrifice)
   * [Weather Idol](#Weather Idol) 

#### Things to look at later
 * SymbolResolver_CrashedShip
    * It looks like it's meant to be generating 3 engines while it's only generating 1. Perhaps don't re-use the same rect and keep changing it?
 * SymbolResolver_PathOfDestruction
    * I'm not sure if this is working? Or what it's meant to do?
 * During the Peace Talks a bunch of null exception errors are being thrown about whether you can trade with the enemy leader if you save and load in the peace talks site
 * RD_Nanites
   * I don't think this was ever implemented?
 * Medical Emergency
   * Instead of kicking players out of the Medical Emergency when they form a new faction, we should wait for players to leave on their own accord and generate the new faction
 * Holo Emitter
   * When a hologram pawn dies, it should teleport back to the hologram base before resurrecting
 * RD_MuffaloMassInsanity
   * The muffalo herd that spawned during testing was **massive**, turning half of them manhunter seems... very harsh
 * Nitralope Overfull alert should point to the nitralopes that need to be milked
 * Weather Idol
   * Could transition this to psi meditation like anima trees
   * Make the weather changes Gizmo's instead of clicks
   
#### Untested
 * Action Triggers

#### Still Broken
Nothing is broken right now

## Details of Fixes

#### RD_RaidEnemyQuest

The problem with this incident worker was that `TryExecuteWorker` was calling `base.TryWorker` instead of `base.TryExecuteWorker`. 
`base.TryWorker` then calls `TryExecuteWorker` to see if it can execute, this caused an infinite loop that would crash Rimworld.
Even with this fixed though, the incident worker for this doesn't seem to **do** anything. It's just a regular enemy raid.
Perhaps it was an intended feature that was never made?

#### RD_Nanites
The site texture was pointing to the wrong location

#### RD_AbandonedLab

The resolver to actually put the items in the places upon world generation appear to be have been done incorrectly. The Chemfuel
generator wasn't be added because the Rect size was defined incorrectly and the power cables weren't being placed because they needed to be
pushed to the resolver stack before the room

#### RD_CrashedShip
`ReconAndDiscovery.Maps.GenStep_RareBeasts.Generate` threw an exception on generating rare beasts because the tile id being passed in
to generate the pawn request was `-1`, which throws an error if you're also telling it to generate an inhabitant pawn. I fixed this 
by just passing in the tile of the generation request.

If it's a Medical Quest then the faction for that quest was a static faction that would sometimes bug out and pick the wrong faction, one that
wasn't the injured pawns that you need to save. To fix this I've made the fact that the quest is for non-static and generated on quest start.
On top of that, if they decided to form a new Faction then they would generate as a faction using the Ancients parameters. The problem here is
that the Ancients don't have any icons to put their settlement on the map, so that would error out when you tried to leave. This was fixed by
changing their faction generation params to OutlanderCivil

#### World Object Generation
This one was breaking pretty much every world site that was being generated. It looks like 1.3 added the idea of raid timers being
triggered after you entered a site. Either the site part needed a `<disallowsAutomaticDetectionTimerStart>true</disallowsAutomaticDetectionTimerStart>`
tag added to them, or the world object needed a `<li Class="WorldObjectCompProperties_TimedDetectionRaids" />` entry in it's comps.
The sites generated by this mod had neither, causing a null exception to occur in `Site.PostGenerateMap` which would then make the site
disappear immediately.

#### RD_RailTeleporter
While trying to spawn in Scythers it didn't have the correct definition for Scythers

#### Stargate
When trying to move to a tile that hasn't been visited yet it tries to generate a new map. The problem is that was trying to set the size
of the map to generate based on the `LinkedTile.Map.Site` even though `LinkedTile.Map` was null

#### RD_AbandonedColony
The XML def for this just had an empty `<genStep Class="GenStep_Outpost"></genStep>` child attribute. I'm not sure which of the missing children
was causing the error where it couldn't generate any suitable pawns, but changing the XML definition to be more inline with how they're defined
in the base game made the error disappear and the colonies to generate just fine

#### RD_PeaceTalks
Peace talks was broken because it was assigning the World Object to the Faction that you went to talk to, so as soon as you entered the map
it counted as you attacking them and you'd immediately be hostile. Changing the faction of the WorldObject to Insects and making
`SitePartWorker_PeaceTalksFaction` fetch the faction from `QuestComp_PeaceTalks` instead of the map tile Faction solves this issue. Additionally
the calculation for how to change your relationship was broken and it was resulting in too much relationship being given. Finally, had to add a line
so that all doors are open on the map and to defog the map on generation so you can always find the person you want to negotiate with

#### RD_TradeFair
Similar to the issue with [RD_PeaceTalks](#rd_peacetalks), the faction owner was set to the faction that was hosting the trade fair. Unfortunately, this
meant that entering the site would count attacking the village of the fair's host. Instead we don't set the site's faction, we spawn in an abandoned colony
instead and we pass in the host and attending factions through a comp rather than trying to fetch them by the world objects faction.

For the moment, I've changed the area that is generated to be that of AbandonedCastle, but it might be a good idea to see if we can't re-use AbandonedColony
somehow.

#### Holo Emitter
It looks like there was a weird condition regarding the CompOsiris and whether it should transfer the pawn back to the emitter or whether it should ressurect it.
I side-stepped it by just always resurrecting it, but I suppose I might have removed it being teleported back to the emitter if the hologram dies. Also, when it was
resurrecting pawns as holograms, it was trying to use the Corpse's position to spawn the hologram even though the Corpse didn't exist anymore, which prevented the spawn
all together. Killing the hologram and then immediately destroying it's corpse during formatting caused an exception with the burial obligation filter, so when
formatting first we set it's ideo to null so that no rituals pop up and then we just destroy the pawn instead of killing it.

#### Seraphites
Taking the Seraphites, while it would work, caused an exception in the health tick. It looks like this was caused by trying to remove both luciferium hediffs at once
so I changed the loop to remove the addiction first and the high in the next tick. Additionally it now only looks for Luciferium addiction instead of removing all
addictions

#### RD_MuffaloHerdMigration
It doesn't send the message that the herd might be going mad soon because in `PostMapGenerate` none of the colonists are in the `map.mapPawns` yet

#### RD_MuffaloMassInsanity
`CanFireNowSub` was calling `CanFireNow`, which causes an infinite loop as `CanFireNow` calls `CanFireNowSub`. Additionally, it wasn't sending the warning message that
a herd would go manhunter if you had any psychic pawns because it was looking for your pawns on the map too early. I solved this by queuing up a warning incident for
100 ticks after entering to give your pawns time to arrive

#### War Idol Sacrifice
War Idol Sacrifice failed when tyring to carry the item because the JobGiver wasn't setting a count for the JobDriver

#### Weather Idol
Made it not consume mana for "Clear weather" if weather was already clear and not consume mana for "Strike down our enemies" if there are no enemies

## Things to Revert
Nothing to revert at this time