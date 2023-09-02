using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Bone : MonoBehaviour
{
    public Transform targetBone;

    public float delay;
    float time = 0.2f;
    void Start()
    {
        targetBone = GameManager.instance.playerBone.bones.Find(x => x.name == name);

        delay = GameManager.instance.delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > Time.time)
        {
            return;
        }

        transform.localRotation = targetBone.localRotation;

        time = Time.time + delay;
    }
}
