using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public GameObject panelToGoBackTo;
    public MenuManager menuManager;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnBackButtonPressed);
    }

    public void OnBackButtonPressed()
    {
        menuManager.ShowPanel(panelToGoBackTo);
        Debug.Log("BackButtonPressed");
    }
}
