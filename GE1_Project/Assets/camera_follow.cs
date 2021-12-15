using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        //offset from pelvis
        offset = new Vector3(0.0f, 10.0f, 5.0f);
        target = GameObject.Find("Pelvis").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //top down
        if (Input.GetKey(KeyCode.I))
        {
            //offset = (0.0f, 10.0f, 5.0f);
            offset.x = 0.0f;
            offset.y = 10.0f;
            offset.z = 5.0f;
        }

        //left front
        if (Input.GetKey(KeyCode.J))
        {
            //offset = (5.0f, 0.0f, 5.0f);
            offset.x = 10.0f;
            offset.y = -2.0f;
            offset.z = 10.0f;
        }

        //back right
        if (Input.GetKey(KeyCode.L))
        {
            //offset = (-5.0f, 0.0f, -5.0f);
            offset.x = -10.0f;
            offset.y = -2.0f;
            offset.z = -10.0f;
        }

        //https://answers.unity.com/questions/1482210/how-to-make-an-object-always-in-front-of-the-ovrpl.html
        //keep camera focusing on model
        transform.position = target.forward + offset;
        transform.LookAt(target);
    }
}
