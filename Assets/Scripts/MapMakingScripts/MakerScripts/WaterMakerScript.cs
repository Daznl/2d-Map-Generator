using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatermakerScript : MonoBehaviour
{
    MapArrayScript M;
    CreateStuffAdvancedFunctions A;
    CreateStuffSimpleFunctions S;
    public MapGenerationFunctions G;
    public SpawnMakerScript T;
    public CreateStuff C;

    private float xPositionFloat;
    private int x;
    private float yPositionFloat;
    private int y;
    private int gameBoundary;
    public GameObject WaterMaker;

    public int waterPopulation;
    public int waterMakerChance;
    public float waterPopulationPercentage;

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

        GrowWater();

    }

    private void UpdateWaterChances()
    {
        int waterPopulation = T.waterCounter;
        CalculateWaterMakerChance(waterPopulation);
    }

        public void CalculateWaterMakerChance(int waterCounter)
    {
        float waterPopulationPercentage = ((float)waterCounter / ((float)GameManager.Instance.mapSize * (float)GameManager.Instance.mapSize)) * 100;

        if (waterPopulationPercentage <= 1)
        {
            waterMakerChance = 100;
        }
        else if (waterPopulationPercentage > 1 && waterPopulationPercentage <= 25)
        {
            waterMakerChance = 40;
        }
        else if (waterPopulationPercentage > 25 && waterPopulationPercentage <= 50)
        {
            waterMakerChance = 35;
        }
        else if (waterPopulationPercentage > 50 && waterPopulationPercentage <= 60)
        {
            waterMakerChance = 30;
        }
        else if (waterPopulationPercentage > 60 && waterPopulationPercentage <= 80)
        {
            waterMakerChance = 20;
        }
        else if (waterPopulationPercentage > 80)
        {
            waterMakerChance = 0;
        }
    }

    void GrowWater()
    {
        UpdateWaterChances();

        if (y + 1 < gameBoundary - 1) // space above is not out of bounds 
        {
            //Random chance of spawning up
            if (Random.Range(0, 100) < waterMakerChance
                & S.Up(x,y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockUp(MapArrayScript.Blocktype.Water, WaterMaker, x, y, ElevationSettings.waterElevation, -2);
                T.waterCounter++;
            }
        }

        if (y - 1 > 0)
        {
            //Random chance of spawning down
            if (Random.Range(0, 100) < waterMakerChance
                & S.Down(x,y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockDown(MapArrayScript.Blocktype.Water, WaterMaker, x, y, ElevationSettings.waterElevation, -2);
                T.waterCounter++;
            }
        }

        if (x - 1 > 0)
        {
            //Random chance of spawning left
            if (Random.Range(0, 100) < waterMakerChance
                & S.Left(x,y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockLeft(MapArrayScript.Blocktype.Water, WaterMaker, x, y, ElevationSettings.waterElevation, -2);
                T.waterCounter++;

            }
        }

        if (x + 1 < gameBoundary - 1)
        {
            //Random chance of spawning right
            if (Random.Range(0, 100) < waterMakerChance
                & S.Right(x,y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockRight(MapArrayScript.Blocktype.Water, WaterMaker, x, y, ElevationSettings.waterElevation, -2);
                T.waterCounter++;
            }
        }
        //Destroy(this.gameObject);
    }
}