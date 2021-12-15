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
        //find hand and elbow
        hand = GameObject.Find("Hand_L").transform;
        elbow = GameObject.Find("Elbow_L").transform;

        //create arm object
        lower_arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lower_arm.transform.position = elbow.position;

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        //draw box between 2 places making it scale up to take up spaces in between the 2 points
        Vector3 dir = hand.position - elbow.position;
        lower_arm.transform.localScale = new Vector3(1, 1, dir.magnitude);

    }

    // Update is called once per frame
    void Update()
    {
        //make sure arm object is at right place (midway and facing lower part)
        mid = elbow.position - hand.position;
        lower_arm.transform.position = elbow.position - (mid / 2.0f);
        lower_arm.transform.LookAt(hand);
    }
}
