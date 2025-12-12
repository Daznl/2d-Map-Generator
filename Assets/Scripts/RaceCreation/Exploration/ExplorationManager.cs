using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.TextCore.Text;
using static BuildingManager;
using UnityEditor.Build;
using static MapArrayScript;
using System.Xml.Serialization;
using Mono.Cecil;
using System;
using UnityEngine.Rendering;
using static UnityEditor.ShaderKeywordFilter.FilterAttribute;
using UnityEditor.VersionControl;


public class ExplorationManager
{
    public bool initialSearch = false;

    public ExpeditionManager expeditionManager;
    public FindSpawnPoints findSpawnPoints;
    public RaceManager raceManager;
    public GameManager gameManager;

    public bool constructingNewTown;

    public class AccessibleBlocks
    {
        public HashSet<GenericCoordinate> ExploredBlocks { get; set; } = new HashSet<GenericCoordinate>();
        public HashSet<GenericCoordinate> SeenBlocks { get; set; } = new HashSet<GenericCoordinate>();
    }

    public AccessibleBlocks accessibleBlocks;

    public struct ResourceScores
    {
        public int FoodScore;
        public int WoodScore;
        public int OreScore;
        public int BonusFoodScore;
        public int BonusWoodScore;
        public int BonusOreScore;

    }
    public bool searchAroundTownsComplete;
    public bool searchAroundTownsInProgress;


    public void Initialise(RaceManager inRaceManager, GameManager inGameManager)
    {
        constructingNewTown = false;
        gameManager = inGameManager;
        expeditionManager = new ExpeditionManager();
        expeditionManager.Initialise();
        findSpawnPoints = new FindSpawnPoints();
        raceManager = inRaceManager;

        expeditionManager.gameManager = inGameManager;
        initialSearch = false;
        searchAroundTownsComplete = false;
        searchAroundTownsInProgress = false;
        accessibleBlocks = new AccessibleBlocks();

        UpdateExplorationAndVisibility(raceManager.spawnPoint, inGameManager.globalMap);
    }

    public void UpdateExplorationAndVisibility(GenericCoordinate currentPosition, Dictionary<GenericCoordinate, Block> globalMap)
    {
        UpdateExploredBlocks(currentPosition, globalMap);
        UpdateSeenBlocksBasedOnExplored(currentPosition, globalMap);
        //Debug.Log("UpdateExplorationAndVisibility");
    }

    // Updates explored blocks from the current position
    private void UpdateExploredBlocks(GenericCoordinate currentPosition, Dictionary<GenericCoordinate, Block> globalMap)
    {
        //Debug.Log("UpdateExploredBlocks");
        // Assuming the radius for directly explored blocks is smaller
        UpdateBlocksAroundPoint(currentPosition, 1, accessibleBlocks.ExploredBlocks, globalMap); // Example radius: 1
    }

    // Updates seen blocks based on the explored blocks
    private void UpdateSeenBlocksBasedOnExplored(GenericCoordinate currentPosition, Dictionary<GenericCoordinate, Block> globalMap)
    {
        //Debug.Log("UpdateSeenBlocksBasedOnExplored");
        // Iterate over each explored block and update surrounding blocks as "seen"
        // Assuming a larger radius for seen blocks around each explored block
        foreach (var exploredCoord in accessibleBlocks.ExploredBlocks)
        {
            UpdateBlocksAroundPoint(exploredCoord, 2, accessibleBlocks.SeenBlocks, globalMap); // Example radius: 2 for seen
        }

        // Directly update seen blocks around the current position with an even larger radius
        // This accounts for areas the explorer can see but hasn't directly explored
        UpdateBlocksAroundPoint(currentPosition, 3, accessibleBlocks.SeenBlocks, globalMap); // Example radius: 3 for seen
    }

    private void UpdateBlocksAroundPoint(GenericCoordinate center, int distance, HashSet<GenericCoordinate> targetSet, Dictionary<GenericCoordinate, Block> globalMap)
    {
        for (int dx = -distance; dx <= distance; dx++)
        {
            for (int dy = -distance; dy <= distance; dy++)
            {
                if (Math.Abs(dx) + Math.Abs(dy) <= distance)
                {
                    GenericCoordinate coord = new GenericCoordinate(center.x + dx, center.y + dy);
                    if (globalMap.ContainsKey(coord))
                    {
                        targetSet.Add(coord);
                    }
                }
            }
        }
    }


    public void HandleExpansion(RaceManager raceManager)
    {
        if (CheckIfShouldExpand(raceManager, raceManager.resourceManager, raceManager.buildingManager))
        {
            if (searchAroundTownsComplete && !constructingNewTown)
            {
                CoordinateWithAmount location = PickExpansionLocation();
                raceManager.buildingManager.ConstructNewTown(location, raceManager);

                constructingNewTown = true;
                Debug.Log("HandleExpansion: FindExpansionsLocation");
            }
            else if (!searchAroundTownsComplete && !searchAroundTownsInProgress)
            {
                Debug.Log("HandleExpansion: Search Around Towns");
                SearchAroundTowns(raceManager.buildingManager.StartingTown, raceManager.jobManager);
                
            }
        }
    }
    public bool CheckIfShouldExpand(RaceManager raceManager, ResourceManager resourceManager, BuildingManager buildingManager)
    {
        int maxFoodOutput = resourceManager.DetermineMaxFoodOutput(raceManager);
        int currentFoodOutput = resourceManager.resourceStruct.FoodProductionRate;
        int currentFood = resourceManager.resourceStruct.Food;

        // Check for foodproduction is Maximised and there is no food
        if (maxFoodOutput == currentFoodOutput && currentFood < 100)
        {
            return true;
        }

        // Then check if all towns, including starting and expansion, are at housing capacity
        return buildingManager.AllTownsAtHousingCapacity();

    }

    public void SearchAroundTowns(Town town, JobManager jobManager)
    {
        List<Job> jobs = new List<Job>();
        Vector2Int location = new Vector2Int(town.Location.x, town.Location.y);

        string[] jobNames = new string[] { "Search North", "Search South", "Search East", "Search West" };
        List<SearchConditions> searchConditions = new List<SearchConditions>();

        foreach (string jobName in jobNames)
        {
            Job job = new Job(jobName, GameResource.Explorer, 0, 10, null);
            jobs.Add(job);
            jobManager.AddJob(job);
            Debug.Log($"Creating job '{jobName}' for Expedition");
            SearchConditions newSearchCondition = new SearchConditions();
            searchConditions.Add(newSearchCondition);

        }

        Expedition expedition = new Expedition(jobs, ExpeditionType.SearchNSEWFromPoint, searchConditions, location);
        expeditionManager.expeditions.Add(expedition);
        searchAroundTownsInProgress = true;
        Debug.Log($"Creating Expedition");
    }

    public CoordinateWithAmount PickExpansionLocation()
    {
        findSpawnPoints.Initialize(gameManager);
        List<CoordinateWithAmount> potentialLocations = findSpawnPoints.FindSpawnPointsForPreference(raceManager.raceProperties.LandPreference, accessibleBlocks.SeenBlocks);
        // Check if there are any potential locations found
        if (potentialLocations.Count == 0)
        {
            Debug.LogError("No potential expansion locations found.");
        }

        // Randomly select one of the potential locations using Unity's Random.Range
        int randomIndex = UnityEngine.Random.Range(0, potentialLocations.Count);
        CoordinateWithAmount selectedLocation = potentialLocations[randomIndex];

        Debug.Log($"Selected Expansion Location: X={selectedLocation.x}, Y={selectedLocation.y}");

        return selectedLocation;

    }
}
