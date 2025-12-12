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

public class ExploreInDirection
{

    public enum ExpeditionPhase { MovingTowardsGoal, Returning, Complete }
    public ExpeditionPhase currentPhase = ExpeditionPhase.MovingTowardsGoal;
    public bool Success { get; private set; } = false;

    public Character explorerCharacter { get; set; }
    public Vector2Int startPosition { get; set; }

   
    public SearchConditions conditions { get; private set; }

    public CreateStuffSimpleFunctions.Direction goalDirection;

    public int distance;
    public int staminaTotal;
    public int stamina;
    public int currentDay;



    public ExplorerFunctions functions;

    public ExploreInDirection(Character character, SearchConditions inputConditions, Expedition expedition, int Distance, CreateStuffSimpleFunctions.Direction Direction, AccessibleBlocks inAccessibleBlocks, int inStamina, GameManager inGameManager)
    {
        explorerCharacter = character;

        distance = Distance;
        conditions = inputConditions;
        startPosition = expedition.startPosition;


        staminaTotal = inStamina;
        currentDay = 0;

        functions = new ExplorerFunctions();
        functions.gameManager = inGameManager;
        functions.TempExploredBlocks = inAccessibleBlocks.ExploredBlocks;
        functions.TempSeenBlocks = inAccessibleBlocks.SeenBlocks;
        functions.currentPosition = startPosition;
        functions.startPosition = startPosition;
        functions.character = character;
        functions.goalDirection = Direction;
        functions.conditions = conditions;
        functions.explorerMain = this;
        functions.visionRadius = 2;

    }

    public void OnDayIncremented()
    {
        currentDay++;

        switch (currentPhase)
        {
            case (ExpeditionPhase.MovingTowardsGoal):
                MoveTowardsRadius();
                break;
            case (ExpeditionPhase.Returning):
                ReturnHome(staminaTotal);
                break;
            case (ExpeditionPhase.Complete):
                HandleExpeditionCompletion();
                break;
        }
    }


    public void MoveTowardsRadius()
    {
        stamina = staminaTotal;
        int maxAttempts = staminaTotal + 10; // Example: Total stamina plus some buffer
        int attempts = 0;

        if (currentDay > 3)
        {
            currentPhase = ExpeditionPhase.Returning; // Transition to returning phase
            Debug.Log($"{explorerCharacter.Data.name} returning home after 3 days unsuccessful");
            ReturnHome(staminaTotal);
        }
        else
        {
            stamina = staminaTotal;

            while (stamina > 0 && attempts < maxAttempts)
            {
                attempts++;
                if (functions.ReachedGoal())
                {
                    Debug.Log($"{explorerCharacter.Data.name} Reached goal successful");
                    Success = true;
                    currentPhase = ExpeditionPhase.Returning; // Transition to returning phase
                    ReturnHome(stamina);
                    break;
                }
                else
                {
                    var nextMove = functions.DecideNextMove();
                    if (nextMove.HasValue)
                    {
                        int staminaToSubtract = functions.MoveCharacter(nextMove);
                        stamina -= staminaToSubtract;
                    }

                    if (nextMove == null && functions.GetPathTotal() > 0)
                    {
                        functions.BackStep();

                        // Adjust stamina as necessary
                        stamina--;

                        // Consider marking this attempt or adjusting logic to prevent immediate return to the same stuck position
                    }
                }
            }
        }
    }

    public void ReturnHome(int startingStamina)
    {
        int maxAttempts = staminaTotal + 10; // Example: Total stamina plus some buffer
        int attempts = 0;

        if (currentDay > 7)
        {
            functions.HandleLost();
        }
        else
        {
            stamina = startingStamina;

            while (stamina > 0 && attempts < maxAttempts)
            {
                attempts++;

                if (functions.ReturnedHome())
                {
                    Debug.Log($"{explorerCharacter.Data.name} returned Home");
                    currentPhase = ExpeditionPhase.Complete; // Transition to returning phase
                    break;
                }

                var nextMove = functions.DecideNextMoveForReturn();
                if (nextMove.HasValue)
                {
                    functions.MoveCharacter(nextMove);
                }

                if (nextMove == null && functions.GetPathTotal() > 0)
                {
                    functions.BackStep();
                    // Adjust stamina as necessary
                    stamina--;

                    // Consider marking this attempt or adjusting logic to prevent immediate return to the same stuck position
                }


            }
        }
    }

    public void HandleExpeditionCompletion()
    {
        if (!Success)
        {
            conditions.Failed = true;
        }

        conditions.Finished = true;
    }





}
