using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_left_lower_arm : MonoBehaviour
{
    public GameObject lower_arm;
    public Transform hand;
    public Transform elbow;

    public Vector3 mid;

    // Start is called before the first frame update
    void Start()
    {
        hand = GameObject.Find("Hand_L").transform;
        elbow = GameObject.Find("Elbow_L").transform;
        lower_arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lower_arm.transform.position = elbow.position;

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        Vector3 dir = hand.position - elbow.position;
        //knee_bone.transform.rotation = Quaternion.LookRotation(dir);
        lower_arm.transform.localScale = new Vector3(1, 1, dir.magnitude);

    }

    // Update is called once per frame
    void Update()
    {
        //knee_bone.transform.position = knee.position;
        mid = elbow.position - hand.position;
        lower_arm.transform.position = elbow.position - (mid / 2.0f);
        lower_arm.transform.LookAt(hand);
    }
}
