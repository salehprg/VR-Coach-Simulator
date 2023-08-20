using System;
using UnityEngine;

public class BoneCompare : MonoBehaviour
{
    public Transform targetBone;
    public float limit_Angle;
    public float similarity = 0;

    public bool debug;

    public float delay;
    float time;
    void Update()
    {
        if (time > Time.time)
        {
            return;
        }

        var angle = Quaternion.Angle(transform.localRotation, targetBone.localRotation);

        similarity = (90 - Mathf.Clamp(angle - limit_Angle, 0 , 90)) / 90;

        if (angle <= limit_Angle)
        {
            if (debug)
                print("OK !");
        }
        else
        {
            if (debug)
            {
                Debug.DrawLine(transform.position, transform.position + transform.localRotation.normalized.eulerAngles * 10);
                print($"Failed ! {angle}");
            }
        }

        time = Time.time + delay;
    }
}
