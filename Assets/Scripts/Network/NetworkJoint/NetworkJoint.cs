using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Network.NetworkJoint
{
    public class NetworkJoint : NetworkBehaviour
    {
        [SerializeField]
        GameObject networkObj;// можно сделать NULL

        [SerializeField]
        uint _breakForce = 30;

        [SerializeField]
        int contacts = 0;
        [SerializeField]
        List<NetworkJoint> joints = new List<NetworkJoint>();

        public uint BreakForce
        {
            get { return _breakForce; }
            private set { _breakForce = value; }
        }

        #region Client RPCs

        [ClientRpc]
        public void RpcBreakSelf()
        {
            if (networkObj == null)
            {
                gameObject.AddComponent<Rigidbody>();
                gameObject.transform.SetParent(null);
                return;
            }

            foreach (NetworkJoint joint in joints)
            {
                joint.RemoveConns(gameObject);
            }

            networkObj.SetActive(true);
            networkObj.transform.SetParent(null);

            gameObject.SetActive(false);
            //gameObject.AddComponent<NetworkTransformUnreliable>(); //хуйня т.к. NetworkTransformUnreliable пытается работать
            //до инициализации NetworkIdentity в общем надо включать network объект и выключать не Network
        }

        #endregion

        #region Server



        #endregion

        #region Commands



        #endregion

        #region Common

        void BreakSelf()
        {
            if (networkObj == null)
            {
                gameObject.AddComponent<Rigidbody>();
                gameObject.transform.SetParent(null);
                return;
            }

            foreach (NetworkJoint joint in joints)
            {
                joint.RemoveConns(gameObject);
            }

            networkObj.SetActive(true);
            networkObj.transform.SetParent(null);

            gameObject.SetActive(false);
            //gameObject.AddComponent<NetworkTransformUnreliable>(); //хуйня т.к. NetworkTransformUnreliable пытается работать
            //до инициализации NetworkIdentity в общем надо включать network объект и выключать не Network
        }

        void Start()
        {
            contacts = joints.Count;

            foreach (NetworkJoint joint in joints)
            {
                joint.AddConns(gameObject);
            }
            networkObj.SetActive(false);
        }

        public void AddConns(GameObject[] objects)
        {
            foreach (GameObject obj in objects)
            {
                var tmp = obj.GetComponent<NetworkJoint>();

                if (tmp != null && !joints.Contains(tmp))
                {
                    joints.Add(tmp);
                    contacts++;
                }
            }
        }

        public void AddConns(GameObject obj)
        {
            var tmp = obj.GetComponent<NetworkJoint>();

            if (tmp != null && !joints.Contains(tmp))
            {
                joints.Add(tmp);
                contacts++;
            }
        }

        public void RemoveConns(GameObject[] objects)
        {
            foreach (GameObject obj in objects)
            {
                var tmp = obj.GetComponent<NetworkJoint>();
                if (tmp != null && joints.Contains(tmp))
                {
                    joints.Remove(tmp);
                    contacts--;
                    if (contacts <= 0)
                    {
                        BreakSelf();
                    }
                }
            }
        }

        public void RemoveConns(GameObject obj)
        {
            var tmp = obj.GetComponent<NetworkJoint>();
            if (tmp != null && joints.Contains(tmp))
            {
                joints.Remove(tmp);
                contacts--;
                if (contacts <= 0)
                {
                    BreakSelf();
                }
            }
        }

        void aoutoAddCons()//может быть в теории когда то будет к 2030
        {

        }

        #endregion


    }
}