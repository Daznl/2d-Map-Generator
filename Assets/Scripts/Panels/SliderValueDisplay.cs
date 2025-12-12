using UnityEngine;
using UnityEngine.UI; // For standard UI Text
using TMPro; // For TextMeshPro

public class SliderValueDisplay : MonoBehaviour
{
    public Slider exampleSlider;
    public TMP_Text valueDisplayText; // For TextMeshPro use TMP_Text, for standard UI use Text

    void Start()
    {
        // Add a listener to the slider's value change event
        exampleSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Method called whenever the slider value changes
    public void ValueChangeCheck()
    {
        // Update the display text to show the slider's value
        valueDisplayText.text = exampleSlider.value.ToString("0"); // Format as needed
    }
}
