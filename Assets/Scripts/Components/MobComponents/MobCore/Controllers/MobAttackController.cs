using System.Collections;
using UnityEngine;

namespace Components.MobComponents.Controllers
{
    public class MobAttackController : MonoBehaviour
    {
        protected enum BaseAttackPhase
        {
            None,
            start,
            goingToTarget,

        }

        public EnamyMobAI mobAI;

        [SerializeField]
        public Transform attackTarget;
        public Transform attackedPlayer = null;
        protected MobNavigationController navController;
        protected EnamyMobAI enamyMobAI;
        protected BaseAttackPhase attackPhase = BaseAttackPhase.start;

        protected virtual void Start()
        {
            navController = GetComponent<MobNavigationController>();
            enamyMobAI = GetComponent<EnamyMobAI>();
        }

        public virtual void Attack()
        {
            
        }
        public virtual void Stop()
        {

        }
    }
}