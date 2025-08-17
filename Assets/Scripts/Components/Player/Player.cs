using log4net;
using Mirror;
using Player.PlayerMovment;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Player
{
    /// Отвечает за сборку/разборку
    /// взаимодействие с другими классами
    /// содержит основные сведения об игроке

    [Tooltip("Общий класс игрока")]
    public class Player : NetworkBehaviour
    {
        #region Start


        static readonly ILog log = Log4NetLogger.SetLogger(typeof(Player));

        public int ConnId;


        [SerializeField]
#pragma warning disable CS0108
        Camera camera;
#pragma warning restore CS0108

        void OnEnable()
        {
            assembling();
        }
        void assembling()
        {
            MoveController moveController = gameObject.GetComponent<MoveController>() ?? 
                gameObject.AddComponent<MoveController>();

            Log4NetLogger.LogDbg($"Создан игрок", log);
        }
        void Start()
        {
            if (isLocalPlayer)
            {
                camera.gameObject.SetActive(true);
            }
        }
        #endregion
    }
}