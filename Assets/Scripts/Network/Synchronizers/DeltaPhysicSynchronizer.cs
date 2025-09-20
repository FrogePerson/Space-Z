using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Network.Synchronizers
{
    [Obsolete("Не работает! Используйте PhysicSynchronizer или другие синхронизаторы!", true)]
    public class DeltaPhysicSynchronizer : PhysicSynchronizer 
    {

        private struct PrivatePacked
        {
            public short deltaServerPositionX;
            public short deltaServerPositionY;
            public short deltaServerPositionZ;

            public short deltaServerVelocityX;
            public short deltaServerVelocityY;
            public short deltaServerVelocityZ;

            public short deltaServerRotationX;
            public short deltaServerRotationY;
            public short deltaServerRotationZ;

            public short deltaServerAngularVelocityX;
            public short deltaServerAngularVelocityY;
            public short deltaServerAngularVelocityZ;
        }

        [SyncVar]
        PrivatePacked shortPacked;

        bool FlagFirstSendTime = false;

        [SyncVar]
        bool sendAllData = false;

        void Start()
        {

        }

        [Server]
        protected override void ServerUpdatePositionState()
        {
            if (FlagFirstSendTime == false)
            {
                base.ServerUpdatePositionState();
                sendAllData = true;
                FlagFirstSendTime = true;

                Debug.Log("Отправлены все данные");
            }
            else if (!SendData())
            {
                base.ServerUpdatePositionState();

                Debug.Log($"НЕ смогли отправить дельта позиции для объекта {gameObject.name}");
                sendAllData = true;
            }
        }

        [Server]
        bool SendData()
        {
            Debug.Log($"SERVER: {rb.position}");

            if(sendAllData) sendAllData = false;

            PrivatePacked newPacked = new PrivatePacked();

            var tmp = rb.position.x - packed.serverPosition.x;
            if (TrySendData(tmp, ref newPacked.deltaServerPositionX) == false) return false;

            tmp = rb.position.y - packed.serverPosition.y;
            if (TrySendData(tmp, ref newPacked.deltaServerPositionY) == false) return false;


            tmp = rb.position.z - packed.serverPosition.z;
            if (TrySendData(tmp, ref newPacked.deltaServerPositionZ) == false) return false;

            //lastPosition = rb.position;


            tmp = rb.linearVelocity.x - packed.serverVelocity.x;
            if (TrySendData(tmp, ref newPacked.deltaServerVelocityX) == false) return false;

            tmp = rb.linearVelocity.y - packed.serverVelocity.y;
            if (TrySendData(tmp, ref newPacked.deltaServerVelocityY) == false) return false;

            tmp = rb.linearVelocity.z - packed.serverVelocity.z;
            if (TrySendData(tmp, ref newPacked.deltaServerVelocityZ) == false) return false;

            //lastVelocity = rb.linearVelocity;


            tmp = rb.rotation.x - packed.serverRotation.x;
            if (TrySendData(tmp, ref newPacked.deltaServerRotationX) == false) return false;

            tmp = rb.rotation.y - packed.serverRotation.y;
            if (TrySendData(tmp, ref newPacked.deltaServerRotationY) == false) return false;

            tmp = rb.rotation.z - packed.serverRotation.z;
            if (TrySendData(tmp, ref newPacked.deltaServerRotationZ) == false) return false;

            //lastRotation = rb.rotation.eulerAngles;


            tmp = rb.angularVelocity.x - packed.serverAngularVelocity.x;
            if (TrySendData(tmp, ref newPacked.deltaServerAngularVelocityX) == false) return false;

            tmp = rb.angularVelocity.y - packed.serverAngularVelocity.y;
            if (TrySendData(tmp, ref newPacked.deltaServerAngularVelocityY) == false) return false;

            tmp = rb.angularVelocity.z - packed.serverAngularVelocity.z;
            if (TrySendData(tmp, ref newPacked.deltaServerAngularVelocityZ) == false) return false;

            //lastAngularVelocity = rb.angularVelocity;

            if(newPacked.deltaServerPositionX == 0 && newPacked.deltaServerPositionY == 0 && newPacked.deltaServerPositionZ == 0)
            {
                return true;
            }
            shortPacked = newPacked;
            return true;
        }

        bool IsShort(float number)
        {
            if (number < -32768 || number > 32768)
            {
                return false;
            }
            
            return true;
        }

        bool TrySendData(float x, ref short sender)
        {
            x *= 100;
            x = Mathf.Round(x);

            if (IsShort(x))
            {
                sender = (short)x;
            }
            else
            {
                return false;
            }
            return true;
        }

        [Client]
        protected override void CalculateErrors()
        {
            if(sendAllData)
            {
                base.CalculateErrors();
                return;
            }

            Vector3 tmp = new Vector3();
            tmp.x = shortPacked.deltaServerPositionX / 100;
            tmp.y = shortPacked.deltaServerPositionY / 100;
            tmp.z = shortPacked.deltaServerPositionZ / 100;

            positionError = (packed.serverPosition + tmp) - rb.position;

            Debug.Log($"bigPacked.serverPosition = {packed.serverPosition} + tmp = {tmp} = {packed.serverPosition + tmp}");


            tmp.x = shortPacked.deltaServerRotationX / 100;
            tmp.y = shortPacked.deltaServerRotationY / 100;
            tmp.z = shortPacked.deltaServerRotationZ / 100;

            Quaternion tmpQuat = Quaternion.Euler(tmp.x, tmp.y, tmp.z);
            Quaternion lastTmpQuat = packed.serverRotation;

            rotationError = (lastTmpQuat * tmpQuat) * Quaternion.Inverse(rb.rotation);
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;


            tmp.x = shortPacked.deltaServerVelocityX / 100;
            tmp.y = shortPacked.deltaServerVelocityY / 100;
            tmp.z = shortPacked.deltaServerVelocityZ / 100;

            velocityBias = (packed.serverVelocity + tmp) - rb.linearVelocity;

            tmp.x = shortPacked.deltaServerAngularVelocityX / 100;
            tmp.y = shortPacked.deltaServerAngularVelocityY / 100;
            tmp.z = shortPacked.deltaServerAngularVelocityZ / 100;

            angularVelocityBias = (packed.serverAngularVelocity + tmp) - rb.angularVelocity;
        }
    }
}