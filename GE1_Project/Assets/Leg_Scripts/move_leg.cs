using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_leg : MonoBehaviour
{
    public float speed = 2.0f;

    public Transform right_control;
    public Transform right_dir;
    public Transform left_control;
    public Transform left_dir;

    public Vector3 right_dir_org;
    public Vector3 left_dir_org;

    public bool is_moving_right_leg;
    public bool is_moving_left_leg;

    public bool right_leg_in_front;

    public bool right_knee_forward;
    public bool right_foot_forward;
    public bool right_foot_down;

    public bool left_knee_forward;
    public bool left_foot_forward;
    public bool left_foot_down;

    public Transform right_leg;
    public Transform right_knee;
    public Transform right_foot;

    public Transform left_leg;
    public Transform left_knee;
    public Transform left_foot;

    public Transform pelvis;

    public Vector3 pelvis_height_adj;
    public GameObject pelvis_dir;

    public Vector3 between_legs;

    public Vector3 right_foot_org;
    public Vector3 left_foot_org;

    public float knee_leg_dist;

    // Start is called before the first frame update
    void Start()
    {
        right_control = GameObject.Find("R_Foot_ctrl").transform;
        right_dir = GameObject.Find("R_Foot_dir").transform;
        right_leg = GameObject.Find("Leg_R").transform;
        right_knee = GameObject.Find("Knee_R").transform;
        right_foot = GameObject.Find("Foot_R").transform;

        left_control = GameObject.Find("L_Foot_ctrl").transform;
        left_dir = GameObject.Find("L_Foot_dir").transform;
        left_leg = GameObject.Find("Leg_L").transform;
        left_knee = GameObject.Find("Knee_L").transform;
        left_foot = GameObject.Find("Foot_L").transform;

        pelvis = GameObject.Find("Pelvis").transform;
        pelvis_dir = new GameObject();
        pelvis_height_adj = new Vector3(0f, 1.5f, 0f);

        right_leg_in_front = false;

        right_knee_forward = false;
        right_foot_forward = false;
        right_foot_down = false;

        left_knee_forward = false;
        left_foot_forward = false;
        left_foot_down = false;

        is_moving_right_leg = false;
        is_moving_left_leg = false;

        right_dir_org = right_dir.position;
        left_dir_org = left_dir.position;

        right_foot_org = right_control.position;
        left_foot_org = left_control.position;

        knee_leg_dist = (right_knee.position - right_foot.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    void move()
    {
        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            if ((right_leg_in_front || is_moving_left_leg) && !is_moving_right_leg)
            {
                right_leg_in_front = false;
                is_moving_left_leg = true;

                left_leg_forward();

                pelvis_follow("left");
            }
            else
            {
                right_leg_in_front = true;
                is_moving_right_leg = true;

                right_leg_forward();

                pelvis_follow("right");

            }

            //Debug.Log("forward left moving: " + is_moving_left_leg);
            //Debug.Log("forward right moving: " + is_moving_right_leg);
        }//end moveforward if

        // move backwards
        if (Input.GetKey(KeyCode.S))
        {
            //Debug.Log("left moving: " + is_moving_left_leg);
            //Debug.Log("right moving: " + is_moving_right_leg);
            Debug.Log("right left front: " + right_leg_in_front);

            if ((right_leg_in_front || is_moving_right_leg) && !is_moving_left_leg)
            {
                is_moving_right_leg = true;
                right_leg_backwards();
                
            }
            else
            {
                is_moving_left_leg = true;
                left_leg_backwards();
                
            }
            //right_control.position += right_control.forward * -speed * Time.deltaTime;
            //right_control.position += right_control.up * -speed * Time.deltaTime;
        }//end move backwards if

        if (Input.GetKey(KeyCode.A))
        {
            right_control.position += right_control.right * -speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            right_control.position += right_control.right * speed * Time.deltaTime;
        }
    }// end move()

    void move_forward_right()
    {
        if (right_foot_down)
        {
            right_knee_forward = false;
            right_foot_forward = false;
        }

        if (right_knee_forward)
        {
            //Debug.Log("knee forward + foot");
            right_control.position += right_control.forward * speed * Time.deltaTime;
            right_control.position += right_control.up * speed * Time.deltaTime;
            right_foot.position = right_control.position;
        }

        if (right_foot_forward)
        {
            //Debug.Log("Foot Forward only");
            right_control.position += right_control.forward * speed * Time.deltaTime;
            right_foot.position = right_control.position;
        }

        if (right_foot_down)
        {
            //Debug.Log("foot down");
            right_control.position += right_control.forward * -speed * Time.deltaTime;
            right_control.position += right_control.up * -speed * Time.deltaTime;
            right_leg.position += right_leg.forward * speed * Time.deltaTime;
            right_dir.position += right_dir.forward * speed * Time.deltaTime;
            right_foot.position = right_control.position;

        }

    }// end move_forward()

    void right_leg_forward()
    {
        if ((right_knee.position - right_dir.position).sqrMagnitude <= 0.25)
        {
            right_knee_forward = false;
            if (Mathf.Abs(right_control.position.x - right_dir_org.x) <= 0.25 && Mathf.Abs(right_control.position.z - right_dir_org.z) <= 0.25)
            {
                right_foot_forward = false;
                if (!(Mathf.Abs(right_leg.position.x - right_dir_org.x) <= 0.25 && Mathf.Abs(right_leg.position.z - right_dir_org.z) <= 0.25))
                {
                    right_foot_down = true;
                    //Debug.Log("foot down");
                }

            }//foot move forward when knee reaches high point
            else
            {
                right_foot_forward = true;
            }
        }//knee move forward first
        else
        {
            right_knee_forward = true;
        }

        if (right_foot_down)
        {
            if (right_control.position.y <= 0.5)
            {
                right_foot_down = false;
                right_dir_org = right_dir.position;
                right_leg_in_front = true;
                is_moving_right_leg = false;
                //Debug.Log("End Foot Down");
            }
        }


        move_forward_right();
    }// end right_leg_forward()

    //move_left
    void move_forward_left()
    {

        if (left_foot_down)
        {
            left_knee_forward = false;
            left_foot_forward = false;
        }

        if (left_knee_forward)
        {
            //Debug.Log("knee forward + foot");
            left_control.position += left_control.forward * speed * Time.deltaTime;
            left_control.position += left_control.up * speed * Time.deltaTime;
        }

        if (left_foot_forward)
        {
            //Debug.Log("Foot Forward only");
            left_control.position += left_control.forward * speed * Time.deltaTime;
        }

        if (left_foot_down)
        {
            //Debug.Log("foot down");
            left_control.position += left_control.forward * -speed * Time.deltaTime;
            left_control.position += left_control.up * -speed * Time.deltaTime;
            left_leg.position += left_leg.forward * speed * Time.deltaTime;
            left_dir.position += left_dir.forward * speed * Time.deltaTime;

        }

    }// end move_forward()

    void left_leg_forward()
    {
        if ((left_knee.position - left_dir.position).sqrMagnitude <= 0.25)
        {
            left_knee_forward = false;
            if (Mathf.Abs(left_control.position.x - left_dir_org.x) <= 0.25 && Mathf.Abs(left_control.position.z - left_dir_org.z) <= 0.25)
            {
                left_foot_forward = false;
                if (!(Mathf.Abs(left_leg.position.x - left_dir_org.x) <= 0.25 && Mathf.Abs(left_leg.position.z - left_dir_org.z) <= 0.25))
                {
                    left_foot_down = true;
                    //Debug.Log("foot down");
                }

            }//foot move forward when knee reaches high point
            else
            {
                left_foot_forward = true;
            }
        }//knee move forward first
        else
        {
            left_knee_forward = true;
        }

        if (left_foot_down)
        {
            if (left_control.position.y <= 0.5)
            {
                left_foot_down = false;
                left_dir_org = left_dir.position;
                right_leg_in_front = false;
                is_moving_left_leg = false;
                //Debug.Log("End Foot Down");
            }
        }


        move_forward_left();
    }// end right_leg_forward()

    //move right leg backwards
    void right_leg_backwards()
    {
        float dist = (right_foot.position - right_foot_org).magnitude;
        //Debug.Log("dist: " + dist);
        //Debug.Log("knee dist: " + knee_leg_dist);
        if(dist > knee_leg_dist / 3.0f)
        {
            right_control.position += right_control.up * -speed * Time.deltaTime;
            right_control.position += right_control.forward * -speed * Time.deltaTime;
            right_leg.position += right_leg.forward * -speed * Time.deltaTime;
            right_dir.position += right_dir.forward * -speed * Time.deltaTime;
            right_dir_org = right_dir.position;

            pelvis_dir.transform.position = right_dir.position + pelvis_height_adj;

            between_legs = right_leg.position - left_leg.position;

            pelvis.position = right_leg.position - (between_legs / 2.0f);

            pelvis.LookAt(pelvis_dir.transform);
        }//move foot upwards
        else
        {
            right_control.position += right_control.up * speed * Time.deltaTime;
            right_control.position += right_control.forward * -speed * 0.5f * Time.deltaTime;
            //right_foot.position = right_control.position;
        }

        if (right_control.position.y <= 0.5)
        {
            is_moving_right_leg = false;
            right_leg_in_front = false;
            right_control.position = right_foot.position;
            right_foot_org = right_control.position;
        }
    }// end right_leg_backwards()

    //move left leg backwards
    void left_leg_backwards()
    {
        float dist = (left_foot.position - left_foot_org).magnitude;
        //Debug.Log("dist: " + dist);
        //Debug.Log("knee dist: " + knee_leg_dist);
        if (dist > knee_leg_dist / 3.0f)
        {
            left_control.position += left_control.up * -speed * Time.deltaTime;
            left_control.position += left_control.forward * -speed * Time.deltaTime;
            left_leg.position += left_leg.forward * -speed * Time.deltaTime;
            left_dir.position += left_dir.forward * -speed * Time.deltaTime;
            left_dir_org = left_dir.position;

            pelvis_dir.transform.position = left_dir.position + pelvis_height_adj;

            between_legs = left_leg.position - right_leg.position;

            pelvis.position = left_leg.position - (between_legs / 2.0f);

            pelvis.LookAt(pelvis_dir.transform);
        }//move foot upwards
        else
        {
            left_control.position += left_control.up * speed * Time.deltaTime;
            left_control.position += left_control.forward * -speed * 0.5f * Time.deltaTime;
            //left_foot.position = left_control.position;
        }

        if(left_control.position.y <= 0.5)
        {
            is_moving_left_leg = false;
            right_leg_in_front = true;
            left_control.position = left_foot.position;
            left_foot_org = left_control.position;
        }
    }// end left_leg_backwards

    //make pelvis follow leg movement, mode "left" and "right"
    void pelvis_follow(string mode)
    {
        if(mode.Equals("right"))
        {
            pelvis_dir.transform.position = left_dir.position + pelvis_height_adj;

            between_legs = right_leg.position - left_leg.position;

            pelvis.position = right_leg.position - (between_legs / 2.0f);

            pelvis.LookAt(pelvis_dir.transform);
        }

        if (mode.Equals("left"))
        {
            pelvis_dir.transform.position = right_dir.position + pelvis_height_adj;

            between_legs = left_leg.position - right_leg.position;

            pelvis.position = left_leg.position - (between_legs / 2.0f);

            pelvis.LookAt(pelvis_dir.transform);
        }

    }//end pelvis_follow()
}
