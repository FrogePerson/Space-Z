using Interfaces;
using log4net;
using Mirror;
using Player.PlayerLamp;
using Player.PlayerMovment;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Player
{
    /// Отвечает за сборку/разборку
    /// взаимодействие с другими классами
    /// содержит основные сведения об игроке

    [Tooltip("Общий класс игрока")]
    public class Player : NetworkBehaviour, IDamageable
    {
        #region Start

        static readonly ILog log = Log4NetLogger.SetLogger(typeof(Player));

        public int ConnId;

        [SyncVar(hook = nameof(OnHpChanged))]
        [SerializeField]
        int _hp = 100;
        public int Hp
        {
            get 
            { 
                return _hp; 
            }
            private set 
            { 
                _hp = value; 
            }
        }

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

                EventBus.Publish(new PlayerHealthChangedEvent(ConnId, Hp));
            }
        }
        #endregion

        #region Hp
        public void TakeDamage(int damage)
        {
            ApplyDamage(damage);
        }

        [Command]
        void ApplyDamage(int damage)
        {
            Hp -= damage;

        }

        void OnHpChanged(int oldValue, int newValue)
        {
            if (isLocalPlayer)
            {
                EventBus.Publish(new PlayerHealthChangedEvent(ConnId, newValue));
            }
        }
        #endregion
    }
}