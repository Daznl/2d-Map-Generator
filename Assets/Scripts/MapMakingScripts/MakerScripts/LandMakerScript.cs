using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMakerScript : MonoBehaviour
{
    MapArrayScript M;
    CreateStuffAdvancedFunctions A;
    CreateStuffSimpleFunctions S;
    CreateStuff C;
    public MapGenerationFunctions G;
    public SpawnMakerScript T;

    public enum Direction
    {
        NoDirection, right, left, up, down,
    };

    public int landPopulation;
    public int landMakerChance;
    public float landPopulationPercentage;

    private float xPositionFloat;
    private int x;
    private float yPositionFloat;
    private int y;

    private int gameBoundary;

    private int landChanceLeft;
    private int landChanceRight;
    private int landChanceUp;
    private int landChanceDown;

    public GameObject LandMaker;
    void Start()
    {
        GameObject mapMaker = GameObject.Find("MapMaker");

        A = mapMaker.GetComponent<CreateStuffAdvancedFunctions>();
        M = mapMaker.GetComponent<MapArrayScript>();
        S = mapMaker.GetComponent<CreateStuffSimpleFunctions>();
        G = mapMaker.GetComponent<MapGenerationFunctions>();
        T = mapMaker.GetComponent<SpawnMakerScript>();
        C = mapMaker.GetComponent<CreateStuff>();

        gameBoundary = GameManager.Instance.mapSize;

        xPositionFloat = transform.position.x;          //get x position
        x = (int)xPositionFloat;             //convert x position to an integer for array use

        yPositionFloat = transform.position.y;          //get y position
        y = (int)yPositionFloat;             //convert y position to an integer for array use

        GrowLand();
    }
    private int GetDistanceFromBorder(Direction direction)
    {
        int distance = 0;
        switch (direction)
        {
            case Direction.right:
                distance = gameBoundary - 1 - x;
                break;
            case Direction.left:
                distance = x;
                break;
            case Direction.up:
                distance = gameBoundary - 1 - y;
                break;
            case Direction.down:
                distance = y;
                break;
        }
        //Debug.Log("Distance from border for direction " + direction.ToString() + ": " + distance);
        return distance;
    }
    public void CalculateLandMakerChance(int landCounter)
    {
        float landPopulationPercentage = ((float)landCounter / ((float)GameManager.Instance.mapSize * (float)GameManager.Instance.mapSize)) * 100;

        if (landPopulationPercentage <= 1)
        {
            landMakerChance = 100;
        }
        else if (landPopulationPercentage > 1 && landPopulationPercentage <= 25)
        {
            landMakerChance = 54;
        }
        else if (landPopulationPercentage > 25 && landPopulationPercentage <= 50)
        {
            landMakerChance = 47;
        }
        else if (landPopulationPercentage > 50 && landPopulationPercentage <= 60)
        {
            landMakerChance = 42;
        }
        else if (landPopulationPercentage > 60 && landPopulationPercentage <= 80)
        {
            landMakerChance = 38;
        }
        else if (landPopulationPercentage > 80 && landPopulationPercentage <= 90)
        {
            landMakerChance = 10;
        }
        else
        {
            landMakerChance = 0;
        }
    }

    private void UpdateLandChances()
    {
        int landPopulation = T.landCounter;
        CalculateLandMakerChance(landPopulation);

        for (int i = 0; i < 4; i++)
        {
            Direction direction = (Direction)(i + 1);
            int distanceFromBorder = GetDistanceFromBorder(direction);

            // Calculate the base land spawning probability for the direction
            int chance = landMakerChance;

            // Adjust the land spawning probability based on the distance from the border
            if (distanceFromBorder <= 20)
            {
                int[] borderChances = new int[20];
                for (int j = 0; j < borderChances.Length; j++)
                {
                    borderChances[i] = chance - (2 * j);
                }
                chance = borderChances[distanceFromBorder - 1];
            }

            SetLandChanceForDirection(direction, chance);
        }
    }

    private void SetLandChanceForDirection(Direction direction, int chance)
    {
        switch (direction)
        {
            case Direction.right:
                landChanceRight = chance;
                break;
            case Direction.left:
                landChanceLeft = chance;
                break;
            case Direction.up:
                landChanceUp = chance;
                break;
            case Direction.down:
                landChanceDown = chance;
                break;
        }
    }
    void GrowLand()
    {
        UpdateLandChances();

        if (y + 1 < gameBoundary - 1)
        {
            //Random chance of spawning up
            if (Random.Range(0, 100) < landChanceUp & S.Up(x,y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockUp(MapArrayScript.Blocktype.Land, LandMaker, x, y, ElevationSettings.landElevation, S.SpawnedFrom(x,y));
                T.landCounter++;
            }
        }

        if (y - 1 > 0)
        {
            //Random chance of spawning down
            if (Random.Range(0, 100) < landChanceDown & S.Down(x, y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockDown(MapArrayScript.Blocktype.Land, LandMaker, x, y, ElevationSettings.landElevation, S.SpawnedFrom(x, y));
                T.landCounter++;
            }
        }
        if (x - 1 > 0)
        {
            //Random chance of spawning left
            if (Random.Range(0, 100) < landChanceLeft & S.Left(x, y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockLeft(MapArrayScript.Blocktype.Land, LandMaker, x, y, ElevationSettings.landElevation, S.SpawnedFrom(x, y));
                T.landCounter++;
            }
        }
        if (x + 1 < gameBoundary - 1)
        {
            //Random chance of spawning right
            if (Random.Range(0, 100) < landChanceRight & S.Right(x, y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockRight(MapArrayScript.Blocktype.Land, LandMaker, x, y, ElevationSettings.landElevation, S.SpawnedFrom(x, y));
                T.landCounter++;
            }
        }
        //Destroy(this.gameObject);
    }
}