using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_left_upper_leg : MonoBehaviour
{
    public GameObject leg_bone;
    public Transform leg;
    public Transform knee;

    public Vector3 mid;

    // Start is called before the first frame update
    void Start()
    {
        //find leg and knee
        leg = GameObject.Find("Leg_L").transform;
        knee = GameObject.Find("Knee_L").transform;

        //create leg object
        leg_bone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leg_bone.transform.position = leg.position;

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        //draw box between 2 places making it scale up to take up spaces in between the 2 points
        Vector3 dir = knee.position - leg.position;
        leg_bone.transform.localScale = new Vector3(1, 1, dir.magnitude);


    }

    // Update is called once per frame
    void Update()
    {
        //make sure leg object is at right place (midway and facing lower part)
        mid = leg.position - knee.position;
        leg_bone.transform.position = leg.position - (mid / 2.0f);
        leg_bone.transform.LookAt(knee);
    }
}
