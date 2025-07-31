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

        void Start ()
        {
            ApplySettings();
        }

        void ApplySettings()
        {
            Application.targetFrameRate = FPS;
        }
    }
}

