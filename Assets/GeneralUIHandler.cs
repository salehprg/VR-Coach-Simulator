using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GeneralUIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject recordHolder, stopHolder, timerHolder;
    
    [SerializeField]
    public TextMeshProUGUI textMeshPro;

    [SerializeField]
    private float stopLimit = 10f;

    private float stopwatchValue = 0f;

    public void Recored() 
    {
        stopwatchValue = 0;
        recordHolder.SetActive(false);
        timerHolder.SetActive(true);
        stopHolder.SetActive(true);
        StartCoroutine(StopwatchCoroutine());
    }

    public void Stop() 
    {
        stopHolder.SetActive(false);
        timerHolder.SetActive(false);
        recordHolder.SetActive(true);
        StopCoroutine(StopwatchCoroutine());
    }

    private IEnumerator StopwatchCoroutine()
    {
        while (stopwatchValue < stopLimit)
        {
            stopwatchValue += Time.deltaTime;

            if (textMeshPro != null)
            {
                textMeshPro.text = stopwatchValue.ToString("F2");
            }
            else
            {
                yield break;
            }

            yield return null; 
        }

        Debug.Log("Stopwatch reached the limit!");
    }
}
