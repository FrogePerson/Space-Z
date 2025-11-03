using System.Collections;
using UnityEngine;


public class PhisycRaggDoll : MonoBehaviour
{
    [SerializeField] 
    Transform _target;
    ConfigurableJoint _joint;
    Quaternion _startRotation;

    void Start()
    {
        _joint = GetComponent<ConfigurableJoint>();
        _startRotation = transform.localRotation;
    }

    void FixedUpdate()
    {
        _joint.targetRotation = Quaternion.Inverse(_target.localRotation) * _startRotation;
    }
}
