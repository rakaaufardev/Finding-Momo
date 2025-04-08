using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public static class VDUtility
{    
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    } 

    public static TKey GetKey<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, int index)
    {
        int count = dictionary.Count;
        TKey[] keys = new TKey[count];
        dictionary.Keys.CopyTo(keys,0);
        TKey result = keys[index];
        return result;
    }

    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int index)
    {
        int count = dictionary.Count;
        TValue[] values = new TValue[count];
        dictionary.Values.CopyTo(values, 0);
        TValue result = values[index];
        return result;
    }

    public static Vector2 WorldToCanvasPosition(Camera camera, Canvas canvas, RectTransform canvasRect, Vector3 worldPosition)
    {
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, out canvasPosition);
        return canvasPosition;
    }
}
