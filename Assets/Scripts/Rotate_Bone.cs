using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Bone : MonoBehaviour
{
    public Transform targetBone;
    public Transform myPrefab;
    public string bone_name;
    public float delay;
    // Start is called before the first frame update
    void Start()
    {
        targetBone = GameManager.instance.avatarBone.bones.Find(x => x.name == name);

        delay = GameManager.instance.delay;
        bone_name = name.Replace("mixamorig:", "");
    }

    // Update is called once per frame
    void Update()
    {
        myPrefab.transform.rotation = targetBone.transform.rotation;
    }
}
