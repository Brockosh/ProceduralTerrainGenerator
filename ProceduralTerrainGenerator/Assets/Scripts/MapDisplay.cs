using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void DrawTexture(Texture2D texture)
    {
        // Shared materials allows us to look at map in editor so we don't have to be in play mode 
        textureRenderer.sharedMaterial.mainTexture = texture;

        // Sets size of plane to size of map
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}
