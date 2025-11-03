using System.Collections;
using UnityEngine;
using Mirror;

namespace Components.MobComponents.Controllers
{
    public class MobAnimationController : NetworkBehaviour
    {
        [SerializeField]
        GameObject animatedObj;
        Animator animator;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
        }

        public virtual void SetAnimation(string animationName)
        {
            animator.SetTrigger(animationName);
        }
    }
}