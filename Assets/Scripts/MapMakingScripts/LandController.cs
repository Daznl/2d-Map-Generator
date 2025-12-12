using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandController : MonoBehaviour
{
    public LandType landType;
    public int altitude;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateLandProperties();
    }

    public void UpdateLandProperties()
    {
        if (landType != null)
        {
            spriteRenderer.sprite = landType.sprite;
            altitude = landType.altitude;
            // Add any additional property updates here based on the LandType properties
            // For example, if you need to update a custom 'altitude' property:
            // altitude = landType.altitude;
        }
    }
}