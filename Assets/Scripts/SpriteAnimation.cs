using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    public int columns = 4; // Number of columns in the sprite sheet
    public int rows = 4;    // Number of rows in the sprite sheet
    public float framesPerSecond = 10f; // Animation speed

    private Renderer quadRenderer;
    private Vector2 textureSize;
    private int totalFrames;
    private int currentFrame;

    void Start()
    {
        // Get the Renderer component from the Quad
        quadRenderer = GetComponent<Renderer>();

        // Calculate the size of each frame based on the number of rows and columns
        textureSize = new Vector2(1f / columns, 1f / rows);

        // Set the initial texture scale (size of one frame)
        /*quadRenderer.material.mainTextureScale = textureSize;*/

        totalFrames = rows * columns;
        currentFrame = 0;
    }

    void Update()
    {
        // Calculate the current frame based on time
        currentFrame = (int)(Time.time * framesPerSecond) % totalFrames;

        // Get the x and y index for the current frame
        int columnIndex = currentFrame % columns;
        int rowIndex = currentFrame / columns;

        // Calculate the texture offset based on the current frame
        Vector2 offset = new Vector2(columnIndex * textureSize.x, 1f - (rowIndex + 1) * textureSize.y);

        // Apply the offset to the material
        quadRenderer.material.mainTextureOffset = offset;
    }
}
