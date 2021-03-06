using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_leg : MonoBehaviour
{
    public GameObject leg_bone;
    public Transform leg;
    public Transform knee;
    // Start is called before the first frame update
    void Start()
    {
        leg = GameObject.Find("Leg_R").transform;
        knee = GameObject.Find("Knee_R").transform;
        leg_bone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leg_bone.transform.position = leg.position;
        //Vector3 mid = leg.position - knee.position;
        ////float distance = mid.magnitude;
        //leg_bone.transform.localScale += mid;
        //leg_bone.transform.position = leg.position - (mid / 2.0f);
        //leg_bone.transform.LookAt(knee);

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        Vector3 dir = knee.position - leg.position;
        //leg_bone.transform.rotation = Quaternion.LookRotation(dir);
        leg_bone.transform.localScale = new Vector3(1, 1, dir.magnitude);


    }

    // Update is called once per frame
    void Update()
    {
        leg_bone.transform.position = leg.position;
        Vector3 mid = leg.position - knee.position;
        //float distance = mid.magnitude;
        leg_bone.transform.position = leg.position - (mid / 2.0f);
        leg_bone.transform.LookAt(knee);
    }
}
