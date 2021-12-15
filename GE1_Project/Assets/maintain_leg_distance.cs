using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maintain_leg_distance : MonoBehaviour
{
    public Transform left_leg;
    public Transform right_leg;

    public Vector3 pos_r;
    public Vector3 pos_l;

    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        //find left and right legs
        right_leg = GameObject.Find("Leg_R").transform;
        left_leg = GameObject.Find("Leg_L").transform;

        //initial distance between 2 legs
        distance = (right_leg.position - left_leg.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        //current position of 2 legs
        pos_r = right_leg.position;
        pos_l = left_leg.position;

        //if position of 2 legs are not same as start
        if ((pos_r - pos_l).magnitude != distance )
        {
            //push the left leg forward or backwards to fit back into the starting distance
            left_leg.position = pos_r + (pos_l - pos_r).normalized * distance;
        }
    }
}
