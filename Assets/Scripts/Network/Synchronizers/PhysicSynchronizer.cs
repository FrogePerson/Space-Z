using System.Collections;
using UnityEngine;
using Mirror;
using System;

namespace Network.Synchronizers
{
    [RequireComponent(typeof(Rigidbody), typeof(NetworkIdentity))]
    public class PhysicSynchronizer : NetworkBehaviour
    {
        [Header("Синхронизация")]
        [SerializeField]
        bool SyncPosition = true;
        [SerializeField]
        bool SyncRotation = true;
        [SerializeField]
        bool SyncVelocity = true;
        [SerializeField]
        bool SyncAngularVelocity = true;

        [SyncVar]
        Vector3 serverPosition;
        [SyncVar]
        Quaternion serverRotation;
        [SyncVar]
        Vector3 serverVelocity;
        [SyncVar]
        Vector3 serverAngularVelocity;

        [Header("Сетевые настройки")]
        [SerializeField]
        float updateTime = 0.05f; //20 фпс, НЕ МЕНЯТЬ В RUN TIME!
        [SerializeField]
        float minUpdateDistance = 0.01f;
        [SerializeField]
        float minUpdateRotation = 0.1f;

        Vector3 lastSendPosition;
        Quaternion lastSendRotation;


        [Header("Силы синхронизации")]
        [SerializeField]
        float positionForce = 100f;
        [SerializeField]
        float rotationForce = 50f;
        [SerializeField]
        float maxForce = 500f;
        [SerializeField]
        float smoothTime = 0.1f;

        Rigidbody rb;
        Vector3 positionError;
        Quaternion rotationError;
        Vector3 velocityBias;
        Vector3 angularVelocityBias;


        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();

            InvokeRepeating(nameof(ServerUpdatePositionState), 0f, updateTime);
        }

        [Server]
        void ServerUpdatePositionState()
        {
            float posDelta = Vector3.Distance(lastSendPosition, rb.position);
            float rotDelta = Quaternion.Angle(rb.rotation, lastSendRotation);

            if (posDelta > minUpdateDistance)
            {
                serverPosition = rb.position;
                serverVelocity = rb.linearVelocity;
                lastSendPosition = serverPosition;
            }

            if (rotDelta > minUpdateRotation)
            {
                serverRotation = rb.rotation;
                serverAngularVelocity = rb.angularVelocity;
                lastSendRotation = serverRotation;
            }

            //serverPosition = rb.position;
            //serverRotation = rb.rotation;
            //serverVelocity = rb.linearVelocity;
            //serverAngularVelocity = rb.angularVelocity;
        }

        #endregion

        #region Client RPCs


        #endregion

        #region Client

        [ClientCallback]
        void FixedUpdate()
        {
            ApplyForces();
        }

        [Client]
        void ApplyForces()
        {
            PD_Regulator();

            //rb.position = serverPosition;
            //rb.rotation = serverRotation;

        }

        void PD_Regulator()
        {
            CalculateErrors();
            ApplyPositionCorrection();
            ApplyRotationCorrection();
        }

        [Client]
        void CalculateErrors()
        {
            positionError = serverPosition - rb.position;

            rotationError = serverRotation * Quaternion.Inverse(rb.rotation);
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            velocityBias = serverVelocity - rb.linearVelocity;
            angularVelocityBias = serverAngularVelocity - rb.angularVelocity;
        }

        [Client]
        void ApplyPositionCorrection()
        {
            //Force = P * error + D * дельта error/дельта t, дельта t = 2
            Vector3 targetForce = positionForce * positionError + velocityBias * (positionForce * 0.5f);

            targetForce = Vector3.ClampMagnitude(targetForce, maxForce);

            rb.AddForce(targetForce, ForceMode.Acceleration);
            Debug.Log($"Применена сила к {gameObject.name} в размере = {targetForce}");
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
                Debug.Log($"Применено вращение к {gameObject.name} в размере = {torque}");
            }
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