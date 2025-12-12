using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxuryResourceMakerScript: MonoBehaviour
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
    public int elevation;

    private int gameBoundary;

    public GameObject luxuryResourceMaker;

    public float forrestChance;

    void Start()
    {
        GameObject mapMaker = GameObject.Find("MapMaker");

        A = mapMaker.GetComponent<CreateStuffAdvancedFunctions>();
        M = mapMaker.GetComponent<MapArrayScript>();
        S = mapMaker.GetComponent<CreateStuffSimpleFunctions>();
        G = mapMaker.GetComponent<MapGenerationFunctions>();
        C = mapMaker.GetComponent<CreateStuff>();

        gameBoundary = GameManager.Instance.mapSize;

        elevation = -5;
        xPositionFloat = transform.position.x;          //get x position
        x = (int)xPositionFloat;             //convert x position to an integer for array use

        yPositionFloat = transform.position.y;          //get y position
        y = (int)yPositionFloat;             //convert y position to an integer for array use

        GrowForest();

    }

    public int ForrestChance(int forrestX, int forrestY)
    {
        int chance;
        if (M.resourceArray[forrestX, forrestY, 5] == true)
        {
            chance = -1;
        }
        else
        {
            if (M.riverNumberArray[forrestX, forrestY] != -1)
            {
                chance = 75;
            }
            else
            {
                switch (M.blockType[forrestX, forrestY])
                {
                    case MapArrayScript.Blocktype.Land:
                        chance = 40;
                        break;
                    case MapArrayScript.Blocktype.Sand:
                        chance = 10;
                        break;
                    case MapArrayScript.Blocktype.Lowland:
                        chance = 20;
                        break;
                    case MapArrayScript.Blocktype.Hill:
                        chance = 40;
                        break;
                    case MapArrayScript.Blocktype.Highland:
                        chance = 30;
                        break;
                    case MapArrayScript.Blocktype.Mountain:
                        chance = 40;
                        break;
                    case MapArrayScript.Blocktype.Peak:
                        chance = 40;
                        break;
                    default:
                        chance = -1;
                        break;

                }
            }
        }
        return chance;
    }

    void GrowForest()
    {
        if (y + 1 < gameBoundary - 1)
        {
            //Random chance of spawning up
            if (Random.Range(0, 100) < ForrestChance(x, S.Up(y)))
            {
                Instantiate(luxuryResourceMaker, new Vector3(x, S.Up(y), elevation), Quaternion.identity);
                M.resourceArray[x, S.Up(y), 5] = true;

                //create generic coordinate for forrest block
                MapArrayScript.CoordinateWithAmount forestCoordinate = new MapArrayScript.CoordinateWithAmount(x, S.Up(y),Random.Range(0,50));

                //add generic coordinate to world forrest block list
                M.world.worldResources.luxuryResource.Add(forestCoordinate);
            }
        }

        if (y - 1 > 0)
        {
            //Random chance of spawning down
            if (Random.Range(0, 100) < ForrestChance(x, S.Down(y)))
            {
                Instantiate(luxuryResourceMaker, new Vector3(x, S.Down(y), elevation), Quaternion.identity);
                M.resourceArray[x, S.Down(y), 5] = true;

                MapArrayScript.CoordinateWithAmount forestCoordinate = new MapArrayScript.CoordinateWithAmount(x, S.Down(y), Random.Range(0, 50));

                M.world.worldResources.luxuryResource.Add(forestCoordinate);
            }
        }

        if (x - 1 > 0)
        {
            //Random chance of spawning left
            if (Random.Range(0, 100) < ForrestChance(S.Left(x), y))
            {
                Instantiate(luxuryResourceMaker, new Vector3(S.Left(x), y, elevation), Quaternion.identity);
                M.resourceArray[S.Left(x), y, 5] = true;

                MapArrayScript.CoordinateWithAmount forestCoordinate = new MapArrayScript.CoordinateWithAmount(S.Left(x), y, Random.Range(0, 50));

                M.world.worldResources.luxuryResource.Add(forestCoordinate);
            }
        }

        if (x + 1 < gameBoundary - 1)
        {
            //Random chance of spawning right
            if (Random.Range(0, 100) < ForrestChance(S.Right(x), y))
            {
                Instantiate(luxuryResourceMaker, new Vector3(S.Right(x), y, elevation), Quaternion.identity);
                M.resourceArray[S.Right(x), y, 5] = true;

                MapArrayScript.CoordinateWithAmount forestCoordinate = new MapArrayScript.CoordinateWithAmount(S.Right(x), y, Random.Range(0, 50));

                M.world.worldResources.luxuryResource.Add(forestCoordinate);
            }
        }
    }
}
