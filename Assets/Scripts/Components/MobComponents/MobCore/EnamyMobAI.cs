using Core;
using NUnit.Framework;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Components.MobComponents
{
    public class EnamyMobAI : MobAICore
    {
        protected List<ActivePlayer> players = new List<ActivePlayer>();
        protected List<Transform> playersTransforms = new List<Transform>();
        bool isFinding = false;

        EnamyMob self;
        protected override void Start()
        {
            base.Start();

            self = GetComponent<EnamyMob>();
        }

        void FillPlayersLists()
        {
            GameObject[] _players = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log($"At FillPlayersLists: {_players.Length}");
            foreach (var p in _players)
            {
                players.Add(p.GetComponent<ActivePlayer>());
                playersTransforms.Add(p.GetComponent<Transform>());
                Debug.Log("1");
            }
        }

        protected override void OnIdle()
        {
            base.OnIdle();
            Debug.Log("OnIdle..");
        }

        protected override void OnStanding()
        {
            base.OnStanding();
            //Debug.Log("OnStandingEnamy..");

            if (!isFinding)
            {
                timer.StartFuncCycle(checkDistanceToPlayers, 100);
                isFinding = true;
            }
            
        }

        protected virtual void checkDistanceToPlayers()
        {
            Debug.Log("Finding.....");

            if(playersTransforms.Count <= 0)
            {
                FillPlayersLists();
            }

            float minDist = int.MaxValue;
            foreach (var playerTransform in playersTransforms)
            {
                float distance = MathOperations.GetDistanceSqr(transform, playerTransform);
                Debug.Log($"{playerTransform.name}.{distance}");
                if (distance < minDist) minDist = distance;
            }

            if(minDist < 1000 && baseStateMachine.HasState(BasicMobState.Standing))
            {
                Debug.Log("TO ATTACKING!!");
                ChangeState(BasicMobState.Standing, BasicMobState.Attacking);
            }
            else if(minDist > 3000 && baseStateMachine.HasState(BasicMobState.Attacking))
            {
                Debug.Log("TO STANDING!!");
                ChangeState(BasicMobState.Attacking, BasicMobState.Standing);
            }
        }


        protected override void InternalCheckStateCollisions()
        {
            base.InternalCheckStateCollisions();
        }
    }
}