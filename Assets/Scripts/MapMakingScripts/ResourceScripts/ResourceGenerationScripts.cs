using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerationScripts : MonoBehaviour
{
    public MapArrayScript M;
    public CreateStuffSimpleFunctions S;
    public CreateStuffAdvancedFunctions A;
    public MapGenerationFunctions G;
    public CreateStuff C;

    public GameObject mainWoodMaker;
    public GameObject bonusWoodMaker;
    public GameObject mainOreMaker;
    public GameObject bonusOreMaker;
    public GameObject luxuryResourceMaker;
    public GameObject bonusFoodMaker;


    public void SpawnResourceMakers()
    {
        // Forest spawning chance (1 out of 1000)
        int forestSpawnChance = 1;
        int bonusWoodSpawnChance = 1;
        int mainOreSpawnChance = 1;
        int bonusOreSpawnChance = 1;
        int bonusFoodSpawnChance = 1;
        int luxuryResourceSpawnChance = 1;

        // Iterate through each block in the blockType array.
        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                // Check if the block is a Land block.
                if (M.blockType[x, y] == MapArrayScript.Blocktype.Land |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Hill |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Highland |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Mountain |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Swamp)
                {

                    // Check if the roll is less than the forestSpawnChance.
                    if (forestSpawnChance >= Random.Range(0, 100))
                    {
                        // Instantiate a ForestMaker object at the current block's position.
                        Instantiate(mainWoodMaker, new Vector3(x, y, -6), Quaternion.identity);

                        //create generic coordinate for forrest block
                        MapArrayScript.CoordinateWithAmount forestCoordinate = new MapArrayScript.CoordinateWithAmount(x, y, 100);

                        //add generic coordinate to world forrest block list
                        M.world.worldResources.mainWood.Add(forestCoordinate);
                    }

                    if (bonusWoodSpawnChance >= Random.Range(0, 600))
                    {
                        // Instantiate a ForestMaker object at the current block's position.
                        Instantiate(bonusWoodMaker, new Vector3(x, y, -6), Quaternion.identity);

                        //create generic coordinate for forrest block
                        MapArrayScript.CoordinateWithAmount bonusWoodCoordinate = new MapArrayScript.CoordinateWithAmount(x, y, 100);

                        //add generic coordinate to world forrest block list
                        M.world.worldResources.bonusWood.Add(bonusWoodCoordinate);
                    }

                    if (mainOreSpawnChance >= Random.Range(0, 250))
                    {
                        // Instantiate a ForestMaker object at the current block's position.
                        Instantiate(mainOreMaker, new Vector3(x, y, -6), Quaternion.identity);

                        //create generic coordinate for forrest block
                        MapArrayScript.CoordinateWithAmount mainOreCoordinate = new MapArrayScript.CoordinateWithAmount(x, y, 100);

                        //add generic coordinate to world forrest block list
                        M.world.worldResources.mainOre.Add(mainOreCoordinate);
                    }

                    if (bonusOreSpawnChance >= Random.Range(0, 600))
                    {
                        // Instantiate a ForestMaker object at the current block's position.
                        Instantiate(bonusOreMaker, new Vector3(x, y, -6), Quaternion.identity);

                        //create generic coordinate for forrest block
                        MapArrayScript.CoordinateWithAmount bonusOreCoordinate = new MapArrayScript.CoordinateWithAmount(x, y, 100);

                        //add generic coordinate to world forrest block list
                        M.world.worldResources.bonusOre.Add(bonusOreCoordinate);
                    }                

                    if (luxuryResourceSpawnChance >= Random.Range(0, 1000))
                    {
                        // Instantiate a ForestMaker object at the current block's position.
                        Instantiate(luxuryResourceMaker, new Vector3(x, y, -6), Quaternion.identity);

                        //create generic coordinate for forrest block
                        MapArrayScript.CoordinateWithAmount luxuryResourceCoordinate = new MapArrayScript.CoordinateWithAmount(x, y, 100);

                        //add generic coordinate to world forrest block list
                        M.world.worldResources.luxuryResource.Add(luxuryResourceCoordinate);
                    }

                }

                if (M.blockType[x, y] == MapArrayScript.Blocktype.Water |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Shallowwater |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Lowland |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Land |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Hill |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Highland |
                    M.blockType[x, y] == MapArrayScript.Blocktype.Mountain|
                    M.blockType[x, y] == MapArrayScript.Blocktype.Peak)
                {
                    if (bonusFoodSpawnChance >= Random.Range(0, 500))
                    {
                        // Instantiate a ForestMaker object at the current block's position.
                        Instantiate(bonusFoodMaker, new Vector3(x, y, -6), Quaternion.identity);

                        //create generic coordinate for forrest block
                        MapArrayScript.CoordinateWithAmount bonusFoodCoordinate = new MapArrayScript.CoordinateWithAmount(x, y, 100);

                        //add generic coordinate to world forrest block list
                        M.world.worldResources.bonusFood.Add(bonusFoodCoordinate);
                    }
                }
            }
        }
    }
}
