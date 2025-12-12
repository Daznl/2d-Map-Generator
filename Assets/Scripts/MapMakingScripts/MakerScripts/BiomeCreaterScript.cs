using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapArrayScript;

public class BiomeCreaterScript : MonoBehaviour
{
    public MapArrayScript M;
    public void TransformToDesert()
    {
        int lastTerritoryId = GameManager.Instance.numberOfTerritories - 1;

        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                // Check if the block belongs to the last territory
                if (M.spawnedFrom[x, y] == lastTerritoryId)
                {
                    // Check the block type and change it if necessary
                    if (M.blockType[x, y] == Blocktype.Land || M.blockType[x, y] == Blocktype.Lowland)
                    {
                        M.blockType[x, y] = Blocktype.Sand;
                    }
                    else if (M.blockType[x, y] == Blocktype.Hill)
                    {
                        M.blockType[x, y] = Blocktype.DryLand;
                    }
                    else if (M.blockType[x, y] == Blocktype.Peak)
                    {
                        M.blockType[x, y] = Blocktype.Mountain;
                    }
                }
            }
        }
    }

    public void TransformToSwamp()
    {
        int lastTerritoryId = GameManager.Instance.numberOfTerritories - 2;
           int swampTerritoryId = FindTerritoryWithLeastBlocks();

        // Check if a valid territory was found
        if (swampTerritoryId == -1)
        {
            Debug.Log("No suitable territory found for swamp");
            return;
        }

        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                // Check if the block belongs to the swamp territory
                if (M.spawnedFrom[x, y] == lastTerritoryId)
                {
                    // Check the block type and change it if necessary
                    if (M.blockType[x, y] == Blocktype.Land)
                    {
                        M.blockType[x, y] = Blocktype.Swamp;
                    }
                    else if (M.blockType[x, y] == Blocktype.Lowland)
                    {
                        M.blockType[x, y] = Blocktype.Swamplowland;
                    }
                    else if (M.blockType[x, y] == Blocktype.Sand)
                    {
                        M.blockType[x, y] = Blocktype.Swamplowland;
                    }
                    else if (M.blockType[x, y] == Blocktype.Hill)
                    {
                        M.blockType[x, y] = Blocktype.Swamplowland;
                    }

                    // Additional transformations can be added here
                }
            }
        }
    }


    public int FindTerritoryWithLeastBlocks()
    {
        Dictionary<int, int> blockCounts = new Dictionary<int, int>();

        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                int territoryId = M.spawnedFrom[x, y];
                if (!blockCounts.ContainsKey(territoryId))
                {
                    blockCounts[territoryId] = 0;
                }
                blockCounts[territoryId]++;
            }
        }

        int minBlockTerritoryId = -1;
        int minBlocks = int.MaxValue;

        foreach (var territory in blockCounts)
        {
            if (territory.Value < minBlocks)
            {
                minBlocks = territory.Value;
                minBlockTerritoryId = territory.Key;
            }
        }

        return minBlockTerritoryId;
    }
}
