using UnityEngine;

public class MapGenerationCamera : MonoBehaviour
{
    public Camera mapGenerationCamera;

    void Start()
    {
        if (mapGenerationCamera == null)
        {
            Debug.LogError("MapGenerationCamera is not assigned!");
            return;
        }

        AdjustCameraToMapSize(GameManager.Instance.mapSize);
    }

    public void AdjustCameraToMapSize(int mapSize)
    {
        if (mapSize <= 0)
        {
            Debug.LogError("Invalid map size!");
            return;
        }

        // Calculate the new position and size
        float newSize = mapSize / 2f;
        Vector3 newPosition = new Vector3(newSize, newSize, mapGenerationCamera.transform.position.z);

        // Apply the new position and size to the camera
        mapGenerationCamera.transform.position = newPosition;
        mapGenerationCamera.orthographicSize = newSize;
    }

    // Optionally, you can have a method to call when mapSize changes
    public void OnMapSizeChanged()
    {
        AdjustCameraToMapSize(GameManager.Instance.mapSize);
    }
}
