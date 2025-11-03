using Components.MobComponents;
using System.Collections;
using UnityEngine;

namespace Components.MobComponents.Mobs.SpaceX.Spiderman
{
    public class SpidermanAI : EnamyMobAI
    {
        protected override void OnIdle()
        {
            base.OnIdle();

            
        }

        protected override void OnStanding()
        {
            base.OnStanding();
            Debug.Log("OnStanding..");
        }
    }
}