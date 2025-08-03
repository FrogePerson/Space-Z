using log4net;
using Mirror;
using Mirror.Examples.Benchmark;
using UnityEngine;


namespace Player.PlayerMovment
{

    [RequireComponent(typeof(Rigidbody))]
    [Tooltip("Класс для реализации передвижения игрока")]
    public class MoveService : NetworkBehaviour
    {
#if UNITY_EDITOR
        static readonly ILog log = LogManager.GetLogger(typeof(PlayerMovement));
        public bool IsDebug = false;
#endif
        public readonly NetworkIdentity playerIdentity;


        Rigidbody rb;

        
        Vector3 groundNormal = Vector3.zero;
        Vector3 relativeVelocity = Vector3.zero;


        [SerializeField]
        float _moveSpeed = 5f;
        public float MoveSpeed
        {
            get
            {
                return _moveSpeed;
            }
            set
            {
                _moveSpeed = value;
            }
        }


        public bool IsGrounded = false;

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
#if UNITY_EDITOR
                    if (IsDebug)
                    {
                        log.Debug($"Player::MoveController::MoveService: Игрок c id  стоит на поверхности");
                    }
#endif
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

#if UNITY_EDITOR
                if (IsDebug)
                {
                    log.Debug($"Player::MoveController::MoveService: Игрок c id " +
                        $"двигается по поверхности с velocityChange = {velocityChange}");
                }
#endif
            }
        }
    }
}