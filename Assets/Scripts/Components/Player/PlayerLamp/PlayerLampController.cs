using log4net;
using Mirror;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.PlayerLamp
{
    public class PlayerLampController : NetworkBehaviour
    {
        static readonly ILog log = Log4NetLogger.SetLogger(typeof(PlayerLampController));

        Player player;

        [SyncVar(hook = nameof(OnLampChanged))]
        [SerializeField]
        bool IsLampON;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        void OnLampChanged(bool oldValue, bool newValue )
        {
            if(player != null) return;
            if (newValue) Log4NetLogger.Log($"Фонарь игрока с id = {player.ConnId}, включен", log);
            else Log4NetLogger.Log($"Фонарь игрока с id = {player.ConnId}, выключен", log);
        }

        [Command]
        void CmdChangeLampState(bool newValue)
        {
            IsLampON = newValue;
        }

        void Update()
        {
            if (player.isLocalPlayer)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // пример отправки сообщения, предпочли пока не использовать
                    //byte[] data = { 1 };

                    //ByteDataMessage message = new ByteDataMessage
                    //{
                    //    data = data
                    //};

                    //NetworkClient.Send(message);

                    if (IsLampON) CmdChangeLampState(false);
                    else CmdChangeLampState(true);

                }
            }
        }
    }
}