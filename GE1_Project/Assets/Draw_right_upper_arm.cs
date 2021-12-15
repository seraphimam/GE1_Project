using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_right_upper_arm : MonoBehaviour
{
    public GameObject upper_arm;
    public Transform arm;
    public Transform elbow;

    public Vector3 mid;

    // Start is called before the first frame update
    void Start()
    {
        arm = GameObject.Find("Arm_R").transform;
        elbow = GameObject.Find("Elbow_R").transform;
        upper_arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upper_arm.transform.position = elbow.position;

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        Vector3 dir = arm.position - elbow.position;
        //knee_bone.transform.rotation = Quaternion.LookRotation(dir);
        upper_arm.transform.localScale = new Vector3(1, 1, dir.magnitude);

    }

    // Update is called once per frame
    void Update()
    {
        //knee_bone.transform.position = knee.position;
        mid = elbow.position - arm.position;
        upper_arm.transform.position = elbow.position - (mid / 2.0f);
        upper_arm.transform.LookAt(arm);
    }
}
