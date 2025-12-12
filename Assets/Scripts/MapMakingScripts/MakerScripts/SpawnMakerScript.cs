using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnMakerScript : MonoBehaviour
{
    public MapArrayScript M;
    public CreateStuffSimpleFunctions S;
    public CreateStuffAdvancedFunctions A;
    public MapGenerationFunctions G;
    public CreateStuff C;

    public int peakInitialChance = 1;
    public int peakMakerInitialChance = 7;

    public GameObject RiverMaker;
    //Counters
    public int landCounter;
    public int waterCounter;
    public void SpawnLandmaker(int spawnAmount, GameObject LandMaker, int elevation)
    {
        int territoryCounter = 0;
        int margin = Mathf.Min(40, GameManager.Instance.mapSize / 10); // Adjust margin based on map size

        while (spawnAmount > 0)    //Repeat coordinate reroll until free spot if found or rerolls exceed 100 attempts
        {
            int xRandom = Random.Range(margin, GameManager.Instance.mapSize - margin);
            int yRandom = Random.Range(margin, GameManager.Instance.mapSize - margin);

            //Debug.Log(xRandom);

            if (M.blockType[xRandom, yRandom] == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlock(MapArrayScript.Blocktype.Land, LandMaker, xRandom, yRandom, elevation, territoryCounter);

                MapArrayScript.GenericCoordinate centerLocation = new MapArrayScript.GenericCoordinate(xRandom, yRandom);
                string territoryName = "Territory " + M.world.territory.Count;
                MapArrayScript.Territory newTerritory = new MapArrayScript.Territory(centerLocation, territoryCounter, territoryName);
                M.world.territory.Add(newTerritory);
                territoryCounter++;
                landCounter++;
                spawnAmount--;
            }
        }
    }
    public void SpawnPeakMaker(GameObject PeakMaker)
    {
        // Get the MapArrayScript instance
        //MapArrayScript mapArrayScript = FindObjectOfType<MapArrayScript>();
        int peakCounter = 0;
        for (int x = 1; x < GameManager.Instance.mapSize - 1; x++)
        {
            for (int y = 1; y < GameManager.Instance.mapSize - 1; y++)
            {
                int roll = Random.Range(0, 1000);
                if (roll < peakInitialChance
                    & S.Self(x, y) == MapArrayScript.Blocktype.Land && M.spawnedFrom[x, y] > -1)
                {
                    S.CreateBlock(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));

                    int territoryIndex = M.spawnedFrom[x, y];
                    MapArrayScript.Territory territory = M.world.territory[territoryIndex];

                    //Create a peak location
                    MapArrayScript.GenericLocation newPeakLocation = new MapArrayScript.GenericLocation();
                    newPeakLocation.x = x;
                    newPeakLocation.y = y;
                    newPeakLocation.name = A.GetRandomLineFromTextFile("TextFiles/MountainNames");
                    territory.peaks.Add(newPeakLocation);
                    //create a mountain range and add the peak location
                    MapArrayScript.GenericArea newMountainRange = new MapArrayScript.GenericArea();
                    newMountainRange.x = x;
                    newMountainRange.y = y;
                    newMountainRange.name = A.GetRandomLineFromTextFile("TextFiles/MountainNames");
                    newMountainRange.blocks = new List<MapArrayScript.GenericCoordinate>();
                    newMountainRange.keyLocations = new List<MapArrayScript.GenericLocation>();
                    newMountainRange.keyLocations.Add(newPeakLocation);
                    territory.mountainRanges.Add(newMountainRange);

                    // Add mountain range and its territory to the dictionary
                    M.peakToMountainRangeMap[peakCounter] = (territory, newMountainRange);

                    M.peakSpawnedFrom[x, y] = peakCounter;

                    //create larger mountain
                    //up
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance
                        & S.Up(x, y) == MapArrayScript.Blocktype.Land
                        & S.Up(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockUp(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[x, S.Up(y)] = peakCounter;
                    }
                    //down
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance
                        & S.Down(x, y) == MapArrayScript.Blocktype.Land
                        & S.Down(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockDown(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[x, S.Down(y)] = peakCounter;
                    }
                    //left
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance
                        & S.Left(x, y) == MapArrayScript.Blocktype.Land
                        & S.Left(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockLeft(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[S.Left(x), y] = peakCounter;
                    }
                    //right
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance
                        & S.Right(x, y) == MapArrayScript.Blocktype.Land
                        & S.Right(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockRight(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[S.Right(x), y] = peakCounter;
                    }
                    //upright
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance - 2
                        & S.RightUp(x, y) == MapArrayScript.Blocktype.Land
                        & S.RightUp(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockRightUp(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[S.Right(x), S.Up(y)] = peakCounter;
                    }
                    //upleft
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance - 2
                        & S.LeftUp(x, y) == MapArrayScript.Blocktype.Land
                        & S.LeftUp(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockLeftUp(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[S.Left(x), S.Up(y)] = peakCounter;
                    }
                    //downright
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance - 2
                        & S.RightDown(x, y) == MapArrayScript.Blocktype.Land
                        & S.RightDown(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockRightDown(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[S.Right(x), S.Down(y)] = peakCounter;
                    }
                    //downleft
                    roll = Random.Range(0, 10);
                    if (roll < peakMakerInitialChance - 2
                        & S.LeftDown(x, y) == MapArrayScript.Blocktype.Land
                        & S.LeftDown(x, y) != MapArrayScript.Blocktype.Peak)
                    {
                        S.CreateBlockLeftDown(MapArrayScript.Blocktype.Peak, PeakMaker, x, y, ElevationSettings.peakElevation, S.SpawnedFrom(x, y));
                        M.peakSpawnedFrom[S.Left(x), S.Down(y)] = peakCounter;
                    }
                    peakCounter++;
                }
            }
        }
    }

    public void SpawnRiversOnMountains()
    {
        int riverNumber = 0; // River counter

        // Loop through all the territories in the world.
        foreach (MapArrayScript.Territory territory in M.world.territory)
        {
            for (int i = 0; i < territory.mountainRanges.Count; i++)
            {
                // Choose a random mountain block as the river's start location
                MapArrayScript.GenericCoordinate startCoordinate = territory.mountainRanges[i].blocks[Random.Range(0, territory.mountainRanges[i].blocks.Count)];

                // Get the mountain peak
                MapArrayScript.GenericCoordinate peakCoordinate = new MapArrayScript.GenericCoordinate(territory.mountainRanges[i].x, territory.mountainRanges[i].y);

                // Generate a random river name from your text file
                string riverName = A.GetRandomLineFromTextFile("TextFiles/MountainNames");

                // Create the river maker object at the start coordinate.
                GameObject riverMaker = Instantiate(RiverMaker, new Vector3(startCoordinate.x, startCoordinate.y, -1), Quaternion.identity);

                // get riverMaker script
                RiverMakerScript riverMakerScript = riverMaker.GetComponent<RiverMakerScript>();

                //assign the current riverNumber counter to RiverMaker
                riverMakerScript.riverNumber = riverNumber;
                    
                //assign that direction to the RiverMaker object 
                riverMakerScript.riverFixedFlowDirection = DetermineFlowDirection(startCoordinate, peakCoordinate);

                //assign the rivernumber to the riverNumber Array this may not be necessary
                M.riverNumberArray[startCoordinate.x, startCoordinate.y] = riverNumber;

                //increment the rivernumber counter
                riverNumber++;
            }
        }
    }
    public CreateStuffSimpleFunctions.Direction DetermineFlowDirection(MapArrayScript.GenericCoordinate startCoordinate, MapArrayScript.GenericCoordinate peakCoordinate)
    {
        CreateStuffSimpleFunctions.Direction flowDirection;

        if (startCoordinate.x < peakCoordinate.x)
        {
            if (startCoordinate.y < peakCoordinate.y)
            {
                if (Random.value < 0.5)
                    flowDirection = CreateStuffSimpleFunctions.Direction.down;
                else
                    flowDirection = CreateStuffSimpleFunctions.Direction.left;
            }
            else if (startCoordinate.y > peakCoordinate.y)
            {
                if (Random.value < 0.5)
                    flowDirection = CreateStuffSimpleFunctions.Direction.up;
                else
                    flowDirection = CreateStuffSimpleFunctions.Direction.left;
            }
            else
            {
                flowDirection = CreateStuffSimpleFunctions.Direction.left;
            }
        }
        else if (startCoordinate.x > peakCoordinate.x)
        {
            if (startCoordinate.y < peakCoordinate.y)
            {
                if (Random.value < 0.5)
                    flowDirection = CreateStuffSimpleFunctions.Direction.down;
                else
                    flowDirection = CreateStuffSimpleFunctions.Direction.right;
            }
            else if (startCoordinate.y > peakCoordinate.y)
            {
                if (Random.value < 0.5)
                    flowDirection = CreateStuffSimpleFunctions.Direction.up;
                else
                    flowDirection = CreateStuffSimpleFunctions.Direction.right;
            }
            else
            {
                flowDirection = CreateStuffSimpleFunctions.Direction.right;
            }
        }
        else // startCoordinate.x == peakCoordinate.x
        {
            if (startCoordinate.y < peakCoordinate.y)
            {
                flowDirection = CreateStuffSimpleFunctions.Direction.down;
            }
            else if (startCoordinate.y > peakCoordinate.y)
            {
                flowDirection = CreateStuffSimpleFunctions.Direction.up;
            }
            else // startCoordinate.y == peakCoordinate.y
            {
                flowDirection = (CreateStuffSimpleFunctions.Direction)Random.Range(0, 4);
            }
        }
        return flowDirection;
    }
}

