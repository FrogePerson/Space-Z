using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.MobComponents
{
    public class MobStateMachine<T> where T : System.Enum, IConvertible
    {
        [SerializeField]
        T _mobState;
        [SerializeField]
        public List<T> addedStates = new List<T>();
        public List<T> removedStates = new List<T>();
        public T State
        {
            get { return _mobState; }
            set { _mobState = value; }
        }
        MobOrder _order = MobOrder.HoldPosition;
        public MobOrder Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public virtual void AddState(T state)
        {
            if (!typeof(T).IsEnum) return;

            int stateValue = Convert.ToInt32(State);
            int addValue = Convert.ToInt32(state);
            State = (T)Enum.ToObject(typeof(T), stateValue | addValue);

            OnStateAdded(state);
        }

        public virtual void RemoveState(T state)
        {
            if (!typeof(T).IsEnum) return;

            int stateValue = Convert.ToInt32(State);
            int removeValue = Convert.ToInt32(state);
            State = (T)Enum.ToObject(typeof(T), stateValue & ~removeValue);

            OnStateRemoved(state);
        }

        public bool HasState(T state)
        {
            if (!typeof(T).IsEnum) return false;

            int stateValue = Convert.ToInt32(State);
            int checkValue = Convert.ToInt32(state);
            return (stateValue & checkValue) == checkValue;
        }

        public virtual void SetState(T newState)
        {
            State = newState;
            OnStateSet(newState);
        }

        protected virtual void OnStateAdded(T state)
        {
            addedStates.Add(state);
        }
        protected virtual void OnStateRemoved(T state)
        {
            removedStates.Add(state);
        }
        protected void OnStateSet(T state) { }
    }
}