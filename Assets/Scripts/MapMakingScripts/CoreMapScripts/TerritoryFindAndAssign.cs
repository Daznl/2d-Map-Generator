using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.PlayerLoop;
using System.Runtime.CompilerServices;
using static MapArrayScript;

public class TerritoryFindandAssign : MonoBehaviour
{
    public MapArrayScript M;
    public CreateStuffSimpleFunctions S;
    public MapGenerationFunctions G;
    public CreateStuffAdvancedFunctions A;
    public CreateStuff C;
    public void AssignBlocksToTerritory()
    {
        // Iterate through every block in the blockType array
        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                // Check if the block belongs to a territory
                int territoryIndex = M.spawnedFrom[x, y];
                if (territoryIndex > -1 && territoryIndex <= M.world.territory.Count)  // Check if index is valid
                {
                    // Get the corresponding territory from the world
                    MapArrayScript.Territory territory = M.world.territory[territoryIndex];

                    MapArrayScript.GenericCoordinate block1 = new MapArrayScript.GenericCoordinate(x, y);
                    territory.blocks.Add(block1);
                }
            }
        }
    }
    public void AssignLakesToTerritorys()
    {
        bool[,] visited = new bool[GameManager.Instance.mapSize, GameManager.Instance.mapSize]; // Create a 2D array to track visited blocks

        // Iterate through each block in the spawnedFrom array
        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                // If the block is water and hasn't been visited yet
                if (M.spawnedFrom[x, y] == -2 && !visited[x, y])
                {
                    // Get the connected water blocks using BFS
                    MapArrayScript.GenericArea lake = doBFS(new MapArrayScript.GenericCoordinate(x, y), visited);

                    // Get the territorys that border these water blocks
                    List<int> borderingTerritorys = getBorderingTerritorys(lake.blocks);

                    // If the water blocks are surrounded by a single territory, add them to that territory's lakes
                    if (borderingTerritorys.Count == 1)
                    {
                        MapArrayScript.Territory territory = M.world.territory[borderingTerritorys[0]];
                        territory.lakes.Add(lake);

                        // Update the spawnedFrom array
                        foreach (MapArrayScript.GenericCoordinate block in lake.blocks)
                        {
                            M.spawnedFrom[block.x, block.y] = territory.id;

                        }
                    }
                }
            }
        }
    }
    /*This function is part of the Assign Lakes To territorys function and not for other functionality outside of this functionality*/
    private List<int> getBorderingTerritorys(List<MapArrayScript.GenericCoordinate> lakeBlocks)
    {
        HashSet<int> borderingTerritorys = new HashSet<int>();

        // Define the possible directions to move (left, right, up, down)
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        foreach (var block in lakeBlocks)
        {
            for (int i = 0; i < 4; i++)
            {
                int newX = block.x + dx[i];
                int newY = block.y + dy[i];

                // Check if the new position is within bounds and is not a water block
                if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize &&
                    M.spawnedFrom[newX, newY] != -2)
                {
                    borderingTerritorys.Add(M.spawnedFrom[newX, newY]);
                }
            }
        }

        return borderingTerritorys.ToList();
    }

    public void AssignMountainsToTerritory()
    {
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        foreach (var item in M.peakToMountainRangeMap)
        {
            int peakId = item.Key;
            var mountainRange = item.Value.Item2;

            Queue<MapArrayScript.GenericCoordinate> queue = new Queue<MapArrayScript.GenericCoordinate>();
            bool[,] visited = new bool[GameManager.Instance.mapSize, GameManager.Instance.mapSize];

            queue.Enqueue(new MapArrayScript.GenericCoordinate(mountainRange.x, mountainRange.y));
            visited[mountainRange.x, mountainRange.y] = true;

            while (queue.Count > 0)
            {
                MapArrayScript.GenericCoordinate current = queue.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    int newX = current.x + dx[i];
                    int newY = current.y + dy[i];

                    if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize &&
                        !visited[newX, newY] && M.peakSpawnedFrom[newX, newY] == peakId)
                    {
                        MapArrayScript.GenericCoordinate newCoordinate = new MapArrayScript.GenericCoordinate(newX, newY);
                        queue.Enqueue(newCoordinate);
                        visited[newX, newY] = true;

                        // Add the block to the corresponding mountain range
                        mountainRange.blocks.Add(newCoordinate);
                    }
                }
            }
        }
    }

    public void AssignCoastalBlocksToTerritories()
    {
        // Define the possible directions to move (left, right, up, down)
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        // Go through all territories
        foreach (MapArrayScript.Territory territory in M.world.territory)
        {
            // Check all blocks in the territory
            foreach (MapArrayScript.GenericCoordinate block in territory.blocks)
            {
                for (int i = 0; i < 4; i++)
                {
                    int newX = block.x + dx[i];
                    int newY = block.y + dy[i];

                    // Check if the new position is within bounds and is a water block
                    if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize &&
                        M.spawnedFrom[newX, newY] == -2)
                    {
                        // If so, add the block to the territory's coast
                        territory.coast.blocks.Add(block);
                        break;
                    }
                }
            }
        }
    }
    public void AssignRemainingBlocksToOcean()
    {
        // Iterate through each block in the spawnedFrom array
        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                // If the block is water and isn't part of a lake
                if (M.spawnedFrom[x, y] == -2)
                {
                    // Add this block to the ocean
                    MapArrayScript.GenericCoordinate coordinate = new MapArrayScript.GenericCoordinate(x, y);
                    M.world.ocean.blocks.Add(coordinate);
                }
            }
        }
    }
    private MapArrayScript.GenericArea doBFS(MapArrayScript.GenericCoordinate start, bool[,] visited)
    {
        MapArrayScript.GenericArea lake = new MapArrayScript.GenericArea(start.x, start.y, A.GetRandomLineFromTextFile("TextFiles/LakeNames"));
        Queue<MapArrayScript.GenericCoordinate> queue = new Queue<MapArrayScript.GenericCoordinate>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        // Define the possible directions to move (left, right, up, down)
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        while (queue.Count > 0)
        {
            MapArrayScript.GenericCoordinate current = queue.Dequeue();
            lake.blocks.Add(current);

            for (int i = 0; i < 4; i++)
            {
                int newX = current.x + dx[i];
                int newY = current.y + dy[i];

                // Check if the new position is within bounds and is a water block
                if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize &&
                    M.spawnedFrom[newX, newY] == -2 && !visited[newX, newY])
                {
                    queue.Enqueue(new MapArrayScript.GenericCoordinate(newX, newY));
                    visited[newX, newY] = true;
                }
            }
        }

        return lake;
    }
    



    public void CheckAndFixDisconnectedTerritorys()
    {
        foreach (var territory in M.world.territory)
        {
            if (!IsTerritoryConnected(territory))
            {
                var disconnectedBlocksMap = FindDisconnectedBlocks(territory);
                ReassignDisconnectedBlocks(disconnectedBlocksMap);
            }
        }
    }

    /*This function ensures that all parts of a territory are reachable such that issues do not arise from inaccesible blocks*/
    public bool IsTerritoryConnected(MapArrayScript.Territory territory)
    {
        bool[,] visited = new bool[GameManager.Instance.mapSize, GameManager.Instance.mapSize]; // Create a 2D array to track visited blocks
        MapArrayScript.GenericCoordinate start = new MapArrayScript.GenericCoordinate(territory.center.x, territory.center.y); // Start from the center of the territory

        // Perform a DFS from the starting block
        doDFS(start, visited, territory.id);

        // Check if there are any unvisited blocks in the territory
        foreach (var block in territory.blocks)
        {
            if (!visited[block.x, block.y])
            {
                return false; // The territory is disconnected
            }
        }

        return true; // The territory is connected
    }
    private void doDFS(MapArrayScript.GenericCoordinate start, bool[,] visited, int territoryId)
    {
        Stack<MapArrayScript.GenericCoordinate> stack = new Stack<MapArrayScript.GenericCoordinate>();

        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        stack.Push(start);
        visited[start.x, start.y] = true;

        while (stack.Count > 0)
        {
            MapArrayScript.GenericCoordinate current = stack.Pop();

            for (int i = 0; i < 4; i++)
            {
                int newX = current.x + dx[i];
                int newY = current.y + dy[i];

                // Check if the new position is within bounds and belongs to the same territory
                if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize &&
                    M.spawnedFrom[newX, newY] == territoryId && !visited[newX, newY])
                {
                    stack.Push(new MapArrayScript.GenericCoordinate(newX, newY));
                    visited[newX, newY] = true;
                }
            }
        }
    }
    /*After completing the DFS, the function iterates over all blocks listed as part of the territory (territory.blocks). For each block, it checks whether it was visited during the DFS.
    If a block was not visited (!visited[block.x, block.y]), it is considered disconnected from the main body of the territory.*/
    public Dictionary<int, List<MapArrayScript.GenericCoordinate>> FindDisconnectedBlocks(MapArrayScript.Territory territory)
    {
        bool[,] visited = new bool[GameManager.Instance.mapSize, GameManager.Instance.mapSize];
        MapArrayScript.GenericCoordinate start = new MapArrayScript.GenericCoordinate(territory.center.x, territory.center.y);
        doDFS(start, visited, territory.id);

        // Map to hold territory ID and the corresponding disconnected blocks
        Dictionary<int, List<MapArrayScript.GenericCoordinate>> disconnectedBlocksMap = new Dictionary<int, List<MapArrayScript.GenericCoordinate>>();

        foreach (var block in territory.blocks)
        {
            if (!visited[block.x, block.y])
            {
                // Find the ID of the surrounding territory
                int surroundingTerritoryId = FindSurroundingTerritory(block);
                if (!disconnectedBlocksMap.ContainsKey(surroundingTerritoryId))
                {
                    disconnectedBlocksMap[surroundingTerritoryId] = new List<MapArrayScript.GenericCoordinate>();
                }
                disconnectedBlocksMap[surroundingTerritoryId].Add(block);
            }
        }

        return disconnectedBlocksMap;
    }
    public void ReassignDisconnectedBlocks(Dictionary<int, List<MapArrayScript.GenericCoordinate>> disconnectedBlocksMap)
    {
        foreach (var kvp in disconnectedBlocksMap)
        {
            foreach (var block in kvp.Value)
            {
                // Reassign the block to the surrounding territory
                M.spawnedFrom[block.x, block.y] = kvp.Key;
            }
        }
    }
    /*The main purpose of this function is to find the ID of a territory that is directly adjacent to a specific block. 
     * This can be crucial for operations like reassigning disconnected blocks to the correct territory, managing border 
     * disputes between territories, or simply understanding the territorial context of a specific location on the map.*/

    private int FindSurroundingTerritory(MapArrayScript.GenericCoordinate block)
    {
        // Define the possible directions to move (left, right, up, down)
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = block.x + dx[i];
            int newY = block.y + dy[i];

            // Check if the new position is within bounds and belongs to a territory
            if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize &&
                M.spawnedFrom[newX, newY] >= 0)
            {
                return M.spawnedFrom[newX, newY];
            }
        }

        return -1;
    }
    public void UpdateTerritoryLandConnections()
    {
        // Define the possible directions to move (left, right, up, down)
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        // Go through each territory
        foreach (var territory in M.world.territory)
        {
            // Clear the existing connections
            territory.landConnections.Clear();

            // Go through each block in the territory
            foreach (var block in territory.blocks)
            {
                // Check each surrounding block
                for (int i = 0; i < 4; i++)
                {
                    int newX = block.x + dx[i];
                    int newY = block.y + dy[i];

                    // Check if the new position is within bounds and belongs to a different territory
                    if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize &&
                        M.spawnedFrom[newX, newY] != territory.id && M.spawnedFrom[newX, newY] >= 0)
                    {
                        // Add the neighboring territory to the list of connections if it's not already there
                        if (!territory.landConnections.Contains(M.spawnedFrom[newX, newY]))
                        {
                            territory.landConnections.Add(M.spawnedFrom[newX, newY]);
                        }
                    }
                }
            }
        }
    }

    public void PopulateAllBorderAreas()
    {
        // Temporary dictionary to hold border blocks grouped by neighboring territory ID for each territory
        Dictionary<int, Dictionary<int, List<GenericCoordinate>>> borderBlocksByNeighborAndTerritory = new Dictionary<int, Dictionary<int, List<GenericCoordinate>>>();

        // Define the possible directions to move (left, right, up, down)
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        // Initialize borderBlocksByNeighborAndTerritory for all territories
        foreach (var territory in M.world.territory)
        {
            borderBlocksByNeighborAndTerritory[territory.id] = new Dictionary<int, List<GenericCoordinate>>();
        }

        // Go through each territory
        foreach (var territory in M.world.territory)
        {
            foreach (var block in territory.blocks)
            {
                for (int i = 0; i < 4; i++)
                {
                    int newX = block.x + dx[i];
                    int newY = block.y + dy[i];

                    // Check if the new position is within bounds
                    if (newX >= 0 && newY >= 0 && newX < GameManager.Instance.mapSize && newY < GameManager.Instance.mapSize)
                    {
                        int neighborId = M.spawnedFrom[newX, newY];
                        // Check if the block at the new position belongs to a different territory
                        if (neighborId != territory.id && neighborId >= 0)
                        {
                            // Ensure there's a list for this neighbor ID within the current territory's dictionary
                            if (!borderBlocksByNeighborAndTerritory[territory.id].ContainsKey(neighborId))
                            {
                                borderBlocksByNeighborAndTerritory[territory.id][neighborId] = new List<GenericCoordinate>();
                            }
                            // Add this block to the list for the appropriate neighboring territory
                            borderBlocksByNeighborAndTerritory[territory.id][neighborId].Add(new GenericCoordinate(block.x, block.y));
                        }
                    }
                }
            }
        }

        // Clear existing borders for all territories before repopulating
        foreach (var territory in M.world.territory)
        {
            territory.borders.Clear();

            // Create a GenericArea for each set of border blocks and add it to the territory's borders
            foreach (var neighborId in borderBlocksByNeighborAndTerritory[territory.id].Keys)
            {
                GenericArea borderArea = new GenericArea();
                borderArea.blocks = borderBlocksByNeighborAndTerritory[territory.id][neighborId];
                borderArea.name = $"Border with Territory {neighborId}";
                territory.borders.Add(borderArea);

                // Log the number of border areas identified for each territory
                Debug.Log($"Territory {territory.id} ({territory.name}) has {territory.borders.Count} border areas identified.");
            }
        }
        Debug.Log("Completed populating border areas for all territories.");
    }

    /*go through each territory, The territory will check each territory.blocks it contains and check 
     * the corresponding teritory.blocks.x and y and check the riverNumberArray[x, y] to see if
     * there is a river there. if there is it will look throught the World.riverBlocks list and assign
     * a reference to the corresponding riverBlock that is contained in it.
     */

    public void RiverAssignToTerritories()
    {
        // Loop through all the territories in the world.
        foreach (var territory in M.world.territory)
        {
            // Loop through all the blocks in the territory.
            foreach (var block in territory.blocks)
            {
                // Check if the block in riverBlockArray has a river.
                var riverData = M.world.riverBlockDataArray[block.x, block.y];
                if (riverData.isRiver)
                {
                    // Add the RiverBlockData to the Territory's rivers list.
                    territory.rivers.Add(riverData);

                    // Optional: Log the addition for debugging
                     /*Debug.Log($"Added RiverBlockData to Territory '{territory.name}': Block coordinates ({block.x},{block.y}), " +
                         $"River details: InternalRiverNumber: {riverData.internalRiverNumber}, RiverNumber: {riverData.riverNumber}, ");*/
                }
            }
        }
    }

    public void ResourceFindandAssign()
    {
        // Loop through all the territories in the world.
        foreach (MapArrayScript.Territory territory in M.world.territory)
        {
            // Loop through all the blocks in the territory.
            foreach (MapArrayScript.GenericCoordinate block in territory.blocks)
            {
                // Check each resource list and add corresponding resource to the Territory's resource list if there's a match.
                AddResourceIfPresent(block, M.world.worldResources.mainFood, territory.TerritoryResources.mainFood);
                AddResourceIfPresent(block, M.world.worldResources.bonusFood, territory.TerritoryResources.bonusFood);
                AddResourceIfPresent(block, M.world.worldResources.mainWood, territory.TerritoryResources.mainWood);
                AddResourceIfPresent(block, M.world.worldResources.bonusWood, territory.TerritoryResources.bonusWood);
                AddResourceIfPresent(block, M.world.worldResources.mainOre, territory.TerritoryResources.mainOre);
                AddResourceIfPresent(block, M.world.worldResources.bonusOre, territory.TerritoryResources.bonusOre);
                AddResourceIfPresent(block, M.world.worldResources.luxuryResource, territory.TerritoryResources.luxuryResource);
            }
        }
    }

    public void AddResourceIfPresent(MapArrayScript.GenericCoordinate block, List<MapArrayScript.CoordinateWithAmount> worldResourceList, List<MapArrayScript.CoordinateWithAmount> territoryResourceList)
    {
        foreach (MapArrayScript.CoordinateWithAmount resource in worldResourceList)
        {
            if (block.x == resource.x && block.y == resource.y)
            {
                // Add the resource to the Territory's resource list.
                territoryResourceList.Add(resource);
                break;
            }
        }
    }
}
