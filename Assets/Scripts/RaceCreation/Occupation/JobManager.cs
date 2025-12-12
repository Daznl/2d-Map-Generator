using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mono.Cecil;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System;
using System.Linq;
using static MapArrayScript;
using UnityEngine.Rendering;
using System.Text;

public class Job
{
    //public int JobId { get; private set; }

    public string JobName;
    public GameResource ResourceType;
    public int ResourcesGathered;
    public int Priority;
    public Character AssignedCharacter;

    // Adjusted constructor
    public Job(/*int jobId,*/ string jobName, GameResource resourceType, int resourcesGathered, int priority, Character assignedCharacter)
    {
        //JobId = jobId; // ID is now passed as a parameter
        JobName = jobName;
        ResourceType = resourceType;
        ResourcesGathered = resourcesGathered;
        Priority = priority;
        AssignedCharacter = assignedCharacter;
    }
}
public class JobManager
{
    private int nextJobId = 1; // Unique job ID generator for this instance

    public List<Job> Jobs;
    public List<Job> availableJobs;
    public List<Job> activeJobs;

    public List<Job> activeBuildJobs;


    public void Initialise(RaceManager raceManager)
    {
        Jobs = new List<Job>();
        activeJobs = new List<Job>();
        availableJobs = new List<Job>();
        activeBuildJobs = new List<Job>();

        CreateJobsAroundSpawn(raceManager);
    }

    public void CreateJobsAroundSpawn(/*RaceProperties raceProperties, */RaceManager raceManager)
    {

        StringBuilder logBuilder = new StringBuilder();
        logBuilder.AppendLine($"Creating jobs around {raceManager.raceProperties.RaceName}, position ({raceManager.spawnPoint.x}, {raceManager.spawnPoint.y})");

        int maxStaminaForMovement = raceManager.raceProperties.Stamina / 2; // Half stamina reserved for the return trip
        GenericCoordinate spawnPoint = raceManager.spawnPoint; // Actual spawn point

        for (int x = -maxStaminaForMovement; x <= maxStaminaForMovement; x++)
        {
            for (int y = -maxStaminaForMovement; y <= maxStaminaForMovement; y++)
            {
                if (x == 0 && y == 0) // Adjust if the spawnPoint is not the center
                {
                    continue; // Skip the spawn point itself
                }

                int adjustedX = spawnPoint.x + x;
                int adjustedY = spawnPoint.y + y;

                // Calculate the total movement cost considering a round trip
                int totalMovementCost = (Math.Abs(x) + Math.Abs(y)) * 2 * raceManager.raceProperties.MovementCost;

                if (totalMovementCost <= raceManager.raceProperties.Stamina - 1)
                {
                    // Assuming resources can be gathered with the remaining stamina
                    int amountOfResourcesGathered = raceManager.raceProperties.Stamina - totalMovementCost;

                    // Check for wood at the current location

                    bool bonusFoodAtLocation = raceManager.allocatedTerritory.TerritoryResources.bonusFood
                        .Any(food => food.x == adjustedX && food.y == adjustedY);

                    bool mainWoodAtLocation = raceManager.allocatedTerritory.TerritoryResources.mainWood
                        .Any(wood => wood.x == adjustedX && wood.y == adjustedY);

                    bool bonusWoodAtLocation = raceManager.allocatedTerritory.TerritoryResources.bonusWood
                        .Any(wood => wood.x == adjustedX && wood.y == adjustedY);

                    bool mainOreAtLocation = raceManager.allocatedTerritory.TerritoryResources.mainOre
                        .Any(ore => ore.x == adjustedX && ore.y == adjustedY);

                    bool bonusOreAtLocation = raceManager.allocatedTerritory.TerritoryResources.bonusOre
                        .Any(ore => ore.x == adjustedX && ore.y == adjustedY);

                    bool luxuryResourceAtLocation = raceManager.allocatedTerritory.TerritoryResources.luxuryResource
                        .Any(luxury => luxury.x == adjustedX && luxury.y == adjustedY);

                    Vector2Int adjustedLocation = new Vector2Int(adjustedX, adjustedY);

                    // Always add a job for food since it's everywhere
                    AddGatheringJob(null, adjustedLocation, amountOfResourcesGathered, GameResource.MainFood, "food", 0, true, false, logBuilder);

                    AddGatheringJob(mainWoodAtLocation, adjustedLocation, amountOfResourcesGathered, GameResource.MainWood, "main wood", 0, true, false, logBuilder);

                    AddGatheringJob(bonusWoodAtLocation, adjustedLocation, amountOfResourcesGathered, GameResource.BonusWood, "bonus wood", 0, true, false, logBuilder);

                    AddGatheringJob(mainOreAtLocation, adjustedLocation, amountOfResourcesGathered, GameResource.MainOre, "main ore", 0, true, false, logBuilder);

                    AddGatheringJob(bonusOreAtLocation, adjustedLocation, amountOfResourcesGathered, GameResource.BonusOre, "bonus ore", 0, true, false, logBuilder);

                    AddGatheringJob(luxuryResourceAtLocation, adjustedLocation, amountOfResourcesGathered, GameResource.LuxuryResource, "luxury resource", 0, true, false, logBuilder);

                }
                else
                {
                    //Debug.Log($"Position ({adjustedX}, {adjustedY}) is out of reach based on stamina and movement cost.");
                }
            }
        }

        logBuilder.AppendLine($"Finished creating jobs. Total available jobs: {availableJobs.Count}");
        Debug.Log(logBuilder.ToString());
    }


    public void AddGatheringJob(bool? check, Vector2Int location, int amountOfResourcesGathered, GameResource resourceType, string resourceTag, int priority, bool isResource, bool isBuilding, StringBuilder logBuilder)
    {
        if (check == false) return; // Early exit if resource check is false

        string jobTypeDescription = isResource ? "Gathering" : isBuilding ? "Building" : "Generic";
        string description = $"{jobTypeDescription} {amountOfResourcesGathered} {resourceTag} at ({location.x}, {location.y})";

        // Append the job creation detail to the logBuilder
        logBuilder.AppendLine($"Adding job for {description}");

        Job newJob = new Job(description, resourceType, amountOfResourcesGathered, priority, null);
        availableJobs.Add(newJob);
        Jobs.Add(newJob);
    }

    public void AddJob(Job job)
    {
        availableJobs.Add(job);
        Jobs.Add(job);
    }


    public void AssignJob(RaceManager raceManager, Character character, Job jobToAssign)
    {
        if (jobToAssign == null || character == null)
        {
            Debug.LogError("AssignJob: jobToAssign or character is null");
            return;
        }

        // Link the job to the character
        character.Data.job = jobToAssign;
        jobToAssign.AssignedCharacter = character;

        // Move the job from available to active
        availableJobs.Remove(jobToAssign);
        activeJobs.Add(jobToAssign);

        // Update character status
        raceManager.aliveCharacters.UnEmployed.Remove(character);
        raceManager.aliveCharacters.Employed.Add(character);
    }

    public void UnassignJob(RaceManager raceManager, Character character, Job jobToUnassign)
    {
        if (jobToUnassign == null || character == null || character.Data.job != jobToUnassign)
        {
            Debug.LogError("UnassignJob: jobToUnassign, character, or character's job is null or does not match jobToUnassign");
            return;
        }

        // Clear the job assignment from the character
        character.Data.job = null;
        jobToUnassign.AssignedCharacter = null;

        // Move the job from active back to available
        activeJobs.Remove(jobToUnassign);
        availableJobs.Add(jobToUnassign);

        // Update character status
        raceManager.aliveCharacters.Employed.Remove(character);
        raceManager.aliveCharacters.UnEmployed.Add(character);
    }

    public void RemoveJob(RaceManager raceManager, Job job)
    {
        activeJobs.Remove(job);
        availableJobs.Remove(job);
        Jobs.Remove(job);

        if (job.AssignedCharacter != null)
        {
            Character character = job.AssignedCharacter;

            character.Data.job = null; // Clear the job reference

            // Remove the character from the employed list and add to the unemployed list
            raceManager.aliveCharacters.Employed.Remove(character);
            if (!raceManager.aliveCharacters.UnEmployed.Contains(character))
            {
                raceManager.aliveCharacters.UnEmployed.Add(character);
            }
        }

        Debug.Log($"{job.JobName} removed successfully from all lists.");
        
    }

    public void CalculateJobPriority(ResourceManager resourceManager)
    {
        StringBuilder logBuilder = new StringBuilder("Job Priorities Calculated:\n");

        foreach (var job in availableJobs)
        {
            // Calculate priority based on the type of resource this job gathers
            switch (job.ResourceType)
            {
                case GameResource.MainFood:
                    job.Priority = (int)(job.ResourcesGathered * resourceManager.resourceStruct.FoodPriority);
                    break;
                case GameResource.MainWood:
                    job.Priority = (int)(job.ResourcesGathered * resourceManager.resourceStruct.WoodPriority);
                    break;
                case GameResource.MainOre:
                    job.Priority = (int)(job.ResourcesGathered * resourceManager.resourceStruct.OrePriority);
                    break;
                case GameResource.BonusFood:
                    job.Priority = (int)(job.ResourcesGathered * resourceManager.resourceStruct.BonusFoodPriority);
                    break;
                case GameResource.BonusWood:
                    job.Priority = (int)(job.ResourcesGathered * resourceManager.resourceStruct.BonusWoodPriority);
                    break;
                case GameResource.BonusOre:
                    job.Priority = (int)(job.ResourcesGathered * resourceManager.resourceStruct.BonusOrePriority);
                    break;
                case GameResource.LuxuryResource:
                    job.Priority = (int)(job.ResourcesGathered * resourceManager.resourceStruct.LuxuryResourcePriority);
                    break;
                    // Add any additional resource types here
            }
            // Append job priority update to logBuilder
            logBuilder.AppendLine($"Job '{job.JobName}' updated to {job.Priority}.");
        }

        // Sort the list of jobs by priority, highest first
        availableJobs.Sort((job1, job2) => job2.Priority.CompareTo(job1.Priority));

        // Output the final sorted job list priorities
        //Debug.Log(logBuilder.ToString());
    }

    public void ReassignJobsBasedOnPriority(RaceManager raceManager)
    {
        raceManager.jobManager.availableJobs.Sort((job1, job2) => job2.Priority.CompareTo(job1.Priority)); // Higher first
        raceManager.jobManager.activeJobs.Sort((job1, job2) => job1.Priority.CompareTo(job2.Priority)); // Lower first

        if (raceManager.jobManager.availableJobs.Count > 0 && raceManager.jobManager.activeJobs.Count > 0 &&
           raceManager.jobManager.availableJobs.First().Priority > raceManager.jobManager.activeJobs.Last().Priority)
        {
            Character characterToReassign = raceManager.jobManager.activeJobs.Last().AssignedCharacter;
            Job oldJob = characterToReassign.Data.job;
            Job newJob = raceManager.jobManager.availableJobs.First();

            // First, unassign the old job
            UnassignJob(raceManager, characterToReassign, oldJob);

            // Then, assign the new job
            AssignJob(raceManager, characterToReassign, newJob);

            // Debug information after reassignment
            Debug.Log($"Reassigning Job for {characterToReassign.Data.name}: from '{oldJob.JobName}' to '{newJob.JobName}'. Now works as '{characterToReassign.Data.job.JobName}'.");
        }
    }

    public void AssignJobsToUnemployed(RaceManager raceManager)
    {
        foreach (Character unemployed in raceManager.aliveCharacters.UnEmployed.ToList()) // Use ToList() to safely modify during iteration
        {
            if (raceManager.jobManager.availableJobs.Count > 0)
            {
                Job jobToAssign = raceManager.jobManager.availableJobs.First(); // Select the first available job
                raceManager.jobManager.AssignJob(raceManager, unemployed, jobToAssign);
            }
        }
    }

}
