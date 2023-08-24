using System;
using UnityEngine;

public class BoneCompare : MonoBehaviour
{
    public Transform targetBone;
    public string bone_name;
    public float limit_Angle;
    public float similarity = 0;

    public bool debug;
    public bool should_check = false;

    public float delay;
    float time;

    private void Start()
    {
        bone_name = name.Replace("mixamorig:", "");
    }

    void Update()
    {
        if (!should_check || time > Time.time)
        {
            return;
        }

        var angle = Quaternion.Angle(transform.localRotation, targetBone.localRotation);

        similarity = (90 - Mathf.Clamp(angle - limit_Angle, 0, 90)) / 90;

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
                if (angle > 0)
                    print($"+ Your {bone_name} is {angle} degree over");
                else
                    print($"- Your {bone_name} is {angle} degree under");
            }
        }

        time = Time.time + delay;
    }
}
