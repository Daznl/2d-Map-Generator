using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using static MapArrayScript;
using System.Net;

public class CreateRaces : MonoBehaviour
{
    public GameManager gameManager;
    public RaceRelatedFunctions raceRelatedFunctions;
    //public RaceManagerFunctions raceManagerFunctions;
    public FindSpawnPoints findSpawnPoints;
    public AssignSpawnPointsToRaces A;
    public GameObject raceManagerPrefab;
    public TerritoryAllocate territoryAllocate;
    public Text[] raceNameTexts;    //references the Text box that lists the RaceProperties
    public Text[] raceCounterTexts; //references the Text box that list the Character counters 
    public World world;

    private Dictionary<string, RaceProperties> raceDictionary;

    public RaceDataHolder raceDataHolder;

    public BuildingSprites spawnPointScriptable;

    public void CreateRacesStart()
    {
        world = gameManager.LoadedWorld;
        int numRaces = world.territory.Count;
        // Create the race dictionary
        raceDictionary = raceRelatedFunctions.CreateRaceDictionary(numRaces);

        // Assign the race dictionary to the RaceDataHolder
        raceDataHolder.SetRaceDictionary(raceDictionary);

        // Write the race names to the UI Text components
        raceRelatedFunctions.WriteRaceProperties(raceDictionary, raceNameTexts);

        foreach (var entry in raceDictionary)
        {
            // Call the CreateRaceManager function to create and initialize a new RaceManager
            RaceManager raceManager = raceRelatedFunctions.CreateRaceManager(entry.Value, raceManagerPrefab, raceCounterTexts[Array.IndexOf(raceDictionary.Keys.ToArray(), entry.Key)]);

            // Set the RaceManager name using entry.Value.RaceName
            raceManager.name = entry.Value.RaceName + " RaceManager";

            // Add the RaceManager reference to the RaceDataHolder
            raceDataHolder.raceManagersList.Add(raceManager);
        }
        raceDataHolder.LogRaceManagers();

        //Ordering and scoring races with locations:
        List<RaceManager> sortedRaceManagers = raceDataHolder.raceManagersList.OrderBy(rm => rm.raceProperties.RaceStart).ToList();
        territoryAllocate.AllocateTerritoriesBasedOnPreferences(sortedRaceManagers, world);

        //creates a list of spawn points for every race inside every territory
        findSpawnPoints.FindSpawnPointsForEachTerritory();

        // Assuming A is the AssignSpawnPointsToRaces instance linked in the inspector
        A.AssignSpawnPoints(gameManager, raceDataHolder.raceManagersList);

        gameManager.InitialiseBuildingArray();

        // Loop through all race managers to set their spawn points and Initialize resource and jobmanager
        foreach (RaceManager raceManager in raceDataHolder.raceManagersList)
        {
            if (raceManager.spawnPoint != null)
            {
                    // Convert the generic coordinate to Vector2Int for the building location
                    Vector2Int spawnPointLocation = new Vector2Int(raceManager.spawnPoint.x, raceManager.spawnPoint.y);

                // Retrieve the spawn point sprite from the scriptable object
                Sprite spawnPointSprite = spawnPointScriptable.spawnPointSprite;

                // Access the RaceBuildingFunctions component to add the spawn point building
                RaceBuildingFunctions buildingFunctions = raceManager.GetComponent<RaceBuildingFunctions>();

                if (buildingFunctions != null)
                {
                    // Add the spawn point building element
                    buildingFunctions.AddBuildingElement(spawnPointLocation, spawnPointSprite, raceManager.GetInstanceID(), RaceBuildingFunctions.BuildingType.SpawnPoint);
                }
                else
                {
                    Debug.LogWarning("RaceBuildingFunctions component not found on " + raceManager.name);
                }

                raceManager.Initialise();
            }
            else
            {
                Debug.LogWarning($"Spawn point not set for {raceManager.name}");
            }
        }

        // Call UpdateBuildingOverlay after adding all spawn points to optimize performance
        gameManager.UpdateBuildingOverlay();
    }
}