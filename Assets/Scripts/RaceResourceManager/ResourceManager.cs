using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TextCore.Text;
using static BuildingManager;

public class ResourceManager
{
    public ResourceStruct resourceStruct;
    public void Initialise()
    {
        resourceStruct = new ResourceStruct(20000, 0, 0, 0, 0, 0, 0);
    }

    public void CalculateProductionAndConsumption(JobManager jobManager, RaceManager raceManager, Town town)
    {
        CalculateResourceProductionRate(jobManager);
        resourceStruct.AddResource();
        CalculateResourceConsumptionRate(raceManager, town);
        resourceStruct.ConsumeResource();
    }

    public void CalculateResourceConsumptionRate(RaceManager raceManager, Town town)
    {
        CalculateFoodConsumptionRate(raceManager);
        CalculateWoodConsumptionRate(raceManager, town);

    }
    public void CalculateFoodConsumptionRate(RaceManager raceManager)
    {
        int aliveCharactersCount = 0;

        foreach (var character in raceManager.aliveCharacters.Characters)
        {
            aliveCharactersCount++;
        }

        // Assuming each alive character consumes 1 unit of food per time period
        // Adjust the consumption rate per character as necessary
        resourceStruct.FoodConsumptionRate = (int)(aliveCharactersCount * raceManager.raceProperties.FoodConsumption * raceManager.foodConsumptionModifier);
    }

    public void CalculateWoodConsumptionRate(RaceManager raceManager, Town town)
    {
        int totalWoodConsumed = 0;

        foreach (Construction construction in town.constructionsInProgress)
        {
            // Iterate through each job within the construction
            foreach (Job job in construction.jobs)
            {
                if (raceManager.jobManager.activeJobs.Contains(job))
                {
                    int workerStamina = raceManager.raceProperties.Stamina; // Assuming this is a fixed value per worker per day
                    totalWoodConsumed += workerStamina; // Assuming 1 wood is consumed per stamina point used
                }
            }
        }

        resourceStruct.WoodConsumptionRate = totalWoodConsumed;
    }
    public void CalculateResourceProductionRate(JobManager jobManager)
    {
        int totalFoodProduction = 0;
        int totalWoodProduction = 0;
        int totalOreProduction = 0;
        int totalBonusFoodProduction = 0;
        int totalBonusWoodProduction = 0;
        int totalBonusOreProduction = 0;
        int totalLuxuryResourceProduction = 0;

        foreach (Job job in jobManager.activeJobs)
        {
            switch (job.ResourceType)
            {
                case GameResource.MainFood:
                    totalFoodProduction += job.ResourcesGathered;
                    break;
                case GameResource.MainWood:
                    totalWoodProduction += job.ResourcesGathered;
                    break;
                case GameResource.MainOre:
                    totalOreProduction += job.ResourcesGathered;
                    break;
                case GameResource.BonusFood:
                    totalBonusFoodProduction += job.ResourcesGathered;
                    break;
                case GameResource.BonusWood:
                    totalBonusWoodProduction += job.ResourcesGathered;
                    break;
                case GameResource.BonusOre:
                    totalBonusOreProduction += job.ResourcesGathered;
                    break;
                case GameResource.LuxuryResource:
                    totalLuxuryResourceProduction += job.ResourcesGathered;
                    break;
                default:
                    // Handle any cases that might have been missed
                    break;
            }
        }

        // Update the resourceStruct with the calculated production rates
        resourceStruct = resourceStruct.SetAllResourceProductionRates(totalFoodProduction,
            totalWoodProduction, totalOreProduction, totalBonusFoodProduction, totalBonusWoodProduction,
            totalBonusOreProduction, totalLuxuryResourceProduction);
    }

    public void DetermineFoodState(RaceManager raceManager)
    {
        float min = 0.1f;
        float veryLow = 0.25f;
        float half = 0.5f;
        float low = 0.75f;
        float standard = 1.0f;
        float high = 1.25f;
        float veryHigh = 1.5f;
        float twice = 2.0f;



        if (resourceStruct.Food <= 1000)
        {
            raceManager.deathModifier = veryHigh;
            raceManager.birthModifier = min;
            raceManager.foodConsumptionModifier = half;
            resourceStruct = resourceStruct.SetAllResourcePriorities(2, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
        }
        else if (resourceStruct.Food <= 5000)
        {
            raceManager.deathModifier = standard;
            raceManager.birthModifier = standard;
            raceManager.foodConsumptionModifier = standard;
            resourceStruct = resourceStruct.SetAllResourcePriorities(1, 1, 1, 1, 1, 1, 1);
        }
        else if (resourceStruct.Food <= 20000)
        {
            raceManager.deathModifier = half;
            raceManager.birthModifier = twice;
            raceManager.foodConsumptionModifier = veryHigh;
            resourceStruct = resourceStruct.SetAllResourcePriorities(0.75f, 1, 1, 1, 1, 1, 1);
        }
        else
        {
            raceManager.deathModifier = veryLow;
            raceManager.birthModifier = veryHigh;
            raceManager.foodConsumptionModifier = twice;
            resourceStruct = resourceStruct.SetAllResourcePriorities(0.5f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f);
        }
    }

    public int DetermineMaxFoodOutput(RaceManager raceManager)
    {
        int maxFoodOutput = 0;
        foreach (Job job in raceManager.jobManager.Jobs)
        {
            if (job.ResourceType == GameResource.MainFood)
            {
                maxFoodOutput += job.ResourcesGathered;
            }
        }

        return maxFoodOutput;
    }
}

//logic such that food does not drop past 0

