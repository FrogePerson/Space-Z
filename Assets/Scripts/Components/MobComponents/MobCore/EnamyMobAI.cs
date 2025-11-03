using System;
using System.Collections;
using UnityEngine;

namespace Components.MobComponents
{
    public class EnamyMobAI : MobAICore
    {
        protected override void OnIdle()
        {
            base.OnIdle();
            Debug.Log("OnIdle..");
        }

        protected override void OnStanding()
        {
            base.OnStanding();
            Debug.Log("OnStandingEnamy..");
        }

        protected override void InternalCheckStateCollisions()
        {
            base.InternalCheckStateCollisions();
            
        }
    }
}