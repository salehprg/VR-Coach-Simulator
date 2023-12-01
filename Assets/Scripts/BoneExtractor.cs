using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class BoneExtractor : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    public float _frame;
    public int maxFrame;
    public bool doesntMatter;

    Animator anim;
    public void SetFrame(float frame)
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetFloat("Frame", frame);
        anim.Update(Time.deltaTime);
    }

    public void ExtractBoneData()
    {
        var extract = gameObject.GetComponent<BoneData>();

        var animDatas = extract.ExtractData();

        var jsonData = JsonConvert.SerializeObject(animDatas, Formatting.Indented);
        File.WriteAllText($"./Frame {_frame}.json", jsonData);
    }

    public void SetBoneData(int frameIndx)
    {
        var boneData = File.ReadAllText($"./Frame {frameIndx}.json");
        List<FrameData> animDatas = JsonConvert.DeserializeObject<List<FrameData>>(boneData);

        var extract = gameObject.GetComponent<BoneData>();
        extract.SetData(animDatas);
    }
}


[CustomEditor(typeof(BoneExtractor))]
public class MyScriptEditor : Editor
{
    public int index;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = target as BoneExtractor;
        myScript._frame = EditorGUILayout.FloatField("Frame Indx", myScript._frame);
        index = EditorGUILayout.IntField("Load Frame Indx", index);

        if (GUILayout.Button("Preview"))
        {
            myScript.SetFrame(myScript._frame);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Extract Data"))
        {
            myScript.ExtractBoneData();
        }

        if (GUILayout.Button("Load Data"))
        {
            myScript.SetBoneData(index);
        }
        EditorGUILayout.EndHorizontal();
    }
}