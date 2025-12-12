using System;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    // Singleton instance
    public static EventSystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Make EventSystem persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Define an event for map updates
    public event Action OnMapUpdate;

    // Method to trigger the map update event
    public void TriggerMapUpdate()
    {
        OnMapUpdate?.Invoke();
    }

    // Additional events can be added here following the same pattern
}