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
            Debug.Log("On Idle...");
        }
    }
}