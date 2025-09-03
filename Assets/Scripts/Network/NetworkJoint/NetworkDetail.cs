using System.Collections;
using UnityEngine;

namespace Network.NetworkJoint
{
    public class NetworkDetail : MonoBehaviour
    {
        public GameObject Parent;
        public NetworkJoint Joint;//может быть NULL

        void Start ()
        {
            Joint = transform.parent.GetComponent<NetworkJoint>();
        }
    }
}