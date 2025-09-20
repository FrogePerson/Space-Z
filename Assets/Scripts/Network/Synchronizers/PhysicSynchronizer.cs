using System.Collections;
using UnityEngine;
using Mirror;
using System;
using Interfaces;

namespace Network.Synchronizers
{
    [RequireComponent(typeof(Rigidbody), typeof(NetworkIdentity))]
    public class PhysicSynchronizer : NetworkBehaviour, ISyncPosition
    {
        [Header("Синхронизация")]
        [SerializeField]
        protected bool SyncPosition = true;
        [SerializeField]
        protected bool SyncRotation = true;
        [SerializeField]
        protected bool SyncVelocity = true;
        [SerializeField]
        protected bool SyncAngularVelocity = true;

        [System.Serializable]
        protected struct Packed
        {
            
            public Vector3 serverPosition;
            
            public Quaternion serverRotation;
            
            public Vector3 serverVelocity;
            
            public Vector3 serverAngularVelocity;
        }

        [SyncVar]
        protected Packed packed;


        //protected Vector3 serverPosition;
        //protected Quaternion serverRotation;
        //protected Vector3 serverVelocity;
        //protected Vector3 serverAngularVelocity;

        [Header("Сетевые настройки")]
        [SerializeField]
        protected bool Extrapolation = false;
        [SerializeField]
        protected float updateTime = 0.05f; //20 фпс, НЕ МЕНЯТЬ В RUN TIME
        [SerializeField]
        protected float minUpdateDistance = 0.1f;
        [SerializeField]
        protected float minUpdateRotation = 0.1f;

        protected Vector3 lastSendPosition;
        protected Quaternion lastSendRotation;


        [Header("Силы синхронизации")]
        [SerializeField]
        protected float positionForce = 100f;
        [SerializeField]
        protected float rotationForce = 100f;
        [SerializeField]
        protected float maxForce = 500f;
        [SerializeField]
        protected float smoothTime = 0.1f;
        [SerializeField]
        protected float gravityCompensation = 1.2f;

        protected Rigidbody rb;
        protected Vector3 positionError;
        protected Quaternion rotationError;
        protected Vector3 velocityBias;
        protected Vector3 angularVelocityBias;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();

            InvokeRepeating(nameof(ServerUpdatePositionState), 0f, updateTime);
        }

        [Server]
        protected virtual void ServerUpdatePositionState()
        {
            float posDelta = Vector3.Distance(packed.serverPosition, rb.position);
            float rotDelta = Quaternion.Angle(packed.serverRotation, rb.rotation);

            if(posDelta > minUpdateDistance || rotDelta > minUpdateDistance)
            {
                Packed newPacked = new Packed();
                if (Extrapolation)
                {
                    newPacked.serverPosition = rb.position + rb.linearVelocity * updateTime;

                    Vector3 angularDisplacementVector = rb.angularVelocity * updateTime;
                    Quaternion angularDisplacementQuaternion = Quaternion.Euler(angularDisplacementVector);

                    newPacked.serverRotation = rb.rotation * angularDisplacementQuaternion;
                    newPacked.serverVelocity = rb.linearVelocity;
                    newPacked.serverAngularVelocity = rb.angularVelocity;

                    packed = newPacked;
                    return;
                }
                
                newPacked.serverPosition = rb.position;
                newPacked.serverRotation = rb.rotation;
                newPacked.serverVelocity = rb.linearVelocity;
                newPacked.serverAngularVelocity = rb.angularVelocity;

                packed = newPacked;
            }
        }

        #endregion

        #region Client RPCs

        public void EMERGENCY_SYNC(Vector3 position, Vector3 rotation)
        {
            rb.position = position;

            Quaternion rotationQuaternion = Quaternion.Euler(rotation);
            rb.rotation = rotationQuaternion;
        }

        #endregion

        #region Client

        [ClientCallback]
        void FixedUpdate()
        {
            if(isServer) return;
            ApplyForces();
        }

        [Client]
        void ApplyForces()
        {
            PD_Regulator();
        }

        void PD_Regulator()
        {
            CalculateErrors();
            ApplyPositionCorrection();
            ApplyRotationCorrection();
            ApplyGravityCompensation();
        }

        [Client]
        protected virtual void CalculateErrors()
        {
            positionError = packed.serverPosition - rb.position;

            rotationError = packed.serverRotation * Quaternion.Inverse(rb.rotation);
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            velocityBias = packed.serverVelocity - rb.linearVelocity;
            angularVelocityBias = packed.serverAngularVelocity - rb.angularVelocity;
        }

        [Client]
        void ApplyPositionCorrection()
        {
            //Force = P * error + D * дельта error/дельта t, дельта t = 2
            Vector3 targetForce = positionForce * positionError + velocityBias * (positionForce * 0.5f);

            targetForce = Vector3.ClampMagnitude(targetForce, maxForce);

            rb.AddForce(targetForce, ForceMode.Acceleration);
        }

        [Client]
        void ApplyRotationCorrection()
        {
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            if(Math.Abs(angle) > 0.1f)// игнорим малые углы
            {
                //Torque = P_rot * angle + D_rot * дельта angular_velocity
                Vector3 torque = axis * (angle * Mathf.Deg2Rad * rotationForce) +
                angularVelocityBias * (rotationForce * 0.3f);

                torque = Vector3.ClampMagnitude(torque, maxForce);
                rb.AddTorque(torque, ForceMode.Acceleration);
            }
        }

        void ApplyGravityCompensation()
        {
            Vector3 gravityForce = Physics.gravity * gravityCompensation * rb.mass;
            rb.AddForce(gravityForce, ForceMode.Force);
        }

        #endregion

        #region Client RPCs


        #endregion

        #region Client


        #endregion

        #region Commands


        #endregion

        #region Common

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        #endregion
    }
}