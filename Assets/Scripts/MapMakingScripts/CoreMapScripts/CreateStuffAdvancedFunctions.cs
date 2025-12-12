using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;


public class CreateStuffAdvancedFunctions : MonoBehaviour
{
   
    public MapArrayScript M;
    public CreateStuffSimpleFunctions S;
    public MapGenerationFunctions G;
    public CreateStuff C;

    public GameObject landPrefab;
    public GameObject riverPrefab;

    //Blocktype
    public LandType deepWaterType;
    public LandType waterType;
    public LandType shallowWaterType;
    public LandType lowlandType;
    public LandType sandType;
    public LandType landType;
    public LandType hillType;
    public LandType highlandType;
    public LandType mountainType;    
    public LandType peakType;
    public LandType DryLandType;
    public LandType SwampType;
    public LandType SwamplowlandType;
    public LandType Test1Type;
    public LandType Test2Type;
    public LandType Test3Type;
    public LandType Test4Type;
    public LandType Test5Type;
    public LandType Test6Type;
    public LandType Test7Type;
    public LandType Test8Type;
    public LandType Test9Type;
    public LandType Test10Type;
    public LandType TestOtherType;

    public GameObject CreateLand(LandType landType, Vector3 position, int elevation, GameObject landPrefab)
    {
        position.z = elevation;
        GameObject newLand = Instantiate(landPrefab, position, Quaternion.identity);
        LandController landController = newLand.GetComponent<LandController>();
        landController.landType = landType;
        landController.UpdateLandProperties();
        return newLand;
    }
    public void PrintTerritoryList()
    {

        foreach (MapArrayScript.Territory territory in M.world.territory)
        {
            string landConnections = string.Join(", ", territory.landConnections);

            string mountainRanges = string.Join(", ", territory.mountainRanges.Select(mr => $"Name: {mr.name}, Start: ({mr.x}, {mr.y}), Blocks: {mr.blocks.Count}"));
            string peaks = string.Join(", ", territory.peaks.Select(p => $"Name: {p.name}, Start: ({p.x}, {p.y})"));

            string lakes = string.Join(", ", territory.lakes.Select(l => $"Name: {l.name}, Start: ({l.x}, {l.y}), Blocks: {l.blocks.Count}"));

            string coast = $"Coast: Blocks: {territory.coast.blocks.Count}";

            string territoryResources = $"Food: {territory.TerritoryResources.mainFood.Count}, BonusFood: {territory.TerritoryResources.bonusFood.Count}, MainWood: {territory.TerritoryResources.mainWood.Count}, BonusWood: {territory.TerritoryResources.bonusWood.Count}, MainOre: {territory.TerritoryResources.mainOre.Count}, BonusOre: {territory.TerritoryResources.bonusOre.Count}, LuxuryResource: {territory.TerritoryResources.luxuryResource.Count}";

            Debug.Log($"Territory ID: {territory.id}\n" +
                      $"Territory Center: ({territory.center.x},{territory.center.y})  Name: {territory.name}, Start: ({territory.center.x}, {territory.center.y}), Blocks: {territory.blocks.Count}\n" +
                      $"Land Connections: {landConnections}\n" +
                      $"Mountain Ranges: {mountainRanges}\n" +
                      $"Peaks: {peaks}\n" +
                      $"Lakes: {lakes}\n" +
                      $"{coast}\n" +
                      $"Resources: {territoryResources}\n" +
                      $"-----------------------------");
        }
    }
    public string GetRandomLineFromTextFile(string filePath)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split('\n');
            int randomIndex = UnityEngine.Random.Range(0, lines.Length);
            return lines[randomIndex].Trim();
        }
        else
        {
            //Debug.LogError("Text file not found at: " + filePath);
            return "NotFound";
        }
    }

    public void DestroyAllObjects()
    {
        GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);

        for (int x = 0; x < GameObjects.Length; x++)
        {
            if (GameObjects[x].tag != "GameController")
            {
                Destroy(GameObjects[x]);
            }

        }
    }
    public void Clean()
    {

        DestroyAllObjects();
       
        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                switch (M.blockType[x, y])
                {
                    case MapArrayScript.Blocktype.Deepwater:
                        CreateLand(deepWaterType, new Vector3(x, y, 0), ElevationSettings.deepWaterElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.deepWaterElevation;
                        break;
                    case MapArrayScript.Blocktype.Water:
                        CreateLand(waterType, new Vector3(x, y, 0), ElevationSettings.waterElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.waterElevation;
                        break;
                    case MapArrayScript.Blocktype.Shallowwater:
                        CreateLand(shallowWaterType, new Vector3(x, y, 0), ElevationSettings.shallowWaterElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.shallowWaterElevation;
                        break;
                    case MapArrayScript.Blocktype.Lowland:
                        CreateLand(lowlandType, new Vector3(x, y, 0), ElevationSettings.lowlandElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.lowlandElevation;
                        break;
                    case MapArrayScript.Blocktype.Sand:
                        CreateLand(sandType, new Vector3(x, y, 0), ElevationSettings.sandElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.sandElevation;
                        break;
                    case MapArrayScript.Blocktype.Land:
                        CreateLand(landType, new Vector3(x, y, 0), ElevationSettings.landElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.landElevation;
                        break;
                    case MapArrayScript.Blocktype.Hill:
                        CreateLand(hillType, new Vector3(x, y, 0), ElevationSettings.hillElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.hillElevation;
                        break;
                    case MapArrayScript.Blocktype.Highland:
                        CreateLand(highlandType, new Vector3(x, y, 0), ElevationSettings.highlandElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.highlandElevation;
                        break;
                    case MapArrayScript.Blocktype.Mountain:
                        CreateLand(mountainType, new Vector3(x, y, 0), ElevationSettings.mountainElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.mountainElevation;
                        break;
                    case MapArrayScript.Blocktype.Peak:
                        CreateLand(peakType, new Vector3(x, y, 0), ElevationSettings.peakElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.peakElevation;
                        break;
                    case MapArrayScript.Blocktype.DryLand:
                        CreateLand(DryLandType, new Vector3(x, y, 0), ElevationSettings.drylandElevation, landPrefab) ;
                        M.altitudeArray[x, y] = ElevationSettings.drylandElevation;
                        break;
                    case MapArrayScript.Blocktype.Swamp:
                        CreateLand(SwampType, new Vector3(x, y, 0), ElevationSettings.swampElevation, landPrefab);
                        M.altitudeArray[x, y] = ElevationSettings.swampElevation;
                        break;
                    case MapArrayScript.Blocktype.Swamplowland:
                        CreateLand(SwamplowlandType, new Vector3(x, y, 0), ElevationSettings.swampLowlandElevation, landPrefab) ;
                        M.altitudeArray[x, y] = ElevationSettings.swampLowlandElevation;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void RecreateMap()
    {
        MapArrayScript.Block[,] blocks = GameManager.Instance.gameMapBlocks;

        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                GenerateLand(blocks[x, y], x, y);
            }
        }
    }

    private void GenerateLand(MapArrayScript.Block block, int x, int y)
    {
        switch (block.blockType)
        {
            case MapArrayScript.Blocktype.Deepwater:
                CreateLand(deepWaterType, new Vector3(x, y, 0), ElevationSettings.deepWaterElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.deepWaterElevation;
                break;
            case MapArrayScript.Blocktype.Water:
                CreateLand(waterType, new Vector3(x, y, 0), ElevationSettings.waterElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.waterElevation;
                break;
            case MapArrayScript.Blocktype.Shallowwater:
                CreateLand(shallowWaterType, new Vector3(x, y, 0), ElevationSettings.shallowWaterElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.shallowWaterElevation;
                break;
            case MapArrayScript.Blocktype.Lowland:
                CreateLand(lowlandType, new Vector3(x, y, 0), ElevationSettings.lowlandElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.lowlandElevation;
                break;
            case MapArrayScript.Blocktype.Sand:
                CreateLand(sandType, new Vector3(x, y, 0), ElevationSettings.sandElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.sandElevation;
                break;
            case MapArrayScript.Blocktype.Land:
                CreateLand(landType, new Vector3(x, y, 0), ElevationSettings.landElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.landElevation;
                break;
            case MapArrayScript.Blocktype.Hill:
                CreateLand(hillType, new Vector3(x, y, 0), ElevationSettings.hillElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.hillElevation;
                break;
            case MapArrayScript.Blocktype.Highland:
                CreateLand(highlandType, new Vector3(x, y, 0), ElevationSettings.highlandElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.highlandElevation;
                break;
            case MapArrayScript.Blocktype.Mountain:
                CreateLand(mountainType, new Vector3(x, y, 0), ElevationSettings.mountainElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.mountainElevation;
                break;
            case MapArrayScript.Blocktype.Peak:
                CreateLand(peakType, new Vector3(x, y, 0), ElevationSettings.peakElevation, landPrefab);
                M.altitudeArray[x, y] = ElevationSettings.peakElevation;
                break;
            default:
                break;
        }

    }
   
public void CleanTerritory()
    {
        int repeat = 20;
        while (repeat != 0)
        {
            for (int x = 0; x < GameManager.Instance.mapSize; x++)
            {
                for (int y = 0; y < GameManager.Instance.mapSize; y++)
                {
                    if (M.spawnedFrom[x, y] == -1 && M.blockType[x, y] != MapArrayScript.Blocktype.Shallowwater && M.blockType[x, y] != MapArrayScript.Blocktype.Water)
                    {
                        int roll = Random.Range(0, 5);
                        switch (roll)
                        {
                            case 0:
                                if (M.spawnedFrom[x, y - 1] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x, y - 1];
                                }
                                break;
                            case 1:
                                if (M.spawnedFrom[x, y + 1] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x, y + 1];
                                }
                                break;
                            case 2:
                                if (M.spawnedFrom[x - 1, y] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x - 1, y];
                                }

                                break;
                            case 3:
                                if (M.spawnedFrom[x + 1, y] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x + 1, y];
                                }
                                break;
                        }
                    }
                    if (M.spawnedFrom[x, y] == -1)
                    {
                        int roll = Random.Range(0, 5);
                        switch (roll)
                        {
                            case 0:
                                if (M.spawnedFrom[x, y - 1] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x, y - 1];
                                }

                                break;
                            case 1:
                                if (M.spawnedFrom[x, y + 1] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x, y + 1];
                                }
                                break;
                            case 2:
                                if (M.spawnedFrom[x - 1, y] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x - 1, y];
                                }
                                break;
                            case 3:
                                if (M.spawnedFrom[x + 1, y - 1] > -1)
                                {
                                    M.spawnedFrom[x, y] = M.spawnedFrom[x + 1, y];
                                }
                                break;
                        }
                    }
                    if (M.spawnedFrom[x, y] > -1)
                    {
                        if (M.spawnedFrom[x, y] != M.spawnedFrom[x, y - 1] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x, y + 1] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x - 1, y] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x + 1, y])
                        {
                            if (25 > Random.Range(0, 100))
                            {
                                M.spawnedFrom[x, y] = M.spawnedFrom[x, y - 1];
                            }
                            if (25 > Random.Range(0, 100))
                            {
                                M.spawnedFrom[x, y] = M.spawnedFrom[x, y + 1];
                            }
                            if (25 > Random.Range(0, 100))
                            {
                                M.spawnedFrom[x, y] = M.spawnedFrom[x - 1, y];
                            }
                            if (100 > Random.Range(0, 100))
                            {
                                M.spawnedFrom[x, y] = M.spawnedFrom[x + 1, y];
                            }
                        }
                        if (M.spawnedFrom[x, y] != M.spawnedFrom[x, y - 1] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x, y + 1] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x - 1, y] &&
                            M.spawnedFrom[x, y] == M.spawnedFrom[x + 1, y] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x + 2, y] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x + 1, y + 1] &&
                            M.spawnedFrom[x, y] != M.spawnedFrom[x + 1, y - 1])
                        {
                            M.spawnedFrom[x, y] = M.spawnedFrom[x - 1, y];
                            M.spawnedFrom[x + 1, y] = M.spawnedFrom[x - 1, y];
                        }
                    }
                    if (x > 10 && x < GameManager.Instance.mapSize - 10 && y > 10 && y < GameManager.Instance.mapSize - 10)
                    {
                        if (M.spawnedFrom[x, y] == -1 &&
                            M.spawnedFrom[x, y + 1] != -1 &&
                            M.spawnedFrom[x, y - 1] != -1 &&
                            M.spawnedFrom[x - 1, y] != -1 &&
                            M.spawnedFrom[x + 1, y] != -1)
                        {
                            M.spawnedFrom[x, y] = M.spawnedFrom[x, y - 1];
                        }
                    }
                    
                }
            }
            repeat--;
        }
    }
    public void ShowTerritory()
    {
        DestroyAllObjects();

        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                switch (M.spawnedFrom[x, y])
                {
                    case -2:
                        CreateLand(deepWaterType, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case -1:
                        CreateLand(TestOtherType, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 0:
                        CreateLand(peakType, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 1:
                        CreateLand(Test1Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 2:
                        CreateLand(Test2Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 3:
                        CreateLand(Test3Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 4:
                        CreateLand(Test4Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 5:
                        CreateLand(Test5Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 6:
                        CreateLand(Test6Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 7:
                        CreateLand(Test7Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 8:
                        CreateLand(Test8Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 9:
                        CreateLand(Test9Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 10:
                        CreateLand(Test10Type, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 11:
                        CreateLand(landType, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                    case 12:
                        CreateLand(sandType, new Vector3(x, y, 0), 0, landPrefab);
                        break;
                }
            }
        }
    }
    public void ShowRivers()
    {
        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                switch (M.riverNumberArray[x, y])
                {
                    case 0:
                        CreateLand(Test1Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 1:
                        CreateLand(Test2Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 2:
                        CreateLand(Test3Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 3:
                        CreateLand(Test4Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 4:
                        CreateLand(Test5Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 5:
                        CreateLand(Test6Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 6:
                        CreateLand(Test7Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 7:
                        CreateLand(Test8Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 8:
                        CreateLand(Test9Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                    case 9:
                        CreateLand(Test10Type, new Vector3(x, y, 0), -5, landPrefab);
                        break;
                }
            }
        }
    }
    public void RiverDraw()
    {

        foreach (RiverBlock block in M.riverBlocks)
        {
            //Debug.Log("draw");
            block.DetermineSprite();
            // Instantiate your GameObject with the sprite, position, and rotation
            GameObject newRiverBlock = Instantiate(riverPrefab, new Vector3(block.x, block.y, -6), block.rotation);
            newRiverBlock.GetComponent<SpriteRenderer>().sprite = block.sprite;
        }
    }

}
