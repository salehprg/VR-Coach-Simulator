using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySelector : MonoBehaviour
{
    public void SelectSky(Material skyMaterial){
        GameManager.skyMaterial = skyMaterial;
    }
}
