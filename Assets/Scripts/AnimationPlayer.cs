using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    public BoneData displayAvatar;
    public bool playAnimtaion;
    public bool playTraining;

    public string animationName = "";

    int lastFrame = 0;
    int counter = 0;


    int _displayedFrame;
    public int displayedFrame
    {
        get { return _displayedFrame; }
        private set { _displayedFrame = value; }
    }
    
    void Start()
    {

    }

    void Update()
    {
        counter++;

        if (playAnimtaion)
        {
            var animData = File.ReadAllText($"{Config.getAnimationPath(animationName)}/animData.data");
            AnimationData animationData = JsonConvert.DeserializeObject<AnimationData>(animData);

            if (counter - lastFrame > animationData.interval)
            {
                if (playAnimtaion)
                {
                    var boneData = File.ReadAllText($"{Config.getAnimationPath(animationName)}/Frame {displayedFrame}.json");

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
    }

    public void SetAnimation(string animName){
        animationName = animName;
        playTraining = true;
    }
    public void NextFrame()
    {
        if (playTraining)
        {
            var animData = File.ReadAllText(Config.getAnimationData(animationName));
            AnimationData animationData = JsonConvert.DeserializeObject<AnimationData>(animData);

            var boneData = File.ReadAllText(Config.getAnimationFrame(animationName, displayedFrame));

            List<FrameData> animDatas = JsonConvert.DeserializeObject<List<FrameData>>(boneData);
            displayAvatar.SetData(animDatas);

            displayedFrame++;

            if (displayedFrame > animationData.frameCount)
            {
                displayedFrame = 0;
            }
        }
    }
}
