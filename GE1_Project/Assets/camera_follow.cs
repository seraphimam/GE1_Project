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
        offset = new Vector3(0.0f, 0.0f, 20.0f);
        target = GameObject.Find("Pelvis").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //https://answers.unity.com/questions/1482210/how-to-make-an-object-always-in-front-of-the-ovrpl.html
        transform.position = target.position + offset;
    }
}
