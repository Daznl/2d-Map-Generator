using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HillMakerScript : MonoBehaviour
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

    public GameObject HillMaker;

    void Start()
    {
        GameObject mapMaker = GameObject.Find("MapMaker");

        A = mapMaker.GetComponent<CreateStuffAdvancedFunctions>();
        M = mapMaker.GetComponent<MapArrayScript>();
        S = mapMaker.GetComponent<CreateStuffSimpleFunctions>();
        G = mapMaker.GetComponent<MapGenerationFunctions>();
        C = mapMaker.GetComponent<CreateStuff>();

        gameBoundary = GameManager.Instance.mapSize;

        xPositionFloat = transform.position.x;          //get x position
        x = (int)xPositionFloat;             //convert x position to an integer for array use

        yPositionFloat = transform.position.y;          //get y position
        y = (int)yPositionFloat;             //convert y position to an integer for array use

        GrowHill();
    }
    void GrowHill()
    {
        if (y + 1 < gameBoundary - 1)                                                         
        {
            //Random chance of spawning up
            if (Random.Range(0, 100) < G.hillMakerChance
                & 
                (S.Up(x,y) == MapArrayScript.Blocktype.Land 
                | S.Up(x, y) == MapArrayScript.Blocktype.Lowland)
                )
            {
                S.CreateBlockUp(MapArrayScript.Blocktype.Hill, HillMaker, x, y, ElevationSettings.hillElevation, S.SpawnedFrom(x, y));
            }
        }

        if (y - 1 > 0)
        {
            //Random chance of spawning down
            if (Random.Range(0, 100) < G.hillMakerChance
                &
                (S.Down(x, y) == MapArrayScript.Blocktype.Land 
                | S.Down(x, y) == MapArrayScript.Blocktype.Lowland)
                )
            {
                S.CreateBlockDown(MapArrayScript.Blocktype.Hill, HillMaker, x, y, ElevationSettings.hillElevation, S.SpawnedFrom(x, y));
            }
        }

        if (x - 1 > 0)
        {
            //Random chance of spawning left
            if (Random.Range(0, 100) < G.hillMakerChance
                & 
                (S.Left(x, y) == MapArrayScript.Blocktype.Land 
                | S.Left(x, y) == MapArrayScript.Blocktype.Lowland)
                )
            {
                S.CreateBlockLeft(MapArrayScript.Blocktype.Hill, HillMaker, x, y, ElevationSettings.hillElevation, S.SpawnedFrom(x, y));
            }
        }

        if (x + 1 < gameBoundary - 1)
        {
            //Random chance of spawning right
            if (Random.Range(0, 100) < G.hillMakerChance
                & 
                (S.Right(x, y) == MapArrayScript.Blocktype.Land 
                | S.Right(x, y) == MapArrayScript.Blocktype.Lowland)
                )
            {
                S.CreateBlockRight(MapArrayScript.Blocktype.Hill, HillMaker, x, y, ElevationSettings.hillElevation, S.SpawnedFrom(x, y));
            }
        }
        //Destroy(this.gameObject);
    }
}