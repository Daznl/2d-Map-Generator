using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject raceInformationPanel;
    public GameObject gameFunctionsPanel;
    public GameObject settingsPanel;
    public GameObject importExportPanel;
    public GameObject raceDisplayOptionsPanel;
    public RaceInfoPanel raceInfoPanel;
    public GameObject panelText;

    void Start()
    {
        raceDisplayOptionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        raceInformationPanel.SetActive(false);
        gameFunctionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        importExportPanel.SetActive(false);
    }
    public void ShowPanel(GameObject panelToShow)
    {
        mainMenuPanel.SetActive(false);
        raceInformationPanel.SetActive(false);
        gameFunctionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        importExportPanel.SetActive(false);
        raceDisplayOptionsPanel.SetActive(false);

        panelToShow.SetActive(true);
    }

    public void OnRaceInformationButtonClicked()
    {
        ShowPanel(raceInformationPanel);
        Debug.Log("Race button clicked: ");
        FindObjectOfType<RaceInfoPanel>().CreateRaceButtons();
    }

    public void OnSettingsButtonClicked()
    {
        ShowPanel(settingsPanel);
        // Do something with the race data, for example:
        Debug.Log("Settings button clicked: ");
    }
    public void OnImportExportButtonClicked()
    {
        ShowPanel(importExportPanel);
        // Do something with the race data, for example:
        Debug.Log("Export button clicked: ");
    }
    public void OnQuitGameButtonClicked()
    {
        // Do something with the race data, for example:
        Debug.Log("Quit button clicked: ");
    }
    public void OnGameFunctionsButtonClicked()
    {
        ShowPanel(gameFunctionsPanel);
        // Do something with the race data, for example:
        //Debug.Log("GameFunctions button clicked: ");
    }
}