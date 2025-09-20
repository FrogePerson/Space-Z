using System.Collections;
using UnityEngine;
using Interfaces;
using System;
using Mirror;

namespace Network.Synchronizers
{
    public class ByteSynchronizer : NetworkBehaviour, ISyncPosition
    {
        struct SmallPacked
        {
            public byte serverPosX;
            public byte serverPosY;
            public byte serverPosZ;

            public byte serverRotY;
        }

        struct Packed
        {
            public byte serverPosX;
            public byte serverPosY;
            public byte serverPosZ;

            public byte serverRotX;
            public byte serverRotY;
            public byte serverRotZ;
        }

        struct FullPacked
        {
            public Vector3 serverPosition;

            public Vector3 serverRotation;
        }

        

        [SyncVar]
        SmallPacked smallPacked;
        [SyncVar]
        Packed packed;
        [SyncVar(hook = nameof(applyFullPocked))]
        FullPacked fullPacked;

        Vector3 lastSendedPosition = Vector3.zero;
        Vector3 lastSendedRotation = Vector3.zero;

        bool IsSendAll = false;

        [Header("Настройки")]
        [SerializeField]
        bool IsUsingSmallPacket = false;
        [SerializeField]
        float updateTime = 0.05f;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();

            InvokeRepeating(nameof(ServerUpdatePositionState), 0f, updateTime);

            lastSendedPosition = transform.position;
        }

        [Server]
        void ServerUpdatePositionState()
        {
            //float posDelta = Vector3.Distance(packed.serverPosition, rb.position);
            //float rotDelta = Quaternion.Angle(packed.serverRotation, rb.rotation); 
            // оптимизация, эт на потом)

            if (IsUsingSmallPacket)
            {
                // не юзай пока
            }
            else
            {
                Packed newPacked = new Packed();

                Vector3 tmp;

                IsSendAll = false;

                byte result = 0;

                tmp = lastSendedPosition - transform.position;
                if (ConvertToByte(tmp.x, ref result)) newPacked.serverPosX = result;

                tmp = lastSendedPosition - transform.position;
                if (ConvertToByte(tmp.y, ref result)) newPacked.serverPosY = result;

                tmp = lastSendedPosition - transform.position;
                if (ConvertToByte(tmp.z, ref result)) newPacked.serverPosZ = result;

                if (IsSendAll)
                {
                    Debug.Log($"ByteSync: отправили FullPacked");
                    sendFullPacked();
                    return;
                }
                Debug.Log($"ByteSync: {newPacked.serverPosX}, {newPacked.serverPosY}, {newPacked.serverPosZ}");
            }
        }

        void sendFullPacked()
        {
            FullPacked newFullPacked = new FullPacked();

            newFullPacked.serverPosition = transform.position;
            newFullPacked.serverRotation = transform.rotation.eulerAngles;

            fullPacked = newFullPacked;

            lastSendedPosition = transform.position;
            lastSendedRotation = transform.rotation.eulerAngles;
        }

        bool ConvertToByte(float value, ref byte result)
        {
            var tmp = Math.Round(value, 2);
            if(Math.Abs(tmp) > 1.27)
            {
                IsSendAll = true;
                result = 0;
                return false;
            }
            else
            {
                tmp *= 100;
                tmp += 128;
                result = (byte)tmp;
                return true;
            }
        }

        #endregion

        #region Client

        void applyFullPocked(FullPacked oldValue, FullPacked newValue)
        {
            lastSendedPosition = fullPacked.serverPosition;
            lastSendedRotation = fullPacked.serverRotation;
        }

        #endregion

        public void EMERGENCY_SYNC(Vector3 position, Vector3 rotation)
        {
            throw new System.NotImplementedException();
        }
    }
}