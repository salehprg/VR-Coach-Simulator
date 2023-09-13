using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model_Controll : MonoBehaviour
{
    public GameObject avatar;
    public GameObject ghost;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (avatar == null)
        {
            if (GameManager.instance.avatarBone != null)
                avatar = GameManager.instance.avatarBone.gameObject;
            return;
        }

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            ghost.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal"));
            avatar.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal"));
        }
    }
}
