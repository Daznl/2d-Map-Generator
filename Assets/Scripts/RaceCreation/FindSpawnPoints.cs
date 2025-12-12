using static MapArrayScript;
using static RacePreference;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindSpawnPoints: MonoBehaviour
{
    private MapArrayScript M; // Reference to your map script
    public GameManager gameManager; // Reference to your GameManager
    public FindSpawnPointsByPreference F;

    public void Initialize(GameManager inGameManager)
    {
        gameManager = inGameManager;
        F = new FindSpawnPointsByPreference();
        F.initialize(inGameManager);
    }

    // Main function that iterates through territories and finds spawn points
    public void FindSpawnPointsForEachTerritory()
    {
        foreach (var territory in gameManager.LoadedWorld.territory)
        {
            var spawnPointsByPreference = new Dictionary<RaceLandPreference, List<CoordinateWithAmount>>();

            // Assuming you have an enum for preferences
            var preferences = new List<RaceLandPreference> {
                RaceLandPreference.Lowland,
                RaceLandPreference.Land,
                RaceLandPreference.Highland,
                RaceLandPreference.Mountain,
                RaceLandPreference.Desert,
                RaceLandPreference.Swamp, 

                // Add other preferences here
            };

            foreach (var preference in preferences)
            {
                HashSet<GenericCoordinate> blocksToChooseFrom = ConvertTerritoryToHashSet(territory);
                var spawnPoints = FindSpawnPointsForPreference(preference, blocksToChooseFrom);
                spawnPointsByPreference[preference] = spawnPoints;
            }

            // Assuming the Territory class has a field to store these preferences
            territory.spawnPointsByPreference = spawnPointsByPreference;
        }
    }

    // General placeholder for preference-based spawn point finding
    public List<CoordinateWithAmount> FindSpawnPointsForPreference(RaceLandPreference preference, HashSet<GenericCoordinate> territory)
    {
        //PrintTerritoryList();
        

        switch (preference)
        {
            case RaceLandPreference.Lowland:
                return F.FindSpawnPointsForLowland(territory);

            case RaceLandPreference.Land:
                return F.FindSpawnPointsForLand(territory);

            case RaceLandPreference.Highland:
                return F.FindSpawnPointsForHighland(territory);

            case RaceLandPreference.Mountain:
                return F.FindSpawnPointsForMountain(territory);

            case RaceLandPreference.Desert:
                return F.FindSpawnPointsForDesert(territory);

            case RaceLandPreference.Swamp:
                return F.FindSpawnPointsForSwamp(territory);



            // Optionally, add cases for other preferences if you have more

            default:
                return new List<CoordinateWithAmount>();
        }
    }

    private HashSet<GenericCoordinate> ConvertTerritoryToHashSet(Territory territory)
    {
        var coordinates = new HashSet<GenericCoordinate>();

        // Assuming 'blocks' contains the main coordinates you're interested in
        foreach (var block in territory.blocks)
        {
            coordinates.Add(block);
        }

        // Add more areas as needed

        return coordinates;
    }


}
