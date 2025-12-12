using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static BuildingManager;

public class BuildingManager
{
    private Dictionary<int, int> townHouseMax = new Dictionary<int, int>
    {
        { 1, 10 },
        { 2, 50 },
        { 3, 100 }
    };

    public int GetTownHouseMaxPerLevel(int townLevel)
    {
        // Check if the dictionary contains the town level
        if (townHouseMax.TryGetValue(townLevel, out int value))
        {
            return value; // Return the corresponding value if found
        }
        else
        {
            return 0; // Return 0 if the town level is not defined
        }
    }

    public Town StartingTown;

    public List<Town> Expansion { get; set; } = new List<Town>();

    public RaceManager raceManager;



    // Initialize with the initial spawn point town
    public void Initialise(RaceManager raceManager)
    {
        //constructionsInProgress = new List<Construction>();
        Vector2Int initialSpawnPoint = new Vector2Int(raceManager.spawnPoint.x, raceManager.spawnPoint.y);
        StartingTown = new Town("InitialSpawnTown", initialSpawnPoint, 1);

    }

    public void HandleBuilding(RaceManager raceManager, Town town)
    {
        if (!town.constructionsInProgress.Any())
        {
            DetermineBuildingToBuild(raceManager, raceManager.resourceManager, town);
        }
    }

    public void DetermineBuildingToBuild(RaceManager raceManager, ResourceManager resourceManager, Town town)
    {
        float housingNeed = CalculateHousingNeed(raceManager, town);

        Vector2Int location = new Vector2Int(town.Location.x, town.Location.y);

        // Determine if a new house is needed based on the ratio
        if (housingNeed > 1 && town.Houses.Count() < GetTownHouseMaxPerLevel(town.TownLevel))
        {
            if (raceManager.resourceManager.resourceStruct.Wood >= 1000)
            {
                int priority = (int)housingNeed * 4;
                Debug.Log($"prioriy: {priority}");
                ConstructNewHome(raceManager, town, priority);
                resourceManager.resourceStruct.AdjustResourcePriority(GameResource.MainWood, 1.5f);
            }
            else
            {
                resourceManager.resourceStruct.AdjustResourcePriority(GameResource.MainWood, 1.0f);
            }
        }
        else if (housingNeed >= 0.5) // there is one house per person
        {
            resourceManager.resourceStruct.AdjustResourcePriority(GameResource.MainWood, 0.5f);
        }
        else// there are more houses than there are people
        {
            resourceManager.resourceStruct.AdjustResourcePriority(GameResource.MainWood, 0.25f);
        }
    }
    public float CalculateHousingNeed(RaceManager raceManager, Town town)
    {
        int totalAliveCharacters = raceManager.aliveCharacters.Characters.Count();
        int totalHouses = town.Houses.Count();

        // Avoid division by zero by ensuring there's at least one house for calculation
        if (totalHouses == 0) return float.MaxValue; // Indicating an infinite need for housing

        float characterToHouseRatio = (float)(totalAliveCharacters / totalHouses);

        //A value greater than 1 means there are more characters than houses
        //A value of 1 suggests a balance between characters and houses.
        //A value less than 1 indicates more houses than characters
        return characterToHouseRatio;
    }
    private void ConstructNewHome(RaceManager raceManager, Town town, int jobPriority)
    {
        // Assuming Building has a constructor that takes a BuildingFunction enum
        Building houseBuilding = new Building(Building.BuildingFunction.House);
        int totalWoodRequired = 1000; // Total wood required

        // Create a new construction using the defined constructor
        Construction newConstruction = new Construction(totalWoodRequired, houseBuilding, town);

        // Add the new construction to the constructionsInProgress list
        town.constructionsInProgress.Add(newConstruction);
        Debug.Log($"Adding construction: {newConstruction}"); // When adding

        // Create a new building job
        CreateNewBuildingJob(newConstruction, raceManager, 3, jobPriority);
    }

    public void ConstructNewTown(MapArrayScript.CoordinateWithAmount location, RaceManager raceManager)
    {
        int totalWoodRequired = 10000;

        int count = Expansion.Count();

        Vector2Int townLocation= new Vector2Int(location.x, location.y);
        Town newTown = new Town($"expansion{count}",townLocation, 1) ;

        TownConstruction newConstruction = new TownConstruction(totalWoodRequired, townLocation, newTown);
        StartingTown.townConstructionInProgress.Add(newConstruction);
        Debug.Log($"Adding town construction: {newConstruction}");

        CreateNewTownJob(newConstruction, raceManager, 10, 10);
    }

    public void CreateNewTownJob(TownConstruction newConstruction, RaceManager raceManager, int numberOfJobs, int jobPriority)
    {
        for (int i = 0; i < numberOfJobs; i++)
        {
            Debug.Log($"Created New Building Job priority {jobPriority} ");
            Job job = new Job("Build Town", GameResource.Builder, 0, jobPriority, null);
            newConstruction.jobs.Add(job);
            raceManager.jobManager.AddJob(job);
            Debug.Log($"Creating {job.JobName} Job");

        }
    }

    public void CreateNewBuildingJob(Construction newConstruction, RaceManager raceManager, int numberOfJobs, int jobPriority)
    {
        for (int i = 0; i < numberOfJobs; i++)
        {
            Debug.Log($"Created New Building Job priority {jobPriority} ");
            Job job = new Job("Build House", GameResource.Builder, 0, jobPriority, null);
            newConstruction.jobs.Add(job);
            raceManager.jobManager.AddJob(job);
            Debug.Log($"Creating {job.JobName} Job");

        }
    }

    public void ProgressBuilding(RaceManager raceManager, Town town)
    {
        List<Construction> completedConstructions = new List<Construction>();

        // Iterate through each construction in progress
        foreach (Construction construction in town.constructionsInProgress)
        {
            int todaysConstructionProgress = 0; // Reset progress for each construction

            // Iterate through each job within the construction
            foreach (Job job in construction.jobs)
            {
                //Debug.Log("foreach (Job job in construction.jobs)");
                if (raceManager.jobManager.activeJobs.Contains(job))
                {
                    //Debug.Log("if (raceManager.jobManager.activeJobs.Contains(job))");
                    int workerStamina = raceManager.raceProperties.Stamina; // Assuming this is a fixed value per worker per day
                    todaysConstructionProgress += workerStamina; // Adjust as necessary for actual progress calculation
                }
                else
                {
                    //Debug.Log($"No job found for with ResourceType=Builder");
                }
            }

            construction.progress += todaysConstructionProgress;

            // Mark construction for completion if done
            if (construction.progress >= construction.totalToConstruct)
            {
                completedConstructions.Add(construction);
                Debug.Log($"Construction completed. Total progress={construction.progress}");
            }
        }

        // Process completed constructions
        foreach (Construction completedConstruction in completedConstructions)
        {
            CompleteConstruction(raceManager, completedConstruction, town);
        }

        List<TownConstruction> completedTownConstruction = new List<TownConstruction>();

        // Iterate through each construction in progress
        foreach (TownConstruction construction in town.townConstructionInProgress)
        {
            int todaysConstructionProgress = 0; // Reset progress for each construction

            // Iterate through each job within the construction
            foreach (Job job in construction.jobs)
            {
                //Debug.Log("foreach (Job job in construction.jobs)");
                if (raceManager.jobManager.activeJobs.Contains(job))
                {
                    //Debug.Log("if (raceManager.jobManager.activeJobs.Contains(job))");
                    int workerStamina = raceManager.raceProperties.Stamina; // Assuming this is a fixed value per worker per day
                    todaysConstructionProgress += workerStamina; // Adjust as necessary for actual progress calculation
                }
                else
                {
                    //Debug.Log($"No job found for with ResourceType=Builder");
                }
            }

            construction.progress += todaysConstructionProgress;

            // Mark construction for completion if done
            if (construction.progress >= construction.totalToConstruct)
            {
                completedTownConstruction.Add(construction);
                Debug.Log($"Town Construction completed. Total progress={construction.progress}");
            }
        }

        // Process completed constructions
        foreach (TownConstruction completedConstruction in completedTownConstruction)
        {
            CompleteTownConstruction(raceManager, completedConstruction);
        }
    }

    public void CompleteConstruction(RaceManager raceManager, Construction constructionToRemove, Town town)
    {
        Debug.Log("Construction complete");

        town.AddUnoccupiedHouse(constructionToRemove.building);

        // Remove associated jobs
        foreach (Job job in constructionToRemove.jobs)
        {
            raceManager.jobManager.RemoveJob(raceManager, job);
        }

        town.constructionsInProgress.Remove(constructionToRemove);
    }

    public void CompleteTownConstruction(RaceManager raceManager, TownConstruction townConstruction)
    {
        Debug.Log("Town Construction complete");

        Expansion.Add(townConstruction.town);

        // Remove associated jobs
        foreach (Job job in townConstruction.jobs)
        {
            raceManager.jobManager.RemoveJob(raceManager, job);
        }

        StartingTown.townConstructionInProgress.Remove(townConstruction);

        raceManager.explorationManager.constructingNewTown = false;
    }

    public bool AllTownsAtHousingCapacity()
    {
        // Check if the starting town is at capacity
        if (StartingTown.Houses.Count < GetTownHouseMaxPerLevel(StartingTown.TownLevel) || StartingTown.UnoccupiedHouse.Any())
        {
            return false; // Starting town has room or unoccupied houses, no need for expansion
        }

        // Check each expansion town
        foreach (var town in Expansion)
        {
            // If any expansion town is not at capacity or has unoccupied houses
            if (town.Houses.Count < GetTownHouseMaxPerLevel(town.TownLevel) || town.UnoccupiedHouse.Any())
            {
                return false; // Found a town that can still expand internally
            }
        }

        // If starting and all expansion towns are at capacity with no unoccupied houses
        return true; // All towns are at capacity, expansion is necessary
    }

}
