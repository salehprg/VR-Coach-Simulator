using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Bone : MonoBehaviour
{
    public Transform targetBone;

    void Start()
    {
        targetBone = GameManager.instance.playerBone.bones.Find(x => x.name == name);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = targetBone.localRotation;
    }
}
