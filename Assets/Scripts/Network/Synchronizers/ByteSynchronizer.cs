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
        struct SmallPacked
        {
            public short serverPosX;
            public short serverPosY;
            public short serverPosZ;

            public short serverRotY;
        }

        struct Packed
        {
            public short serverPosX;
            public short serverPosY;
            public short serverPosZ;

            public short serverRotX;
            public short serverRotY;
            public short serverRotZ;
        }

        struct FullPacked
        {
            public Vector3 serverPosition;

            public Quaternion serverRotation;
        }

        

        [SyncVar]
        SmallPacked smallPacked;
        [SyncVar]
        Packed packed;
        //[SyncVar(hook = nameof(applyFullPocked))]
        //FullPacked fullPacked;

        [SerializeField]
        [SyncVar]
        Vector3 lastSendedPosition = Vector3.zero;
        Quaternion lastSendedRotation = Quaternion.identity;

        bool IsSendAll = false;

        [Header("Настройки")]
        [SerializeField]
        bool IsUsingSmallPacket = false;
        [SerializeField]
        float minDistanceToSend = 0.01f;
        [SerializeField]
        float updateTime = 0.05f;

        Vector3 newPos = Vector3.zero;
        float interpolationSpeed = 2f;

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
            //Vector3 rotDistance = transform.rotation - lastSendedRotation;

            if(posDistance.magnitude < minDistanceToSend) return;

            bool IsNeedSendFullPacked = false;

            if(math.abs(posDistance.x) > 327.68) IsNeedSendFullPacked = true;
            if (math.abs(posDistance.y) > 327.68) IsNeedSendFullPacked = true;
            if (math.abs(posDistance.z) > 327.68) IsNeedSendFullPacked = true;

            if (IsNeedSendFullPacked)
            {
                sendFullPacked();
                Debug.Log("Отправили FullPacked!!");
                return;
            }

            Packed newPacked = new Packed();

            newPacked.serverPosX = ConvertToShort(posDistance.x);
            newPacked.serverPosY = ConvertToShort(posDistance.y);
            newPacked.serverPosZ = ConvertToShort(posDistance.z);

            packed = newPacked;
            //Debug.Log($"{packed.serverPosX}, {packed.serverRotY}");
        }

        void sendFullPacked()
        {
            lastSendedPosition = transform.position;
            lastSendedRotation = transform.rotation;
        }

        short ConvertToShort(float value)
        {
            var tmp = Math.Round(value, 2);
            tmp *= 100;
            return (short)tmp;
        }

        #endregion

        #region Client

        void Start()
        {
            //lastSendedPosition = transform.position;
            //lastSendedRotation = transform.rotation.eulerAngles;
        }

        //void applyFullPocked(FullPacked oldValue, FullPacked newValue)
        //{
        //    lastSendedPosition = fullPacked.serverPosition;
        //    lastSendedRotation = fullPacked.serverRotation;
        //}

        [ClientCallback]
        void Update()
        {
            if (!isServer)
            {
                InterpolatePosition();
            }
        }

        void InterpolatePosition()
        {
            Vector3 deltaPos = new Vector3();
            newPos = new Vector3();

            //Debug.Log($"packed.serverPosY = {packed.serverPosY}");

            deltaPos.x = (float)packed.serverPosX / 100f;
            deltaPos.y = (float)packed.serverPosY / 100f;
            deltaPos.z = (float)packed.serverPosZ / 100f;

            newPos = lastSendedPosition + deltaPos;

            //Debug.Log($"packed.serverPosY = {packed.serverPosY}");

            //Debug.Log($"deltaPos = {deltaPos}");
            //Debug.Log($"lastReceivedPosition = {lastReceivedPosition}");

            //Debug.Log($"Новая позиция на клиенте: {newPos}");

            float distanceToTarget = Vector3.Distance(transform.position, newPos);

            float interpolationSpeed = distanceToTarget / updateTime;

            transform.position = Vector3.MoveTowards(transform.position, newPos, interpolationSpeed * Time.deltaTime);

            //Debug.Log($"{packed.serverPosX}, {packed.serverPosY}, {packed.serverPosZ}");
        }

        #endregion

        public void EMERGENCY_SYNC(Vector3 position, Vector3 rotation)
        {
            int a = 0;
            var b = a;

            return;
        }
    }
}