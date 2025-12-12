using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using static RacePreference;

public class RacePreferenceManager: MonoBehaviour
{
    private static List<RaceLandPreference> availableLandPreferences = Enum.GetValues(typeof(RaceLandPreference)).Cast<RaceLandPreference>().ToList();

    public RaceLandPreference GetRandomLandPreference()
    {
        // Reset the list if all preferences have been used
        if (availableLandPreferences.Count == 0)
        {
            availableLandPreferences = Enum.GetValues(typeof(RaceLandPreference)).Cast<RaceLandPreference>().ToList();
            Debug.Log("Resetting available land preferences.");
        }

        // Randomly select from the available preferences
        int landIndex = UnityEngine.Random.Range(0, availableLandPreferences.Count);
        RaceLandPreference landPref = availableLandPreferences[landIndex];
        availableLandPreferences.RemoveAt(landIndex); // Remove the selected preference

        // Debug to show which preference was selected
        //Debug.Log($"Selected Land Preference: {landPref}");

        return landPref;
    }
}
