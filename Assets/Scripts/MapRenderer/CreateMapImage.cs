using UnityEngine;
using UnityEngine.UI;

public class CreateMapImage : MonoBehaviour
{
    public Camera renderCamera; // Assign this in the inspector
    public Image displayImage; // Assign this in the inspector
    private RenderTexture renderTexture;

    public void CreateMapCameraImage()
    {
        // Adjust the size of the RenderTexture
        int resolution = 6000; // Desired resolution (3000x3000)
        renderTexture = new RenderTexture(resolution, resolution, 24);
        renderCamera.targetTexture = renderTexture;

        // Render the camera's view
        renderCamera.Render();

        // Create a new Texture2D with the same resolution
        Texture2D newTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        newTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        newTexture.Apply();
        RenderTexture.active = null;

        // Convert Texture2D to Sprite
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0.0f, 0.0f, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

        // Assign the sprite to the UI Image
        displayImage.sprite = newSprite;
    }
}
