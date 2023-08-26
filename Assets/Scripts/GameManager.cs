using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.AI;

public class GameManager : MonoBehaviour
{
    public BoneData avatarBone;
    public BoneData playerBone;
    public TMP_Text scoreValue;
    public TMP_Text errorText;
    public static GameManager instance;
    public float score = 0;
    public static Material skyMaterial;
    public List<string> bonesToCheck;
    public Transform coach_place;
    public static GameObject coach;


    string currentBoneError = "";

    List<BoneCompare> boneCompares = new();

    public float delay = 0.2f;
    float time;

    private void Awake()
    {
        instance = this;
        avatarBone = coach.GetComponentInChildren<BoneData>();
    }
    
    void Start()
    {
        Instantiate(coach , coach_place.position , coach_place.rotation);
        
        boneCompares = playerBone.transform.GetComponentsInChildren<BoneCompare>().ToList();
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

    public void SetNewPos()
    {
        foreach (var checkBone in bonesToCheck)
        {
            var bone = boneCompares.Find(x => x.bone_name == checkBone);
            if (bone != null)
                bone.should_check = true;
            else
                bone.should_check = false;
        }
    }

    public void ShowError(string boneName, string error)
    {
        if (currentBoneError == "" || (currentBoneError == boneName && error != errorText.text))
        {
            currentBoneError = boneName;
            errorText.text = error;
        }
    }

    public void ClearError(string boneName)
    {
        if (currentBoneError == boneName)
        {
            currentBoneError = "";
            errorText.text = "";
        }
    }
}
