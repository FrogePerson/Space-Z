using log4net;
using System;
using UnityEngine;


/// Обёртка для логирования
/// использовать только ёё!

public static class Log4NetLogger
{
    public static ILog SetLogger(Type type)
    {
#if UNITY_EDITOR
        return LogManager.GetLogger(type);
#endif
    }

    public static void Log(string txt, ILog ExternLog)
    {
#if UNITY_EDITOR
        if (txt == null)
        {
            Debug.LogError("Log4NetLogger:Нет текста лога!");
        }
        if (ExternLog == null)
        {
            Debug.LogError("Log4NetLogger: Логгер NULL!");
        }
        ExternLog.Info(txt);
#endif
    }
    public static void Log(string txt, ILog ExternLog, bool IsLog)
    {
#if UNITY_EDITOR
        if (txt == null)
        {
            Debug.LogError("Log4NetLogger:Нет текста лога!");
        }
        if (ExternLog == null)
        {
            Debug.LogError("Log4NetLogger: Логгер NULL!");
        }
        if (IsLog) { ExternLog.Info(txt); }
#endif
    }

    public static void LogDbg(string txt, ILog ExternLog)
    {
#if UNITY_EDITOR
        if (txt == null)
        {
            Debug.LogError("Log4NetLogger:Нет текста лога!");
        }
        if (ExternLog == null)
        {
            Debug.LogError("Log4NetLogger: Логгер NULL!");
        }
        ExternLog.Debug(txt);
#endif
    }
    public static void LogDbg(string txt, ILog ExternLog, bool IsLog)
    {
#if UNITY_EDITOR
        if (txt == null)
        {
            Debug.LogError("Log4NetLogger:Нет текста лога!");
        }
        if (ExternLog == null)
        {
            Debug.LogError("Log4NetLogger: Логгер NULL!");
        }
        if (IsLog) { ExternLog.Debug(txt); }
#endif
    }

    public static void LogError(string txt, ILog ExternLog)
    {
#if UNITY_EDITOR
        if (txt == null)
        {
            Debug.LogError("Log4NetLogger:Нет текста лога!");
        }
        if (ExternLog == null)
        {
            Debug.LogError("Log4NetLogger: Логгер NULL!");
        }
        ExternLog.Error(txt);
#endif
    }
    public static void LogError(string txt, ILog ExternLog, bool IsLog)
    {
#if UNITY_EDITOR
        if (txt == null)
        {
            Debug.LogError("Log4NetLogger:Нет текста лога!");
        }
        if (ExternLog == null)
        {
            Debug.LogError("Log4NetLogger: Логгер NULL!");
        }
        if (IsLog) { ExternLog.Error(txt); }
#endif
    }
}

