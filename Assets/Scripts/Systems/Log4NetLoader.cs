using UnityEngine;
using log4net;
using log4net.Config;
using System.IO;
using System;

public static class Log4NetLoader
{
    static readonly ILog log = LogManager.GetLogger(typeof(Log4NetLoader));

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

                log.Info($"Log4NetLoader: Log4Net запущен");
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