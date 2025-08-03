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

#if UNITY_EDITOR
        static readonly ILog log = LogManager.GetLogger(typeof(Settings));
#endif

        void Start ()
        {
            ApplySettings();
        }

        void ApplySettings()
        {
            Application.targetFrameRate = FPS;

#if UNITY_EDITOR
            log.Debug("Settings: Применены настройки игрока");
#endif
        }
        void OnApplicationQuit()
        {
            log4net.LogManager.Shutdown();
            log.Info("Выключили log4net");
        }
    }

}

