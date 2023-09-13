using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class BoneData : MonoBehaviour
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

            // bone.position = convertToVec3(animData.position);
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


[CustomEditor(typeof(BoneData))]
public class BoneData_Editor : Editor
{
    public int index;
    public string oldName;
    public string newName;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = target as BoneData;

        if (GUILayout.Button("Load BoneData"))
        {
            myScript.bones = myScript.rootBone.GetComponentsInChildren<Transform>().ToList();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Old Name");
        oldName = GUILayout.TextField("mixamorig6:");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("New Name");
        newName = GUILayout.TextField("mixamorig:");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Rename Bones"))
        {
            for (int i = 0; i < myScript.bones.Count; i++)
            {
                var bone = myScript.bones[i];

                if (bone.name.Contains(oldName))
                {
                    bone.name = bone.name.Replace(oldName, newName);
                }
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add BoneCompare Script"))
        {
            for (int i = 0; i < myScript.bones.Count; i++)
            {
                var bone = myScript.bones[i];

                var player_bone_comp = bone.gameObject.AddComponent<BoneCompare>();

                player_bone_comp.limit_Angle = 7.5f;
            }
        }
        if (GUILayout.Button("Remove BoneCompare Script"))
        {
            for (int i = 0; i < myScript.bones.Count; i++)
            {
                var bone = myScript.bones[i];

                var comp = bone.gameObject.GetComponent<BoneCompare>();
                if (comp != null)
                    DestroyImmediate(comp);
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add Bone Follow Script"))
        {
            for (int i = 0; i < myScript.bones.Count; i++)
            {
                var bone = myScript.bones[i];

                var rotate = bone.gameObject.AddComponent<Rotate_Bone>();
                
            }
        }

    }
}