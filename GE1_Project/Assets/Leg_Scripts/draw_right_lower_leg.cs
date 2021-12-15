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
        //find foot and knee
        foot = GameObject.Find("Foot_R").transform;
        knee = GameObject.Find("Knee_R").transform;

        //create leg object
        knee_bone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        knee_bone.transform.position = knee.position;

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        //draw box between 2 places making it scale up to take up spaces in between the 2 points
        Vector3 dir = foot.position - knee.position;
        knee_bone.transform.localScale = new Vector3(1, 1, dir.magnitude);

    }

    // Update is called once per frame
    void Update()
    {
        //make sure leg object is at right place (midway and facing lower part)
        mid = knee.position - foot.position;
        knee_bone.transform.position = knee.position - (mid / 2.0f);
        knee_bone.transform.LookAt(foot);
    }
}
