using Interfaces;
using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Components.MobComponents
{
    public abstract class Mob : NetworkBehaviour, IDamageable
    {
        [SyncVar(hook = nameof(OnHpChanged))]
        [SerializeField]
        int _hp = 100;

        [SerializeField]
        protected Transform attackTarget;
        [SerializeField]
        protected Transform moveTarget;
        public int Hp
        {
            get
            {
                return _hp;
            }
            private set
            {
                _hp = value;
            }
        }

        [SyncVar(hook = nameof(OnMaxHpChanged))]
        [SerializeField]
        int _maxHp = 100;
        public int MaxHp
        {
            get
            {
                return _maxHp;
            }
            private set
            {
                _maxHp = value;
            }
        }

        MobAICore mobAICore;

        protected virtual void Start()
        {
            mobAICore = GetComponent<MobAICore>();
        }


        virtual protected void OnHpChanged(int oldValue, int newValue) { }
        virtual protected void OnMaxHpChanged(int oldValue, int newValue) { }
        virtual protected void Die() { }
        virtual protected void Attack() { }
        virtual protected void _TakeDamage(int damage)
        {
        }
        virtual public void OrderMove()
        {

        }
        virtual public void OrderStop()
        {

        }

        public void TakeDamage(int damage)
        {
            _TakeDamage(damage);
        }
    }
}