using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static MapArrayScript;
using System.Text;

public class CreateStuff : MonoBehaviour
{

    public CreateStuffAdvancedFunctions A;
    public MapArrayScript M;
    public MapGenerationFunctions G;
    public TerritoryFindandAssign F;
    public SpawnMakerScript S;
    public ResourceGenerationScripts R;
    public BlockArrayCreator B;
    public CreateMapImage C;
    public StopwatchUtility W;
    public Json J;
    public BiomeCreaterScript BC;

    public Camera mainGameCamera;
    public Camera mapGenerationCamera;

    //Maker Objects
    public GameObject LowlandMaker;
    public GameObject SandMaker;
    public GameObject HillMaker;
    public GameObject PeakMaker;
    public GameObject HighlandMaker;
    public GameObject DeepWaterMaker;
    public GameObject ShallowWaterMaker;
    public GameObject WaterMaker;
    public GameObject LandMaker;
    public GameObject SwampLowlandMaker;

    //Stable objects
    public GameObject DeepWater;
    public GameObject Highland;
    public GameObject Hill;
    public GameObject Land;
    public GameObject Lowland;
    public GameObject Mountain;
    public GameObject Sand;
    public GameObject ShallowWater;
    public GameObject Water;
    public GameObject Dryland;
    public GameObject Wetland;
    public GameObject Swamp;
    public GameObject Swamplowland;

    public IEnumerator GenerateMap()
    {
        /****** Sequence 1  LandGeneration ********/

        Debug.Log("CreateStuff/GenerateMap");
        yield return new WaitForSeconds(0);
        W.StartTimer();
        S.SpawnLandmaker(GameManager.Instance.numberOfTerritories, LandMaker, 1);
        W.StopAndLogTimer("1. SpawnLandMaker ");

        yield return new WaitForSeconds(2.5f);

        W.StartTimer();
        G.Fill(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Land, Land, ElevationSettings.landElevation, 1);
        W.StopAndLogTimer("2. Fill Land Blocks");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Grow(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Land, Land, ElevationSettings.landElevation, 4);
        W.StopAndLogTimer("3. Grow land blocks ");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Land, Land, ElevationSettings.landElevation, 1);
        W.StopAndLogTimer("4. Fill land blocks");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.SpawnNextTo(ShallowWaterMaker, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Shallowwater, ElevationSettings.shallowWaterElevation, G.shallowWaterInitialChance);
        W.StopAndLogTimer("5. Spawn shallow water ");

        yield return new WaitForSeconds(1);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Shallowwater, MapArrayScript.Blocktype.Shallowwater, MapArrayScript.Blocktype.Shallowwater, ShallowWater, ElevationSettings.shallowWaterElevation, 1);
        W.StopAndLogTimer("6. Fill shallow water");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Grow(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Shallowwater, MapArrayScript.Blocktype.Shallowwater, MapArrayScript.Blocktype.Shallowwater, ShallowWater, ElevationSettings.shallowWaterElevation, 4);
        W.StopAndLogTimer("7. Grow shallow water ");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.SpawnNextTo(WaterMaker, MapArrayScript.Blocktype.Shallowwater, MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Water, ElevationSettings.waterElevation, G.waterInitialChance);
        W.StopAndLogTimer("8. Spawn water ");

        yield return new WaitForSeconds(2.5f);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Water, MapArrayScript.Blocktype.Water, MapArrayScript.Blocktype.Water, Water, ElevationSettings.waterElevation, 1);
        W.StopAndLogTimer("9. Fill water");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Grow(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Water, MapArrayScript.Blocktype.Water, MapArrayScript.Blocktype.Water, Water, ElevationSettings.waterElevation, 4);
        W.StopAndLogTimer("10. Grow water");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Nothing, MapArrayScript.Blocktype.Water, MapArrayScript.Blocktype.Water, MapArrayScript.Blocktype.Water, Water, ElevationSettings.waterElevation, 1);
        W.StopAndLogTimer("11. Fill water");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        G.EmptySpaceFill(DeepWater, ElevationSettings.deepWaterElevation);
        W.StopAndLogTimer("12. Fill empty spaces ");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        S.SpawnPeakMaker(PeakMaker);
        W.StopAndLogTimer("13. Spawn peaks");

        yield return new WaitForSeconds(1f);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Mountain, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Mountain, Mountain, ElevationSettings.mountainElevation, 4);
        W.StopAndLogTimer("14. Spawn mountains");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Grow(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Mountain, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Mountain, Mountain, ElevationSettings.mountainElevation, 4);
        W.StopAndLogTimer("15. Grow mountains ");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.SpawnNextTo(HighlandMaker, MapArrayScript.Blocktype.Mountain, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Highland, ElevationSettings.highlandElevation, G.highlandMakerInitialChance);
        W.StopAndLogTimer("16. Spawn highlands");
        yield return new WaitForSeconds(1f);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Highland, Highland, ElevationSettings.highlandElevation, 4);
        W.StopAndLogTimer("17. Fill highlands");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Grow(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Highland, Highland, ElevationSettings.highlandElevation, 4);
        W.StopAndLogTimer("18. Grow highlands");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Mountain, MapArrayScript.Blocktype.Highland, Highland, ElevationSettings.highlandElevation, 4);
        W.StopAndLogTimer("19. Fill highlands");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.SpawnNextTo(HillMaker, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Hill, ElevationSettings.hillElevation, G.hillMakerInitialChance);
        G.SpawnInOpenspace(HillMaker, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Hill, ElevationSettings.hillElevation, G.hillMakerInitialLandChance);
        W.StopAndLogTimer("20. Spawn hills");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Hill, MapArrayScript.Blocktype.Hill, Hill, ElevationSettings.hillElevation, 4);
        W.StopAndLogTimer("21. Fill hills ");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Grow(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Hill, MapArrayScript.Blocktype.Hill, Hill, ElevationSettings.hillElevation, 4);
        W.StopAndLogTimer("22. Grow hills");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Highland, MapArrayScript.Blocktype.Mountain, MapArrayScript.Blocktype.Hill, Hill, ElevationSettings.hillElevation, 4);
        W.StopAndLogTimer("23. Fill hills ");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.SpawnNextTo(SandMaker, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Shallowwater, MapArrayScript.Blocktype.Sand, ElevationSettings.sandElevation, G.sandMakerInitialChance);
        W.StopAndLogTimer("24. Spawn sand");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        //A.DestroyAllObjects();
        G.SpawnInOpenspace(LowlandMaker, MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Lowland, ElevationSettings.lowlandElevation, G.lowlandMakerInitialChance);
        W.StopAndLogTimer("25. Spawn lowlands");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Lowland, Lowland, ElevationSettings.lowlandElevation, 4);
        W.StopAndLogTimer("26. Fill lowlands");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        G.Grow(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Lowland, Lowland, ElevationSettings.lowlandElevation, 4);
        W.StopAndLogTimer("27. Grow lowlands ");

        yield return new WaitForSeconds(0);

        W.StartTimer();
        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Lowland, Lowland, ElevationSettings.lowlandElevation, 4);
        W.StopAndLogTimer("28. Fill lowlands");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        BC.TransformToDesert();
        W.StopAndLogTimer("29. Create desert (removed currently)");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        G.Grow(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.DryLand, MapArrayScript.Blocktype.Sand, Sand, ElevationSettings.sandElevation, 4);
        G.Grow(MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.DryLand, MapArrayScript.Blocktype.Sand, Sand, ElevationSettings.sandElevation, 2);
        G.Grow(MapArrayScript.Blocktype.Hill, MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.DryLand, MapArrayScript.Blocktype.Sand, Sand, ElevationSettings.sandElevation, 2);

        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.DryLand, MapArrayScript.Blocktype.Sand, Sand, ElevationSettings.sandElevation, 4);
        G.Fill(MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.DryLand, MapArrayScript.Blocktype.Sand, Sand, ElevationSettings.sandElevation, 4);
        G.Fill(MapArrayScript.Blocktype.Hill, MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.DryLand, MapArrayScript.Blocktype.DryLand, Dryland, ElevationSettings.drylandElevation, 4);

        W.StopAndLogTimer("28. ");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        BC.TransformToSwamp();
        W.StopAndLogTimer("29. ");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        G.SpawnInOpenspace(SwampLowlandMaker, MapArrayScript.Blocktype.Swamp, MapArrayScript.Blocktype.Swamplowland, ElevationSettings.swampLowlandElevation, G.lowlandMakerInitialChance);
        W.StopAndLogTimer("29. Spawn swamps (remove currently)");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        G.Grow(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Swamp, MapArrayScript.Blocktype.Swamplowland, MapArrayScript.Blocktype.Swamp, Swamp, ElevationSettings.swampElevation, 4);
        G.Grow(MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Swamp, MapArrayScript.Blocktype.Swamplowland, MapArrayScript.Blocktype.Swamplowland, Swamplowland, ElevationSettings.swampLowlandElevation, 2);
        G.Grow(MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.Swamp, MapArrayScript.Blocktype.Swamplowland, MapArrayScript.Blocktype.Swamplowland, Swamplowland, ElevationSettings.swampLowlandElevation, 2);

        G.Fill(MapArrayScript.Blocktype.Land, MapArrayScript.Blocktype.Swamp, MapArrayScript.Blocktype.Swamplowland, MapArrayScript.Blocktype.Swamp, Swamp, ElevationSettings.swampElevation, 4);
        G.Fill(MapArrayScript.Blocktype.Lowland, MapArrayScript.Blocktype.Swamp, MapArrayScript.Blocktype.Swamplowland, MapArrayScript.Blocktype.Swamplowland, Swamplowland, ElevationSettings.swampLowlandElevation, 4);
        G.Fill(MapArrayScript.Blocktype.Sand, MapArrayScript.Blocktype.Swamp, MapArrayScript.Blocktype.Swamplowland, MapArrayScript.Blocktype.Swamplowland, Swamplowland, ElevationSettings.swampLowlandElevation, 4);

        W.StopAndLogTimer("28. Grow swamps");

        yield return new WaitForSeconds(0.5f);
        W.StartTimer();
        A.Clean();
        W.StopAndLogTimer("30. Clean");

        yield return new WaitForSeconds(5.0f);

        W.StartTimer();
        M.PopulateWorldBlockTypes();
        W.StopAndLogTimer("29. Create blockytype array ");

        yield return new WaitForSeconds(2f);

        /******* Sequence 2  Territories ******/

        W.StartTimer();
        F.AssignLakesToTerritorys();
        W.StopAndLogTimer("30. Assign lakes to territories");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        F.AssignRemainingBlocksToOcean();
        W.StopAndLogTimer("31. Assign ocean blocks");


        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        F.AssignBlocksToTerritory();
        W.StopAndLogTimer("32. Assign blocks to territories");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        F.AssignCoastalBlocksToTerritories();
        W.StopAndLogTimer("33. Assign coastal blocks to territories ");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        F.CheckAndFixDisconnectedTerritorys();
        W.StopAndLogTimer("34. Fix disconnected territories");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        A.CleanTerritory();
        W.StopAndLogTimer("35. Clean territories");

        yield return new WaitForSeconds(2f);

        W.StartTimer();
        A.ShowTerritory();
        W.StopAndLogTimer("36. Show territories");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        A.Clean();
        W.StopAndLogTimer("39. Clean");

        
        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        F.UpdateTerritoryLandConnections();
        W.StopAndLogTimer("37. Update territory land connections");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        F.AssignMountainsToTerritory();
        W.StopAndLogTimer("38. Assign mountains to territories");


        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        A.Clean();
        W.StopAndLogTimer("39. Clean");

        yield return new WaitForSeconds(0.5f);

        /******* Sequence 3  Rivers *********/

        W.StartTimer();
        S.SpawnRiversOnMountains();
        W.StopAndLogTimer("40. Spawn rivers");

        yield return new WaitForSeconds(15f);

        W.StartTimer();
        A.RiverDraw();
        W.StopAndLogTimer("41. Draw rivers");

        yield return new WaitForSeconds(2f);

        W.StartTimer();
        M.ConvertRiverBlocksToArray();
        W.StopAndLogTimer("42. Create river block array");

        yield return new WaitForSeconds(0.1f);

        W.StartTimer();
        F.RiverAssignToTerritories();
        W.StopAndLogTimer("43. Assign rivers to territories ");

        yield return new WaitForSeconds(0.1f);

        W.StartTimer();
        F.PopulateAllBorderAreas();
        W.StopAndLogTimer("43. Determine territory borders");

        yield return new WaitForSeconds(0.1f);

        /******* Sequence 4 Spawn Resources *******/

        W.StartTimer();
        R.SpawnResourceMakers();
        W.StopAndLogTimer("44. Spawn resources");

        yield return new WaitForSeconds(0.1f);

        W.StartTimer();
        F.ResourceFindandAssign();
        W.StopAndLogTimer("45. Assign resources to territories");

        yield return new WaitForSeconds(0.1f);

        W.StartTimer();
        A.PrintTerritoryList();
        W.StopAndLogTimer("46. Print territories");

        yield return new WaitForSeconds(0.5f);

       

        W.StartTimer();
        //C.CreateMapCameraImage();
        W.StopAndLogTimer("47. (removed)");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        A.Clean();
        W.StopAndLogTimer("48. Clean");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        J.SaveGameData(M.world, "world1");
        W.StopAndLogTimer("49. Save Game Data to Json");

        yield return new WaitForSeconds(0.5f);

        W.StartTimer();
        MapArrayScript.World loadedWorld = J.LoadGameData("world1");
        W.StopAndLogTimer("50. Load the world");

        yield return new WaitForSeconds(0.5f);


        A.PrintTerritoryList();
        PrintWorldDetails(M.world);

        W.StartTimer();
        A.DestroyAllObjects();
        //A.RecreateMap();
        mapGenerationCamera.gameObject.SetActive(false);
        mainGameCamera.gameObject.SetActive(true);
        W.StopAndLogTimer("53. Print territory and world details ");

        Debug.Log("All functions completed");

    }

    public void PrintWorldDetails(MapArrayScript.World loadedWorld)
    {
        StringBuilder details = new StringBuilder();
        int nonNothingBlockCount = 0;
        foreach (var block in loadedWorld.worldBlockTypesArray)
        {
            if (block != MapArrayScript.Blocktype.Nothing)
                nonNothingBlockCount++;
        }

        int oceanBlockCount = loadedWorld.ocean.blocks.Count;

        details.AppendLine($"World Size: {loadedWorld.worldSize}");
        details.AppendLine($"Number of Territories: {loadedWorld.territory.Count}");
        details.AppendLine($"Number of Ocean Blocks: {oceanBlockCount}");

        foreach (var river in loadedWorld.rivers)
        {
            details.AppendLine($"River '{river.name}' has {river.blocks.Count} blocks");
        }

        int totalFood = 0, totalBonusFood = 0, totalMainWood = 0, totalBonusWood = 0, totalMainOre = 0, totalBonusOre = 0, totalLuxuryResource = 0;

        foreach (var territory in loadedWorld.territory)
        {
            details.AppendLine($"Territory '{territory.name}' has {territory.blocks.Count} blocks");

            // Sum up resources for each territory
            totalFood += SumResourceAmounts(territory.TerritoryResources.mainFood);
            totalBonusFood += SumResourceAmounts(territory.TerritoryResources.bonusFood);
            totalMainWood += SumResourceAmounts(territory.TerritoryResources.mainWood);
            totalBonusWood += SumResourceAmounts(territory.TerritoryResources.bonusWood);
            totalMainOre += SumResourceAmounts(territory.TerritoryResources.mainOre);
            totalBonusOre += SumResourceAmounts(territory.TerritoryResources.bonusOre);
            totalLuxuryResource += SumResourceAmounts(territory.TerritoryResources.luxuryResource);
        }

        details.AppendLine($"Food: {totalFood}, Bonus Food: {totalBonusFood}, Wood: {totalMainWood}, Bonus Wood: {totalBonusWood}, Ore: {totalMainOre}, Bonus Ore: {totalBonusOre}, Luxury Resources: {totalLuxuryResource}");

        // Finally, log or display the entire details string
        Debug.Log(details.ToString());
    }

    private int SumResourceAmounts(List<MapArrayScript.CoordinateWithAmount> resourceList)
    {
        int sum = 0;
        foreach (var resource in resourceList)
        {
            sum += resource.amount;
        }
        return sum;
    }

    public void LogMissingBlockTypes(Territory territory, Blocktype missingType)
    {
        foreach (var block in territory.blocks)
        {
            int x = block.x;
            int y = block.y;

            if (
                M.blockType[x, y] == missingType)
            {
                Debug.Log("Missing Block Type at Coordinates: " + x + ", " + y);
            }
        }
        Debug.Log("Logged");
    }
    public void CheckForDuplicateBlocksInDesertTerritory(Territory desertTerritory)
    {
        HashSet<string> seenBlocks = new HashSet<string>();

        foreach (var block in desertTerritory.blocks)
        {
            string blockCoordinates = block.x + "," + block.y;

            if (seenBlocks.Contains(blockCoordinates))
            {
                Debug.Log("Duplicate Block Found at Coordinates: " + blockCoordinates);
            }
            else
            {
                seenBlocks.Add(blockCoordinates);
            }
        }
    }


}





