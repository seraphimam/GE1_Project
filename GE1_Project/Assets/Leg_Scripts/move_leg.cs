using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_leg : MonoBehaviour
{
    public float speed = 2.0f;
    public float rotate_speed = 30.0f;
    public float init_rotation = 0.0f;

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

    public bool is_turning_left;
    public bool fin_left;
    public bool is_turning_right;
    public bool fin_right;

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

    public float knee_foot_dist;
    public float leg_dist;
    public float leg_dir_dist;

    public Vector3 right_dir_offset;
    public Vector3 left_dir_offset;

    public Vector3 right_offset;
    public Vector3 left_offset;

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
        pelvis_height_adj = new Vector3(0f, 1.0f, 0f);

        right_leg_in_front = false;

        right_knee_forward = false;
        right_foot_forward = false;
        right_foot_down = false;

        left_knee_forward = false;
        left_foot_forward = false;
        left_foot_down = false;

        is_moving_right_leg = false;
        is_moving_left_leg = false;

        is_turning_right = false;
        fin_right = false;
        is_turning_left = false;
        fin_left = false;

        right_dir_org = right_dir.position;
        left_dir_org = left_dir.position;

        right_foot_org = right_control.position;
        left_foot_org = left_control.position;

        knee_foot_dist = (right_knee.position - right_foot.position).magnitude;
        leg_dir_dist = (right_leg.position - right_dir.position).magnitude;
        leg_dist = (right_leg.position - left_leg.position).magnitude;

        right_dir_offset = right_leg.position - right_dir.position;
        left_dir_offset = left_leg.position - left_dir.position;

        left_offset = new Vector3(0, 0, 0);
        right_offset = new Vector3(0, 0, 0);

        //Debug.Log("start dist: " + (right_leg.position - right_dir.position).magnitude);
    }

    // Update is called once per frame
    void Update()
    {
        if(!is_turning_left && !is_turning_right)
        {
            move();
        }
        else
        {
            if (is_turning_left)
            {
                //Debug.Log("angle: " + left_leg.localRotation.eulerAngles.y);
                //Debug.Log("init: " + init_rotation);
                float left_angle = left_leg.localRotation.eulerAngles.y;
                if(init_rotation >= 330)
                {
                    left_angle += 360;
                }

                if (Input.GetKeyUp(KeyCode.A) || Mathf.Abs(left_angle - init_rotation) >= 30.0f || fin_left)
                {
                    fin_left = true;
                    finish_left_turn();
                }
                else
                {
                    turn_left();
                }
                
            }

            if (is_turning_right)
            {
                //Debug.Log("angle: " + right_leg.localRotation.eulerAngles.y);
                //Debug.Log("init: " + init_rotation);
                float right_angle = right_leg.localRotation.eulerAngles.y;
                if (init_rotation <= 30)
                {
                    right_angle -= 360;
                }

                if (Input.GetKeyUp(KeyCode.D) || Mathf.Abs(init_rotation - right_angle) >= 30.0f || fin_right)
                {
                    //Debug.Log("fin");
                    fin_right = true;
                    finish_right_turn();
                }
                else
                {
                    turn_right();
                }
            }
        }

        //dist_correction();
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
            //Debug.Log("right left front: " + right_leg_in_front);

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
        }//end move backwards if

        if (Input.GetKey(KeyCode.A))
        {
            if (!is_moving_right_leg && !is_moving_left_leg)
            {
                if (!is_turning_left)
                {
                    init_rotation = left_leg.localRotation.eulerAngles.y;
                }

                turn_left();
            }
            else
            {
                if (is_moving_right_leg)
                {
                    right_control.position += right_control.up * -speed * Time.deltaTime;

                    if (right_control.position.y <= 0.5)
                    {
                        is_moving_right_leg = false;
                    }
                }
                else if (is_moving_left_leg)
                {
                    left_control.position += left_control.up * -speed * Time.deltaTime;

                    if (left_control.position.y <= 0.5)
                    {
                        is_moving_left_leg = false;
                    }
                }
                
            }
        }//end turn left (A)

        if (Input.GetKey(KeyCode.D))
        {
            //Debug.Log("DDDD");
            if (!is_moving_right_leg && !is_moving_left_leg)
            {
                //Debug.Log("not moving");
                if (!is_turning_right)
                {
                    init_rotation = right_leg.localRotation.eulerAngles.y;
                }

                turn_right();
            }
            else
            {
                //Debug.Log("else");
                if (is_moving_right_leg)
                {
                    right_control.position += right_control.up * -speed * Time.deltaTime;

                    if (right_control.position.y <= 0.5)
                    {
                        is_moving_right_leg = false;
                    }
                }
                else if (is_moving_left_leg)
                {
                    left_control.position += left_control.up * -speed * Time.deltaTime;

                    if (left_control.position.y <= 0.5)
                    {
                        is_moving_left_leg = false;
                    }
                }

            }
        }//end turn right(D)
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
        //Debug.Log("knee dist: " + knee_foot_dist);
        if(dist > knee_foot_dist / 3.0f)
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
        //Debug.Log("knee dist: " + knee_foot_dist);
        if (dist > knee_foot_dist / 3.0f)
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

    //turn left
    void turn_left()
    {
        is_turning_left = true;

        left_dir.parent = left_leg;
        //right_dir.parent = right_leg;

        left_control.parent = left_foot;
        //right_control.parent = right_foot;

        left_leg.Rotate(Vector3.up, rotate_speed * Time.deltaTime);
        //right_leg.Rotate(Vector3.up, rotate_speed * Time.deltaTime);
        
    }

    void finish_left_turn()
    {
        //Debug.Log("finishing left turn");

        if (is_turning_left)
        {
            //is_turning_left = false;

            left_dir.parent = null;
            //right_dir.parent = null;

            left_control.parent = null;
            //right_control.parent = null;
        }

        //swing leg
        float angle = Vector3.SignedAngle(right_leg.position - left_leg.position, left_leg.forward, Vector3.up);

        right_knee.parent = null;
        right_foot.parent = null;

        if (Mathf.Abs(angle) >= 89.9 && Mathf.Abs(angle) <= 90.1)
        //if (Mathf.Abs(angle) == 90.0f)
        {
            right_knee.parent = right_leg;
            right_foot.parent = right_knee;
            //Debug.Log("dist: " + (right_leg.position - right_dir.position).magnitude);
            right_foot_org = right_control.position;
            right_dir_org = right_dir.position;
            fin_left = false;
            is_turning_left = false;

            left_foot_org = left_control.position;
            left_dir_org = left_dir.position;
            right_leg_in_front = true;

        }
        else
        {
            right_leg.RotateAround(left_leg.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_knee.RotateAround(left_knee.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_foot.RotateAround(left_foot.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_control.RotateAround(left_control.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_dir.RotateAround(left_leg.position, Vector3.up, rotate_speed * Time.deltaTime);
        }
        //if(Mathf.Abs(angle) >= 89.9 && Mathf.Abs(angle) <= 90.1)
        //{
        //    //right_foot_down = true;
        //    //right_control.position += right_control.forward * -speed * Time.deltaTime;
        //    right_control.position += right_control.up * -speed * Time.deltaTime;

        //    if(right_control.position.y <= 0.5)
        //    {
        //        right_foot_org = right_control.position;
        //        fin_left = false;
        //        is_turning_left = false;
        //    }
        //}
        //else
        //{
        //    if((right_knee.position - right_dir.position).sqrMagnitude <= 0.25)
        //    {
        //        right_leg.position += right_leg.forward * speed * Time.deltaTime;
        //        right_dir.position += right_dir.forward * speed * Time.deltaTime;
        //    }
        //    else
        //    {
        //        right_control.position += right_control.forward * speed * Time.deltaTime;
        //        right_control.position += right_control.up * speed * Time.deltaTime;
        //    }
        //}
    }//end finish_left_turn()

    //turn right
    void turn_right()
    {
        //Debug.Log("right");
        is_turning_right = true;

        right_dir.parent = right_leg;
        //right_dir.parent = right_leg;

        right_control.parent = right_foot;
        //right_control.parent = right_foot;

        right_leg.Rotate(0, -rotate_speed * Time.deltaTime, 0);
        //right_leg.Rotate(Vector3.up, rotate_speed * Time.deltaTime);

    }

    void finish_right_turn()
    {
        //Debug.Log("finishing right turn");

        if (is_turning_right)
        {
            //is_turning_left = false;

            right_dir.parent = null;
            //right_dir.parent = null;

            right_control.parent = null;
            //right_control.parent = null;
        }

        //swing leg
        float angle = Vector3.SignedAngle(left_leg.position - right_leg.position, right_leg.forward, Vector3.up);

        left_knee.parent = null;
        left_foot.parent = null;

        if (Mathf.Abs(angle) >= 89.9 && Mathf.Abs(angle) <= 90.1)
        //if (Mathf.Abs(angle) == 90.0f)
        {
            left_knee.parent = left_leg;
            left_foot.parent = left_knee;
            //Debug.Log("dist: " + (right_leg.position - right_dir.position).magnitude);
            right_foot_org = right_control.position;
            right_dir_org = right_dir.position;
            fin_right = false;
            is_turning_right = false;

            left_foot_org = left_control.position;
            left_dir_org = left_dir.position;
            right_leg_in_front = false;

        }
        else
        {
            //Debug.Log("swing left leg");
            left_leg.RotateAround(right_leg.position, Vector3.up, -rotate_speed * Time.deltaTime);
            left_knee.RotateAround(right_knee.position, Vector3.up, -rotate_speed * Time.deltaTime);
            left_foot.RotateAround(right_foot.position, Vector3.up, -rotate_speed * Time.deltaTime);
            left_control.RotateAround(right_control.position, Vector3.up, -rotate_speed * Time.deltaTime);
            left_dir.RotateAround(right_leg.position, Vector3.up, -rotate_speed * Time.deltaTime);
        }
    }//end finish_right_turn()

    //void dist_correction()
    //{
    //    //if legs are not at same distance
    //    if((right_leg.position - left_leg.position).magnitude != leg_dist)
    //    {
    //        for (int i = 0; i < 10; i++)
    //        {
    //            //legs too far apart
    //            if ((right_leg.position - left_leg.position).magnitude > leg_dist)
    //            {
    //                if (!is_moving_left_leg)
    //                {
    //                    right_leg.position = Vector3.MoveTowards(right_leg.position, left_leg.position, 0.2f);
    //                }
    //                else if (!is_moving_right_leg)
    //                {
    //                    left_leg.position = Vector3.MoveTowards(left_leg.position, right_leg.position, 0.2f);
    //                }
    //            }
    //            else
    //            {
    //                if (!is_moving_left_leg)
    //                {
    //                    right_leg.LookAt(left_leg);
    //                    right_leg.Rotate(0, 180, 0);
    //                    right_leg.position += right_leg.forward * speed * 0.1f * Time.deltaTime;
    //                }
    //                else if (!is_moving_right_leg)
    //                {
    //                    left_leg.LookAt(left_leg);
    //                    left_leg.Rotate(0, 180, 0);
    //                    left_leg.position += left_leg.forward * speed * 0.1f * Time.deltaTime;
    //                }
    //            }
    //        }
    //    }

    //    //if right leg and dir not at same distance as at start
    //    if ((right_leg.position - right_dir.position).magnitude != leg_dir_dist)
    //    {
    //        for (int i = 0; i < 10; i++)
    //        {
    //            if ((right_leg.position - right_dir.position).magnitude > leg_dir_dist)
    //            {
    //                right_dir.position += right_dir.forward * -speed * 0.1f * Time.deltaTime;
    //            }

    //            if ((right_leg.position - right_dir.position).magnitude < leg_dir_dist)
    //            {
    //                right_dir.position += right_dir.forward * speed * 0.1f * Time.deltaTime;
    //            }
    //        }  
    //    }

    //    //if left leg and dir not at same distance as at start
    //    if ((left_leg.position - left_dir.position).magnitude != leg_dir_dist)
    //    {
    //        for (int i = 0; i < 10; i++)
    //        {
    //            if ((left_leg.position - left_dir.position).magnitude > leg_dir_dist)
    //            {
    //                left_dir.position += left_dir.forward * -speed * 0.1f * Time.deltaTime;
    //            }

    //            if ((left_leg.position - left_dir.position).magnitude < leg_dir_dist)
    //            {
    //                left_dir.position += left_dir.forward * speed * 0.1f * Time.deltaTime;
    //            }
    //        }
    //    }

    //}//end dist_correction()
}
