# GitHub Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose
The "Recon and Discovery" mod for RimWorld enhances gameplay by adding a variety of new adventures, incidents, and world conditions that players can encounter. It is primarily aimed at expanding the depth and excitement within the game by introducing new challenges, structures, and events that enrich the colony's story.

## Key Features and Systems
- **Adventure Generation**: Includes various "GenStep_AdventureGenerator" classes for creating complex adventure scenarios such as abandoned labs, crashed ships, and ancient castles.
- **Incident Management**: Contains classes like "IncidentWorker" to handle unique incidents, e.g., "Muffalo Mass Insanity" and "Seraphites Quest."
- **Site Part Workers**: Implements objects and events at generated sites, such as computer terminals, enemy raids, and peace talk negotiations.
- **Symbol Resolvers**: Defines how objects are placed in generated environments using classes like "SymbolResolver_AbandonedLab."
- **Job Drivers and Work Givers**: Adds new roles for pawns, such as "JobDriver_DiscoverStargates" and "WorkGiver_Sacrifice."
- **Harmony Patching**: Utilizes Harmony to ensure optimal modification of vanilla methods dynamically.

## Coding Patterns and Conventions
- **Naming Conventions**: Class names are descriptive and use PascalCase, e.g., `IncidentWorker_MuffaloHerd`. Private methods are prefixed with lowercase letters followed by PascalCase, e.g., `private void TryPlaceDoor(IntVec3 loc)`.
- **Structure**: Classes are organized with public classes at the top of the file, followed by private methods.
- **Modular Approach**: Uses inheritance extensively to promote code reuse, as seen with `GenStep_AdventureGenerator` and its subclasses.
- **XML Integration**: Define game objects and configurations within XML files, integrated using C# classes like `AdventureDef` and `ThingDefOfReconAndDiscovery`.

## XML Integration
The mod leverages RimWorld's XML structure to define new game elements:
- **Defs**: Uses XML to define new incidents, jobs, and structures via `IncidentDef`, `JobDef`, and others.
- **XML and C# Communication**: C# scripts interact with XML definitions through classes derived from base RimWorld classes like `Def` and `ThingComp`.

## Harmony Patching
Harmony is used to patch existing RimWorld methods to avoid altering the game's base code directly:
- **Patch Application**: Uses Harmony to apply patches in the mod's initialization method in classes like `RaD_Mod`, ensuring compatibility and minimizing conflicts.
- **Example Patching**: Hooks into game lifecycle events to add custom logic, often in `Mod` or initialization methods.

## Suggestions for Copilot
- When suggesting code updates or completions, focus on maintaining existing coding conventions.
- Promote modular and reusable code suggestions to fit within the mod's structure.
- In XML integration or handling, suggest patterns such as loading defs from XML files using existing RimWorld utilities.
- For Harmony patches, assist in creating correct transpiler methods or prefix/postfix patches to modify game behavior safely.
- Ensure that method names suggest their purpose clearly, adhering to conventions seen in the mod like `IncidentWorker_*` for incident-related functionalities.
- Assist with generating new adventure scenarios by suggesting possible methods to override or existing classes to extend.
