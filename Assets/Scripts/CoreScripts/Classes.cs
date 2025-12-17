using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TextCore.Text;
using static MapArrayScript;

[System.Serializable]

public class ElevationSettings
{
    public static int deepWaterElevation { get; private set; } = -3;
    public static int waterElevation { get; private set; } = -2;
    public static int shallowWaterElevation { get; private set; } = -1;
    public static int lowlandElevation { get; private set; } = -1;
    public static int landElevation { get; private set; } = 0;
    public static int sandElevation { get; private set; } = 0;
    public static int hillElevation { get; private set; } = 1;
    public static int highlandElevation { get; private set; } = 2;
    public static int mountainElevation { get; private set; } = 3;
    public static int peakElevation { get; private set; } = 4;
    public static int drylandElevation { get; private set; } = 1;
    public static int swampElevation { get; private set; } = 0;
    public static int swampLowlandElevation { get; private set; } = -1;
}

public class Couple
{
    public List<Character> characters { get; set; }

    public Couple(Character partner1, Character partner2)
    {
        characters = new List<Character> { partner1, partner2 };
    }
}
public class Building
{
    public enum BuildingFunction
    {
        House, // Represents homes where characters live
               // Other building functions can be added here in the future
    }

    public BuildingFunction Function { get; set; } // Specifies the function of the building
    //public LivingGroup OccupantGroup { get; set; }
    /*public List<Character> Occupants { get; set; } = new List<Character>();*/ // Characters living in the building

    public Building(BuildingFunction function)
    {
        Function = function;
    }

    //public void SetOccupantGroup(LivingGroup group)
    //{
    //    OccupantGroup = group;
    //    if (group != null)
    //    {
    //        group.IsHoused = true;
    //        group.CurrentHome = this;
    //    }
    //}

    //public void ClearOccupantGroup()
    //{
    //    if (OccupantGroup != null)
    //    {
    //        OccupantGroup.IsHoused = false;
    //        OccupantGroup.CurrentHome = null;
    //        OccupantGroup = null;
    //    }
    //}
}

public class SearchToRadius
{
    public Vector2Int startingPostion;
    public Vector2Int currentposition;
    public int endRadius;
    public HashSet<GenericCoordinate> ExploredBlocks;
    public HashSet<GenericCoordinate> SeenBlocks;

    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public struct FlowPositions
    {
        public Position Straight { get; set; }
        public Position Left { get; set; }
        public Position Right { get; set; }
    }

    public FlowPositions flowPositions;
    public struct FlowDirections
    {
        public CreateStuffSimpleFunctions.Direction Straight { get; set; }
        public CreateStuffSimpleFunctions.Direction Left { get; set; }
        public CreateStuffSimpleFunctions.Direction Right { get; set; }
    }

    public FlowDirections flowDirections;

    public struct CanMoveDirections
    {
        public bool Straight { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Back { get; set; }
    }
}

//public class Town
//{
//    public string Name { get; set; }
//    public Vector2Int Location { get; set; }

//    public int TownLevel { get; set; }

//    public List<Construction> constructionsInProgress = new List<Construction>();
//    public List<TownConstruction> townConstructionInProgress = new List<TownConstruction>();

//    public List<Building> Houses { get; private set; } = new List<Building>();
//    public List<Building> OccupiedHouse { get; private set; } = new List<Building>();
//    public List<Building> UnoccupiedHouse { get; private set; } = new List<Building>();

//    public Town(string name, Vector2Int location, int level)
//    {
//        Name = name;
//        Location = location;
//        TownLevel = level;
//    }

//    // Method to add a building as unoccupied initially
//    public void AddUnoccupiedHouse(Building building)
//    {
//        UnoccupiedHouse.Add(building);
//        Houses.Add(building);
//    }

//    // Method to move a building from unoccupied to occupied
//    // This could be called when characters move into an unoccupied home
//    public void OccupyHouse(Building building)
//    {
//        if (UnoccupiedHouse.Remove(building))
//        {
//            OccupiedHouse.Add(building);
//        }
//    }
//}

//public class TownConstruction
//{
//    //public Town town;
//    public int totalToConstruct;
//    public int progress;
//    public List<Job> jobs;
//    public Vector2Int location;
//    public Town town;
//    public TownConstruction(int inTotalToConstruct, Vector2Int inLocation, Town inTown)
//    {
//        totalToConstruct = inTotalToConstruct;
//        progress = 0; // Assuming progress should start at 0
//        jobs = new List<Job>(); // Initialize the jobs list
//        location = inLocation;
//        town = inTown;
//        //this.town = town;
//    }
//}
//public class Construction
//{
//    //public Town town;
//    public int totalToConstruct;
//    public int progress;
//    public Building building;
//    public List<Job> jobs;
//    public Construction(int inTotalToConstruct, Building building, Town town)
//    {
//        totalToConstruct = inTotalToConstruct;
//        progress = 0; // Assuming progress should start at 0
//        building = building;
//        jobs = new List<Job>(); // Initialize the jobs list
//        //this.town = town;
//    }
//}

//public class LivingGroup
//{
//    public List<Character> Members { get; set; }
//    public bool IsHoused { get; set; }
//    public Building CurrentHome { get; set; } // This could be null for homeless groups

//    public LivingGroup(List<Character> members, bool isHoused, Building currentHome = null)
//    {
//        Members = members;
//        IsHoused = isHoused;
//        CurrentHome = currentHome;
//    }
//}

//public enum Gender
//{
//    Male,
//    Female
//}
//public struct Date
//{
//    public int Years { get; set; }
//    public int Days { get; set; }

//    public Date(int years, int days)
//    {
//        Years = years;
//        Days = days;
//    }

//    public override string ToString()
//    {
//        return $"Year: {Years}, Day: {Days}";
//    }

//}

public enum GameResource
{
    MainFood,
    MainWood,
    MainOre,
    BonusFood,
    BonusWood,
    BonusOre,
    LuxuryResource,
    Builder,
    Explorer
}

public struct ResourceStruct
{
    public int Food, Wood, Ore, BonusFood, BonusWood, BonusOre, LuxuryResource;
    public int FoodProductionRate, WoodProductionRate, OreProductionRate, BonusFoodProductionRate, BonusWoodProductionRate, BonusOreProductionRate, LuxuryResourceProductionRate;
    public int FoodConsumptionRate, WoodConsumptionRate, OreConsumptionRate, BonusFoodConsumptionRate, BonusWoodConsumptionRate, BonusOreConsumptionRate, LuxuryResourceConsumptionRate;
    public float FoodPriority, WoodPriority, OrePriority, BonusFoodPriority, BonusWoodPriority, BonusOrePriority, LuxuryResourcePriority;

    public ResourceStruct(
        int foodTotal, int woodTotal, int oreTotal, int bonusFoodTotal, int bonusWoodTotal, int bonusOreTotal, int luxuryResourceTotal)
    {
        Food = foodTotal;
        Wood = woodTotal;
        Ore = oreTotal;
        BonusFood = bonusFoodTotal;
        BonusWood = bonusWoodTotal;
        BonusOre = bonusOreTotal;
        LuxuryResource = luxuryResourceTotal;

        FoodProductionRate = 0;
        WoodProductionRate = 0;
        OreProductionRate = 0;
        BonusFoodProductionRate = 0;
        BonusWoodProductionRate = 0;
        BonusOreProductionRate = 0;
        LuxuryResourceProductionRate = 0;

        FoodConsumptionRate = 0;
        WoodConsumptionRate = 0;
        OreConsumptionRate = 0;
        BonusFoodConsumptionRate = 0;
        BonusWoodConsumptionRate = 0;
        BonusOreConsumptionRate = 0;
        LuxuryResourceConsumptionRate = 0;

        FoodPriority = 0;
        WoodPriority = 0;
        OrePriority = 0;
        BonusFoodPriority = 0;
        BonusWoodPriority = 0;
        BonusOrePriority = 0;
        LuxuryResourcePriority = 0;
    }
    public ResourceStruct SetAllResourcePriorities(float foodPriority, float woodPriority, float orePriority, float bonusFoodPriority, float bonusWoodPriority, float bonusOrePriority, float luxuryResourcePriority)
    {
        // Create a new instance of ResourceManagement with the current values
        ResourceStruct updated = this;

        // Update priorities
        updated.FoodPriority = foodPriority;
        updated.WoodPriority = woodPriority;
        updated.OrePriority = orePriority;
        updated.BonusFoodPriority = bonusFoodPriority;
        updated.BonusWoodPriority = bonusWoodPriority;
        updated.BonusOrePriority = bonusOrePriority;
        updated.LuxuryResourcePriority = luxuryResourcePriority;

        // Return the updated struct
        return updated;
    }

    // Method to adjust the priority of a single resource
    public void AdjustResourcePriority(GameResource resource, float newPriority)
    {
        switch (resource)
        {
            case GameResource.MainFood:
                FoodPriority = newPriority;
                break;
            case GameResource.MainWood:
                WoodPriority = newPriority;
                break;
            case GameResource.MainOre:
                OrePriority = newPriority;
                break;
            case GameResource.BonusFood:
                BonusFoodPriority = newPriority;
                break;
            case GameResource.BonusWood:
                BonusWoodPriority = newPriority;
                break;
            case GameResource.BonusOre:
                BonusOrePriority = newPriority;
                break;
            case GameResource.LuxuryResource:
                LuxuryResourcePriority = newPriority;
                break;
                // Add cases for other resources as necessary
        }
    }

    public ResourceStruct SetAllResourceProductionRates(int foodProduction, int woodProduction, int oreProduction, int bonusFoodProduction, int bonusWoodProduction, int bonusOreProduction, int luxuryResourceProduction)
    {
        // Create a new instance of ResourceManagement with the current values
        ResourceStruct updated = this;

        // Update production rates
        updated.FoodProductionRate = foodProduction;
        updated.WoodProductionRate = woodProduction;
        updated.OreProductionRate = oreProduction;
        updated.BonusFoodProductionRate = bonusFoodProduction;
        updated.BonusWoodProductionRate = bonusWoodProduction;
        updated.BonusOreProductionRate = bonusOreProduction;
        updated.LuxuryResourceProductionRate = luxuryResourceProduction;

        // Return the updated struct
        return updated;
    }

    public void AddResource()
    {
        this.Food += this.FoodProductionRate;
        this.Wood += this.WoodProductionRate;
        this.Ore += this.OreProductionRate;
        this.BonusFood += this.BonusFoodProductionRate;
        this.BonusWood += this.BonusWoodProductionRate;
        this.BonusOre += this.BonusOreProductionRate;
        this.LuxuryResource += this.LuxuryResourceProductionRate;
    }
    public void ConsumeResource()
    {
        Food = Math.Max(0, Food - FoodConsumptionRate);
        Wood = Math.Max(0, Wood - WoodConsumptionRate);
        Ore = Math.Max(0, Ore - OreConsumptionRate);
        BonusFood = Math.Max(0, BonusFood - BonusFoodConsumptionRate);
        BonusWood = Math.Max(0, BonusWood - BonusWoodConsumptionRate);
        BonusOre = Math.Max(0, BonusOre - BonusOreConsumptionRate);
        LuxuryResource = Math.Max(0, LuxuryResource - LuxuryResourceConsumptionRate);
    }
}


public class Classes : MonoBehaviour
{

}
