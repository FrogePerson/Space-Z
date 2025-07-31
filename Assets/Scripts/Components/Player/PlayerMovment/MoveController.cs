using Mirror;
using UnityEngine;

namespace Player.PlayerMovment
{
    [RequireComponent(typeof(MoveService))]
    [Tooltip("Класс для обработки логики передвижения игрока")]
    public class MoveController : NetworkBehaviour
    {

        Player player;

        [SyncVar]
        Vector3 input = Vector3.zero;
        MoveService service;

        void OnEnable()
        {
            service = GetComponent<MoveService>();
            player = GetComponent<Player>();
        }

        private void FixedUpdate()
        {
            service.Move(input);
        }
        void Update()
        {

            if (player.isLocalPlayer)
            {
                input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            }

            input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));  
            //Debug.Log($"Player::MoveController:Нжатие {input}");
        }
    }
}

