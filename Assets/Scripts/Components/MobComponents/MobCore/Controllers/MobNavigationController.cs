using Mirror;
using System.Collections;
using UnityEngine;

namespace Components.MobComponents.Controllers
{
    public class MobNavigationController : NetworkBehaviour
    {
        [SerializeField]
        public Transform moveTarget;

        protected MobDamageController damageController;

        protected virtual void Start()
        {
            damageController = GetComponent<MobDamageController>();
        }
        public virtual void Move()
        {

        }
        public virtual void Stand()
        {

        }
        public virtual void Stop()
        {

        }
        public virtual void Jump()
        {

        }
        public virtual void  Fail()
        {

        }
    }
}