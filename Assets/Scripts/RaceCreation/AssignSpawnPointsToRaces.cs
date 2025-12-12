using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using static MapArrayScript;

public class AssignSpawnPointsToRaces : MonoBehaviour
{
    System.Random random = new System.Random(); // For random selection.

    public void AssignSpawnPoints(GameManager gameManager, List<RaceManager> raceManagers)
    {
        System.Random random = new System.Random(); // For random selection.

        foreach (var raceManager in raceManagers)
        {
            var raceName = raceManager.raceProperties.RaceName;
            var territory = gameManager.LoadedWorld.territory.FirstOrDefault(t => t.occupantRaceName == raceName);

            if (territory != null)
            {
                if (territory.spawnPointsByPreference.ContainsKey(raceManager.raceProperties.LandPreference))
                {
                    List<CoordinateWithAmount> spawnPoints = territory.spawnPointsByPreference[raceManager.raceProperties.LandPreference];

                    if (spawnPoints.Count > 0)
                    {
                        var topChoices = spawnPoints.Take(Math.Min(10, spawnPoints.Count)).ToList();
                        int selectedIndex = random.Next(topChoices.Count);
                        var selectedSpawnPoint = topChoices[selectedIndex];

                        // Assign the selected spawn point to the RaceManager
                        raceManager.spawnPoint = new GenericCoordinate(selectedSpawnPoint.x, selectedSpawnPoint.y);

                        Debug.Log($"Assigned spawn point for {raceName} in territory '{territory.name}' at location ({selectedSpawnPoint.x}, {selectedSpawnPoint.y}).");
                    }
                }
                else
                {
                    Debug.LogError($"Land preference '{raceManager.raceProperties.LandPreference}' not found for race '{raceName}'.");
                }
            }
        }
    }
}
