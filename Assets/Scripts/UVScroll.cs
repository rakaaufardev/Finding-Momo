using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeedX;
    [SerializeField] private float scrollSpeedY;

    [SerializeField] private Renderer rend;
    private Vector2 offset;

    private void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        rend.sharedMaterial.SetTextureOffset("_BaseMap", offset);
    }

    private void OnDisable()
    {
        rend.sharedMaterial.SetTextureOffset("_BaseMap", Vector2.zero);
    }
}
