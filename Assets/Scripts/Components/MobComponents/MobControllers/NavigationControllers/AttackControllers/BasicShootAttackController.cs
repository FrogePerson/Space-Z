using Components.MobComponents.Controllers;
using System.Collections;
using UnityEngine;

namespace Components.MobComponents.Controllers
{
    public class BasicShootAttackController : MobAttackController
    {
        public override void Attack()
        {
            base.Attack();
            if(attackPhase == BaseAttackPhase.start)
            {
                attackedPlayer = enamyMobAI.GetNearestPlayerTransform();
                attackTarget.position = attackedPlayer.position;
                attackPhase = BaseAttackPhase.goingToTarget;

                navController.moveTarget.position = attackTarget.position;
            }
            attackTarget.position = attackedPlayer.position;
            navController.moveTarget.position = attackTarget.position;
        }
    }
}