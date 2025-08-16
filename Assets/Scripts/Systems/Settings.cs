using log4net;
using UnityEngine;

namespace Settings
{
    public class Settings : MonoBehaviour
    {
        [Tooltip("Максимальный FPS")]
        [SerializeField]
        int FPS = 60;
        [Tooltip("Использовать ограничение FPS?")]
        [SerializeField]
        bool IsClampFPS = true;


        static readonly ILog log = Log4NetLogger.SetLogger(typeof(Settings));

        void Start ()
        {
            ApplySettings();
        }

        void ApplySettings()
        {
            Application.targetFrameRate = FPS;

            Log4NetLogger.LogDbg("Применены настройки игрока", log);
        }
        void OnApplicationQuit()
        {
            log4net.LogManager.Shutdown();
            Log4NetLogger.Log("Выключили log4net", log);
        }
    }

}

