using UnityEngine;
using Mirror;

namespace Player.PlayerWeapons
{
    public class PlayerWeaponController : NetworkBehaviour
    {
        [SerializeField]
        PlayerWeapon weapon;

        [Command]
        void CmdUse()
        {
            weapon.use();
        }

        void Update()
        {
            if(isLocalPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    CmdUse();
                }
            }         
        }
    }
}