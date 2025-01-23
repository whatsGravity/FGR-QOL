# FGR_QOL Design
The goal of this mod is to make full-game run challenges more fun and accessible, while also making it easier to track stats.

## Features
- Instant restarts (restart a run without needing to delete and recreate a file).
- Stat tracking across runs
- Practice mode?
- Custom start & end-points ?
- Segments?
- CCT-like overlay
- OH PacePings


## Instant Restarts
At any point in time, player should be able to restart the run.
Modes:
 - Manual Restart (Add a menu option)
 - Restart on death (Act like a golden, run's should automatically resart on death)
 - Restart on death before PB

Restart types:
 - Restart to a setup "save file"
   - This will be a "fake" save file (Just render a save file UI for the correct campaign)
   - entering basically acts the same as entering any other new file, just in FGR mode
 - Restart into GP
   - What it sounds like, just restart back to the first campaign chapter

Re-entering a FGR save file:
 - "Practice"
   - 
 - "Run"
 - "Restart Run"


### Stat tracking
Infinite possibilities -- for the short term, keep it simple.
 - Keep a snapshot of each runs journal
 - 

## Integrations
- Hardcore Mod
- CCT

## High-Level design

Each save-file is actually three save files (someone will kill me for this)
 - file 1: Practice file
   - This is just so you don't need to practice on a separate physical file -- people might still choose to, that's ok, but the option is there
 - file 2: FGR file 
   - wait this might not be a file -- what I actually want is just to save journal stats per run
 - file 3: Current run file (gets cleared)

Creating a file in FGR mode generates these three files. Only the practice and Current run files are actually enterable, FGR file is just to store stats across the grind.

When entering a previously existing FGR file, you can choose between:
1. Starting a new run
2. Continuing the existing run
3. Entering the practice file


