using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    public BoneData recorderAvatar;
    public BoneData displayAvatar;
    public bool record;
    public bool playAnimtaion;

    public int interval = 5;
    public int frameCount = 0;
    public string animationName = "";

    int lastFrame = 0;
    int counter = 0;

    [SerializeField] int displayedFrame = 0;
    void Start()
    {

    }

    string getAnimationPath(string animationName)
    {
        return $"./Animation-{animationName}/";
    }


    void Update()
    {
        counter++;

        if (playAnimtaion)
        {
            var animData = File.ReadAllText($"{getAnimationPath(animationName)}/animData.data");
            AnimationData animationData = JsonConvert.DeserializeObject<AnimationData>(animData);

            if (counter - lastFrame > animationData.interval)
            {
                if (playAnimtaion)
                {
                    var boneData = File.ReadAllText($"{getAnimationPath(animationName)}/Frame {displayedFrame}.json");

                    List<FrameData> animDatas = JsonConvert.DeserializeObject<List<FrameData>>(boneData);
                    displayAvatar.SetData(animDatas);

                    displayedFrame++;

                    if (displayedFrame > animationData.frameCount)
                    {
                        displayedFrame = 0;
                    }
                }
                lastFrame = counter;

            }

            return;
        }

        if (counter - lastFrame > interval)
        {
            if (record)
            {
                if (!Directory.Exists(getAnimationPath(animationName)))
                {
                    Directory.CreateDirectory(getAnimationPath(animationName));
                }

                var frameDatas = recorderAvatar.ExtractData();

                var jsonData = JsonConvert.SerializeObject(frameDatas, Formatting.Indented);
                File.WriteAllText($"{getAnimationPath(animationName)}/Frame {frameCount}.json", jsonData);

                var animData = new AnimationData
                {
                    animationName = animationName,
                    frameCount = frameCount,
                    interval = interval
                };
                var animjsonData = JsonConvert.SerializeObject(animData, Formatting.Indented);

                File.WriteAllText($"{getAnimationPath(animationName)}/animData.data", animjsonData);
                frameCount++;
            }

            lastFrame = counter;
        }
    }
}
