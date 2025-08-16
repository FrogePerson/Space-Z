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


        static readonly ILog log = Log4NetLogger.SetLogger(typeof(Settings));

        void Start ()
        {
            ApplySettings();
        }

        void ApplySettings()
        {
            Application.targetFrameRate = FPS;

            Log4NetLogger.LogDbg("��������� ��������� ������", log);
        }
        void OnApplicationQuit()
        {
            log4net.LogManager.Shutdown();
            Log4NetLogger.Log("��������� log4net", log);
        }
    }

}

