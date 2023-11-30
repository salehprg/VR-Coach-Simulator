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

    string currentBoneError = "";
    public static GameObject coach;
    public GameObject _coach;

    AudioSource audioSource;
    string pendingVoice = "";

    List<BoneCompare> boneCompares = new();
    public float delay = 0.2f;
    public float rotationSpeed = 5.0f;
    public float minAngle = 5.0f;
    public bool visualizeZ = false;

    [SerializeField]
    PoseWorldLandmarkListAnnotationController poseWorldLandmarkListAnnotationController;
    float time;

    private void Awake()
    {
        instance = this;
        if (coach == null)
            coach = _coach;

        avatarBone = coach.GetComponentInChildren<BoneData>();
    }

    void Start()
    {
        if (coach_prefab == null)
            coach_prefab = _coach_prefab;

        var coachAvatar = Instantiate(coach_prefab, coach_place.position, coach_place.rotation);
        avatarBone = coachAvatar.GetComponent<BoneData>();
        audioSource = GetComponent<AudioSource>();

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

        if (!string.IsNullOrEmpty(pendingVoice) && !audioSource.isPlaying)
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

    public void SetNewPos(string poseName)
    {
        var description = File.ReadAllText($"./Data/Frame {poseName}_desc.txt");
        poseDescription.text = description;

        var fileData = File.ReadAllText($"./Data/Frame {poseName}.json");
        List<AnimData> animDatas = JsonConvert.DeserializeObject<List<AnimData>>(fileData);

        var boneData = avatarBone.GetComponent<BoneData>();
        boneData.SetData(animDatas);

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
            pendingVoice = error;
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
