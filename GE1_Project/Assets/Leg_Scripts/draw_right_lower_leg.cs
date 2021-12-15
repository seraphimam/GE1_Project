using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_right_lower_leg : MonoBehaviour
{
    public GameObject knee_bone;
    public Transform foot;
    public Transform knee;

    public Vector3 mid;

    // Start is called before the first frame update
    void Start()
    {
        foot = GameObject.Find("Foot_R").transform;
        knee = GameObject.Find("Knee_R").transform;
        knee_bone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        knee_bone.transform.position = knee.position;

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        Vector3 dir = foot.position - knee.position;
        //knee_bone.transform.rotation = Quaternion.LookRotation(dir);
        knee_bone.transform.localScale = new Vector3(1, 1, dir.magnitude);

    }

    // Update is called once per frame
    void Update()
    {
        //knee_bone.transform.position = knee.position;
        mid = knee.position - foot.position;
        knee_bone.transform.position = knee.position - (mid / 2.0f);
        knee_bone.transform.LookAt(foot);
    }
}
