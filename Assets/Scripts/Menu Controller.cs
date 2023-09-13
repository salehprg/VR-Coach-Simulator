using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void SetCoach(GameObject character){
        GameManager.coach_prefab = character;
    }

    public void ShowPanel(GameObject nextPanel){
        nextPanel.SetActive(true);
    }
    public void HidePanel(GameObject panel){
        panel.SetActive(false);
    }

    public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}
