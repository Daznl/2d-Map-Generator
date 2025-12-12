using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Include other namespaces if needed

public class MapOptionsController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject mapOptionsPanel;
    public TMP_Dropdown mapSizeDropdown;
    public TMP_InputField riversInputField;
    //public TMP_InputField mountainRangesInputField;
    //public Slider landPercentageSlider;
    public Slider territoriesSlider;
    //public CreateStuff createStuffScript;

    public Camera mainGameCamera;
    public Camera mapGenerationCamera;
    public MapGenerationCamera mapGenerationCameraScript;
    public MapArrayScript mapArrayScript;

    private Dictionary<string, int> mapSizeOptions = new Dictionary<string, int>
    {
        {"50x50", 50},
        {"100x100", 100},
        {"150x150", 150},
        {"200x200", 200},
        {"250x250", 250},
        {"300x300", 300},
        {"Danger(500)", 500},
    };

    // References to UI elements like Dropdowns, Sliders, InputFields, et
    // 
    // public Dropdown exampleDropdown;
    // public Slider exampleSlider;
    // ... and so on

    // Call this method when the "Map Options" button is clicked
    public void ShowMapOptionsPanel()
    {
        mapOptionsPanel.SetActive(true);
    }

    // Call this method from the "Back" button's onClick event
    public void HideMapOptionsPanel()
    {
        mapOptionsPanel.SetActive(false);
    }

    // Call this method from the "Generate New Map" button's onClick event
    public void GenerateNewMap()
    {
        // Read values from UI elements
         string selectedOption = mapSizeDropdown.options[mapSizeDropdown.value].text;

         if (mapSizeOptions.TryGetValue(selectedOption, out int selectedMapSize))
         {
             GameManager.Instance.mapSize = selectedMapSize;
             mapArrayScript.CreateArrays();
         }
         else
         {
             Debug.LogError("Selected map size not found in dictionary.");
         }

         
         int territories = (int)territoriesSlider.value;
/*         if (int.TryParse(riversInputField.text, out int numberOfRivers))
         {
             GameManager.Instance.numberOfRivers = numberOfRivers;
         }
         else
         {
             Debug.LogError("Invalid input for number of rivers.");
             return; // Prevent further execution if input is invalid
         }*/

        GameManager.Instance.numberOfTerritories = territories;

        //float landPercentage = landPercentageSlider.value;
        //createStuffScript.percentageOfLand = landPercentage;
        //createStuffScript.numberOfMountainRanges = numberOfMountainRanges;

        // Trigger map generation
        StartCoroutine(gameManager.createStuff.GenerateMap());
        mainGameCamera.gameObject.SetActive(false);
        mapGenerationCamera.gameObject.SetActive(true);

        if (mapGenerationCameraScript != null)
        {
            mapGenerationCameraScript.AdjustCameraToMapSize(GameManager.Instance.mapSize);
        }

    }
}
