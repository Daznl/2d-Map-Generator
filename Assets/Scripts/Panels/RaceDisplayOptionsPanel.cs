using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceDisplayOptionsPanel : MonoBehaviour
{
    public RaceDataHolder raceDataHolder;
    public GameTextDisplay gameTextDisplay;
    public Button printCharacterListButton;
    public Button randomCharacterDetailsButton;

    void Start()
    {
        raceDataHolder = FindObjectOfType<RaceDataHolder>();

        GameObject gameTextDisplayObject = GameObject.Find("ContentText");
        gameTextDisplay = gameTextDisplayObject.GetComponent<GameTextDisplay>();

        // Update onClick events for both buttons
        printCharacterListButton.onClick.AddListener(() => {
            gameTextDisplay.lastActionPerformed = GameTextDisplay.LastActionPerformed.ShowCharacterList;
            gameTextDisplay.DisplayCharacterList();
        });

        randomCharacterDetailsButton.onClick.AddListener(() => {
            gameTextDisplay.lastActionPerformed = GameTextDisplay.LastActionPerformed.ShowRandomCharacterDetails;
            gameTextDisplay.ShowRandomCharacterDetails();
        });
    }
}
