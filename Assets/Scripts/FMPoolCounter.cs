using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMPoolCounter
{
    private int currentIndex;

    public void Next()
    {
        currentIndex++;
    }

    public void Reset()
    {
        currentIndex = 0;
    }

    public int GetIndex()
    {
        return currentIndex;
    }

    public bool IsEndPool(int poolCount)
    {
        bool result = currentIndex >= poolCount;
        return result;
    }
}
