using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void ShowPanel(GameObject nextPanel){
        nextPanel.SetActive(true);
    }
    public void HidePanel(GameObject panel){
        panel.SetActive(false);
    }

    public void LoadScene(UnityEngine.SceneManagement.Scene scene){
        SceneManager.LoadScene(scene.name);
    }
}
