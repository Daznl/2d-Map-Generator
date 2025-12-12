using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static MapArrayScript;
using static RacePreference;

public struct RaceProperties
{
    public string RaceName;
    public int LifeSpan;
    public int RaceStart;
    public int WorkingAge;
    public int BirthingAge;
    public int MaximumChildAmount;
    public int TimeBeforeCanGiveBirth;
    public int BirthChance;
    public int LinkingChance;
    public int DeLinkingChance;
    public int RandomDeathChance;
    public int BirthStaticChance;
    public int RandomDeathStaticChance;
    public int LinkStaticChance;
    public string Description;
    // New fields for land and resource preferences
    public RacePreference.RaceLandPreference LandPreference;
    //public RacePreference.RaceResourcePreference ResourcePreference;

    //Stats
    public int Stamina;
    public int GatheringRate;
    public int MovementCost;
    public int FoodConsumption;
}

public class CreateRacesFunction : MonoBehaviour
{
    //public RaceRelatedFunctions raceRelatedFunctions;
    public RacePreference racePreference;
    public RacePreferenceManager racePreferenceManager;
    public RaceProperties CreateRace()
    {
        //Debug.Log("Create Races Script");
        RaceProperties properties = new RaceProperties();

        properties.BirthStaticChance = 5000;
        properties.RandomDeathStaticChance = 1000000;
        properties.LinkStaticChance = 20000;

        // Set the race name
        properties.RaceName = TextFileFunctions.GetRandomLineFromTextFile("TextFile/RaceNames");

        // Set random values for the other fields
        properties.LifeSpan = UnityEngine.Random.Range(20, 100);

        properties.RaceStart = 10/*(int)(-(properties.LifeSpan) + 500)*/;
        //Debug.Log("LifeSpan = " + properties.LifeSpan + "RandomRaceStart = " + properties.RaceStart);

        properties.WorkingAge = 20;

        properties.BirthingAge = (int)(0.2f * (properties.LifeSpan + 5));
        //Debug.Log("LifeSpan = " + properties.LifeSpan + "BirthingAge = " + properties.BirthingAge);

        properties.MaximumChildAmount = 3;

        properties.TimeBeforeCanGiveBirth = (int)(0.01f * (properties.LifeSpan));

        properties.BirthChance = (int)(-0.03f * (properties.LifeSpan + 20) + 26);
        //Debug.Log("Birthchance = " + properties.BirthChance);

        properties.LinkingChance = (int)(-0.09f * ((properties.LifeSpan) - 500) + 10);
        //Debug.Log("LifeSpan = " + properties.LifeSpan + "RandomLinkingChance = " + properties.LinkingChance);

        properties.DeLinkingChance = (int)(0.05*(-0.09f * ((properties.LifeSpan) - 500)));

        //between 50 and 0
        properties.RandomDeathChance = (int)(-0.0833f * ((properties.LifeSpan) + 50) + 50);
        //Debug.Log("LifeSpan = " + properties.LifeSpan + "RandomDeathchance = " + properties.RandomDeathChance);

        RaceLandPreference landPref = racePreferenceManager.GetRandomLandPreference();

        properties.LandPreference = landPref;
        /*properties.ResourcePreference = resourcePref;*/

        //Stats
        properties.Stamina = 7;
        properties.GatheringRate = 1;
        properties.MovementCost = 1;
        properties.FoodConsumption = 2;
        

        return properties;
    }
}
