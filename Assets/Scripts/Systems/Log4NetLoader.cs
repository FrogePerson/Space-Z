using log4net;
using log4net.Config;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Log4NetLoader
{
#if UNITY_EDITOR
    static readonly ILog log = LogManager.GetLogger(typeof(Log4NetLoader));
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Configure()
    {
        string logDir = Path.Combine(Application.dataPath, "../Logs");

        Directory.CreateDirectory(logDir);
        string logPath = Path.Combine(logDir, "UnityLog.txt");
        GlobalContext.Properties["LogPath"] = logPath;

        try
        {
            var configFile = Resources.Load<TextAsset>("log4net");
            if (configFile != null)
            {
                XmlConfigurator.Configure(new MemoryStream(configFile.bytes));

#if UNITY_EDITOR
                log.Info($"Log4NetLoader: Log4Net запущен");
#endif

            }
            else
            {
                Debug.LogError("log4net.config not found in Resources!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Log4Net config error: {ex}");
        }
    }
}