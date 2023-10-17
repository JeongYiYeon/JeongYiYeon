using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngineInternal;

#if BUILD_MODE
public static class Debug
{
    public static void Log(object message)
    {
    }

    public static void Log(object message, UnityEngine.Object context)
    {
    }

    public static void LogError(object message)
    {
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
    }

    public static void LogWarning(object message)
    {
    }

    public static void LogWarning(object message, UnityEngine.Object context)
    {
    }

    public static void LogFormat(object message, params object[] args)
    {
    }

    public static void LogFormat(object message, string format, params object[] args)
    {
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
    }
    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
    }

    public static void Assert(bool condition)
    {
        if (!condition) throw new Exception();
    }
}
#endif