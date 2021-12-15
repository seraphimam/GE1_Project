using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Left_Leg_IK : MonoBehaviour
{
    public int max_len = 2;

    public int iter = 10;

    public float delta = 0.001f;

    //public float backwards_Str = 1f;

    public Transform[] bones; //each bone of leg
    public float[] bone_length; //length of each bone
    public float full_len; //full length of leg
    public Vector3[] pos; //positions for each bone

    public Transform left_control;
    public Transform left_dir;
    public Transform left_foot;

    // Start is called before the first frame update
    void Start()
    {
        left_control = GameObject.Find("L_Foot_ctrl").transform;
        left_dir = GameObject.Find("L_Foot_dir").transform;
        left_foot = GameObject.Find("Foot_L").transform;

        IK_Init();
    }

    void IK_Init()
    {
        bones = new Transform[max_len + 1];
        pos = new Vector3[max_len + 1];
        bone_length = new float[max_len];
        full_len = 0;

        var current = left_foot;
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;

            if (i != bones.Length - 1)
            {
                bone_length[i] = (bones[i + 1].position - current.position).magnitude;
                full_len += bone_length[i];
            }

            current = current.parent;
        }

    }

    // Update is called once per frame
    void Update()
    {
        IK_run();
    }

    void IK_run()
    {
        //if no left_control to move to
        if (left_control == null)
            return;

        //init IK
        if (bone_length.Length != max_len)
        {
            IK_Init();
        }

        //get original pos
        for (int i = 0; i < bones.Length; i++)
        {
            pos[i] = bones[i].position;
        }

        //calc
        if ((left_control.position - bones[0].position).sqrMagnitude >= full_len * full_len)
        {
            var dir = (left_control.position - pos[0]).normalized;

            for (int i = 1; i < pos.Length; i++)
            {
                pos[i] = pos[i - 1] + dir * bone_length[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < iter; i++)
            {
                //back
                for (int j = pos.Length - 1; j > 0; j--)
                {
                    if (j == pos.Length - 1)
                    {
                        pos[j] = left_control.position;
                    }
                    else
                    {
                        pos[j] = pos[j + 1] + (pos[j] - pos[j + 1]).normalized * bone_length[j];
                    }
                }

                //forward
                for (int j = 1; j < pos.Length; j++)
                {
                    pos[j] = pos[j - 1] + (pos[j] - pos[j - 1]).normalized * bone_length[j - 1];
                }

                if ((pos[pos.Length - 1] - left_control.position).sqrMagnitude < delta * delta)
                {
                    break;
                }
            }
        }

        for (int i = 1; i < pos.Length - 1; i++)
        {
            var plane = new Plane(pos[i + 1] - pos[i - 1], pos[i - 1]);
            var projection_angle = plane.ClosestPointOnPlane(left_dir.position);
            var projected_bone = plane.ClosestPointOnPlane(pos[i]);
            var angle = Vector3.SignedAngle(projected_bone - pos[i - 1], projection_angle - pos[i - 1], plane.normal);
            pos[i] = Quaternion.AngleAxis(angle, plane.normal) * (pos[i] - pos[i - 1]) + pos[i - 1];
        }

        //set pos to after calc
        for (int i = 0; i < pos.Length; i++)
        {
            bones[i].position = pos[i];
        }
    }//end IK_run()
}
