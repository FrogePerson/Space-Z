using Mirror;
using System.Collections;
using UnityEngine;

namespace Network.Synchronizers
{
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
        PrivatePacked _packed;

        [SerializeField]
        Vector3 lastPosition = Vector3.zero;
        [SerializeField]
        Vector3 lastVelocity = Vector3.zero;
        [SerializeField]
        Vector3 lastRotation = Vector3.zero;
        [SerializeField]
        Vector3 lastAngularVelocity = Vector3.zero;

        void Start()
        {
            lastPosition = rb.position;
            lastVelocity = rb.linearVelocity;
            lastRotation = rb.rotation.eulerAngles;
            lastAngularVelocity = rb.angularVelocity;
        }

        [Server]
        protected override void ServerUpdatePositionState()
        {
            if (!SendData())
            {
                base.ServerUpdatePositionState();

                Debug.Log($"НЕ смогли отправить дельта позиции для объекта {gameObject.name}");

                lastPosition = rb.position;
                lastVelocity = rb.linearVelocity;
                lastRotation = rb.rotation.eulerAngles;
                lastAngularVelocity = rb.angularVelocity;
            }
        }

        bool SendData()
        {
            PrivatePacked newPacked = new PrivatePacked();

            var tmp = rb.position.x - lastPosition.x;
            if (TrySendData(tmp, newPacked.deltaServerPositionX) == false) return false;

            tmp = rb.position.y - lastPosition.y;
            if (TrySendData(tmp, newPacked.deltaServerPositionY) == false) return false;


            tmp = rb.position.z - lastPosition.z;
            if (TrySendData(tmp, newPacked.deltaServerPositionZ) == false) return false;

            lastPosition = rb.position;


            tmp = rb.linearVelocity.x - lastVelocity.x;
            if (TrySendData(tmp, newPacked.deltaServerVelocityX) == false) return false;

            tmp = rb.linearVelocity.y - lastVelocity.y;
            if (TrySendData(tmp, newPacked.deltaServerVelocityY) == false) return false;

            tmp = rb.linearVelocity.z - lastVelocity.z;
            if (TrySendData(tmp, newPacked.deltaServerVelocityZ) == false) return false;

            lastVelocity = rb.linearVelocity;


            tmp = rb.rotation.x - lastRotation.x;
            if (TrySendData(tmp, newPacked.deltaServerRotationX) == false) return false;

            tmp = rb.rotation.y - lastRotation.y;
            if (TrySendData(tmp, newPacked.deltaServerRotationY) == false) return false;

            tmp = rb.rotation.z - lastRotation.z;
            if (TrySendData(tmp, newPacked.deltaServerRotationZ) == false) return false;

            lastRotation = rb.rotation.eulerAngles;


            tmp = rb.angularVelocity.x - lastAngularVelocity.x;
            if (TrySendData(tmp, newPacked.deltaServerAngularVelocityX) == false) return false;

            tmp = rb.angularVelocity.y - lastAngularVelocity.y;
            if (TrySendData(tmp, newPacked.deltaServerAngularVelocityY) == false) return false;

            tmp = rb.angularVelocity.z - lastAngularVelocity.z;
            if (TrySendData(tmp, newPacked.deltaServerAngularVelocityZ) == false) return false;

            lastAngularVelocity = rb.angularVelocity;



            _packed = newPacked;
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

        bool TrySendData(float x, short sender)
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
            Vector3 tmp = new Vector3();
            tmp.x = _packed.deltaServerPositionX / 100;
            tmp.y = _packed.deltaServerPositionY / 100;
            tmp.z = _packed.deltaServerPositionZ / 100;

            positionError = lastPosition + tmp - rb.position;
            lastPosition = tmp;

            tmp.x = _packed.deltaServerRotationX / 100;
            tmp.y = _packed.deltaServerRotationY / 100;
            tmp.z = _packed.deltaServerRotationZ / 100;

            Quaternion tmpQuat = Quaternion.Euler(tmp.x, tmp.y, tmp.z);
            Quaternion lastTmpQuat = Quaternion.Euler(lastRotation.x, lastRotation.y, lastRotation.z);

            rotationError = tmpQuat * Quaternion.Inverse(rb.rotation);
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            lastRotation = tmpQuat.eulerAngles;


            tmp.x = _packed.deltaServerVelocityX / 100;
            tmp.y = _packed.deltaServerVelocityY / 100;
            tmp.z = _packed.deltaServerVelocityZ / 100;

            velocityBias =lastVelocity + tmp - rb.linearVelocity;
            lastVelocity = tmp;

            tmp.x = _packed.deltaServerAngularVelocityX / 100;
            tmp.y = _packed.deltaServerAngularVelocityY / 100;
            tmp.z = _packed.deltaServerAngularVelocityZ / 100;

            angularVelocityBias =lastAngularVelocity + tmp - rb.angularVelocity;
            lastAngularVelocity = tmp;
        }
    }
}