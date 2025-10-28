using Interfaces;
using Mirror;
using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Network.Synchronizers
{
    public class ByteSynchronizer : NetworkBehaviour, ISyncPosition
    {
        struct Packed
        {
            public short serverPosX;
            public short serverPosY;
            public short serverPosZ;

            public short serverRotX;
            public short serverRotY;
            public short serverRotZ;
        }

        [SyncVar]
        Packed packed;

        [SerializeField]
        [SyncVar]
        Vector3 lastSendedPosition = Vector3.zero;
        [SyncVar]
        Quaternion lastSendedRotation = Quaternion.identity;

        [Header("Настройки")]
        [SerializeField]
        float minDistanceToSend = 0.01f;
        [SerializeField]
        float minAngleToSend = 1f;
        [SerializeField]
        float updateTime = 0.05f;

        Vector3 newPos = Vector3.zero;
        Quaternion newRot = Quaternion.identity;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            InvokeRepeating(nameof(ServerUpdatePositionState), 0f, updateTime);
            lastSendedPosition = transform.position;
            lastSendedRotation = transform.rotation;
        }

        [Server]
        void ServerUpdatePositionState()
        {
            Vector3 posDistance = transform.position - lastSendedPosition;
            float distance = posDistance.magnitude;

            float angle = Quaternion.Angle(transform.rotation, lastSendedRotation);

            if (distance < minDistanceToSend && angle < minAngleToSend)
                return;

            bool needFullReset = Mathf.Abs(posDistance.x) > 327.68f ||
                               Mathf.Abs(posDistance.y) > 327.68f ||
                               Mathf.Abs(posDistance.z) > 327.68f;

            if (needFullReset)
            {
                lastSendedPosition = transform.position;
                lastSendedRotation = transform.rotation;
                return;
            }

            Quaternion relativeRot = transform.rotation * Quaternion.Inverse(lastSendedRotation);
            Vector3 eulerRelative = GetNormalizedEulerAngles(relativeRot);

            Packed newPacked = new Packed
            {
                serverPosX = ConvertToShort(posDistance.x),
                serverPosY = ConvertToShort(posDistance.y),
                serverPosZ = ConvertToShort(posDistance.z),
                serverRotX = ConvertToShort(eulerRelative.x),
                serverRotY = ConvertToShort(eulerRelative.y),
                serverRotZ = ConvertToShort(eulerRelative.z)
            };

            packed = newPacked;
            lastSendedPosition = transform.position;
            lastSendedRotation = transform.rotation;
        }

        Vector3 GetNormalizedEulerAngles(Quaternion quat)
        {
            Vector3 euler = quat.eulerAngles;

            // Нормализуем углы в диапазон [-180, 180]
            euler.x = NormalizeAngle(euler.x);
            euler.y = NormalizeAngle(euler.y);
            euler.z = NormalizeAngle(euler.z);

            return euler;
        }

        float NormalizeAngle(float angle)
        {
            angle = angle % 360f;
            if (angle > 180f)
                angle -= 360f;
            if (angle < -180f)
                angle += 360f;
            return angle;
        }

        short ConvertToShort(float value)
        {
            return (short)Mathf.RoundToInt(value * 100f);
        }

        #endregion

        #region Client

        [ClientCallback]
        void Update()
        {
            if (!isServer)
            {
                Interpolate();
            }
        }

        void Interpolate()
        {
            Vector3 deltaPos = new Vector3(
                packed.serverPosX * 0.01f,
                packed.serverPosY * 0.01f,
                packed.serverPosZ * 0.01f
            );

            newPos = lastSendedPosition + deltaPos;

            float distanceToTarget = Vector3.Distance(transform.position, newPos);
            float posSpeed = distanceToTarget / updateTime;
            transform.position = Vector3.MoveTowards(transform.position, newPos, posSpeed * Time.deltaTime);

            Vector3 deltaRot = new Vector3(
                packed.serverRotX * 0.01f,
                packed.serverRotY * 0.01f,
                packed.serverRotZ * 0.01f
            );

            Quaternion relativeRot = Quaternion.Euler(deltaRot);

            newRot = lastSendedRotation * relativeRot;

 
            float angleToTarget = Quaternion.Angle(transform.rotation, newRot);
            float rotSpeed = angleToTarget / updateTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, rotSpeed * Time.deltaTime);

            //Debug.Log($"Вращение: текущее {transform.rotation.eulerAngles}, целевое {newRot.eulerAngles}");
        }

        #endregion

        public void EMERGENCY_SYNC(Vector3 position, Vector3 rotation)
        {
            return;
        }
    }
}