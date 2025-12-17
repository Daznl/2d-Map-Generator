using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this line
using static GameManager;
using static MapArrayScript;

public class MapRenderer : MonoBehaviour
{
    public GameManager gameManager;
    public SpriteMaker spriteMaker;
    public MapArrayScript M;
    public RiverSpriteManager riverSpriteManager;

    public Image staticMapImage;
    public Image displayImage;

    // Sprites for each block type and resources
    public Sprite deepWaterSprite;
    public Sprite waterSprite;
    public Sprite shallowWaterSprite;
    public Sprite lowlandSprite;
    public Sprite sandSprite;
    public Sprite landSprite;
    public Sprite hillSprite;
    public Sprite highlandSprite;
    public Sprite mountainSprite;
    public Sprite peakSprite;
    public Sprite SwampSprite;
    public Sprite SwampLowlandSprite;

    // ... other sprites for different block types
    public Sprite foodResourceSprite;
    public Sprite bonusFoodResourceSprite;
    public Sprite woodResourceSprite;
    public Sprite bonusWoodResourceSprite;
    public Sprite mainOreResourceSprite;
    public Sprite bonusOreResourceSprite;
    public Sprite luxuryResourceSprite;

    // ... other sprites for different resources

    public void RenderMap()
    {
        MapArrayScript.Block[,] gameMapBlocks = gameManager.gameMapBlocks;

        for (int x = 0; x < gameMapBlocks.GetLength(0); x++)
        {
            for (int y = 0; y < gameMapBlocks.GetLength(1); y++)
            {
                MapArrayScript.Block currentBlock = gameMapBlocks[x, y];
                Sprite baseSprite = GetBaseSprite(currentBlock.blockType);
                Sprite[] resourceSprites = GetResourceSprites(currentBlock);

                // Merge land and resource sprites first
                Sprite combinedSprite = (resourceSprites.Length > 0) ?
                    spriteMaker.MergeSprites(baseSprite, resourceSprites) : baseSprite;

                // If there is a river, get the river sprite and merge it
                if (currentBlock.river.isRiver)
                {
                    Sprite riverSprite = riverSpriteManager.DetermineSprite(currentBlock.river);
                    combinedSprite = spriteMaker.MergeSprites(combinedSprite, new Sprite[] { riverSprite });
                }

                // Set the final sprite for the block
                currentBlock.blockSprite = combinedSprite;

            }
        }
    }

    private Sprite GetBaseSprite(MapArrayScript.Blocktype blockType)
    {
        switch (blockType)
        {
            case MapArrayScript.Blocktype.Deepwater: return deepWaterSprite;
            case MapArrayScript.Blocktype.Water: return waterSprite;
            case MapArrayScript.Blocktype.Shallowwater: return shallowWaterSprite;
            case MapArrayScript.Blocktype.Lowland: return lowlandSprite;
            case MapArrayScript.Blocktype.Sand: return sandSprite;
            case MapArrayScript.Blocktype.Land: return landSprite;
            case MapArrayScript.Blocktype.Hill: return hillSprite;
            case MapArrayScript.Blocktype.Highland: return highlandSprite;
            case MapArrayScript.Blocktype.Mountain: return mountainSprite;
            case MapArrayScript.Blocktype.Peak: return peakSprite;
            case MapArrayScript.Blocktype.Swamp: return SwampSprite;
            case MapArrayScript.Blocktype.Swamplowland: return SwampLowlandSprite;
            default: return null; // Or some default sprite
        }
    }

    private Sprite[] GetResourceSprites(MapArrayScript.Block block)
    {
        // Example: Let's say you have food and wood as resources
        // You can add more cases for other resources
        var resourceSprites = new System.Collections.Generic.List<Sprite>();
        if (block.hasFood) resourceSprites.Add(foodResourceSprite);
        if (block.hasBonusFood) resourceSprites.Add(bonusFoodResourceSprite);
        if (block.hasWood) resourceSprites.Add(woodResourceSprite);
        if (block.hasBonusWood) resourceSprites.Add(bonusWoodResourceSprite);
        if (block.hasMainOre) resourceSprites.Add(mainOreResourceSprite);
        if (block.hasBonusOre) resourceSprites.Add(bonusOreResourceSprite);
        if (block.hasLuxuryResource) resourceSprites.Add(luxuryResourceSprite);

        return resourceSprites.ToArray();
    }

    public void GenerateFullMapImage()
    {
        int numberOfPixels = GameManager.Instance.mapSize * 20;
        int mapSize = GameManager.Instance.mapSize;
        MapArrayScript.Block[,] gameMapBlocks = gameManager.gameMapBlocks;

        // Create a new texture for the combined image
        Texture2D combinedTexture = new Texture2D(numberOfPixels, numberOfPixels);
        combinedTexture.filterMode = FilterMode.Point; // Set filter mode to Point to avoid blurring

        Debug.Log("gameMapBlocks dimensions: " + gameMapBlocks.GetLength(0) + "x" + gameMapBlocks.GetLength(1));
        //Debug.Log("GameManager.Instance.mapSize: " + GameManager.Instance.mapSize);

        for (int x = 0; x < mapSize; x++) // Assuming the size of gameMapBlocks is 300x300
        {
            for (int y = 0; y < mapSize; y++)
            {
                MapArrayScript.Block currentBlock = gameMapBlocks[x, y];

                // Assuming each block has a sprite assigned
                if (currentBlock.blockSprite != null)
                {
                    Color[] pixels = currentBlock.blockSprite.texture.GetPixels(
                        (int)currentBlock.blockSprite.rect.x,
                        (int)currentBlock.blockSprite.rect.y,
                        20, 20);
                    combinedTexture.SetPixels(x * 20, y * 20, 20, 20, pixels);
                }
            }
        }

        combinedTexture.Apply();

        // Convert the Texture2D to a Sprite and assign it to your image
        Sprite combinedSprite = Sprite.Create(combinedTexture, new Rect(0, 0, numberOfPixels, numberOfPixels), new Vector2(0.5f, 0.5f), 20);
        staticMapImage.sprite = combinedSprite;
        displayImage.sprite = combinedSprite;
    }

    //public void OverlayBuildings(Texture2D mapTexture)
    //{
    //    BuildingInfo[,] mapBuildings = gameManager.mapBuildings; // Reference to your building info array
    //    int buildingPixelSize = 20; // Assuming each building occupies a 20x20 pixel space

    //    for (int x = 0; x < mapBuildings.GetLength(0); x++)
    //    {
    //        for (int y = 0; y < mapBuildings.GetLength(1); y++)
    //        {
    //            BuildingInfo building = mapBuildings[x, y];
    //            if (building != null && building.BuildingSprite != null)
    //            {
    //                Color[] pixels = building.BuildingSprite.texture.GetPixels(
    //                    (int)building.BuildingSprite.rect.x,
    //                    (int)building.BuildingSprite.rect.y,
    //                    buildingPixelSize, buildingPixelSize);
    //                mapTexture.SetPixels(x * buildingPixelSize, y * buildingPixelSize, buildingPixelSize, buildingPixelSize, pixels);
    //            }
    //        }
    //    }

    //    mapTexture.Apply(); // Apply changes to the texture
    //}

}
