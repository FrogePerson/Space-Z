using UnityEngine;
using Mirror;

namespace Network.NetworkJoint
{
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkJointParent : NetworkBehaviour
    {
        [SerializeField] private float breakForce = 50f;
        [SerializeField] private float checkRadius = 0.0001f;

        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        #region Server Side

        [ServerCallback]
        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Collider[] hitColliders = Physics.OverlapSphere(contact.point, checkRadius);

                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.transform.IsChildOf(transform))
                    {
                        Vector3 relativeVelocity = rb.GetPointVelocity(contact.point);
                        Vector3 normal = contact.normal;

                        float impactAngleFactor = Mathf.Abs(Vector3.Dot(normal, relativeVelocity.normalized));
                        float impulseMagnitude = relativeVelocity.magnitude * rb.mass;
                        float effectiveImpulse = impulseMagnitude * impactAngleFactor;

                        Vector3 force = effectiveImpulse * relativeVelocity.normalized;

                        if (force.magnitude >= 0)
                        {
                            Debug.Log($"Уебали: {contact.thisCollider.name}, с силой: {force.magnitude}, под углом: {impactAngleFactor}");
                        }
                        
                        if(force.magnitude >= breakForce)
                        {
                            BreakJoint(contact.thisCollider.gameObject);
                        }
                    }
                }
            }
        }

        [Server]
        void BreakJoint(GameObject obj)
        {
            var tmp = obj.GetComponent<NetworkJoint>();

            tmp.BreakSelf();
        }

        #endregion

    }
}