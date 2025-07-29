using System.Collections;
using UnityEngine;

namespace Player.PlayerMovment
{

    [RequireComponent(typeof(Rigidbody))]
    [Tooltip("Класс для реализации передвижения игрока")]
    public class MoveService : MonoBehaviour
    {

        [Header("Movment")]

        [SerializeField]
        private float _moveSpeed = 5f;
        public float MoveSpeed
        {
            get { return _moveSpeed; }
            set { _moveSpeed = value; }
        }
        [SerializeField]
        private float maxgroundAngle = 45f;
        [SerializeField]

        Rigidbody rb;
        Vector3 groundNormal = Vector3.zero;
        Vector3 relativeVelocity = Vector3.zero;

        bool IsGrounded = false;

        void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    IsGrounded = true;
                    groundNormal = contact.normal;
                    //счётчик коллизий
                }

            }
        }

        public void Move(Vector3 input)
        {
            //если счётчик == 0 то мы ввоздухе
            //счётчик коллизий обнуляем
            if (IsGrounded)
            {
                Vector3 moveDirection = Vector3.ProjectOnPlane(input.normalized, groundNormal);
                moveDirection = transform.TransformDirection(moveDirection);
                Vector3 targetVelocity = (moveDirection * MoveSpeed) + relativeVelocity;
                Vector3 velocityChange = targetVelocity - rb.linearVelocity;

                velocityChange.y = 0f;

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
                Debug.Log($"input = {input}, velocityChange = {velocityChange}");
            }
        }
    }
}