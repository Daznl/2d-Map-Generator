using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this line

public class MiniMapNavigation : MonoBehaviour
{
    public MapRenderer mapRenderer; // Reference to MapRenderer
    public Image miniMapDisplayImage; // Mini-map display image
    public GameManager gameManager;

    private int miniMapSize = 20; // Size of the mini-map (30x30)
    private Vector2Int currentMiniMapStart = Vector2Int.zero; // Starting coordinate of the mini-map section
    private float keyHoldTime = 0.05f; // Time interval for movement steps
    private float timeSinceLastMove = 0f; // Timer to track time since last mini-map move

    private const int MinMapSize = 5;
    private const int MaxMapSize = 50;

    //int minimapSize = 20;

    void Update()
    {
        Vector2Int direction = Vector2Int.zero;

        // Check arrow key inputs
        if (Input.GetKey(KeyCode.A)) direction += new Vector2Int(-1, 0);
        if (Input.GetKey(KeyCode.D)) direction += new Vector2Int(1, 0);
        if (Input.GetKey(KeyCode.W)) direction += new Vector2Int(0, 1);
        if (Input.GetKey(KeyCode.S)) direction += new Vector2Int(0, -1);

        // Update timer and move mini-map if necessary
        if (direction != Vector2Int.zero)
        {
            timeSinceLastMove += Time.deltaTime;
            if (timeSinceLastMove >= keyHoldTime)
            {
                MoveMiniMap(direction);
                timeSinceLastMove = 0f; // Reset timer after moving
            }
        }
        else
        {
            timeSinceLastMove = keyHoldTime; // Reset timer if no key is held
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Key for zooming in
        {
            ZoomIn();
        }
        if (Input.GetKeyDown(KeyCode.E)) // Key for zooming out
        {
            ZoomOut();
        }
    }

    private void MoveMiniMap(Vector2Int direction)
    {
        int mapSize = GameManager.Instance.mapSize;

        // Update the starting coordinate of the mini-map section
        currentMiniMapStart += direction;
        ClampMiniMapStart(); // Adjust the start position if needed

        // Generate and display the mini-map
        GenerateMiniMap();
    }

    private void ZoomIn()
    {
        if (miniMapSize > MinMapSize)
        {
            miniMapSize /= 2; // Halve the mini-map size
            ClampMiniMapStart(); // Adjust the start position if needed
            GenerateMiniMap();
        }
    }

    private void ZoomOut()
    {
        if (miniMapSize < MaxMapSize && miniMapSize < GameManager.Instance.mapSize)
        {
            miniMapSize = Mathf.Min(miniMapSize * 2, GameManager.Instance.mapSize); // Double the mini-map size
            ClampMiniMapStart(); // Adjust the start position if needed
            GenerateMiniMap();
        }
    }

    private void ClampMiniMapStart()
    {
        int mapSize = GameManager.Instance.mapSize;
        int offset = miniMapSize / 2;
        int max_start_x = mapSize - offset;
        int max_start_y = mapSize - offset;

        // Ensure currentMiniMapStart doesn't exceed the maximum starting point
        currentMiniMapStart = Vector2Int.Max(currentMiniMapStart, new Vector2Int(offset, offset));
        currentMiniMapStart = Vector2Int.Min(currentMiniMapStart, new Vector2Int(max_start_x, max_start_y));
    }

    public void GenerateMiniMap()
    {
        int pixelSize = 20; // Assuming each block is 20x20 pixels
        Texture2D miniMapTexture = new Texture2D(miniMapSize * pixelSize, miniMapSize * pixelSize);
        miniMapTexture.filterMode = FilterMode.Point;

        // Calculate the offset to keep the mini-map centered
        int offset = miniMapSize / 2; // For odd sizes, it will automatically floor the division

        for (int x = 0; x < miniMapSize; x++)
        {
            for (int y = 0; y < miniMapSize; y++)
            {
                int worldX = currentMiniMapStart.x + x - offset;
                int worldY = currentMiniMapStart.y + y - offset;

                // Check bounds to ensure it doesn't go outside the main map
                if (worldX >= 0 && worldX < gameManager.gameMapBlocks.GetLength(0) &&
                    worldY >= 0 && worldY < gameManager.gameMapBlocks.GetLength(1))
                {
                    // First, draw the terrain block
                    MapArrayScript.Block block = gameManager.gameMapBlocks[worldX, worldY];
                    if (block.blockSprite != null)
                    {
                        Color[] pixels = block.blockSprite.texture.GetPixels(
                            (int)block.blockSprite.rect.x,
                            (int)block.blockSprite.rect.y,
                            pixelSize, pixelSize);
                        miniMapTexture.SetPixels(x * pixelSize, y * pixelSize, pixelSize, pixelSize, pixels);
                    }

                    // Then, overlay the building if there is one
                    //if (gameManager.mapBuildings != null && worldX < gameManager.mapBuildings.GetLength(0) && worldY < gameManager.mapBuildings.GetLength(1))
                    //{
                    //    GameManager.BuildingInfo building = gameManager.mapBuildings[worldX, worldY];
                    //    if (building != null && building.BuildingSprite != null)
                    //    {
                    //        // Ensure the building sprite's texture is readable
                    //        if (building.BuildingSprite.texture.isReadable)
                    //        {
                    //            Color[] buildingPixels = building.BuildingSprite.texture.GetPixels(
                    //                (int)building.BuildingSprite.textureRect.x,
                    //                (int)building.BuildingSprite.textureRect.y,
                    //                pixelSize, pixelSize);
                    //            miniMapTexture.SetPixels(x * pixelSize, y * pixelSize, pixelSize, pixelSize, buildingPixels);
                    //        }
                    //        else
                    //        {
                    //            Debug.LogWarning("Building sprite texture is not readable. Make sure 'Read/Write Enabled' is checked in the texture import settings.");
                    //        }
                    //    }
                    //}
                }
            }
        }

        miniMapTexture.Apply();
        Sprite miniMapSprite = Sprite.Create(miniMapTexture, new Rect(0, 0, miniMapTexture.width, miniMapTexture.height), new Vector2(0.5f, 0.5f));
        miniMapDisplayImage.sprite = miniMapSprite;
    }

    /*public void GenerateMiniMap()
    {
        int pixelSize = 20; // Assuming each block is 20x20 pixels
        Texture2D miniMapTexture = new Texture2D(miniMapSize * pixelSize, miniMapSize * pixelSize);
        miniMapTexture.filterMode = FilterMode.Point;

        // Calculate the offset to keep the mini-map centered
        int offset = miniMapSize / 2; // For odd sizes, it will automatically floor the division

        for (int x = 0; x < miniMapSize; x++)
        {
            for (int y = 0; y < miniMapSize; y++)
            {
                int worldX = currentMiniMapStart.x + x - offset;
                int worldY = currentMiniMapStart.y + y - offset;

                // Check bounds to ensure it doesn't go outside the main map
                if (worldX >= 0 && worldX < gameManager.gameMapBlocks.GetLength(0) &&
                    worldY >= 0 && worldY < gameManager.gameMapBlocks.GetLength(1))
                {
                    MapArrayScript.Block block = gameManager.gameMapBlocks[worldX, worldY];
                    if (block.blockSprite != null)
                    {
                        Color[] pixels = block.blockSprite.texture.GetPixels(
                            (int)block.blockSprite.rect.x,
                            (int)block.blockSprite.rect.y,
                            pixelSize, pixelSize);
                        miniMapTexture.SetPixels(x * pixelSize, y * pixelSize, pixelSize, pixelSize, pixels);
                    }
                }
            }
        }

        miniMapTexture.Apply();
        Sprite miniMapSprite = Sprite.Create(miniMapTexture, new Rect(0, 0, miniMapTexture.width, miniMapTexture.height), new Vector2(0.5f, 0.5f));
        miniMapDisplayImage.sprite = miniMapSprite;
    }*/
}
