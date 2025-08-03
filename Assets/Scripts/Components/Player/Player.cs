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

#if UNITY_EDITOR
        static readonly ILog log = LogManager.GetLogger(typeof(Player));
#endif
        [SerializeField]
        Camera camera;

        void OnEnable()
        {
            assembling();
        }
        void assembling()
        {
            MoveController moveController = gameObject.GetComponent<MoveController>() ?? gameObject.AddComponent<MoveController>();

#if UNITY_EDITOR
            log.Debug($"Player: Создан игрок");
#endif
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