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


[Serializable]
public class StringIntPair
{
    public int MediaPipe1;
    public int MediaPipe2 = -1;
    public string AvatarBone;
}

public class MediapipeGameManager : MonoBehaviour
{
    public MyPoseListAnnotation listAnnotation;

    [SerializeField] private List<StringIntPair> exposedDictionary;
    [SerializeField] private BoneData boneDatas;

    public Dictionary<(int, int), string> bonesDictionary;

    void Awake()
    {
        bonesDictionary = new Dictionary<(int, int), string>();

        foreach (StringIntPair pair in exposedDictionary)
            bonesDictionary[(pair.MediaPipe1, pair.MediaPipe2)] = pair.AvatarBone;
    }

    public void InitBoneTransfer()
    {
        var connections = listAnnotation.getConnections();
        var landmarks = listAnnotation.getLandmarks();

        foreach (var mediaIndex in bonesDictionary.Keys)
        {
            var foundBone = boneDatas.bones.Find(x => x.name.EndsWith(bonesDictionary.GetValueOrDefault(mediaIndex)));
            if (foundBone != null)
            {
                var pair = connections.Find(x => x.Item1 == mediaIndex.Item1 && (mediaIndex.Item2 != -1 ? x.Item2 == mediaIndex.Item2 : true));
                foundBone.GetComponent<Rotate_Bone>().SetTarget(landmarks[pair.Item1].transform, landmarks[pair.Item2].transform);
            }
        }
    }
}
