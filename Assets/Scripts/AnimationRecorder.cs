using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class AnimationRecorder : MonoBehaviour
{
    public BoneData recorderAvatar;
    bool _record;
    public bool record
    {
        get
        {
            return _record;
        }

        set
        {
            _record = value;
            startRecordTime = 0;
            timer = 0;
            timerText.text = timerBeforeRecord.ToString();
        }
    }

    public TMP_Text timerText;
    public int timerBeforeRecord = 5;

    public int interval = 5;
    public int frameCount = 0;
    public string animationName = "";

    int lastFrame = 0;
    int counter = 0;
    public int timer = 0;
    public float startRecordTime = 0;

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
        timerText.gameObject.SetActive(false);
        GameManager.instance.LoadAnimations();
    }

    public void SetAnimationName(string animName){
        animationName = animName;
    }

    void Update()
    {
        if (record)
        {
            if (startRecordTime == 0)
            {
                startRecordTime = Time.time;
                timerText.gameObject.SetActive(true);
            }

            if (timer < timerBeforeRecord)
            {
                timer = (int)(Time.time - startRecordTime);
                timerText.text = (timerBeforeRecord - timer).ToString();
                return;
            }

            timerText.gameObject.SetActive(false);

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
                    displayName = animationName,
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
