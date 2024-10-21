using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        // Returns number of rows
        int width = noiseMap.GetLength(0);
        // Returns number of columns
        int height = noiseMap.GetLength(1);


        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // y * width gives us the index of the row we are on and adding x gives us column
                // Noise map at the end applies a percentage between 0 and 1 between black and white
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        // Shared materials allows us to look at map in editor so we don't have to be in play mode 
        textureRenderer.sharedMaterial.mainTexture = texture;

        // Sets size of plane to size of map
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}
