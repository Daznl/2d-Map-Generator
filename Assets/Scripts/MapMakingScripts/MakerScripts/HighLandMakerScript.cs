using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLandMakerScript : MonoBehaviour
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

    private CreateStuff createStuffScript;
    public GameObject HighlandMaker;
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

        GrowHighLand();
    }
    void GrowHighLand()
    {
        if (y + 1 < gameBoundary - 1)                                                                 
        {
            //Random chance of spawning up
            if (Random.Range(0, 100) <  G.highlandMakerChance 
                & 
                (S.Up(x, y) == MapArrayScript.Blocktype.Land 
                | S.Up(x, y) == MapArrayScript.Blocktype.Lowland)
                )                                               
            {
                S.CreateBlockUp(MapArrayScript.Blocktype.Highland, HighlandMaker, x, y, ElevationSettings.highlandElevation, S.SpawnedFrom(x, y));
                M.peakSpawnedFrom[x, y + 1] = M.peakSpawnedFrom[x, y];
            }
        }

        if (y - 1 > 0)
        {
            //Random chance of spawning down
            if (Random.Range(0, 100) < G.highlandMakerChance 
                & 
                (S.Down(x, y) == MapArrayScript.Blocktype.Land
                | S.Down(x, y) == MapArrayScript.Blocktype.Lowland)
                )
            {
                S.CreateBlockDown(MapArrayScript.Blocktype.Highland, HighlandMaker, x, y, ElevationSettings.highlandElevation, S.SpawnedFrom(x, y));
                M.peakSpawnedFrom[x, y - 1] = M.peakSpawnedFrom[x, y];
            }
        }

        if (x - 1 > 0)
        {
            //Random chance of spawning left
            if (Random.Range(0, 100) < G.highlandMakerChance 
                & 
                (S.Left(x, y) == MapArrayScript.Blocktype.Land
                | S.Left(x, y) == MapArrayScript.Blocktype.Lowland)
                )
            {
                S.CreateBlockLeft(MapArrayScript.Blocktype.Highland, HighlandMaker, x, y, ElevationSettings.highlandElevation, S.SpawnedFrom(x, y));
                M.peakSpawnedFrom[x - 1, y] = M.peakSpawnedFrom[x, y];
            }
        }

        if (x + 1 < gameBoundary - 1)
        {
            //Random chance of spawning right
            if (Random.Range(0, 100) < G.highlandMakerChance 
                & 
                (S.Right(x,y) == MapArrayScript.Blocktype.Land
                | S.Right(x, y) == MapArrayScript.Blocktype.Lowland)
                )
            {
                S.CreateBlockRight(MapArrayScript.Blocktype.Highland, HighlandMaker, x, y, ElevationSettings.highlandElevation, S.SpawnedFrom(x, y));
                M.peakSpawnedFrom[x + 1, y] = M.peakSpawnedFrom[x, y];
            }
        }
        //Destroy(this.gameObject);
    }
}
