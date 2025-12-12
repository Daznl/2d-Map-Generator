using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class RaceRelatedFunctions : MonoBehaviour
{
    public TimeCounter timeCounter;
    public CreateRacesFunction createRacesFunction;
    //public RaceManagerFunctions raceManagerFunctions;
    private void Awake()
    {
        //raceManagerFunctions = new RaceManagerFunctions();
        timeCounter = FindObjectOfType<TimeCounter>();
    }

    public RaceManager CreateRaceManager(RaceProperties raceProperties, GameObject raceManagerPrefab, Text counterText)
    {
        // Debug.Log("Create Race Manager");
        GameObject raceManagerObj = Instantiate(raceManagerPrefab);
        RaceManager raceManager = raceManagerObj.GetComponent<RaceManager>();
        raceManager.totalDisplay = counterText;
        raceManager.raceProperties = raceProperties;
        return raceManager;
    }
    public Dictionary<string, RaceProperties> CreateRaceDictionary(int numRaces)
    {
        Dictionary<string, RaceProperties> raceDictionary = new Dictionary<string, RaceProperties>();
        //Debug.Log("CreateRaceDictonary");
        for (int i = 1; i <= numRaces; i++)
        {
            RaceProperties race = createRacesFunction.CreateRace();
            raceDictionary.Add("Race" + i, race);
            // Debug.Log("Added race " + i + " to the dictionary: " + race.ToString());
        }

        return raceDictionary;
    }
    public void WriteRaceProperties(Dictionary<string, RaceProperties> raceDictionary, UnityEngine.UI.Text[] raceNameTexts)
    {
        //writes the raceproperties to the text objects
        int i = 0;

        foreach (string raceName in raceDictionary.Keys)
        {
            if (i >= raceNameTexts.Length)
            {
                // Debug.LogError("The raceNameTexts array is not large enough to hold all the race names.");
                break;
            }

            RaceProperties race = raceDictionary[raceName];
            string raceText = string.Format("<color=#000000>RaceName:</color> <color=#FFFFFF>{0}</color>\n<color=#000000>LifeSpan:</color> <color=#FFFFFF>{1}</color>\n<color=#000000>RaceStart:</color> <color=#FFFFFF>{2}</color>\n<color=#000000>BirthingAge:</color> <color=#FFFFFF>{3}</color>\n<color=#000000>MaximumChildAmount:</color> <color=#FFFFFF>{4}</color>\n<color=#000000>TimeBeforeCanGiveBirth:</color> <color=#FFFFFF>{5}</color>\n<color=#000000>BirthChance:</color> <color=#FFFFFF>{6}%</color>\n<color=#000000>LinkingChance:</color> <color=#FFFFFF>{7}%</color>\n<color=#000000>DeLinkingChance:</color> <color=#FFFFFF>{8}%</color>\n<color=#000000>RandomDeathChance:</color> <color=#FFFFFF>{9}%</color>",
            race.RaceName, race.LifeSpan, race.RaceStart, race.BirthingAge, race.MaximumChildAmount, race.TimeBeforeCanGiveBirth, (float)(race.BirthChance / (float)race.BirthStaticChance * 100f), (float)(race.LinkingChance / (float)race.LinkStaticChance * 100f), (float)(race.DeLinkingChance / (float)race.LinkStaticChance * 100f), (float)(race.RandomDeathChance / (float)race.RandomDeathStaticChance * 100f));
            raceNameTexts[i].text = raceText;//this is the array of text objects
            i++;
        }
    }
}
