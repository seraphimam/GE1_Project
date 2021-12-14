using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_IK : MonoBehaviour
{
    public int max_len = 2;
    //public float speed = 5.0f;

    public int iter = 10;

    public float delta = 0.001f;

    public float backwards_Str = 1f;

    public Transform[] bones; //each bone of leg
    public float[] bone_length; //length of each bone
    public float full_len; //full length of leg
    public Vector3[] pos; //positions for each bone

    public Transform right_control;
    public Transform right_dir;
    //public Transform left_control;
    //public Transform left_dir;

    //public Vector3 org_pos_r;
    //public Vector3 org_pos_l;

    //public bool isMoving;

    //public bool right_leg_in_front;

    //public bool right_knee_forward;
    //public bool right_foot_forward;
    //public bool right_foot_down;

    //public bool left_knee_forward;
    //public bool left_foot_forward;
    //public bool left_foot_down;

    //public Transform right_leg;
    //public Transform right_knee;
    public Transform right_foot;

    //public Transform left_leg;
    //public Transform left_knee;
    //public Transform left_foot;


    // Start is called before the first frame update
    void Start()
    {
        right_control = GameObject.Find("R_Foot_ctrl").transform;
        right_dir = GameObject.Find("R_Foot_dir").transform;
        //right_leg = GameObject.Find("Leg_R").transform;
        //right_knee = GameObject.Find("Knee_R").transform;
        right_foot = GameObject.Find("Foot_R").transform;

        //left_control = GameObject.Find("L_Foot_ctrl").transform;
        //left_dir = GameObject.Find("L_Foot_dir").transform;
        //left_leg = GameObject.Find("Leg_L").transform;
        //left_knee = GameObject.Find("Knee_L").transform;
        //left_foot = GameObject.Find("Foot_L").transform;

        //right_leg_in_front = false;

        //right_knee_forward = false;
        //right_foot_forward = false;
        //right_foot_down = false;

        //left_knee_forward = false;
        //left_foot_forward = false;
        //left_foot_down = false;

        //isMoving = false;

        //org_pos_r = right_dir.position;
        //org_pos_l = left_dir.position;

        IK_Init();

    }

    void IK_Init()
    {
        //Debug.Log("IK");
        bones = new Transform[max_len + 1];
        pos = new Vector3[max_len + 1];
        bone_length = new float[max_len];
        full_len = 0;

        var current = right_foot;
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;

            if(i != bones.Length - 1)
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
        //leg.LookAt(knee);
        //knee.LookAt(foot);

        //isMoving = false;
        
        //move();
    }

    void IK_run()
    {
        //if no right_control to move to
        if(right_control == null)
            return;

        //init IK
        if(bone_length.Length != max_len)
        {
            IK_Init();
        }

        //get original pos
        for(int i = 0; i < bones.Length; i++)
        {
            pos[i] = bones[i].position;
        }

        //calc
        if((right_control.position - bones[0].position).sqrMagnitude >= full_len * full_len)
        {
            var dir = (right_control.position - pos[0]).normalized;

            for(int i = 1; i < pos.Length; i++)
            {
                pos[i] = pos[i - 1] + dir * bone_length[i - 1];
            }
        }
        else
        {
            for(int i = 0; i < iter; i++)
            {
                //back
                for(int j = pos.Length - 1; j > 0; j--)
                {
                    if(j == pos.Length - 1)
                    {
                        pos[j] = right_control.position;
                    }
                    else
                    {
                        pos[j] = pos[j + 1] + (pos[j] - pos[j + 1]).normalized * bone_length[j];
                    }
                }

                //forward
                for(int j = 1; j < pos.Length; j++)
                {
                    pos[j] = pos[j - 1] + (pos[j] - pos[j - 1]).normalized * bone_length[j - 1];
                }

                if((pos[pos.Length - 1] - right_control.position).sqrMagnitude < delta * delta)
                {
                    break;
                }
            }
        }

        for (int i = 1; i < pos.Length - 1; i++)
        {
            var plane = new Plane(pos[i + 1] - pos[i - 1], pos[i - 1]);
            var projection_angle = plane.ClosestPointOnPlane(right_dir.position);
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


    //void move()
    //{
    //    //move forward
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        if (right_leg_in_front)
    //        {
    //            left_leg_forward();
    //        }
    //        else
    //        {
    //            right_leg_forward();
    //        }
            

    //    }//end moveforward if

    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        right_control.position += right_control.forward * -speed * Time.deltaTime;
    //        //right_control.position += right_control.up * -speed * Time.deltaTime;
    //    }

    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        right_control.position += right_control.right * -speed * Time.deltaTime;
    //    }

    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        right_control.position += right_control.right * speed * Time.deltaTime;
    //    }
    //}// end move()

    //void move_forward_right()
    //{
    //    isMoving = true;

    //    if (right_foot_down)
    //    {
    //        right_knee_forward = false;
    //        right_foot_forward = false;
    //    }

    //    if (right_knee_forward)
    //    {
    //        //Debug.Log("knee forward + foot");
    //        right_control.position += right_control.forward * speed * Time.deltaTime;
    //        right_control.position += right_control.up * speed * Time.deltaTime;
    //    }

    //    if (right_foot_forward)
    //    {
    //        //Debug.Log("Foot Forward only");
    //        right_control.position += right_control.forward * speed * Time.deltaTime;
    //    }

    //    if (right_foot_down)
    //    {
    //        //Debug.Log("foot down");
    //        right_control.position += right_control.forward * -speed * Time.deltaTime;
    //        right_control.position += right_control.up * -speed * Time.deltaTime;
    //        right_leg.position += right_leg.forward * speed * Time.deltaTime;
    //        right_dir.position += right_dir.forward * speed * Time.deltaTime;
            
    //    }

    //}// end move_forward()

    //void right_leg_forward()
    //{
    //    if ((right_knee.position - right_dir.position).sqrMagnitude <= 0.25)
    //    {
    //        right_knee_forward = false;
    //        if (Mathf.Abs(right_control.position.x - org_pos_r.x) <= 0.25 && Mathf.Abs(right_control.position.z - org_pos_r.z) <= 0.25)
    //        {
    //            right_foot_forward = false;
    //            if (!(Mathf.Abs(right_leg.position.x - org_pos_r.x) <= 0.25 && Mathf.Abs(right_leg.position.z - org_pos_r.z) <= 0.25))
    //            {
    //                right_foot_down = true;
    //                //Debug.Log("foot down");
    //            }

    //        }//foot move forward when knee reaches high point
    //        else
    //        {
    //            right_foot_forward = true;
    //        }
    //    }//knee move forward first
    //    else
    //    {
    //        right_knee_forward = true;
    //    }

    //    if (right_foot_down)
    //    {
    //        if (right_control.position.y <= 0.5)
    //        {
    //            right_foot_down = false;
    //            org_pos_r = right_dir.position;
    //            //Debug.Log("End Foot Down");
    //        }
    //    }


    //    move_forward_right();
    //}// end right_leg_forward()

    ////move_left
    //void move_forward_left()
    //{
    //    isMoving = true;

    //    if (left_foot_down)
    //    {
    //        left_knee_forward = false;
    //        left_foot_forward = false;
    //    }

    //    if (left_knee_forward)
    //    {
    //        //Debug.Log("knee forward + foot");
    //        left_control.position += left_control.forward * speed * Time.deltaTime;
    //        left_control.position += left_control.up * speed * Time.deltaTime;
    //    }

    //    if (left_foot_forward)
    //    {
    //        //Debug.Log("Foot Forward only");
    //        left_control.position += left_control.forward * speed * Time.deltaTime;
    //    }

    //    if (left_foot_down)
    //    {
    //        //Debug.Log("foot down");
    //        left_control.position += left_control.forward * -speed * Time.deltaTime;
    //        left_control.position += left_control.up * -speed * Time.deltaTime;
    //        left_leg.position += left_leg.forward * speed * Time.deltaTime;
    //        left_dir.position += left_dir.forward * speed * Time.deltaTime;

    //    }

    //}// end move_forward()

    //void left_leg_forward()
    //{
    //    if ((left_knee.position - left_dir.position).sqrMagnitude <= 0.25)
    //    {
    //        left_knee_forward = false;
    //        if (Mathf.Abs(left_control.position.x - org_pos_l.x) <= 0.25 && Mathf.Abs(left_control.position.z - org_pos_l.z) <= 0.25)
    //        {
    //            left_foot_forward = false;
    //            if (!(Mathf.Abs(left_leg.position.x - org_pos_l.x) <= 0.25 && Mathf.Abs(left_leg.position.z - org_pos_l.z) <= 0.25))
    //            {
    //                left_foot_down = true;
    //                //Debug.Log("foot down");
    //            }

    //        }//foot move forward when knee reaches high point
    //        else
    //        {
    //            left_foot_forward = true;
    //        }
    //    }//knee move forward first
    //    else
    //    {
    //        left_knee_forward = true;
    //    }

    //    if (left_foot_down)
    //    {
    //        if (left_control.position.y <= 0.5)
    //        {
    //            left_foot_down = false;
    //            org_pos_l = left_dir.position;
    //            //Debug.Log("End Foot Down");
    //        }
    //    }


    //    move_forward_left();
    //}// end right_leg_forward()
}
