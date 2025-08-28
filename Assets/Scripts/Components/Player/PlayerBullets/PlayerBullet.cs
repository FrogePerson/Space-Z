using Interfaces;
using Mirror;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Player.PlayerBullets
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerBullet : NetworkBehaviour, IPoolObj
    {
        [Header("Пулл:")]
        [SerializeField]
        protected Pool pool;
        public int MyNumber = 0;

        [Header("Неастройки:")]
        [SerializeField]
        protected float shootForce = 1.0f;
        [SerializeField]
        protected Vector3 target = Vector3.forward;
        [SerializeField]
        Transform SpawnObject;

        Rigidbody _rb;
        protected Rigidbody rb
        {
            get
            {
                if (_rb == null) _rb = GetComponent<Rigidbody>();
                return _rb;
            }
            set
            {
                _rb = value;
            }
        }

        [ServerCallback]
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (pool != null)
            {
                pool.ServerPush(gameObject);
            }
        }

        

        protected virtual void OnEnable()
        {
            transform.SetParent(null);
            Shoot();
        }


        protected virtual void OnDisable()
        {
            
        }
        protected virtual void Shoot()
        {
            rb.AddForce(target * shootForce, ForceMode.Impulse);

        }

        public void ReturnToPool()
        {
            transform.position = SpawnObject.position;
            transform.rotation = SpawnObject.rotation;
            transform.SetParent(SpawnObject.transform);

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;


            rb.ResetCenterOfMass();
            rb.ResetInertiaTensor();


            rb.Sleep();
            rb.WakeUp(); 
        }
    }
}