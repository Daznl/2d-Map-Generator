using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static MapArrayScript;
using static RaceBuildingFunctions;

public class GameManager : MonoBehaviour
{
    public CreateStuff createStuff;
    public MapArrayScript mapArrayScript;
    public Json json;
    public BlockArrayCreator blockArrayCreator;
    public MapRenderer R;
    public MiniMapNavigation miniMapNavigation;
    public TMP_Text mapInfoText;
    public CreateRaces createRaces;


    /*private string saveDirectory = "/Resources/JsonSaves";
    private string saveFileName = "world1.json";*/


    //Details of map
    public Dictionary<GenericCoordinate, Block> globalMap;
    public int mapSize;
    public string mapName;
    public int numberOfTerritories;
    public float percentageOfLand;
    public int numberOfRivers;
    public int numberOfMountainRanges;

    // Singleton instance
    public static GameManager Instance { get; private set; }
    

    // The loaded world
    public MapArrayScript.World LoadedWorld { get; private set; }
    public Block[,] gameMapBlocks;

    public BuildingInfo[,] mapBuildings;

    [System.Serializable]
    public class BuildingInfo
    {
        public Vector2Int Coordinates;
        public Sprite BuildingSprite;
        public int OwnerID;
        public BuildingType Type; // Add building type

        public BuildingInfo(Vector2Int coordinates, Sprite buildingSprite, int ownerID, BuildingType type)
        {
            Coordinates = coordinates;
            BuildingSprite = buildingSprite;
            OwnerID = ownerID;
            Type = type;
        }
    }

    private void Awake()
    {
        

        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitialiseBuildingArray()
    {
        mapBuildings = new BuildingInfo[mapSize, mapSize];
    }

    public void UpdateBuildingOverlay()
    {
        // Assuming GetStaticMapSprite() correctly returns a sprite with a non-read-only texture
        Sprite staticMapSprite = R.staticMapImage.sprite; // Ensure mapRenderer is correctly referenced
        Texture2D mapTexture = staticMapSprite.texture;

        Texture2D textureCopy = new Texture2D(mapTexture.width, mapTexture.height, mapTexture.format, true); // true to enable mipmaps
        Graphics.CopyTexture(mapTexture, textureCopy);

        // Overlay buildings
        R.OverlayBuildings(textureCopy); // Pass textureCopy to be modified

        // Replace the dynamic overlay sprite
        Sprite updatedMapSprite = Sprite.Create(textureCopy, new Rect(0, 0, textureCopy.width, textureCopy.height), new Vector2(0.5f, 0.5f), 100.0f);
        R.displayImage.sprite = updatedMapSprite; // Make sure raceMapImage is set up to display the dynamic layer
        UpdateMiniMap();
    }

    void UpdateMiniMap()
    {
        miniMapNavigation.GenerateMiniMap();
    }
    public void LoadMap()
    {    
        // Setup the game with the loaded world

        Debug.Log("Setting up game/CreateBlockArray");
        gameMapBlocks = blockArrayCreator.CreateBlockArray(LoadedWorld);

        InitialiseGlobalMap();

        Debug.Log("Setting up game/RenderMap");
        R.RenderMap();

        Debug.Log("setting up game/GenerateFullMapImage");
        R.GenerateFullMapImage();

        Debug.Log("setupGame/GenerateMiniMap");
        miniMapNavigation.GenerateMiniMap();
    }

    public void LoadWorld()
    {

        LoadedWorld = json.LoadGameData("world1");

        UpdateMapInformation(LoadedWorld);
        SetLoadedWorld(LoadedWorld);
    }

    public void UpdateMapInformation(World world)
    {
        mapSize = world.worldSize;
        mapName = world.mapName;
        numberOfTerritories = world.territory.Count;

        

        DisplayMapInformation();
    }

    public void InitialiseGlobalMap()
    {
        Debug.Log("Initialise GlobalMap");
        globalMap = new Dictionary<GenericCoordinate, Block>();
        for (int x = 0; x < gameMapBlocks.GetLength(0); x++)
        {
            for (int y = 0; y < gameMapBlocks.GetLength(1); y++)
            {
                var block = gameMapBlocks[x, y];
                globalMap.Add(block.Coordinate, block);
            }
        }
    }

    public void SetLoadedWorld(MapArrayScript.World world)
    {
        LoadedWorld = world;
    }

    public void DisplayMapInformation()
    {
        string mapInfo = $"<b><size=+2>{mapName}</size></b>\n" +  // Make map name bold and slightly larger
                         $"World size: {mapSize}x{mapSize}\n" +
                         $"Number of Territories: {numberOfTerritories}";

        mapInfoText.text = mapInfo;
    }

    public void CreateRaces()
    {
        createRaces.CreateRacesStart();
    }
}

