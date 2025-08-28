using UnityEngine;
using Mirror;

namespace Player.PlayerWeapons
{
    public abstract class PlayerGun : PlayerWeapon
    {
        [SerializeField]
        Pool Pool;

        bool IsPoolExist = true;

        protected override void Start()
        {
            base.Start();
            if (Pool == null ) IsPoolExist = false;
        }
        public override void use()
        {
            shoot();
        }

        protected virtual void shoot()
        {
            if(!IsPoolExist) return;

            Pool.Pop();
        }
    }
}
