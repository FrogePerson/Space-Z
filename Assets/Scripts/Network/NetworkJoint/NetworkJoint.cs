using Mirror;
using System.Collections;
using UnityEngine;

namespace Network.NetworkJoint
{
    public class NetworkJoint : NetworkBehaviour
    {
        [SerializeField]
        GameObject networkObj;// можно сделать NULL

        [SerializeField]
        uint _breakForce = 30;
        public uint BreakForce
        {
            get { return _breakForce; }
            private set { _breakForce = value; }
        }

        [ClientRpc]
        public void BreakSelf()
        {
            if (networkObj == null)
            {
                gameObject.AddComponent<Rigidbody>();
                gameObject.transform.SetParent(null);
                return;
            }

            networkObj.SetActive(true);
            networkObj.transform.SetParent(null);

            gameObject.SetActive(false);
            //gameObject.AddComponent<NetworkTransformUnreliable>(); //хуйня т.к. NetworkTransformUnreliable пытается работать
            //до инициализации NetworkIdentity в общем надо включать network объект и выключать не Network
        }
    }
}