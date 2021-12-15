using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_left_lower_leg : MonoBehaviour
{
    public GameObject lower_leg;
    public Transform foot;
    public Transform knee;

    public Vector3 mid;

    // Start is called before the first frame update
    void Start()
    {
        //find foot and knee
        foot = GameObject.Find("Foot_L").transform;
        knee = GameObject.Find("Knee_L").transform;

        //create leg object
        lower_leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lower_leg.transform.position = knee.position;

        //https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html
        //draw box between 2 places making it scale up to take up spaces in between the 2 points
        Vector3 dir = foot.position - knee.position;
        lower_leg.transform.localScale = new Vector3(1, 1, dir.magnitude);

    }

    // Update is called once per frame
    void Update()
    {
        //make sure leg object is at right place (midway and facing lower part)
        mid = knee.position - foot.position;
        lower_leg.transform.position = knee.position - (mid / 2.0f);
        lower_leg.transform.LookAt(foot);
    }
}
