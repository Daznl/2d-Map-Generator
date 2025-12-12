using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwampLowlandMakerScript : MonoBehaviour
{
    public MapArrayScript M;
    public CreateStuffAdvancedFunctions A;
    CreateStuffSimpleFunctions S;
    public MapGenerationFunctions G;
    public CreateStuff C;

    private float xPositionFloat;
    private int x;
    private float yPositionFloat;
    private int y;

    private int gameBoundary;

    private int Up;
    private int Down;
    private int Left;
    private int Right;
    private int Up2;
    private int Down2;
    private int Left2;
    private int Right2;

    private CreateStuff createStuffScript;
    public GameObject SwampLowlandMaker;


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

        Up = y + 1;
        Down = y - 1;
        Left = x - 1;
        Right = x + 1;
        Up2 = y + 2;
        Down2 = y - 2;
        Left2 = x - 2;
        Right2 = x + 2;


        Invoke("GrowLowland", 0.01f);

    }
    void GrowLowland()
    {
        if (y + 1 < gameBoundary - 1) // space above is not out of bounds 
        {
            //Random chance of spawning up
            if (Random.Range(0, 100) < G.lowlandMakerChance
                & S.Up(x, y) == MapArrayScript.Blocktype.Swamp
                & M.blockType[Right, Up] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Left, Up] == MapArrayScript.Blocktype.Swamp
                & S.Up2(x, y) == MapArrayScript.Blocktype.Swamp
                & M.blockType[Left, Up2] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Left2, Up2] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Right, Up2] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Right2, Up2] == MapArrayScript.Blocktype.Swamp
                /*& M.blockType[Right, yPosition] == MapArrayScript.Blocktype.Land
                & M.blockType[Left, yPosition] == MapArrayScript.Blocktype.Land*/)
            {
                S.CreateBlockUp(MapArrayScript.Blocktype.Swamplowland, SwampLowlandMaker, x, y, ElevationSettings.lowlandElevation, S.SpawnedFrom(x, y)) ;
            }
        }

        if (y - 1 > 0)
        {
            //Random chance of spawning down
            if (Random.Range(0, 100) < G.lowlandMakerChance
                & S.Down(x, y) == MapArrayScript.Blocktype.Swamp
                & M.blockType[Right, Down] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Left, Down] == MapArrayScript.Blocktype.Swamp
                & S.Down2(x, y) == MapArrayScript.Blocktype.Swamp
                & M.blockType[Right, Down2] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Right2, Down2] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Left, Down2] == MapArrayScript.Blocktype.Swamp
                & M.blockType[Left2, Down2] == MapArrayScript.Blocktype.Swamp
                /*& M.blockType[Left, yPosition] == MapArrayScript.Blocktype.Land
                & M.blockType[Right, yPosition] == MapArrayScript.Blocktype.Land*/)
            {
                S.CreateBlockDown(MapArrayScript.Blocktype.Swamplowland, SwampLowlandMaker, x, y, ElevationSettings.lowlandElevation, S.SpawnedFrom(x, y));
            }
        }

        if (x - 1 > 0)
        {
            //Random chance of spawning left
            if (Random.Range(0, 100) < G.lowlandMakerChance
                & S.Left(x, y) == MapArrayScript.Blocktype.Swamp
                //& M.blockType[Left, Up] == MapArrayScript.Blocktype.Land
                //& M.blockType[Left, Down] == MapArrayScript.Blocktype.Land
                & S.Left2(x, y) == MapArrayScript.Blocktype.Swamp
                & M.blockType[Left2, Down] == MapArrayScript.Blocktype.Swamp
                //& M.blockType[Left2, Down2] == MapArrayScript.Blocktype.Land
                & M.blockType[Left2, Up] == MapArrayScript.Blocktype.Swamp
                //& M.blockType[Left2, Up2] == MapArrayScript.Blocktype.Land
                /*& M.blockType[xPosition, Up] == MapArrayScript.Blocktype.Land
                & M.blockType[xPosition, Down] == MapArrayScript.Blocktype.Land*/)
            {
                S.CreateBlockLeft(MapArrayScript.Blocktype.Swamplowland, SwampLowlandMaker, x, y, ElevationSettings.lowlandElevation, S.SpawnedFrom(x, y));
            }
        }

        if (x + 1 < gameBoundary - 1)
        {
            //Random chance of spawning right
            if (Random.Range(0, 100) < G.lowlandMakerChance
                & S.Right(x, y) == MapArrayScript.Blocktype.Swamp
                //& M.blockType[Right, Up] == MapArrayScript.Blocktype.Land
                //& M.blockType[Right, Down] == MapArrayScript.Blocktype.Land
                & S.Right2(x, y) == MapArrayScript.Blocktype.Swamp
                & M.blockType[Right2, Down] == MapArrayScript.Blocktype.Swamp
                //& M.blockType[Right2, Down2] == MapArrayScript.Blocktype.Land
                & M.blockType[Right2, Up] == MapArrayScript.Blocktype.Swamp
                //& M.blockType[Right2, Up2] == MapArrayScript.Blocktype.Land
                /*& M.blockType[xPosition, Up] == MapArrayScript.Blocktype.Land
                & M.blockType[xPosition, Down] == MapArrayScript.Blocktype.Land*/)
            {
                S.CreateBlockRight(MapArrayScript.Blocktype.Swamplowland, SwampLowlandMaker, x, y, ElevationSettings.lowlandElevation, S.SpawnedFrom(x, y));
            }
        }
        //Destroy(this.gameObject);
    }
}
