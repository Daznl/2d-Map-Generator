using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static MapArrayScript;
using System;

public class RaceManager : MonoBehaviour
{
    public class AliveCharacters
    {
        public List<Character> Characters { get; set; } = new List<Character>();

        public List<Character> OnExpedition { get; set; } = new List<Character>();
        public List<Character> Children { get; set; } = new List<Character>();
        public List<Character> Adults { get; set; } = new List<Character>();
        public List<Character> NotWorkers { get; set; } = new List<Character>();
        public List<Character> Workers { get; set; } = new List<Character>();
        public List<Character> UnEmployed { get; set; } = new List<Character>();
        public List<Character> Employed { get; set; } = new List<Character>();
        public List<Couple> CoupleLookingForHome { get; set; } = new List<Couple>();
        public List<Character> SingleLookingForHome { get; set; } = new List<Character>();
    }

    public class DeadCharacters
    {
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<Character> Children { get; set; } = new List<Character>();
        public List<Character> Adults { get; set; } = new List<Character>();

    }

    public event Action DayIncrementedForCharacters;

    public AliveCharacters aliveCharacters;
    public DeadCharacters deadCharacters;

    public Text totalDisplay;
    public TimeCounter timeCounter;
    public RaceProperties raceProperties;
    public CharacterFunctions characterFunctions;
    public bool hasSpawnedInitialCharacters = false;

    public List<List<Character>> LivingTogether;

    public List<Couple> CoupleLookingForHome;
    public List<Character> SingleLookingForHome;

    public float deathModifier = 1.0f;
    public float birthModifier = 1.0f;
    public float foodConsumptionModifier = 1.0f;

    public MapArrayScript.Territory allocatedTerritory { get; set; }
    public GenericCoordinate spawnPoint;

    public GameManager gameManager;
    public BuildingManager buildingManager;
    public ResourceManager resourceManager;
    public JobManager jobManager;
    public ExplorationManager explorationManager;
    public LivingTogetherManager livingTogetherManager;
    public LivingGroupMovement livingGroupMovement;


    void Awake()
    {
        if (GetComponent<RaceBuildingFunctions>() == null)
        {
            gameObject.AddComponent<RaceBuildingFunctions>();
        }

        characterFunctions = gameObject.AddComponent<CharacterFunctions>();

        timeCounter = FindObjectOfType<TimeCounter>();

        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnDestroy()
    {
        timeCounter.OnYearIncremented -= OnYearIncremented;
        timeCounter.OnMonthIncremented -= OnMonthIncremented;
        timeCounter.OnDayIncremented -= OnDayIncremented;
    }

    public void Initialise()
    {
        aliveCharacters = new AliveCharacters();
        deadCharacters = new DeadCharacters();

        CoupleLookingForHome = new List<Couple>();
        SingleLookingForHome = new List<Character>();

        timeCounter.SubscribeToRaceStart(raceProperties.RaceStart, _ => characterFunctions.SpawnInitialCharacters(this, timeCounter));

        resourceManager = new ResourceManager();
        resourceManager.Initialise();

        jobManager = new JobManager();
        jobManager.Initialise(this);

        livingTogetherManager = new LivingTogetherManager();
        livingTogetherManager.Initilise();

        buildingManager = new BuildingManager();
        buildingManager.Initialise(this);

        explorationManager = new ExplorationManager();
        explorationManager.Initialise(this,gameManager);

        livingGroupMovement = new LivingGroupMovement();

        timeCounter.OnYearIncremented += OnYearIncremented;
        timeCounter.OnMonthIncremented += OnMonthIncremented;
        timeCounter.OnDayIncremented += OnDayIncremented;
    }

    private void OnYearIncremented()
    {
        characterFunctions.IncrementAge(this);
        characterFunctions.UpdateAdultStatus(this);
        characterFunctions.UpdateWorkingStatus(this);
        characterFunctions.UpdateCounterText(this);
    }

    private void OnMonthIncremented()
    {
        resourceManager.DetermineFoodState(this);
        buildingManager.HandleBuilding(this, buildingManager.StartingTown);

        explorationManager.HandleExpansion(this);

        jobManager.CalculateJobPriority(resourceManager);

        jobManager.AssignJobsToUnemployed(this);

        jobManager.ReassignJobsBasedOnPriority(this);
        characterFunctions.UpdateCounterText(this);
        livingGroupMovement.MoveGroupsIntoHomes(this);
        livingGroupMovement.UpdateHouseOccupancyStatus(this, buildingManager.StartingTown);
    }
    private void OnDayIncremented()
    {
        DayIncrementedForCharacters?.Invoke();
        explorationManager.expeditionManager.UpdateExpeditions(this);

        resourceManager.CalculateProductionAndConsumption(jobManager, this, buildingManager.StartingTown);
        characterFunctions.HandleCharacterDeath(this);
        characterFunctions.UpdateAdultThings(this);
        characterFunctions.UpdateCounterText(this);

        buildingManager.ProgressBuilding(this, buildingManager.StartingTown);
    }
}
