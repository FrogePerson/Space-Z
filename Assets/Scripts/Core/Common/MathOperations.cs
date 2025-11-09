using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class MathOperations
    {
        public static float GetDistanceSqr(Transform pos1, Transform pos2)
        {
            Vector3 direction = pos1.position - pos2.position;
            return direction.sqrMagnitude;
        }
    }
}
