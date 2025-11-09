using Mirror;
using UnityEngine;

namespace Player.PlayerMovment
{
    [RequireComponent(typeof(MoveService))]
    [Tooltip(" ласс дл€ обработки логики передвижени€ игрока")]
    public class MoveController : NetworkBehaviour
    {

        ActivePlayer player;

        [SyncVar]
        Vector3 input = Vector3.zero;
        MoveService service;

        void Start()
        {
            service = GetComponent<MoveService>();
            player = GetComponent<ActivePlayer>();

            service.playerIdentity = player.ConnId;
        }

        void FixedUpdate()
        {
            service.Move(input);
        }
        void Update()
        {

            if (player.isLocalPlayer)
            {
                input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                if(Input.GetKeyDown(KeyCode.X))
                {
                    player.TakeDamage(5);//TEST
                }
            }
        }
    }
}

