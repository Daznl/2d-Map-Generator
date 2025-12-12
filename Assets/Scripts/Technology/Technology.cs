using System.Collections.Generic;
using UnityEngine;

public class Technology : MonoBehaviour
{
    public string technologyName;
    public float progress = 0f; // Progress towards unlocking
    public float demand = 0f; // Demand for this technology
    public List<Technology> prerequisites = new List<Technology>(); // Prerequisites
    public bool unlocked = false;

    void Start()
    {
        // Initialization if needed
    }

    // Check if all prerequisites are unlocked
    public bool CheckPrerequisites()
    {
        foreach (Technology tech in prerequisites)
        {
            if (!tech.unlocked)
            {
                return false;
            }
        }
        return true;
    }

    // Update the progress towards unlocking this technology
    public void UpdateProgress(float amount)
    {
        if (!unlocked && CheckPrerequisites())
        {
            progress += amount;
            if (progress >= 100f)
            {
                progress = 100f;
                Unlock();
            }
        }
    }

    // Unlock this technology and apply its effects
    public void Unlock()
    {
        unlocked = true;
        ApplyEffects();
    }

    // Override this method in subclasses to implement the effects of unlocking the technology
    protected virtual void ApplyEffects()
    {
        Debug.Log(technologyName + " technology unlocked!");
    }
}
