using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_AnimationSelector : MonoBehaviour
{
    public TMP_Text text;
    string _AnimationName;

    public string AnimationName
    {
        get{
            return _AnimationName;
        }
        set{
            _AnimationName = value;
            if(text != null){
                text.text = DisplayName;
            }
        }
    }
    public string DisplayName;

    GameManager gameManager;

    void Start(){
        text = GetComponentInChildren<TMP_Text>();

        if(text != null){
            text.text = DisplayName;
        }
    }
    public void OnClick(){
        GameManager.instance.StartTraining(AnimationName);
    }
}
