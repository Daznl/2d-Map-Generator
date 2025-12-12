using UnityEngine;

public class SpriteMaker : MonoBehaviour
{
    public Sprite MergeSprites(Sprite baseSprite, Sprite[] resourceSprites)
    {
        // Ensure base sprite is valid
        if (baseSprite == null)
        {
            Debug.LogError("Base sprite is null");
            return null;
        }

        // Initialize merged texture with base sprite's dimensions and pixels
        Texture2D mergedTexture = new Texture2D((int)baseSprite.rect.width, (int)baseSprite.rect.height);
        Color[] mergedPixels = baseSprite.texture.GetPixels(
            (int)baseSprite.textureRect.x,
            (int)baseSprite.textureRect.y,
            (int)baseSprite.textureRect.width,
            (int)baseSprite.textureRect.height);
        mergedTexture.SetPixels(mergedPixels);

        // Loop through each resource sprite and merge
        foreach (var resourceSprite in resourceSprites)
        {
            // Ensure resource sprite is valid
            if (resourceSprite == null)
            {
                Debug.LogWarning("Resource sprite is null, skipping");
                continue;
            }

            // Get pixels from resource sprite
            Color[] resourcePixels = resourceSprite.texture.GetPixels(
                (int)resourceSprite.textureRect.x,
                (int)resourceSprite.textureRect.y,
                (int)resourceSprite.textureRect.width,
                (int)resourceSprite.textureRect.height);

            // Merge pixels
            for (int i = 0; i < resourcePixels.Length && i < mergedPixels.Length; i++)
            {
                Color baseColor = mergedPixels[i];
                Color resourceColor = resourcePixels[i];
                float alpha = resourceColor.a;
                mergedPixels[i] = new Color(
                    baseColor.r * (1 - alpha) + resourceColor.r * alpha,
                    baseColor.g * (1 - alpha) + resourceColor.g * alpha,
                    baseColor.b * (1 - alpha) + resourceColor.b * alpha,
                    Mathf.Max(baseColor.a, resourceColor.a));
            }

            // Update merged texture
            mergedTexture.SetPixels(mergedPixels);
            mergedTexture.Apply();
        }

        // Create and return new sprite
        return Sprite.Create(mergedTexture, new Rect(0, 0, mergedTexture.width, mergedTexture.height), new Vector2(0.5f, 0.5f));
    }
}