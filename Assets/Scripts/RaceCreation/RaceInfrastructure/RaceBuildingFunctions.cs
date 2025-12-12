using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class RaceBuildingFunctions : MonoBehaviour
{
    public List<RaceBuildingElement> BuildingElements { get; private set; } = new List<RaceBuildingElement>();
    public BuildingSprites buildingSprites;
    public GameManager gameManager;

    [System.Serializable]
    public enum BuildingType
    {
        SpawnPoint,
        Town,
        Road,
        // Add additional building types here
    }

    // Represents a single building element
    [System.Serializable]
    public class RaceBuildingElement
    {
        public Vector2Int Location { get; set; }
        public Sprite BuildingSprite { get; set; }
        public int RaceID { get; set; } // Identifies which race the building belongs to
        public BuildingType Type { get; set; } // Include the building type

        // Constructor
        public RaceBuildingElement(Vector2Int location, Sprite buildingSprite, int raceID, BuildingType type)
        {
            Location = location;
            BuildingSprite = buildingSprite;
            RaceID = raceID;
            Type = type;
        }
    }
    void Awake()
    {
        // Find the GameManager in the scene and set it
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    // Removes a building based on location
    public void AddBuildingElement(Vector2Int location, Sprite buildingSprite, int raceID, BuildingType type)
    {
        // Create the building element with type
        RaceBuildingElement newBuildingElement = new RaceBuildingElement(location, buildingSprite, raceID, type);
        BuildingElements.Add(newBuildingElement); // Add to the local list

        if (location.x >= 0 && location.x < gameManager.mapBuildings.GetLength(0) && location.y >= 0 && location.y < gameManager.mapBuildings.GetLength(1))
        {
            // Update the GameManager's mapBuildings array with the new building info
            gameManager.mapBuildings[location.x, location.y] = new GameManager.BuildingInfo(location, buildingSprite, raceID, type);

            // Call a method to update the map overlay, reflecting the new building
            gameManager.UpdateBuildingOverlay();
        }
        else
        {
            Debug.LogError("Tried to add building outside of map bounds.");
        }

        // Optionally, create a BuildingInfo instance if needed to update the gameManager's mapBuildings array
        //BuildingInfo newBuildingInfo = new BuildingInfo(location, buildingSprite, raceID, type); // BuildingInfo might need adjustment to include type
    }
}
