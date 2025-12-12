using UnityEngine;
using UnityEngine.UI;

public class RaceButtons : MonoBehaviour
{
    public RaceProperties raceProperties;
    //public GameObject raceDisplayOptionPanel;
    public MenuManager menuManager;
    public RaceDataHolder raceDataHolder;
    public GameTextDisplay gameTextDisplay;


    void Start()
    {
        raceDataHolder = FindObjectOfType<RaceDataHolder>();

        // Get the Button component attached to this game object
        Button button = GetComponent<Button>();

        // Add an onClick event to the button that calls the OnButtonClick method
        button.onClick.AddListener(OnButtonClick);

        // Find the MenuManager object in the scene and assign it to the menuManager variable
        GameObject menuManagerObject = GameObject.Find("MenuManager");
        menuManager = menuManagerObject.GetComponent<MenuManager>();

        GameObject gameTextDisplayObject = GameObject.Find("ContentText");
        gameTextDisplay = gameTextDisplayObject.GetComponent<GameTextDisplay>();

        //raceDisplayOptionPanel = GameObject.Find("RaceDisplayOptionsPanel");
    }

    public void OnButtonClick()
    {
        // Get the reference to the RaceDisplayOptionsPanel from the MenuManager script
        GameObject raceDisplayOptionsPanel = menuManager.raceDisplayOptionsPanel;
        menuManager.ShowPanel(raceDisplayOptionsPanel);

        RaceManager raceManager = raceDataHolder.GetRaceManagerByProperties(raceProperties);

        int raceManagerIndex = raceDataHolder.raceManagersList.IndexOf(raceManager);
        raceDataHolder.currentRaceManagerIndex = raceManagerIndex; // Add this line
        gameTextDisplay.ShowRaceManagerCharacters(raceManagerIndex);
    }
}