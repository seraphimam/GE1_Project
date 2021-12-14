using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_IK : MonoBehaviour
{
    public int max_len = 2;
    public float speed = 5.0f;

    public Transform target;
    public Transform move_dir;

    public int iter = 10;

    public float delta = 0.001f;

    public float backwards_Str = 1f;

    public Transform[] bones; //each bone of leg
    public float[] bone_length; //length of each bone
    public float full_len; //full length of leg
    public Vector3[] pos; //positions for each bone

    public Vector3 org_pos;

    public bool isMoving;
    public bool knee_forward;
    public bool foot_forward;
    public bool foot_down;

    public Transform leg;
    public Transform knee;
    public Transform foot;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Foot_ctrl").transform;
        move_dir = GameObject.Find("Foot_dir").transform;
        knee_forward = false;
        foot_forward = false;
        foot_down = false;
        IK_Init();

        isMoving = false;
        org_pos = move_dir.position;
        Debug.Log("org x: " + org_pos.x);
        Debug.Log("org y: " + org_pos.y);
        Debug.Log("org z: " + org_pos.z);
    }

    void IK_Init()
    {
        bones = new Transform[max_len + 1];
        pos = new Vector3[max_len + 1];
        bone_length = new float[max_len];
        full_len = 0;

        var current = transform;
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

        isMoving = false;
        
        move();
    }

    void IK_run()
    {
        //if no target to move to
        if(target == null)
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
        if((target.position - bones[0].position).sqrMagnitude >= full_len * full_len)
        {
            var dir = (target.position - pos[0]).normalized;

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
                        pos[j] = target.position;
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

                if((pos[pos.Length - 1] - target.position).sqrMagnitude < delta * delta)
                {
                    break;
                }
            }
        }

        for (int i = 1; i < pos.Length - 1; i++)
        {
            var plane = new Plane(pos[i + 1] - pos[i - 1], pos[i - 1]);
            var projection_angle = plane.ClosestPointOnPlane(move_dir.position);
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


    void move()
    {
        leg = GameObject.Find("Leg").transform;
        knee = GameObject.Find("Knee").transform;
        foot = GameObject.Find("Foot").transform;
        

        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            if ((knee.position - move_dir.position).sqrMagnitude <= 0.25)
            {
                knee_forward = false;
                if (Mathf.Abs(target.position.x - org_pos.x) <= 0.25 && Mathf.Abs(target.position.z - org_pos.z) <= 0.25)
                {
                    foot_forward = false;
                    if (! (Mathf.Abs(leg.position.x - org_pos.x) <= 0.25 && Mathf.Abs(leg.position.z - org_pos.z) <= 0.25))
                    {
                        foot_down = true;
                        Debug.Log("foot down");
                    }

                }//foot move forward when knee reaches high point
                else
                {
                    foot_forward = true;
                }
            }//knee move forward first
            else
            {
                knee_forward = true;
            }

            if (foot_down)
            {
                if (target.position.y <= 0.5)
                {
                    foot_down = false;
                    org_pos = move_dir.position;
                    Debug.Log("End Foot Down");
                }
            }

            


            move_forward();
        }//end moveforward if

        if (Input.GetKey(KeyCode.S))
        {
            target.position += target.forward * -speed * Time.deltaTime;
            //target.position += target.up * -speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            target.position += target.right * -speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            target.position += target.right * speed * Time.deltaTime;
        }
    }// end move()

    void move_forward()
    {
        isMoving = true;

        if (foot_down)
        {
            knee_forward = false;
            foot_forward = false;
        }

        if (knee_forward)
        {
            //Debug.Log("knee forward + foot");
            target.position += target.forward * speed * Time.deltaTime;
            target.position += target.up * speed * Time.deltaTime;
        }

        if (foot_forward)
        {
            //Debug.Log("Foot Forward only");
            target.position += target.forward * speed * Time.deltaTime;
        }

        if (foot_down)
        {
            //Debug.Log("foot down");
            target.position += target.forward * -speed * Time.deltaTime;
            target.position += target.up * -speed * Time.deltaTime;
            leg.position += leg.forward * speed * Time.deltaTime;
            move_dir.position += move_dir.forward * speed * Time.deltaTime;

            //Debug.Log("org x: " + org_pos.x);
            //Debug.Log("org y: " + org_pos.y);
            //Debug.Log("org z: " + org_pos.z);
        }

        //Debug.Log("org x: " + org_pos.x);
        //Debug.Log("org y: " + org_pos.y);
        //Debug.Log("org z: " + org_pos.z);




        //if (target.position.y > 0)
        //{
        //    target.position += target.up * -speed * Time.deltaTime;
        //}
        //else
        //{

        //}

        //leg.position += leg.forward * speed * Time.deltaTime;
        //move_dir.position += move_dir.forward * speed * Time.deltaTime;

    }// end move_forward()
}
