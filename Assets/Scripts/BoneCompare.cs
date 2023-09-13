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
        delay = GameManager.instance.delay;
        bone_name = name.Replace("mixamorig:", "");
    }

    void Update()
    {
        if (targetBone == null)
        {
            if (GameManager.instance.avatarBone != null)
                targetBone = GameManager.instance.avatarBone.bones.Find(x => x.name == name);
            return;
        }

        if (!should_check || time > Time.time)
        {
            return;
        }

        var angle = Quaternion.Angle(transform.localRotation, targetBone.localRotation);

        similarity = (90 - Mathf.Clamp(angle - limit_Angle, 0, 90)) / 90;

        if (angle <= limit_Angle)
        {
            if (debug)
            {
                print("OK !");
            }
            GameManager.instance.ClearError(bone_name);
        }
        else
        {
            if (debug)
            {
                Debug.DrawLine(transform.position, transform.position + transform.localRotation.normalized.eulerAngles * 10);
                print($"Failed ! {angle}");
            }

            string errorText = "";
            string degreeHelperText = "";

            if (angle < 30)
            {
                degreeHelperText = "slightly";
            }
            else
            {
                degreeHelperText = "much";
            }

            if (angle > 0)
            {
                errorText = $"Your {bone_name} is {degreeHelperText} Higher";
            }
            else
            {
                errorText = $"Your {bone_name} is {degreeHelperText} Lower";
            }

            errorText = $"{errorText} about {(int)angle} degree";
            print(errorText);
            GameManager.instance.ShowError(bone_name, errorText);
        }

        time = Time.time + delay;
    }
}
