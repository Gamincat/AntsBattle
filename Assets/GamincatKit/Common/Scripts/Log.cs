using System.Diagnostics;
using UnityEngine;

namespace GamincatKit.Common
{
    public enum LogLevel : byte
    {
        Verbose = 5,
        Debug = 4,
        Info = 3,
        Warn = 2,
        Error = 1,
        Suppress = 0
    }

    public class Log
    {
        public static LogLevel LogLevel { get; set; } = LogLevel.Debug;

        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public static void Verbose(object message)
        {
            if (LogLevel < LogLevel.Verbose) return;

            UnityEngine.Debug.unityLogger.Log(LogType.Log, message);
        }

        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public static void Debug(object message)
        {
            if (LogLevel < LogLevel.Debug) return;

            UnityEngine.Debug.Log(message);
        }

        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public static void Info(object message)
        {
            if (LogLevel < LogLevel.Info) return;

            UnityEngine.Debug.Log(message);
        }

        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public static void Warn(object message)
        {
            if (LogLevel < LogLevel.Warn) return;

            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public static void Error(object message)
        {
            if (LogLevel < LogLevel.Error) return;

            UnityEngine.Debug.LogError(message);
        }
    }
}