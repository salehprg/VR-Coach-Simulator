using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.AI;

public class GameManager : MonoBehaviour
{
    public ExtractAnim avatarBone;
    public ExtractAnim playerBone;
    public TMP_Text scoreValue;
    public bool call;
    public static GameManager instance;
    public float score = 0;
    public static Material skyMaterial;

    List<Transform> avatar_bones;
    List<Transform> player_bones;
    List<BoneCompare> boneCompares = new List<BoneCompare>();

    public float delay = 0.2f;
    float time;

    void Start()
    {
        instance = this;
        avatar_bones = avatarBone.bones;
        player_bones = playerBone.bones;

        for (int i = 0; i < avatar_bones.Count; i++)
        {
            var bone = avatar_bones[i];

            var targetBone = player_bones.Find(x => x.name == bone.name);
            var player_bone_comp = targetBone.gameObject.AddComponent<BoneCompare>();

            player_bone_comp.targetBone = bone;
            player_bone_comp.limit_Angle = 5.0f;
            boneCompares.Add(player_bone_comp);
        }

        RenderSettings.skybox = skyMaterial;
    }

    void Update()
    {
        if (time > Time.time)
        {
            return;
        }
        time = Time.time + delay;

        float _score = 0.0f;
        int checkCount = 0;
        foreach (var bone in boneCompares)
        {
            if (bone.should_check)
            {
                _score += bone.similarity;
                checkCount++;
            }
        }

        score = _score / checkCount;

        scoreValue.text = Math.Round(score * 100, 2).ToString();
    }
}
