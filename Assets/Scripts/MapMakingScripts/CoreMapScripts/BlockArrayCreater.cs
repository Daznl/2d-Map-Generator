using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using static MapArrayScript;

public class BlockArrayCreator : MonoBehaviour
{
    private Block[,] loadedMapBlocks; // The array of blocks

    public Block[,] CreateBlockArray(World world)
    {
        Debug.Log("Creating Block Array");
        int mapSize = world.worldSize;
        loadedMapBlocks = new Block[mapSize, mapSize];

        // Populate blocks based on world data
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                bool isBlockOcean = IsBlockOcean(world, i, j);
                bool isBlockRiver = IsBlockRiver(world, i, j);
                RiverBlockData riverData = world.riverBlockDataArray[i, j];

                loadedMapBlocks[i, j] = new Block
                {
                    Coordinate = new GenericCoordinate(i, j),
                    blockType = world.worldBlockTypesArray[i, j],
                    isOcean = isBlockOcean,
                    ocean = isBlockOcean ? 1 : -1,
                    isRiver = isBlockRiver,
                    river = isBlockRiver ? riverData : default(RiverBlockData) // Assign RiverBlockData if isRiver is true
                    // ... other properties ...
                };

            }
        }

        // Assign territories and resources to blocks
        AssignTerritoriesToBlocks(world);
        AssignResourcesToBlocks(world);

        return loadedMapBlocks;
    }
    private bool IsBlockOcean(World world,int x, int y)
    {
        // Assuming ocean area is a list of coordinates or has a way to check if a coordinate is within it
        foreach (var oceanBlock in world.ocean.blocks)
        {
            if (oceanBlock.x == x && oceanBlock.y == y)
                return true;
        }
        return false;
    }
    private bool IsBlockRiver(World world, int x, int y)
    {
        return world.riverBlockDataArray[x, y].isRiver;
    }

    private RiverBlockData GetRiverBlock(World world, int x, int y)
    {
        return world.riverBlockDataArray[x, y];
    }

    public void AssignTerritoriesToBlocks(World world)
    {
        foreach (Territory territory in world.territory)
        {
            // Assign territory properties to blocks within this territory
            foreach (GenericCoordinate coord in territory.blocks)
            {
                loadedMapBlocks[coord.x, coord.y].isTerritory = true;
                loadedMapBlocks[coord.x, coord.y].territory = territory.id;

                // Check and assign properties for lakes
                for (int lakeIndex = 0; lakeIndex < territory.lakes.Count; lakeIndex++)
                {
                    if (territory.lakes[lakeIndex].blocks.Any(block => block.x == coord.x && block.y == coord.y))
                    {
                        loadedMapBlocks[coord.x, coord.y].isLake = true;
                        loadedMapBlocks[coord.x, coord.y].Lake = lakeIndex; // Set the lake ID
                    }
                }

                // Check and assign properties for mountain ranges
                for (int mountainRangeIndex = 0; mountainRangeIndex < territory.mountainRanges.Count; mountainRangeIndex++)
                {
                    if (territory.mountainRanges[mountainRangeIndex].blocks.Any(block => block.x == coord.x && block.y == coord.y))
                    {
                        loadedMapBlocks[coord.x, coord.y].isMountainRange = true;
                        loadedMapBlocks[coord.x, coord.y].mountainRange = mountainRangeIndex; // Set the mountain range ID
                    }
                }

                // Check and assign properties for the coast
                if (territory.coast.blocks.Any(block => block.x == coord.x && block.y == coord.y))
                {
                    loadedMapBlocks[coord.x, coord.y].isCoast = true;
                }
            }
        }
    }

    public void AssignResourcesToBlocks(World world)
    {
        // Assign food resources
        foreach (var foodResource in world.worldResources.mainFood)
        {
            loadedMapBlocks[foodResource.x, foodResource.y].resourcesAmount[0] += foodResource.amount;
            loadedMapBlocks[foodResource.x, foodResource.y].hasFood = true;
        }
        // Assign bonus food resources
        foreach (var bonusFoodResource in world.worldResources.bonusFood)
        {
            loadedMapBlocks[bonusFoodResource.x, bonusFoodResource.y].resourcesAmount[1] += bonusFoodResource.amount;
            loadedMapBlocks[bonusFoodResource.x, bonusFoodResource.y].hasBonusFood = true;
        }
        // Assign main wood resources
        foreach (var mainWoodResource in world.worldResources.mainWood)
        {
            loadedMapBlocks[mainWoodResource.x, mainWoodResource.y].resourcesAmount[2] += mainWoodResource.amount;
            loadedMapBlocks[mainWoodResource.x, mainWoodResource.y].hasWood = true;
        }
        // Assign bonus wood resources
        foreach (var bonusWoodResource in world.worldResources.bonusWood)
        {
            loadedMapBlocks[bonusWoodResource.x, bonusWoodResource.y].resourcesAmount[3] += bonusWoodResource.amount;
            loadedMapBlocks[bonusWoodResource.x, bonusWoodResource.y].hasBonusWood = true;
        }
        // Assign main ore resources
        foreach (var mainOreResource in world.worldResources.mainOre)
        {
            loadedMapBlocks[mainOreResource.x, mainOreResource.y].resourcesAmount[4] += mainOreResource.amount;
            loadedMapBlocks[mainOreResource.x, mainOreResource.y].hasMainOre = true;
        }
        // Assign bonus ore resources
        foreach (var bonusOreResource in world.worldResources.bonusOre)
        {
            loadedMapBlocks[bonusOreResource.x, bonusOreResource.y].resourcesAmount[5] += bonusOreResource.amount;
            loadedMapBlocks[bonusOreResource.x, bonusOreResource.y].hasBonusOre = true;
        }
        // Assign luxury resources
        foreach (var luxuryResource in world.worldResources.luxuryResource)
        {
            loadedMapBlocks[luxuryResource.x, luxuryResource.y].resourcesAmount[6] += luxuryResource.amount;
            loadedMapBlocks[luxuryResource.x, luxuryResource.y].hasLuxuryResource = true;
        }
    }
    public void PrintRandomBlockDetails(World world)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < 20; i++)
        {
            // Generating random coordinates within the array boundaries
            int x = random.Next(world.worldSize);
            int y = random.Next(world.worldSize);

            // Accessing the block at the random coordinates
            Block block = loadedMapBlocks[x, y];

            // Creating a string to log the details of the block
            string blockDetails = $"Block at ({x}, {y}): " +
                                  $"Type: {block.blockType}, " +
                                  $"Is Ocean: {block.isOcean}, " +
                                  $"Is River: {block.isRiver}, " +
                                  $"Is Territory: {block.isTerritory}, " +
                                  $"Territory ID: {block.territory}, " +
                                  $"Is Lake: {block.isLake}, " +
                                  $"Lake ID: {block.Lake}, " +
                                  $"Is Mountain Range: {block.isMountainRange}, " +
                                  $"Mountain Range ID: {block.mountainRange}, " +
                                  $"Is Coast: {block.isCoast}";

            // Append resource details if the block has resources
            if (block.resourcesAmount.Any(amount => amount > 0))
            {
                blockDetails += ", Resources: ";
                if (block.hasFood) blockDetails += $"Food: {block.resourcesAmount[0]}, ";
                if (block.hasBonusFood) blockDetails += $"Bonus Food: {block.resourcesAmount[1]}, ";
                if (block.hasWood) blockDetails += $"Main Wood: {block.resourcesAmount[2]}, ";
                if (block.hasBonusWood) blockDetails += $"Bonus Wood: {block.resourcesAmount[3]}, ";
                if (block.hasMainOre) blockDetails += $"Main Ore: {block.resourcesAmount[4]}, ";
                if (block.hasBonusOre) blockDetails += $"Bonus Ore: {block.resourcesAmount[5]}, ";
                if (block.hasLuxuryResource) blockDetails += $"Luxury Resource: {block.resourcesAmount[6]}";
                blockDetails = blockDetails.TrimEnd(',', ' '); // Removing trailing comma and space
            }

            // Logging the details to the console
            Debug.Log(blockDetails);
        }
    }
}