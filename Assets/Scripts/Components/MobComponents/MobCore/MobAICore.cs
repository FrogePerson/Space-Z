using Components.MobComponents.Controllers;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Components.MobComponents
{
    public class MobAICore: NetworkBehaviour
    {
        protected MobStateMachine<BasicMobState> baseStateMachine;
        protected Dictionary<string, Action> stateHandlers = new Dictionary<string, Action>();
        protected List<Action> activeStates = new List<Action>();

        protected MobAttackController attackController;
        protected MobNavigationController navigationController;
        protected MobDamageController damageController;

        [SerializeField]
        BasicMobState stateView;

        static Dictionary<BasicMobState, string> _defaultStatesMap;
        static protected Dictionary<BasicMobState, string> defaultStatesMap
        {
            get
            {
                if (_defaultStatesMap == null)
                {
                    _defaultStatesMap = new Dictionary<BasicMobState, string>
                    {
                        { BasicMobState.Idle, "Idle" },
                        { BasicMobState.Moving, "Moving" },
                        { BasicMobState.Standing, "Standing" },
                        { BasicMobState.Stopped, "Stopped" },
                        { BasicMobState.Jumping, "Jumping" },
                        { BasicMobState.Falled, "Falled" },
                        { BasicMobState.Dying, "Dying" },
                        { BasicMobState.Dead, "Dead" },
                        { BasicMobState.Attacking, "Attacking" },
                        { BasicMobState.Stunned, "Stunned" },
                        { BasicMobState.Damaged, "Damaged" }
                    };
                }
                return _defaultStatesMap;
            }
        }
        protected virtual void Start()
        {
            baseStateMachine = new MobStateMachine<BasicMobState>();
            baseStateMachine.SetState(BasicMobState.Idle);

            stateHandlers[defaultStatesMap[BasicMobState.Idle]] = OnIdle;
            stateHandlers[defaultStatesMap[BasicMobState.Moving]] = OnMoving;
            stateHandlers[defaultStatesMap[BasicMobState.Standing]] = OnStanding;
            stateHandlers[defaultStatesMap[BasicMobState.Stopped]] = OnStopped;
            stateHandlers[defaultStatesMap[BasicMobState.Jumping]] = OnJumping;
            stateHandlers[defaultStatesMap[BasicMobState.Falled]] = OnFalled;
            stateHandlers[defaultStatesMap[BasicMobState.Dying]] = OnDying;
            stateHandlers[defaultStatesMap[BasicMobState.Dead]] = OnDead;
            stateHandlers[defaultStatesMap[BasicMobState.Attacking]] = OnAttacking;
            stateHandlers[defaultStatesMap[BasicMobState.Stunned]] = OnStunned;
            stateHandlers[defaultStatesMap[BasicMobState.Damaged]] = OnDamaged;

            activeStates.Add(stateHandlers[defaultStatesMap[BasicMobState.Idle]]);

            setControllers();
        }

        protected virtual void setControllers()
        {
            attackController = GetComponent<MobAttackController>();
            navigationController = GetComponent<MobNavigationController>();
            damageController = GetComponent<MobDamageController>();
        }

        protected virtual void Update()
        {
            stateView = baseStateMachine.State;
            UpdateState();
        }

        protected virtual void CheckStateCollisions()
        {
            if((baseStateMachine.addedStates.Count <= 0) && (baseStateMachine.removedStates.Count <= 0)) return;



            baseStateMachine.addedStates.Clear();
            baseStateMachine.removedStates.Clear();
        }

        protected virtual void UpdateState()
        {
            foreach (var activeState in activeStates)
            {
                activeState();
            }
        }

        protected virtual void OnIdle() { }
        protected virtual void OnMoving() { navigationController.Move(); }
        protected virtual void OnStanding() { navigationController.Stand(); }
        protected virtual void OnStopped() { navigationController.Stop(); attackController.Stop(); }
        protected virtual void OnJumping() { navigationController.Jump(); }
        protected virtual void OnFalled() { damageController.Fail(); }
        protected virtual void OnDying() { damageController.Die(); }
        protected virtual void OnDead() { damageController.OnDead(); }
        protected virtual void OnAttacking() { attackController.Attack(); }
        protected virtual void OnStunned() { damageController.Stunning(); }
        protected virtual void OnDamaged() { damageController.GetDamage(); }

    }
}
