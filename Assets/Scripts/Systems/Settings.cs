using log4net;
using UnityEngine;

namespace Settings
{
    public class Settings : MonoBehaviour
    {
        [Tooltip("������������ FPS")]
        [SerializeField]
        int FPS = 60;
        [Tooltip("������������ ����������� FPS?")]
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
            log.Debug("Settings: ��������� ��������� ������");
#endif
        }
        void OnApplicationQuit()
        {
            log4net.LogManager.Shutdown();
            log.Info("��������� log4net");
        }
    }

}

