using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArrayScript: MonoBehaviour
{
    public CreateStuffSimpleFunctions S;
    public CreateStuff C;
    //public FunctionsScript functionsScript;
    public enum Blocktype
    {
        Nothing, Water, Shallowwater, Deepwater, Land, Mountain, Highland,
        Hill, Peak, Sand, Lowland, DryLand, Swamp, Swamplowland,
    }

    public class Block
    {
        public Sprite blockSprite { get; set; }
        public GenericCoordinate Coordinate { get; set; }
        public Blocktype blockType { get; set; }
        public bool isOcean { get; set; }
        public int ocean { get; set; }
        public bool isRiver { get; set; }

        public RiverBlockData river;
        public bool isLake { get; set; }
        public int Lake { get; set; }
        public bool isTerritory { get; set; }
        public int territory { get; set; }
        public bool isMountainRange { get; set; }
        public int mountainRange { get; set; }
        public bool isCoast { get; set; }
        public bool hasFood { get; set; }
        public bool hasBonusFood { get; set; }
        public bool hasWood { get; set; }
        public bool hasBonusWood { get; set; }
        public bool hasMainOre { get; set; }
        public bool hasBonusOre { get; set; }
        public bool hasLuxuryResource { get; set; }
        public int[] resourcesAmount;

        public Block()
        {
            resourcesAmount = new int[7]; // Assuming 7 different resources

            // Default values for boolean properties
            isOcean = false;
            isRiver = false;
            isLake = false;
            isTerritory = false;
            isMountainRange = false;
            isCoast = false;
            hasFood = false;
            hasBonusFood = false;
            hasWood = false;
            hasBonusWood = false;
            hasMainOre = false;
            hasBonusOre = false;
            hasLuxuryResource = false;

            // Default values for integer properties
            ocean = -1; // Assuming -1 indicates no ocean
            territory = -1;
            Lake = -1;
            mountainRange = -1;

            UpdateSprite();
        }

        public void UpdateSprite()
        {
            // Logic to choose the appropriate sprite
            Sprite newSprite = GetSpriteBasedOnProperties();
            blockSprite = newSprite;
        }

        private Sprite GetSpriteBasedOnProperties()
        {
            Sprite blockSprite = null;
            return blockSprite;
        }
    }
    public class ResourceList
    {
        public List<CoordinateWithAmount> mainFood;
        public List<CoordinateWithAmount> bonusFood;
        public List<CoordinateWithAmount> mainWood;
        public List<CoordinateWithAmount> bonusWood;
        public List<CoordinateWithAmount> mainOre;
        public List<CoordinateWithAmount> bonusOre;
        public List<CoordinateWithAmount> luxuryResource;

        public ResourceList()
        {
            mainFood = new List<CoordinateWithAmount>();
            bonusFood = new List<CoordinateWithAmount>();
            mainWood = new List<CoordinateWithAmount>();
            bonusWood = new List<CoordinateWithAmount>();
            mainOre = new List<CoordinateWithAmount>();
            bonusOre = new List<CoordinateWithAmount>();
            luxuryResource = new List<CoordinateWithAmount>();
        }
    }

    public class Resources
    {
        public int food;
        public int bonusFood;
        public int mainWood;
        public int bonusWood;
        public int mainOre;
        public int bonusOre;
        public int luxuryResource;
    }
    public class GenericCoordinate
    {
        public int x;
        public int y;
        public GenericCoordinate(int xVal = 0, int yVal = 0)
        {
            x = xVal;
            y = yVal;
        }
    }

    public class CoordinateWithAmount
    {
        public int x;
        public int y;
        public int amount;
        public CoordinateWithAmount(int xVal = 0, int yVal = 0, int amountvalue = 0)
        {
            x = xVal;
            y = yVal;
            amount = amountvalue;
        }
    }

    public class GenericLocation
    {
        public int x;
        public int y;
        public string name;

        public GenericLocation(int xVal = 0, int yVal = 0, string nameVal = "")
        {
            x = xVal;
            y = yVal;
            name = nameVal;
        }
    }

    public class GenericArea
    {
        public int x;
        public int y;
        public string name;
        public int number;
        public List<GenericCoordinate> blocks;
        public List<GenericLocation> keyLocations;

        public GenericArea(int xVal = 0, int yVal = 0, string nameVal = "")
        {
            x = xVal;
            y = yVal;
            name = nameVal;
            blocks = new List<GenericCoordinate>();
            keyLocations = new List<GenericLocation>();
        }
    }

    public class River
    {
        public List<RiverBlock> riverBlocks;

        public River()
        {
            riverBlocks = new List<RiverBlock>();
        }
    }

    [System.Serializable]
    public struct RiverBlockData
    {
        public int x;
        public int y;
        public bool isRiver;
        public int riverNumber;
        public int internalRiverNumber;
        public CreateStuffSimpleFunctions.Direction previousRiverDirection;
        public CreateStuffSimpleFunctions.Direction nextRiverDirection;
        public CreateStuffSimpleFunctions.Direction firstMergeDirection;
        public CreateStuffSimpleFunctions.Direction secondMergeDirection;
    }

    public class Territory
    {
        public string name;
        public int id;
        public GenericCoordinate center;
        public List<GenericCoordinate> blocks;
        public List<GenericLocation> peaks;
        public List<GenericArea> mountainRanges;
        public List<GenericArea> lakes;
        public List<int> landConnections;
        public List<GenericArea> borders;
        public GenericArea coast;
        public List<RiverBlockData> rivers;
        public ResourceList TerritoryResources;
        public bool isAllocated;
        public string occupantRaceName { get; set; }
        //public Dictionary<RaceLandPreference, List<CoordinateWithAmount>> spawnPointsByPreference;

        public Territory(GenericCoordinate centerVal, int intid, string continentName)
        {
            name = continentName;
            landConnections = new List<int>();
            id = intid; 
            center = centerVal;
            blocks = new List<GenericCoordinate>();
            peaks = new List<GenericLocation>();
            mountainRanges = new List<GenericArea>();
            lakes = new List<GenericArea>();
            coast = new GenericArea();
            rivers = new List<RiverBlockData>();
            TerritoryResources = new ResourceList();
            isAllocated = false;
            borders = new List<GenericArea>();

            //spawnPointsByPreference = new Dictionary<RaceLandPreference, List<CoordinateWithAmount>>();
        }
    }

    public class World
    {
        public string mapName;
        public int worldSize;
        public Blocktype[,] worldBlockTypesArray;
        public List<Territory> territory;
        public GenericArea ocean;
        public List<GenericArea> rivers;
        public RiverBlockData[,] riverBlockDataArray;
        public List<GenericArea> forests;
        public List<GenericCoordinate> forestBlocks;
        public ResourceList worldResources;

        public World(int size)
        {
            //mapName = FunctionsScript.GetRandomLineFromTextFile("TextFile/RaceNames");
            worldSize = size;
            worldBlockTypesArray = new Blocktype[size, size];
            territory = new List<Territory>();
            ocean = new GenericArea();
            rivers = new List<GenericArea>();
            riverBlockDataArray = new RiverBlockData[size, size];
            worldResources = new ResourceList();
        }

        public List<Territory> GetContinents()
        {
            return territory;
        }

        public void SetBlockTypes(Blocktype[,] blockTypes)
        {
            for (int i = 0; i < blockTypes.GetLength(0); i++)
            {
                for (int j = 0; j < blockTypes.GetLength(1); j++)
                {
                    worldBlockTypesArray[i, j] = blockTypes[i, j];
                }
            }
        }
    }

    public void ConvertRiverBlocksToArray()
    {
        foreach (RiverBlock riverBlock in riverBlocks)
        {
            world.riverBlockDataArray[riverBlock.x, riverBlock.y] = new RiverBlockData
            {

                isRiver = true,
                riverNumber = riverBlock.riverNumber,
                internalRiverNumber = riverBlock.internalRiverNumber,
                previousRiverDirection = riverBlock.previousRiverDirection,
                nextRiverDirection = riverBlock.nextRiverDirection,
                firstMergeDirection = riverBlock.firstMergeDirection,
                secondMergeDirection = riverBlock.secondMergeDirection,
                x = riverBlock.x, 
                y = riverBlock.y 
            };
        }
    }
    public void PopulateWorldBlockTypes()
    {
        // Assuming blockType array is fully populated at this point
        world.SetBlockTypes(blockType);
    }

    // Define the array size as constants or variables, so you can change them easily
    public Blocktype[,] blockType;
    public int[,] spawnedFrom;
    public int[,] peakSpawnedFrom;
    public int[,] riverNumberArray;
    public int[,] altitudeArray;
    public bool[,,] resourceArray;
    public int[,] territoryArray;
    public List<RiverBlock> riverBlocks;

    //0 mainWood
    //1 bonusWood
    //2 mainOre
    //3 bonusOre
    //4 bonusFood
    //5 luxuryResource
    
    public World world;
    //continentblock list containing every block in the continent

    public Dictionary<int, (Territory territory, GenericArea mountainRange)> peakToMountainRangeMap = new Dictionary<int, (Territory territory, GenericArea mountainRange)>();
    public void CreateArrays()
    {


        blockType = new Blocktype[GameManager.Instance.mapSize, GameManager.Instance.mapSize];
        spawnedFrom = new int[GameManager.Instance.mapSize,GameManager.Instance.mapSize];
        peakSpawnedFrom = new int[GameManager.Instance.mapSize, GameManager.Instance.mapSize];
        riverNumberArray = new int[GameManager.Instance.mapSize, GameManager.Instance.mapSize];
        altitudeArray = new int[GameManager.Instance.mapSize, GameManager.Instance.mapSize];
        resourceArray = new bool[GameManager.Instance.mapSize, GameManager.Instance.mapSize,6];
        territoryArray = new int[GameManager.Instance.mapSize, GameManager.Instance.mapSize];
        riverBlocks = new List<RiverBlock>();

        world = new World(GameManager.Instance.mapSize);
        world.mapName = TextFileFunctions.GetRandomLineFromTextFile("TextFile/RaceNames");

        spawnedFrom = new int[GameManager.Instance.mapSize, GameManager.Instance.mapSize];
        peakSpawnedFrom = new int[GameManager.Instance.mapSize, GameManager.Instance.mapSize];

        for (int i = 0; i < GameManager.Instance.mapSize; i++)
        {
            for (int j = 0; j < GameManager.Instance.mapSize; j++)
            {
                spawnedFrom[i, j] = -2;
                peakSpawnedFrom[i, j] = -1;
                riverNumberArray[i, j] = -1;
                altitudeArray[i, j] = 0;
                territoryArray[i, j] = -1;

                resourceArray[i, j, 0] = false;
                resourceArray[i, j, 1] = false;
                resourceArray[i, j, 2] = false;
                resourceArray[i, j, 2] = false;
                resourceArray[i, j, 3] = false;
                resourceArray[i, j, 4] = false;
                resourceArray[i, j, 5] = false;
            }
        }
    }
}