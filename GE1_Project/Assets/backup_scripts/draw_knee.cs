using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_knee : MonoBehaviour
{
    public GameObject knee_bone;
    public Transform foot;
    public Transform knee;

    // Start is called before the first frame update
    void Start()
    {
        foot = GameObject.Find("Foot_R").transform;
        knee = GameObject.Find("Knee_R").transform;
        knee_bone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        knee_bone.transform.position = knee.position;
        //Vector3 mid = leg.position - knee.position;
        ////float distance = mid.magnitude;
        //leg_bone.transform.localScale += mid;
        //leg_bone.transform.position = leg.position - (mid / 2.0f);
        //leg_bone.transform.LookAt(knee);

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        Vector3 dir = foot.position - knee.position;
        //knee_bone.transform.rotation = Quaternion.LookRotation(dir);
        knee_bone.transform.localScale = new Vector3(1, 1, dir.magnitude);
        
    }

    // Update is called once per frame
    void Update()
    {
        knee_bone.transform.position = knee.position;
        Vector3 mid = knee.position - foot.position;
        //float distance = mid.magnitude;
        knee_bone.transform.position = knee.position - (mid / 2.0f);
        knee_bone.transform.LookAt(foot);
    }
}
