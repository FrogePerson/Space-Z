using UnityEngine;

namespace Player.PlayerMovment
{
    [RequireComponent(typeof(MoveService))]
    [Tooltip("Класс для обработки логики передвижения игрока")]
    public class MoveController : MonoBehaviour
    {

        Vector3 input = Vector3.zero;
        MoveService service;

        void OnEnable()
        {
            service = GetComponent<MoveService>();
        }

        private void FixedUpdate()
        {
            service.Move(input);
        }
        void Update()
        {
            input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //Debug.Log($"Player::MoveController:Нжатие {input}");
        }
    }
}

