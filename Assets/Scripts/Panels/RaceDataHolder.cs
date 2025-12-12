using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RaceDataHolder : MonoBehaviour
{
    [SerializeField]
    public Dictionary<string, RaceProperties> _raceDictionary;
    public List<RaceManager> raceManagersList = new List<RaceManager>();
    public int currentRaceManagerIndex { get; set; }

    public Dictionary<string, RaceProperties> RaceDictionary
    {
        get => _raceDictionary;
        set => _raceDictionary = value;
    }
    public Dictionary<string, RaceProperties> GetRaceDictionary()
    {
        return _raceDictionary;
    }

    public void SetRaceDictionary(Dictionary<string, RaceProperties> dictionary)
    {
        _raceDictionary = dictionary;
    }

    public RaceManager GetCurrentRaceManager()
    {
        // Check if the list is null or empty
        if (raceManagersList == null || raceManagersList.Count == 0)
        {
            Debug.LogError("GetCurrentRaceManager: raceManagersList is null or empty.");
            return null; // or return a default value or handle this case as needed
        }

        // Check if the current index is within the valid range
        if (currentRaceManagerIndex < 0 || currentRaceManagerIndex >= raceManagersList.Count)
        {
            Debug.LogError($"GetCurrentRaceManager: Index out of range. Index: {currentRaceManagerIndex}, List Count: {raceManagersList.Count}");
            return null; // or return a default value or handle this case as needed
        }

        // Optionally, log the current RaceManager being returned for debugging
        //Debug.Log($"Returning RaceManager at index {currentRaceManagerIndex}: {raceManagersList[currentRaceManagerIndex].name}"); // Assuming RaceManager has a name property for identification

        return raceManagersList[currentRaceManagerIndex];
    }
    public RaceManager GetRaceManagerByProperties(RaceProperties raceProperties)
    {
        foreach (RaceManager raceManager in raceManagersList)
        {
            if (AreRacePropertiesEqual(raceManager.raceProperties, raceProperties))
            {
                return raceManager;
            }
        }

        Debug.LogWarning($"Could not find RaceManager for RaceProperties with RaceName: {raceProperties.RaceName}");
        Debug.Log("Available RaceManagers:");
        foreach (RaceManager raceManager in raceManagersList)
        {
            Debug.Log($"RaceManager with RaceProperties.RaceName: {raceManager.raceProperties.RaceName}");
        }

        return null;
    }

    private bool AreRacePropertiesEqual(RaceProperties a, RaceProperties b)
    {
        return a.RaceName == b.RaceName;
    }

    public void LogRaceManagers()
    {
        //Debug.Log("RaceDataHolder initialized with " + raceManagersList.Count + " RaceManagers");

        foreach (RaceManager rm in raceManagersList)
        {
            if (Equals(rm.raceProperties, null))
            {
                Debug.LogWarning("RaceManager without RaceProperties found");
            }
            else
            {
                //Debug.Log("RaceManager with RaceProperties: " + rm.raceProperties.RaceName);
            }
        }
    }
}