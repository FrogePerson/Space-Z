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
        static readonly ILog log = LogManager.GetLogger(typeof(Player));
        [SerializeField]
        Camera camera;

        void OnEnable()
        {
            assembling();
        }
        void assembling()
        {
            MoveController moveController = gameObject.GetComponent<MoveController>() ?? gameObject.AddComponent<MoveController>();

            log.Debug($"Player: Создан игрок");
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