using System.Diagnostics.Contracts;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.AI;
using System.IO;
using Newtonsoft.Json;
using LMNT;
using Mediapipe.Unity;

public class GameManager : MonoBehaviour
{
    public BoneData avatarBone;
    public BoneData playerBone;
    public TMP_Text frameText;
    public TMP_Text scoreValue;
    public TMP_Text errorText;
    public TMP_Text poseDescription;
    public static GameManager instance;
    public float score = 0;
    public static Material skyMaterial;
    public List<string> bonesToCheck;
    public Transform coach_place;
    public static GameObject coach_prefab;
    public GameObject _coach_prefab;
    public AnimationPlayer animationPlayer;
    public Transform trainingsParent;
    public GameObject trainingButton;

    string currentBoneError = "";

    AudioSource audioSource;
    string pendingVoice = "";

    public List<BoneCompare> boneCompares = new();
    public float delay = 0.2f;
    public float rotationSpeed = 5.0f;
    public float minAngle = 5.0f;
    public float Visibility = 0.7f;
    public float nextMoveScore = 0.7f;

    public bool showError = false;
    public bool playVoice = false;
    public bool visualizeZ = false;
    public bool rotate90Degree = false;

    [SerializeField]
    MyPoseWorldLanMarkController poseWorldLanMarkController;
    float time;

    public List<Transform> trainButtosTransforms;
    List<AnimationData> animationDatas;


    private void Awake()
    {
        instance = this;
        // if (coach == null)
        //     coach = _coach;

        // avatarBone = coach.GetComponentInChildren<BoneData>();
    }

    void Start()
    {
        // if (coach_prefab == null)
        //     coach_prefab = _coach_prefab;

        // var coachAvatar = Instantiate(coach_prefab, coach_place.position, coach_place.rotation);
        // avatarBone = coachAvatar.GetComponent<BoneData>();
        audioSource = GetComponent<AudioSource>();

        boneCompares = playerBone.transform.GetComponentsInChildren<BoneCompare>().ToList();
        RenderSettings.skybox = skyMaterial;

        if (rotate90Degree)
        {
            poseWorldLanMarkController.transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        animationPlayer = GetComponent<AnimationPlayer>();

        LoadAnimations();
    }

    public void LoadAnimations()
    {
        animationDatas = new List<AnimationData>();

        trainButtosTransforms = trainingsParent.GetComponentsInChildren<Transform>().ToList();
        foreach (var trans in trainButtosTransforms)
        {
            if (trans != trainingsParent)
                Destroy(trans.gameObject);
        }

        string prefix = "Animation";

        string[] animationDirectories = Directory.GetDirectories(Directory.GetCurrentDirectory(), $"{prefix}*");

        foreach (var directory in animationDirectories)
        {
            var animData = File.ReadAllText($"{directory}/animData.data");
            AnimationData animationData = JsonConvert.DeserializeObject<AnimationData>(animData);

            animationDatas.Add(animationData);
            var temp = Instantiate(trainingButton, trainingsParent);
            var uiComp = temp.GetComponent<UI_AnimationSelector>();
            uiComp.AnimationName = animationData.animationName;
            uiComp.DisplayName = animationData.animationName;
        }
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

        if (score >= nextMoveScore)
        {
            NextFrame();
        }

        if (scoreValue != null)
            scoreValue.text = Math.Round(score * 100, 2).ToString();

        if (playVoice && !string.IsNullOrEmpty(pendingVoice) && !audioSource.isPlaying)
        {
            var oldSpeech = gameObject.GetComponent<LMNTSpeech>();

            if (oldSpeech != null)
            {
                Destroy(oldSpeech);
            }

            var speech = gameObject.AddComponent<LMNTSpeech>();
            speech.dialogue = pendingVoice;
            speech.voice = oldSpeech.voice;

            audioSource.clip = null;
            StartCoroutine(speech.Talk());

            pendingVoice = "";
        }
    }

    public void NextFrame()
    {
        animationPlayer.NextFrame();

        if (frameText != null)
        {
            frameText.text = animationPlayer.displayedFrame.ToString();
        }

        foreach (var checkBone in bonesToCheck)
        {
            var bone = boneCompares.Find(x => x.bone_name == checkBone);
            if (bone != null)
                bone.should_check = true;
            else
                bone.should_check = false;
        }
    }

    public void StartTraining(string animName)
    {
        animationPlayer.SetAnimation(animName);

        NextFrame();
    }

    public void ShowError(string boneName, string error)
    {
        if (showError)
        {
            if (currentBoneError == "" || (currentBoneError == boneName && error != errorText.text))
            {
                pendingVoice = error;
                currentBoneError = boneName;

                if (errorText != null)
                    errorText.text = error;
            }
        }
    }

    public void ClearError(string boneName)
    {
        if (currentBoneError == boneName)
        {
            currentBoneError = "";

            if (errorText != null)
                errorText.text = "";
        }
    }
}
