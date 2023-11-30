using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Bone : MonoBehaviour
{
    public Transform targetBone1;
    public Transform targetBone2;
    public float length;
    public Vector3 minRange = new Vector3(-360, 0, 0);
    public Vector3 maxRange = new Vector3(360, 0, 0);
    public Vector3 offset = new Vector3(90, 0, 0);

    public Vector3 rotation = new Vector3(90, 0, 0);
    public Vector3 _rotation2 = new Vector3(90, 0, 0);

    public bool debug;


    void Start()
    {
        offset = new Vector3(90, 0, 0);
        // targetBone = GameManager.instance.playerBone.bones.Find(x => x.name == name);
    }

    public void SetTarget(Transform _target1, Transform _target2)
    {
        targetBone1 = _target1;
        targetBone2 = _target2;
    }
    // Update is called once per frame
    void Update()
    {
        if (targetBone1 != null && targetBone2 != null)
        {

            // Get the direction from the sourceObject to the targetObject.
            Vector3 direction = targetBone2.position - targetBone1.position;


            // Use the direction to orient the objectToRotate.
            rotation = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            rotation += offset;

            _rotation2.x = Mathf.Clamp(rotation.x, minRange.x, maxRange.x);
            _rotation2.y = Mathf.Clamp(rotation.y, minRange.y, maxRange.y);
            _rotation2.z = Mathf.Clamp(rotation.z, minRange.z, maxRange.z);
            if (minRange.z == maxRange.z)
            {
                _rotation2.z = rotation.z;
            }
            if (minRange.y == maxRange.y)
            {
                _rotation2.y = rotation.y;
            }

            var targetRotation = Quaternion.Euler(_rotation2);

            if (Quaternion.Angle(transform.rotation, targetRotation) > GameManager.instance.minAngle)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * GameManager.instance.rotationSpeed);

            if (debug)
            {
                print($"{gameObject.name} {direction}");
                Debug.DrawLine(targetBone1.position, targetBone1.position + (direction * length), Color.blue, 1);
            }
        }
    }

    Quaternion CalculateRotationDifference(Transform source, Transform target)
    {
        Vector3 forwardDirection = target.position - source.position;
        Quaternion rotation = Quaternion.LookRotation(-forwardDirection, Vector3.up);

        return rotation;
    }
}
