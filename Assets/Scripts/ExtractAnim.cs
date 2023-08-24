using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class ExtractAnim : MonoBehaviour
{
    public Transform rootBone;
    public List<Transform> bones;

    void Awake()
    {
        bones = rootBone.GetComponentsInChildren<Transform>().ToList();
    }
    

    public Vector3 convertToVec3(string value)
    {
        var _values = value.Replace("(", "").Replace(")", "").Split(",");

        return new Vector3(float.Parse(_values[0]), float.Parse(_values[1]), float.Parse(_values[2]));
    }
    public Quaternion convertToQuaternion(string value)
    {
        var _values = value.Replace("(", "").Replace(")", "").Split(",");

        return new Quaternion(float.Parse(_values[0]), float.Parse(_values[1]), float.Parse(_values[2]), float.Parse(_values[3]));
    }

    public void SetData(List<AnimData> animDatas)
    {
        var childs = rootBone.GetComponentsInChildren<Transform>();

        for (int i = 0; i < childs.Length; i++)
        {
            var animData = animDatas[i];
            var bone = childs[i];

            bone.position = convertToVec3(animData.position);
            bone.rotation = convertToQuaternion(animData.rotation);
        }
    }

    public List<AnimData> ExtractData()
    {
        List<AnimData> animDatas = new List<AnimData>();

        for (int i = 0; i < bones.Count; i++)
        {
            var bone = bones[i];

            var data = new AnimData
            {
                boneName = bone.name,
                position = bone.position.ToString(),
                rotation = bone.rotation.ToString()
            };
            animDatas.Add(data);
        }

        return animDatas;
    }
}
