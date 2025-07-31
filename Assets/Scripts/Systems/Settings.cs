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

