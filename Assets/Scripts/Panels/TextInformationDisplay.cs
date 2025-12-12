using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInformationDisplay : MonoBehaviour
{
    public GameObject textInformationPanel;
    public void ShowTextInformationPanel()
    {
        textInformationPanel.SetActive(true);
    }

    // Call this method from the "Back" button's onClick event
    public void HideTextInformationPanel()
    {
        textInformationPanel.SetActive(false);
    }
}
