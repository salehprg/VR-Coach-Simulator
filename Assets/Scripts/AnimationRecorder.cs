using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{
    public BoneData recorderAvatar;
    public bool record;

    public int interval = 5;
    public int frameCount = 0;
    public string animationName = "";

    int lastFrame = 0;
    int counter = 0;

    void Start()
    {

    }

    public void StartRecord()
    {
        record = true;
    }

    public void StopRecord()
    {
        record = false;
    }

    void Update()
    {
        if (record)
        {
            counter++;
            if (counter - lastFrame > interval)
            {

                if (!Directory.Exists(Config.getAnimationPath(animationName)))
                {
                    Directory.CreateDirectory(Config.getAnimationPath(animationName));
                }

                var frameDatas = recorderAvatar.ExtractData();

                var jsonData = JsonConvert.SerializeObject(frameDatas, Formatting.Indented);
                File.WriteAllText($"{Config.getAnimationPath(animationName)}/Frame {frameCount}.json", jsonData);

                var animData = new AnimationData
                {
                    animationName = animationName,
                    frameCount = frameCount,
                    interval = interval
                };
                var animjsonData = JsonConvert.SerializeObject(animData, Formatting.Indented);

                File.WriteAllText($"{Config.getAnimationPath(animationName)}/animData.data", animjsonData);
                frameCount++;

                lastFrame = counter;
            }
        
        }
    }
}
