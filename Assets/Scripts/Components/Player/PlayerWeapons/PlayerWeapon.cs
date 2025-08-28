using UnityEngine;
using Mirror;

namespace Player.PlayerWeapons
{
    public abstract class PlayerWeapon : NetworkBehaviour
    {
        protected virtual void Start()
        {
            //if(transform.parent != null)
            //{
            //    NetworkIdentity identity = GetComponent<NetworkIdentity>();

            //    Destroy(GetComponent<NetworkIdentity>());
            //}
        }

        public abstract void use();
    }
}