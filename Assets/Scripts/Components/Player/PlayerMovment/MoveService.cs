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
        LayerMask groundMask;

        Rigidbody rb;
        Vector3 groundNormal = Vector3.zero;
        Vector3 relativeVelocity = Vector3.zero;

        float groundCheckRadius = 0.5f;
        float groundDistance = 1;
        bool IsGrounded = false;

        void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            groundMask = LayerMask.GetMask("Ground");
        }

        //public void CheckGround()
        //{
        //     RaycastHit hit;

        //     Debug.DrawRay(transform.position + Vector3.up * groundCheckRadius,
        //     Vector3.down * groundDistance,
        //     Color.red, 1f);

        //    if (Physics.SphereCast(transform.position + Vector3.up * groundCheckRadius,
        //        groundCheckRadius,
        //        Vector3.down,
        //        out hit,
        //        groundDistance,
        //        groundMask
        //        ))
        //    {
        //        if (hit.collider.bounds.size.magnitude < 1f)
        //        {
        //            return;
        //        }

        //        float angle = Vector3.Angle(hit.normal, Vector3.up);
        //        IsGrounded = angle <= maxgroundAngle;

        //        if (IsGrounded)
        //        {
        //            groundNormal = hit.normal;
        //        }
        //    }
        //    else
        //    {
        //        IsGrounded= false;
        //        groundNormal = Vector3.up;
        //    }
        //}

        void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    IsGrounded = true;
                    groundNormal = contact.normal;

                }

            }
        }

        public void Move(Vector3 input)
        {

            if (IsGrounded)
            {
                Vector3 moveDirection = Vector3.ProjectOnPlane(input.normalized, groundNormal);
                Vector3 targetVelocity = (moveDirection * MoveSpeed) + relativeVelocity;
                Vector3 velocityChange = targetVelocity - rb.linearVelocity;

                velocityChange.y = 0f;

                rb.AddForce(velocityChange, ForceMode.VelocityChange); 
            }
        }
    }
}