using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SandMakerScript : MonoBehaviour
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

    private int Up;
    private int Down;
    private int Left;
    private int Right;

    private int gameBoundary;
    public GameObject SandMaker;
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

        Invoke("GrowSand", 0.01f);
    }
    void GrowSand()
    {
        {
            if (Up < gameBoundary - 1
                & Down > 0
                & Left > 0
                & Right < gameBoundary - 1)
            {
                //Random Chance of spawning Up
                if (Random.Range(0, 100) < 
                    G.sandMakerChance
                    &
                    (
                    (S.Up(x,y) == MapArrayScript.Blocktype.Land
                    & S.LeftUp(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.Up(x,y) == MapArrayScript.Blocktype.Land
                    & S.RightUp(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )
                {
                    S.CreateBlockUp(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(x,S.Up(y)));
                }

                //Random Chance of spawning Down
                if (Random.Range(0, 100) < G.sandMakerChance
                &
                    (
                    (S.Down(x,y) == MapArrayScript.Blocktype.Land
                    & S.LeftDown(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.Down(x,y) == MapArrayScript.Blocktype.Land
                    & S.RightDown(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )
                {
                    S.CreateBlockDown(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(x, S.Down(y)));
                }
                //Random Chance of spawning Left
                if (Random.Range(0, 100) < G.sandMakerChance
                    &
                    (
                    (S.Left(x,y) == MapArrayScript.Blocktype.Land
                    & S.LeftUp(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.Left(x,y) == MapArrayScript.Blocktype.Land
                    & S.LeftDown(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )
                {
                    S.CreateBlockLeft(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(S.Left(x), y));
                }
                //Random Chance of spawning Right
                if (Random.Range(0, 100) < G.sandMakerChance
                    &
                    (
                    (S.Right(x,y) == MapArrayScript.Blocktype.Land
                    & S.RightUp(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.Right(x,y) == MapArrayScript.Blocktype.Land
                    & S.RightDown(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )

                {
                    S.CreateBlockRight(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(S.Right(x), y));
                }

                //Random Chance of spawning Up and Right
                if (Random.Range(0, 100) < G.sandMakerChance
                    &
                    (
                    (S.RightUp(x,y) == MapArrayScript.Blocktype.Land
                    & S.Right(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.RightUp(x,y) == MapArrayScript.Blocktype.Land
                    & S.Up(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )

                {
                    S.CreateBlockRightUp(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(S.Right(x), S.Up(y)));
                }

                //Random Chance of spawning Up and Left
                if (Random.Range(0, 100) < G.sandMakerChance
                    &
                    (
                    (S.LeftUp(x,y) == MapArrayScript.Blocktype.Land
                    & S.Up(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.LeftUp(x,y) == MapArrayScript.Blocktype.Land
                    & S.Left(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )
                {
                    S.CreateBlockLeftUp(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(S.Left(x), S.Up(y)));
                }

                //Random Chance of spawning Down and Left
                if (Random.Range(0, 100) < G.sandMakerChance
                    &
                    (
                    (S.LeftDown(x,y) == MapArrayScript.Blocktype.Land
                    & S.Left(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.LeftDown(x,y) == MapArrayScript.Blocktype.Land
                    & S.Down(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )
                {
                    S.CreateBlockLeftDown(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(S.Left(x),S.Down(y)));
                }

                //Random Chance of spawning Down and Right
                if (Random.Range(0, 100) < G.sandMakerChance
                    &
                    (
                    (S.RightDown(x,y) == MapArrayScript.Blocktype.Land
                    & S.Right(x,y)== MapArrayScript.Blocktype.Shallowwater)
                    |
                    (S.RightDown(x,y) == MapArrayScript.Blocktype.Land
                    & S.Down(x,y) == MapArrayScript.Blocktype.Shallowwater)
                    )
                )
                {
                    S.CreateBlockRightDown(MapArrayScript.Blocktype.Sand, SandMaker, x, y, ElevationSettings.sandElevation, S.SpawnedFrom(S.Right(x),S.Down(y)));
                }
            }
            //Destroy(this.gameObject);
        }
    }
}
