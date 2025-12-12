using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceInfoPanel : MonoBehaviour
{
    public GameObject raceInfoButtonPrefab;
    public GameObject backButtonPrefab;
    public GameObject raceInformationPanel;
    public MenuManager menuManager;
    public RaceDataHolder raceDataHolder;
    private bool buttonsCreated = false;

    public void CreateRaceButtons()
    {
        if (!buttonsCreated)
        {
            foreach (KeyValuePair<string, RaceProperties> raceEntry in raceDataHolder.GetRaceDictionary())
            {
                RaceProperties race = raceEntry.Value;
                GameObject newButton = Instantiate(raceInfoButtonPrefab, raceInformationPanel.transform);
                newButton.GetComponentInChildren<Text>().text = race.RaceName;

                // Assign the RaceProperties object to the RaceButtons script component
                RaceButtons raceButtonsComponent = newButton.GetComponent<RaceButtons>();
                if (raceButtonsComponent != null)
                {
                    raceButtonsComponent.raceProperties = race;
                }
                else
                {
                    Debug.LogError("Failed to find RaceButtons component on the instantiated prefab");
                }

                // Add an onClick event to the button that calls OnRaceButtonClicked with the corresponding RaceProperties data
                newButton.GetComponent<Button>().onClick.AddListener(() => OnRaceButtonClicked(race));
            }
            buttonsCreated = true;
        }
    }

    private void OnRaceButtonClicked(RaceProperties race)
    {
        menuManager.ShowPanel(raceInformationPanel);
        Debug.Log("Race button clicked: " + race.RaceName);
    }
}