using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;
using static ExplorationManager;
using static MapArrayScript;


public class Expedition
{
    public List<Job> jobs;
    public ExpeditionType type;
    public List<SearchConditions> conditions;
    public Vector2Int startPosition;
    public ExplorerManager exploreManager;

    public struct ExplorerManager
    {
        public List<ExploreInDirection> directionExplorers;
        public List<Character> character;

        // Initialize the lists in the struct using a method since constructors aren't allowed
        public void InitializeLists()
        {
            directionExplorers = new List<ExploreInDirection>();
            character = new List<Character>();
        }
    }

    public Expedition(List<Job> newJob, ExpeditionType Type, List<SearchConditions> searchConditions, Vector2Int point)
    {
        jobs = newJob;
        type = Type;
        conditions = searchConditions;
        startPosition = point;
        exploreManager = new ExplorerManager();
        exploreManager.InitializeLists();
    }
}

public enum ExpeditionType
{
    SearchNSEWFromPoint
    // Other types of expeditions can be added here in the future.
}

public class SearchConditions
{

    public bool Started { get;  set; } = false;
    //public bool InProgress { get;  set; } = false;
    public bool Finished { get;  set; } = false;
    public bool Failed { get; set; } = false;
    public bool Death { get; set; } = false;

    // Constructor to set an initial state, if necessary
    public SearchConditions()
    {
        Started = false;
        //InProgress = false;
        Finished = false;
        Failed = false;
        Death = false;
    }
}

public class ExpeditionManager
{
    public List<Expedition> expeditions;
    public GameManager gameManager;
    public void Initialise()
    {
        expeditions = new List<Expedition>();
    }

    public void UpdateExpeditions(RaceManager raceManager)
    {
        // Create a list to hold expeditions that will be finished
        List<Expedition> finishedExpeditions = new List<Expedition>();

        // Iterate through each expedition in progress
        foreach (Expedition expedition in expeditions)
        {
            if (!IsExpeditionComplete(expedition))
            {
                switch (expedition.type)
                {
                    case ExpeditionType.SearchNSEWFromPoint:
                        // Execute the logic for SearchNSEWFromPoint type expeditions
                        SearchNSEWFromPoint(raceManager, raceManager.jobManager, expedition);
                        break;
                        // Other expedition types can be handled in additional cases
                }
            }
            else
            {
                switch (expedition.type)
                {
                    case ExpeditionType.SearchNSEWFromPoint:
                        // Execute the logic for SearchNSEWFromPoint type expeditions
                        SearchNSEWFromPoint(raceManager, raceManager.jobManager, expedition);
                        break;
                        // Other expedition types can be handled in additional cases
                }

                finishedExpeditions.Add(expedition);
            }
        }

        // Second pass: safely remove finished expeditions and call FinishExpedition
        foreach (Expedition finishedExpedition in finishedExpeditions)
        {
            FinishExpedition(raceManager, finishedExpedition);
        }
    }

    private bool IsExpeditionComplete( Expedition expedition)
    {
        switch (expedition.type)
        {
            case ExpeditionType.SearchNSEWFromPoint:
                // Execute the logic for SearchNSEWFromPoint type expeditions
                if (expedition.conditions[0].Finished == true
                 && expedition.conditions[1].Finished == true
                 && expedition.conditions[2].Finished == true
                 && expedition.conditions[3].Finished == true)
                {
                    Debug.Log("IsExpeditionComplete: returning true all tasks done");
                    return true;
                }
                else
                {
                    //raceManager.explorationManager.searchAround
                    return false;
                }
            // Other expedition types can be handled in additional cases
            default:
                return false;
        }
    }

    public void SearchNSEWFromPoint(RaceManager raceManager, JobManager jobManager, Expedition expedition)
    {
        
        foreach (Job job in expedition.jobs)
        {
            if (jobManager.activeJobs.Contains(job))
            {
                switch (job.JobName)
                {
                    case "Search North":
                        DetermineStart(raceManager, expedition ,expedition.conditions[0], CreateStuffSimpleFunctions.Direction.up, 9, job.AssignedCharacter, job);
                        break;
                    case "Search South":
                        DetermineStart(raceManager, expedition, expedition.conditions[1], CreateStuffSimpleFunctions.Direction.down, 9, job.AssignedCharacter, job);
                        break;
                    case "Search East":
                        DetermineStart(raceManager, expedition, expedition.conditions[2], CreateStuffSimpleFunctions.Direction.right, 9, job.AssignedCharacter, job);
                        break;
                    case "Search West":
                        DetermineStart(raceManager, expedition, expedition.conditions[3], CreateStuffSimpleFunctions.Direction.left, 9, job.AssignedCharacter, job);
                        break;
                    default:
                        Debug.Log("Invalid direction");
                        break;
                }
            }
        }
    }

    public void DetermineStart(RaceManager raceManager, Expedition expedition,  SearchConditions condition, CreateStuffSimpleFunctions.Direction direction, int radius, Character character, Job job)
    {
        if (!condition.Started)
        {
            //Start
            ExploreInDirection explorer = new ExploreInDirection(character,condition, expedition, radius, direction, raceManager.explorationManager.accessibleBlocks, raceManager.raceProperties.Stamina, gameManager);
            raceManager.DayIncrementedForCharacters += explorer.OnDayIncremented;
            expedition.exploreManager.directionExplorers.Add(explorer);
            expedition.exploreManager.character.Add(character);
            condition.Started = true;

            raceManager.aliveCharacters.OnExpedition.Add(character);

            Debug.Log($"{character.Data.name} StartedExpedition in the direction: {direction}");

        }
        if (condition.Finished)
        {
            if (condition.Death)
            {
                Debug.Log($"DetermineStart: {character.Data.name} died");
                raceManager.characterFunctions.ProcessDeath(raceManager, character);
                

                //create New search expedition that is not a NSEW search
            }

            if (condition.Failed)
            {
                //create New search expedition that is not a NSEW search
            }
            else
            {
                Debug.Log($"DetermineStart: {character.Data.name} got back safetly and successfully");
                
            }

            raceManager.jobManager.RemoveJob(raceManager, job);
            raceManager.aliveCharacters.OnExpedition.Remove(character);

            MergeExplorerData(raceManager, character, expedition);
        }

    }

    public void MergeExplorerData(RaceManager raceManager, Character character, Expedition expedition )
    {
        var explorer = expedition.exploreManager.directionExplorers
                               .FirstOrDefault(e => e.explorerCharacter == character);

        if (explorer != null)
        {
            int addedExploredBlocks = 0;
            int addedSeenBlocks = 0;
            // Assuming ExplorationManager's accessible blocks are global and accessible here
            foreach (var block in explorer.functions.TempExploredBlocks)
            {
                raceManager.explorationManager.accessibleBlocks.ExploredBlocks.Add(block);
                addedExploredBlocks++;
            }

            foreach (var block in explorer.functions.TempSeenBlocks)
            {

                raceManager.explorationManager.accessibleBlocks.SeenBlocks.Add(block);
                addedSeenBlocks++;

            }

            Debug.Log($"Explorer '{character.Data.name}' added {addedExploredBlocks} explored blocks and {addedSeenBlocks} seen blocks to global data.");
        }
        else
        {
            Debug.Log("MergerExplorerData: explorer = null");
        }
    }

    private void FinishExpedition(RaceManager raceManager, Expedition expedition)
    {
        expeditions.Remove(expedition);
        raceManager.explorationManager.searchAroundTownsComplete = true;
        raceManager.explorationManager.searchAroundTownsInProgress = false;

    }
}
