using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FMDownloadProgressChecker
{
    private static int totalCheck;
    private static int currentChecked;

    public static void AddCheck(int checkCount)
    {
        totalCheck += checkCount;
    }

    public static void SetAsComplete(int completeCount)
    {
        currentChecked += completeCount;
    }

    public static float GetCheckingProgress()
    {
        float result = (float)currentChecked / totalCheck;
        return result;
    }
}
