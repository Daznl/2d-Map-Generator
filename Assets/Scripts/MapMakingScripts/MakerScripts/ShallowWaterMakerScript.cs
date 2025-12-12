using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShallowWaterMakerScript : MonoBehaviour
{
    MapArrayScript M;
    CreateStuffAdvancedFunctions A;
    CreateStuffSimpleFunctions S;
    public MapGenerationFunctions G;
    public CreateStuff C;

    private float xPositionFloat;
    private int x;
    private float yPositionFloat;
    private int y;

    private int gameBoundary;
    public GameObject ShallowWaterMaker;

    void Start()
    {
        GameObject mapMaker = GameObject.Find("MapMaker");

        A = mapMaker.GetComponent<CreateStuffAdvancedFunctions>();
        M = mapMaker.GetComponent<MapArrayScript>();
        S = mapMaker.GetComponent<CreateStuffSimpleFunctions>();
        G = mapMaker.GetComponent<MapGenerationFunctions>();
        C = mapMaker.GetComponent<CreateStuff>();

        gameBoundary = GameManager.Instance.mapSize;
            ;

        xPositionFloat = transform.position.x;          //get x position
        x = (int)xPositionFloat;             //convert x position to an integer for array use

        yPositionFloat = transform.position.y;          //get y position
        y = (int)yPositionFloat;             //convert y position to an integer for array use

        GrowShallowWater();

    }
        void GrowShallowWater()
    {
        if (y + 1< gameBoundary - 1) // space above is not out of bounds 
        {
            //Random chance of spawning up
            if (Random.Range(0, 100) < G.shallowWaterChance & S.Up(x,y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockUp(MapArrayScript.Blocktype.Shallowwater, ShallowWaterMaker, x, y, ElevationSettings.shallowWaterElevation, -2);
            }
        }

        if (y - 1 > 0)
        {
            //Random chance of spawning down
            if (Random.Range(0, 100) < G.shallowWaterChance & S.Down(x, y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockDown(MapArrayScript.Blocktype.Shallowwater, ShallowWaterMaker, x, y, ElevationSettings.shallowWaterElevation, -2);
            }
        }

        if (y + 1 > 0)
        {
            //Random chance of spawning left
            if (Random.Range(0, 100) < G.shallowWaterChance & S.Left(x, y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockLeft(MapArrayScript.Blocktype.Shallowwater, ShallowWaterMaker, x, y, ElevationSettings.shallowWaterElevation, -2);
            }
        }

        if (y - 1 < gameBoundary - 1)
        {
            //Random chance of spawning right
            if (Random.Range(0, 100) < G.shallowWaterChance & S.Right(x, y) == MapArrayScript.Blocktype.Nothing)
            {
                S.CreateBlockRight(MapArrayScript.Blocktype.Shallowwater, ShallowWaterMaker, x, y, ElevationSettings.shallowWaterElevation, -2);
            }
        }
        //Destroy(this.gameObject);
    }
}