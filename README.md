# Grid-based building system

Work-in-progress prototype of a grid-based building system for my tower defense game 

## Overview
This project is a work-in-progress prototype of a grid building system developed in C# using Unity Engine.
The goal of the projects is to simulate a typical RTS building system with tile aware structure placements.
The focus of the project is data-driven grid logic, object placement validation, and a clean separation between game state and visuals.

## Core features

### Logical tile grid system
- Custom Tile management system build on top of Unity´s build-in ``` Grid ``` commponent
- Each Grid cell is represented by a ``` GameTile ``` data object
- Tiles are stored in a Dictionary with ``` Vector3Int ``` as key value and ``` GameTile ``` as return type
- Individual tiles track:
    - Local grid position
    - Global world position
    - Debug name derived from grid coordinates
    - Tile state (isEmpty)
    - Reference to placed structure Object and tile-type
 
### Building Placement System
- Mouse position is raycasted into world space and snapped to grid
- Structures can occupy multiple tiles
- Placement validation with:
  - Boundary Checks
  - Occupancy checks
  - Overlap checks
- Supports primitive rotation (work in progress)

### Preview System
- Real-time placement preview alignment to the grid
- Visual feedback for placement validity
- Dynamic preview updates based on structure footprint and rotation

## Showcase

![PreviewBuildingSystem] (PreviewBuildingSystem.gif)


## Technical Highlights

### C# / Unity
- MonoBehaviour lifecycle management
- ScriptableObjects for data-driven structures
- Grid bases coordinates conversion (world to cell)

### Data Structures
- Dictionary-based spatial indexing for fast tile lookup, instead of Unity´s build in ``` Tilemap ```
- Seperation of tille logic from Unity scene
- Clean ownership of global grid state via singelton pattern

### System Design
- Logical grid independent of visuals
- Extensible tile metadata system


> ** Note: **
> All code in this project was written by me without the use of advanced language models.
> Language models were only used for spelling and grammar review of this README.
> You are free to use this code as you wish; however, please note that this is a work-in-progress prototype and may require adaptation for different use cases or production environments

