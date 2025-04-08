using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VD
{
    public class VDLogHandler : ILogHandler
    {
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            Console.WriteLine($"{logType}: {string.Format(format, args)}");
        }

        public void LogException(System.Exception exception, UnityEngine.Object context)
        {
            Console.WriteLine($"Exception: {exception}");
        }
    }

    public static class VDLog
    {
        public static void Init()
        {
#if ENABLE_CHEAT
            Debug.unityLogger.logHandler = new VDLogHandler();
#endif
        }

        public static void Log(string message)
        {
#if ENABLE_CHEAT
            Debug.unityLogger.Log(LogType.Log, message);
#endif
        }

        public static void LogError(string message)
        {
#if ENABLE_CHEAT
            Debug.unityLogger.Log(LogType.Error, message);
#endif
        }

        public static void LogWarning(string message)
        {
#if ENABLE_CHEAT
            Debug.unityLogger.Log(LogType.Warning, message);
#endif
        }

        public static void LogAssert(string message)
        {
#if ENABLE_CHEAT
            Debug.unityLogger.Log(LogType.Assert, message);
#endif
        }
    }
}
