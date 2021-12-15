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
        offset = new Vector3(0.0f, 10.0f, 5.0f);
        target = GameObject.Find("Pelvis").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.I))
        {
            //offset = (0.0f, 10.0f, 5.0f);
            offset.x = 0.0f;
            offset.y = 10.0f;
            offset.z = 5.0f;
        }

        if (Input.GetKey(KeyCode.J))
        {
            //offset = (5.0f, 0.0f, 5.0f);
            offset.x = 5.0f;
            offset.y = 0.0f;
            offset.z = 5.0f;
        }

        if (Input.GetKey(KeyCode.L))
        {
            //offset = (-5.0f, 0.0f, -5.0f);
            offset.x = -5.0f;
            offset.y = 0.0f;
            offset.z = -5.0f;
        }

        //https://answers.unity.com/questions/1482210/how-to-make-an-object-always-in-front-of-the-ovrpl.html
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
