using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FindSpawnPointsByPreference;
using static MapArrayScript;

public class SpawnPointFilterFunctions : MonoBehaviour
{
    public GameManager gameManager;

    public void Initialize(GameManager inGameManager)
    {
        gameManager = inGameManager;
    }
    public int CalculateWaterFeatureScore(GenericCoordinate block, WaterFeatureScores scores)
    {
        int waterScore = 0;
        if (IsRiverBlock(block)) waterScore += scores.RiverScore;
        if (IsLakeAdjacent(block)) waterScore += scores.LakeScore;
        if (IsCoastBlock(block)) waterScore += scores.CoastScore;
        return waterScore;
    }
    public bool IsRiverBlock(GenericCoordinate block)
    {
        int checkRange = 3; // Define the range to check around the block

        for (int dx = -checkRange; dx <= checkRange; dx++)
        {
            for (int dy = -checkRange; dy <= checkRange; dy++)
            {
                int checkX = block.x + dx;
                int checkY = block.y + dy;

                if (checkX >= 0 && checkX < gameManager.gameMapBlocks.GetLength(0) && checkY >= 0 && checkY < gameManager.gameMapBlocks.GetLength(1))
                {
                    if (gameManager.gameMapBlocks[checkX, checkY].isRiver)
                    {
                        // River found near block
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsLakeAdjacent(GenericCoordinate block)
    {
        // Directions to check for adjacent lake blocks
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, -1, 0, 1 };

        for (int i = 0; i < 4; i++)
        {
            int adjX = block.x + dx[i];
            int adjY = block.y + dy[i];

            // Check bounds and then if the adjacent block is part of a lake
            if (adjX >= 0 && adjX < gameManager.gameMapBlocks.GetLength(0) && adjY >= 0 && adjY < gameManager.gameMapBlocks.GetLength(1) && gameManager.gameMapBlocks[adjX, adjY].isLake)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCoastBlock(GenericCoordinate block)
    {
        return gameManager.gameMapBlocks[block.x, block.y].isCoast;
    }

    public int ScoreForLandType(Blocktype blockType, LandTypeScores scores)
    {
        switch (blockType)
        {
            case Blocktype.Lowland: return scores.Lowland;
            case Blocktype.Land: return scores.Land;
            case Blocktype.Hill: return scores.Hill;
            case Blocktype.Sand: return scores.Sand;
            default: return 0; // Default score for unspecified land types
        }
    }

    public int ScoreResourcesAroundBlock(GenericCoordinate block, ResourceScores scores)
    {
        int totalScore = 0;
        int range = 3; // Check 3 blocks in each direction

        List<GenericCoordinate> coordinatesToCheck = new List<GenericCoordinate>();
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int checkX = block.x + dx;
                int checkY = block.y + dy;
                if (checkX >= 0 && checkX < gameManager.gameMapBlocks.GetLength(0) && checkY >= 0 && checkY < gameManager.gameMapBlocks.GetLength(1))
                {
                    coordinatesToCheck.Add(new GenericCoordinate(checkX, checkY));
                }
            }
        }


        // Calculations
        totalScore += CalculateTotalResourceScore(coordinatesToCheck, scores.MainFoodMultiplier, block => block.hasFood);
        totalScore += CalculateTotalResourceScore(coordinatesToCheck, scores.BonusFoodMultiplier, block => block.hasBonusFood);
        totalScore += CalculateTotalResourceScore(coordinatesToCheck, scores.MainWoodMultiplier, block => block.hasWood);
        totalScore += CalculateTotalResourceScore(coordinatesToCheck, scores.BonusWoodMultiplier, block => block.hasBonusWood);
        totalScore += CalculateTotalResourceScore(coordinatesToCheck, scores.MainOreMultiplier, block => block.hasMainOre);
        totalScore += CalculateTotalResourceScore(coordinatesToCheck, scores.BonusOreMultiplier, block => block.hasBonusOre);
        totalScore += CalculateTotalResourceScore(coordinatesToCheck, scores.LuxuryResourceMultiplier, block => block.hasLuxuryResource);

        //Debug.Log($"Total Score for block at ({block.x}, {block.y}): {totalScore}");

        return totalScore;
    }

    private int CalculateTotalResourceScore(List<GenericCoordinate> checkCoordinates, float baseScore, Func<Block, bool> resourcePresenceCheck)
    {
        int score = 0;
        foreach (var coord in checkCoordinates)
        {
            Block block = gameManager.gameMapBlocks[coord.x, coord.y];
            if (resourcePresenceCheck(block))
            {
                score += (int)baseScore;
            }
        }
        return score;
    }
}
