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

        static readonly ILog log = LogManager.GetLogger(typeof(Log4NetLoader));

        void Start ()
        {
            ApplySettings();
        }

        void ApplySettings()
        {
            Application.targetFrameRate = FPS;
            log.Debug("Settings: Применены настройки игрока");
        }
    }
}

