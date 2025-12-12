using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using static ExplorationManager;
using static MapArrayScript;
using Unity.Collections.LowLevel.Unsafe;
using static UnityEditor.FilePathAttribute;
using System.Text.RegularExpressions;
using UnityEngine.TextCore.Text;
using static BuildingManager;
using UnityEditor.Build;
using System.Xml.Serialization;
using Mono.Cecil;
using System;
using UnityEngine.Rendering;
using UnityEditor.Experimental.GraphView;

public class ExplorerFunctions
{
    bool IsAccessible(Blocktype blockType)
    {
        switch (blockType)
        {
            case Blocktype.Water:
            case Blocktype.Deepwater:
            case Blocktype.Mountain:
            case Blocktype.Peak:
            case Blocktype.Swamplowland:
                return false;
            default:
                return true;
        }
    }

    public Character character;
    public CreateStuffSimpleFunctions.Direction goalDirection;
    public Vector2Int currentPosition;
    public Vector2Int startPosition;
    Stack<Vector2Int> pathHistory = new Stack<Vector2Int>();
    private Vector2Int? lastBacksteppedPosition = null;
    public SearchConditions conditions;
    public int visionRadius;


    public HashSet<GenericCoordinate> TempExploredBlocks = new HashSet<GenericCoordinate>();
    public HashSet<GenericCoordinate> TempSeenBlocks = new HashSet<GenericCoordinate>();

    public GameManager gameManager;
    public ExploreInDirection explorerMain;





    public int GetPathTotal()
    {
        return pathHistory.Count;
    }



    public Vector2Int? DecideNextMove()
    {
        List<Vector2Int> potentialMoves = GetPotentialMoves();
        Vector2Int? bestMove = null;
        int currentElevation = GetBlockElevation(gameManager.gameMapBlocks[currentPosition.x, currentPosition.y].blockType);
        int minElevationChange = int.MaxValue;

        foreach (var move in potentialMoves)
        {
            if (!TempExploredBlocks.Contains(new GenericCoordinate(move.x, move.y)) &&
                (!lastBacksteppedPosition.HasValue || lastBacksteppedPosition.Value != move))
            {
                Blocktype blockType = gameManager.gameMapBlocks[move.x, move.y].blockType;
                if (IsAccessible(blockType))
                {
                    int moveElevation = GetBlockElevation(blockType);
                    int elevationChange = Math.Abs(moveElevation - currentElevation);

                    if (elevationChange < minElevationChange)
                    {
                        bestMove = move;
                        minElevationChange = elevationChange;
                    }
                }
            }
        }

        if (bestMove.HasValue)
        {
            TempExploredBlocks.Add(new GenericCoordinate(bestMove.Value.x, bestMove.Value.y));
            lastBacksteppedPosition = null; // Reset last backstepped position after making a new move
            return bestMove;
        }

        return null; // No unexplored moves found, may need different logic for returning
    }

    public int MoveCharacter(Vector2Int? nextMove)
    {
        int staminaToSubtract = 0;
        // Calculate elevation difference
        int currentElevation = GetBlockElevation(gameManager.gameMapBlocks[currentPosition.x, currentPosition.y].blockType);
        int nextElevation = GetBlockElevation(gameManager.gameMapBlocks[nextMove.Value.x, nextMove.Value.y].blockType);
        int elevationDifference = nextElevation - currentElevation;

        // Save the current position before moving
        pathHistory.Push(currentPosition);

        // Move to the next position
        currentPosition = nextMove.Value;


        // Optionally, mark the current position as explored
        TempExploredBlocks.Add(new GenericCoordinate(currentPosition.x, currentPosition.y));

        if (gameManager.gameMapBlocks[currentPosition.x, currentPosition.y].isRiver)
        {
            staminaToSubtract += 2;

        }

        if (gameManager.gameMapBlocks[currentPosition.x, currentPosition.y].hasWood)
        {
            staminaToSubtract += 1;
        }

        if (elevationDifference > 0 || elevationDifference < 0)
        {
            staminaToSubtract += 2; // Costs 2 stamina to move uphill (1 for move + 1 for elevation)
        }
        else
        {
            staminaToSubtract += 1; // Costs 1 stamina to move on flat ground or downhill
        }

        UpdateSeenBlocks();

        Debug.Log($" moved to ({currentPosition.x},{currentPosition.y}) consuming {staminaToSubtract}");

        return staminaToSubtract;
    }
    public void BackStep()
    {

        Vector2Int previousPosition = pathHistory.Pop();
        lastBacksteppedPosition = currentPosition; // Remember the current position before backstepping
        currentPosition = previousPosition;
        Debug.Log($"{character.Data.name} backstepped to ({currentPosition.x},{currentPosition.y})");
    }

    public void HandleLost()
    {
        Debug.Log($"{character.Data.name} got lost");
        int roll = UnityEngine.Random.Range(0, 3); // Generates 0, 1, or 2

        switch (roll)
        {
            case 0:
                // 33% chance - Character dies
                HandleCharacterDeath(conditions);
                break;
            case 1:
                Debug.Log($"{character.Data.name} while lost forgot the report but returned home");
                // 33% chance - Character makes it home but cannot report back the hashsets
                TempExploredBlocks = new HashSet<GenericCoordinate>();
                TempSeenBlocks = new HashSet<GenericCoordinate>();
                explorerMain.currentPhase = ExploreInDirection.ExpeditionPhase.Complete;
                break;
            case 2:
                // 33% chance - Character returns home successfully and reports their hashsets
                Debug.Log($"{character.Data.name} while lost found the way home and remembered the way");
                explorerMain.currentPhase = ExploreInDirection.ExpeditionPhase.Complete;
                break;
        }
    }

    public Vector2Int? DecideNextMoveForReturn()
    {
        List<Vector2Int> potentialMoves = GetPotentialMoves(); // Within one unit distance
        Vector2Int? bestMove = null;
        int shortestDistanceToStart = int.MaxValue;

        foreach (var move in potentialMoves)
        {
            if (IsAccessible(gameManager.gameMapBlocks[move.x, move.y].blockType))
            {
                if (IsMoveTowardsTargetFavourable(currentPosition, move, startPosition))
                {
                    int distanceToStart = ManhattanDistance(move, startPosition);
                    if (distanceToStart < shortestDistanceToStart)
                    {
                        bestMove = move;
                        shortestDistanceToStart = distanceToStart;
                    }
                }
            }
        }

        return bestMove;
    }

    private bool IsMoveTowardsTargetFavourable(Vector2Int currentPosition, Vector2Int move, Vector2Int targetPosition)
    {
        // Calculate the Manhattan distance from the current position to the target before and after the move
        int currentDistance = ManhattanDistance(currentPosition, targetPosition);
        int newDistance = ManhattanDistance(currentPosition + move, targetPosition);

        // If the new distance is less than the current distance, the move is towards the target
        return newDistance < currentDistance;
    }

    public int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Math.Abs(a.x - currentPosition.x) + Math.Abs(a.y - b.y);
    }

    void UpdateSeenBlocks()
    {
        for (int dx = -visionRadius; dx <= visionRadius; dx++)
        {
            for (int dy = -visionRadius; dy <= visionRadius; dy++)
            {
                if (Math.Abs(dx) + Math.Abs(dy) <= visionRadius && !(dx == 0 && dy == 0))
                {
                    Vector2Int seenPosition = new Vector2Int(currentPosition.x + dx, currentPosition.y + dy);
                    // Here you might want to check if seenPosition is within bounds (if your map has boundaries)
                    TempSeenBlocks.Add(new GenericCoordinate(seenPosition.x, seenPosition.y));
                }
            }
        }
    }

    private bool IsDirectionallyFavorable(int dx, int dy, CreateStuffSimpleFunctions.Direction direction)
    {
        // This function determines if a move is in the general preferred direction
        // It's a simplification. You might need a more sophisticated approach based on your game's requirements
        switch (direction)
        {
            case CreateStuffSimpleFunctions.Direction.up:
                return dy > 0;
            case CreateStuffSimpleFunctions.Direction.down:
                return dy < 0;
            case CreateStuffSimpleFunctions.Direction.left:
                return dx < 0;
            case CreateStuffSimpleFunctions.Direction.right:
                return dx > 0;
            default:
                return false; // Should not happen
        }
    }

    List<Vector2Int> GetPotentialMoves()
    {
        List<Vector2Int> potentialMoves = new List<Vector2Int>();

        // Directions for immediate surrounding blocks (up, down, left, right)
        var directions = new List<(int dx, int dy)>
        {
            (0, 1),  // up
            (0, -1), // down
            (-1, 0), // left
            (1, 0)   // right
        };

        foreach (var (dx, dy) in directions)
        {
            Vector2Int targetPosition = new Vector2Int(currentPosition.x + dx, currentPosition.y + dy);

            // Here, we assume all immediate moves are potential as they are one unit away
            // You might want to check if the target position is valid (e.g., within map bounds)
            if (IsDirectionallyFavorable(dx, dy, goalDirection))
            {
                potentialMoves.Add(targetPosition);
            }
        }

        return potentialMoves;
    }



    int GetBlockElevation(Blocktype blockType)
    {
        switch (blockType)
        {
            case Blocktype.Deepwater: return ElevationSettings.deepWaterElevation;
            case Blocktype.Water: return ElevationSettings.waterElevation;
            case Blocktype.Shallowwater: return ElevationSettings.shallowWaterElevation;
            case Blocktype.Lowland: return ElevationSettings.lowlandElevation;
            case Blocktype.Land: return ElevationSettings.landElevation;
            case Blocktype.Sand: return ElevationSettings.sandElevation;
            case Blocktype.Hill: return ElevationSettings.hillElevation;
            case Blocktype.Highland: return ElevationSettings.highlandElevation;
            case Blocktype.Mountain: return ElevationSettings.mountainElevation;
            case Blocktype.Peak: return ElevationSettings.peakElevation;
            case Blocktype.DryLand: return ElevationSettings.drylandElevation;
            case Blocktype.Swamp: return ElevationSettings.swampElevation;
            case Blocktype.Swamplowland: return ElevationSettings.swampLowlandElevation;
            default: return 0; // Default for types not specified
        }
    }



    public void HandleCharacterDeath(SearchConditions conditions)
    {
        conditions.Finished = true;
        conditions.Death = true;
        //Clear exploration
        TempExploredBlocks = new HashSet<GenericCoordinate>();
        TempSeenBlocks = new HashSet<GenericCoordinate>();
    }

    public bool ReachedGoal()
    {
        return ManhattanDistance(startPosition, currentPosition) >= 9;
    }

    public bool ReturnedHome()
    {
        if (currentPosition == startPosition)
        {
            return true;
        }
        return false;
    }
    
}
