using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MapArrayScript;
using static RacePreference;

public class TerritoryAllocate : MonoBehaviour
{
    public CreateRaces createRaces;
    public RacePreference racePreference;
    public GameManager gameManager;
    public void AllocateTerritoriesBasedOnPreferences(List<RaceManager> sortedRaceManagers, World world)
    {
        Debug.Log($"Starting territory allocation for {sortedRaceManagers.Count} race managers.");
        // Initialize a structure to hold scores and preferences
        var territoryScoresByPreference = new Dictionary<RaceLandPreference, List<(Territory territory, int score, RaceManager raceManager)>>();

        // 1. Calculate scores for each territory for each race
        foreach (var raceManager in sortedRaceManagers)
        {
            foreach (var territory in world.territory.Where(t => !t.isAllocated))
            {
                //Debug.Log($"Evaluating territories for race: {raceManager.raceProperties.RaceName}");

                int score = CalculateTerritoryScore(territory, raceManager.raceProperties);
                var preference = raceManager.raceProperties.LandPreference;

                if (!territoryScoresByPreference.ContainsKey(preference))
                {
                    territoryScoresByPreference[preference] = new List<(Territory, int, RaceManager)>();
                }

                territoryScoresByPreference[preference].Add((territory, score, raceManager));
            }
        }

        // 2. Allocate territories based on scores, ensuring diversity of preferences
        foreach (var preference in territoryScoresByPreference.Keys)
        {
            var scores = territoryScoresByPreference[preference];

            // Sort by score, then by RaceStart if scores are equal
            var sortedScores = scores.OrderByDescending(s => s.score).ThenBy(s => s.raceManager.raceProperties.RaceStart).ToList();

            foreach (var (territory, score, raceManager) in sortedScores)
            {
                if (!territory.isAllocated)
                {
                    AssignTerritoryToRace(raceManager, territory);
                    territory.isAllocated = true;
                    Debug.Log($"Allocated {territory.name} to {raceManager.raceProperties.RaceName} with preference {preference} and score {score}");
                    break; // Move to the next preference after allocation
                }
            }
        }
        Debug.Log("Territory allocation completed.");
    }

    private int CalculateTerritoryScore(Territory territory, RaceProperties raceProperties)
    {
        //Debug.Log($"Calculating score for territory: {territory.name} for race: {raceProperties.RaceName}");
        int score = 0;

        // Iterate over each block in the territory
        foreach (var blockCoord in territory.blocks)
        {
            // Retrieve the block type from the world's array using the block's coordinates
            var blockType = gameManager.LoadedWorld.worldBlockTypesArray[blockCoord.x, blockCoord.y];

            // Use CalculateScoreForBlockType to get the score for each block based on land preference
            score += CalculateScoreForBlockType(blockType, raceProperties);

        }

        return score;
    }

    private int CalculateScoreForBlockType(MapArrayScript.Blocktype blockType, RaceProperties raceProperties)
    {
        int score = 0;

        // Assuming raceProperties includes a direct or indirect reference to the RaceLandPreference
        var landPreferences = racePreference.LandPreferences[raceProperties.LandPreference];
        if (landPreferences.ContainsKey(blockType))
        {
            score += landPreferences[blockType];
        }

        return score;
    }

    private void AssignTerritoryToRace(RaceManager raceManager, Territory bestTerritory)
    {
        // Mark the territory as allocated to prevent it from being allocated again
        bestTerritory.isAllocated = true;

        // Optionally, if you have a mechanism to track which territory is assigned to which race,
        // update the race manager with the allocated territory. 
        // This might involve adding a property to RaceManager to store the assigned Territory.
        raceManager.allocatedTerritory = bestTerritory;

        // Log the allocation for debugging purposes
        Debug.Log($"{raceManager.raceProperties.RaceName} has been allocated territory {bestTerritory.name}");

        // If your game logic requires, update the territory with information about its new occupant
        bestTerritory.occupantRaceName = raceManager.raceProperties.RaceName;

        // Any additional logic needed to integrate the territory with the race manager
        // For example, updating UI, adjusting game state, etc.
    }
}
