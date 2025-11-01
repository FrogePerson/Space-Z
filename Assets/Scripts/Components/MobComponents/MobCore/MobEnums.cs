using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.MobComponents
{
    [System.Flags]
    public enum BasicMobState
    {
        None = 0,
        Idle = 1 << 0,
        Moving = 1 << 1,
        Standing = 1 << 2,
        Stopped = 1 << 3,
        Jumping = 1 << 4,
        Falled = 1 << 5,
        Dying = 1 << 6,
        Dead = 1 << 7,
        Attacking = 1 << 8,
        Stunned = 1 << 9,
        Damaged = 1 << 10,
    }

    public enum MobOrder
    {
        None,
        Attack,
        Move,
        Patrol,
        HoldPosition
    }
}
