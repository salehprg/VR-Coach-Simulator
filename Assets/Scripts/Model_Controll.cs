using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model_Controll : MonoBehaviour
{
    public GameObject ghost;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            ghost.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal"));
            GameManager.instance.avatarBone.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal"));
        }
    }
}
