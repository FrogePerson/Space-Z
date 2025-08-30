using Mirror;
using System.Collections;
using UnityEngine;

namespace Network.NetworkJoint
{
    public class NetworkJoint : NetworkBehaviour
    {
        [SerializeField]
        GameObject networkObj;

        [ClientRpc]
        public void BreakSelf()
        {
            transform.SetParent(null);
            gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<NetworkIdentity>();
            //gameObject.AddComponent<NetworkTransformUnreliable>(); //хуйня т.к. NetworkTransformUnreliable пытается работать
            //до инициализации NetworkIdentity в общем надо включать network объект и выключать не Network
        }
    }
}