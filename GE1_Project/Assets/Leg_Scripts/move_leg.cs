using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_leg : MonoBehaviour
{
    //speed for movement and rotation angle
    public float speed = 2.0f;
    public float rotate_speed = 30.0f;

    //angle of initial rotation of leg before turning
    public float init_rotation = 0.0f;

    //control points for legs and direction pointing
    public Transform right_control;
    public Transform right_dir;
    public Transform left_control;
    public Transform left_dir;

    // original position of directional pointer
    public Vector3 right_dir_org;
    public Vector3 left_dir_org;

    // booleans for making sure behaviour
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

    public bool is_moving_forward;
    public bool is_moving_backwards;

    // cast leg parts to simulate movements
    public Transform right_leg;
    public Transform right_knee;
    public Transform right_foot;

    public Transform left_leg;
    public Transform left_knee;
    public Transform left_foot;

    //cast pelvis
    public Transform pelvis;

    // give point of which pelvis should point towards when moving forward
    public Vector3 pelvis_height_adj;
    public GameObject pelvis_dir;

    // vector from left to right leg
    public Vector3 between_legs;

    // original position of controllers before moving
    public Vector3 right_foot_org;
    public Vector3 left_foot_org;

    // initial distances to ensure behaviour
    public float knee_foot_dist;
    public float leg_dist;
    public float leg_dir_dist;

    // offset for directional pointer
    public Vector3 right_dir_offset;
    public Vector3 left_dir_offset;

    public Vector3 right_offset;
    public Vector3 left_offset;

    // Start is called before the first frame update
    void Start()
    {
        // find corresponding objects
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

        //create invisible gameobject for the pelvis to tilt towards when legs move
        pelvis_dir = new GameObject();
        pelvis_height_adj = new Vector3(0f, 1.0f, 0f);

        //boolean controls
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

        is_moving_forward = false;
        is_moving_backwards = false;

        //initial position of directional controls
        right_dir_org = right_dir.position;
        left_dir_org = left_dir.position;

        //initial position of movement controls
        right_foot_org = right_control.position;
        left_foot_org = left_control.position;

        //initial distance for readjustments
        knee_foot_dist = (right_knee.position - right_foot.position).magnitude;
        leg_dir_dist = (right_leg.position - right_dir.position).magnitude;
        leg_dist = (right_leg.position - left_leg.position).magnitude;

        //offsets for turning
        right_dir_offset = right_leg.position - right_dir.position;
        left_dir_offset = left_leg.position - left_dir.position;

        left_offset = new Vector3(0, 0, 0);
        right_offset = new Vector3(0, 0, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        //if model is stationary, move according to user input
        if (!is_turning_left && !is_turning_right && !is_moving_backwards && !is_moving_forward)
        {
            move();
        }
        else
        {
            //otherwise follow last instruction until finished

            //continue to turn left until finish
            if (is_turning_left)
            {
                //find rotational angle of left leg
                float left_angle = left_leg.localRotation.eulerAngles.y;

                //range 0-360, turn left is added onto, 
                //so needs to lock range of 30 degrees max turn rate per command
                if(init_rotation >= 330)
                {
                    left_angle += 360;
                }

                //if user released A or turn angle >= 30 degrees (force end turn) or still completing turn motion
                if (Input.GetKeyUp(KeyCode.A) || Mathf.Abs(left_angle - init_rotation) >= 30.0f || fin_left)
                {
                    //keep completing the turn
                    fin_left = true;
                    finish_left_turn();
                }
                else
                {
                    //just start turning left (angle adjustment)
                    turn_left();
                }
                
            }

            //continue to turn right until finish
            if (is_turning_right)
            {
                //find rotational angle of right leg
                float right_angle = right_leg.localRotation.eulerAngles.y;

                //range 0-360, turn left is subtracted from,
                //so needs to lock range of 30 degrees max turn rate per command
                if (init_rotation <= 30)
                {
                    right_angle -= 360;
                }

                //if user released D or turn angle >= 30 degrees (force end turn) or still completing turn motion
                if (Input.GetKeyUp(KeyCode.D) || Mathf.Abs(init_rotation - right_angle) >= 30.0f || fin_right)
                {
                    //keep completing the turn
                    fin_right = true;
                    finish_right_turn();
                }
                else
                {
                    //just start turning right (angle adjustment)
                    turn_right();
                }
            }

            //if still moving forward
            if (is_moving_forward)
            {
                //move the leg in front forward
                if (right_leg_in_front)
                {
                    right_leg_forward();
                }
                else
                {
                    left_leg_forward();
                }
            }

            //if still moving backwards
            if (is_moving_backwards)
            {
                //move the leg in front backwards (leg order unchanged until finish moving)
                if (right_leg_in_front)
                {
                    right_leg_backwards();
                }
                else
                {
                    left_leg_backwards();
                }
            }
        }

        //used to use for turn distance correction
        //dist_correction();
    }

    //move according to user input
    void move()
    {
        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            //move leg behind or the one that is already moving
            if ((right_leg_in_front || is_moving_left_leg) && !is_moving_right_leg)
            {
                //left leg in front
                right_leg_in_front = false;

                //left leg is moving
                is_moving_left_leg = true;

                //moving forward
                is_moving_forward = true;

                //move left leg forward
                left_leg_forward();

                //make pelvis tilt to correct side following movement
                pelvis_follow("left");
            }
            else
            {
                //right leg in front
                right_leg_in_front = true;
                
                //right leg is moving
                is_moving_right_leg = true;

                //moving forward
                is_moving_forward = true;

                //move right leg forward
                right_leg_forward();

                //make pelvis tilt to correct side following movement
                pelvis_follow("right");

            }
            
        }//end moveforward if

        // move backwards
        if (Input.GetKey(KeyCode.S))
        {
            //move leg in front or leg already moving backwards
            if ((right_leg_in_front || is_moving_right_leg) && !is_moving_left_leg)
            {
                //move right leg backwards
                //Debug.Log("moving right");
                is_moving_right_leg = true;
                is_moving_backwards = true;

                right_leg_backwards();
                
            }
            else
            {
                //move left leg backwards
                //Debug.Log("moving left");
                is_moving_left_leg = true;
                is_moving_backwards = true;

                left_leg_backwards();
                
            }
        }//end move backwards if

        //turn left
        if (Input.GetKey(KeyCode.A))
        {
            //when legs are both on ground
            if (!is_moving_right_leg && !is_moving_left_leg)
            {
                //if starting to turn left, record initial rotation (for 30 degrees lock)
                if (!is_turning_left)
                {
                    init_rotation = left_leg.localRotation.eulerAngles.y;
                }

                //start turn left
                turn_left();
            }
            else
            {
                //set the moving foot back down onto ground
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


        //turn right
        if (Input.GetKey(KeyCode.D))
        {
            //when both legs on ground
            if (!is_moving_right_leg && !is_moving_left_leg)
            {
                //record starting rotation
                if (!is_turning_right)
                {
                    init_rotation = right_leg.localRotation.eulerAngles.y;
                }

                //start turn right
                turn_right();
            }
            else
            {
                //set moving foot back down to ground
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

    //move right leg forward
    void move_forward_right()
    {
        //if foot is to start setting down, don't move knee nor foot forward
        if (right_foot_down)
        {
            right_knee_forward = false;
            right_foot_forward = false;
        }

        //move knee forward
        if (right_knee_forward)
        {
            //Debug.Log("knee forward + foot");
            right_control.position += right_control.forward * speed * Time.deltaTime;
            right_control.position += right_control.up * speed * Time.deltaTime;
            right_foot.position = right_control.position;
        }

        //move foot forward
        if (right_foot_forward)
        {
            //Debug.Log("Foot Forward only");
            right_control.position += right_control.forward * speed * Time.deltaTime;
            right_foot.position = right_control.position;
        }

        //set foot down
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

    //boolean determinate for right leg move forward
    void right_leg_forward()
    {
        //if knee at directional controller, stop moving knee forward
        if ((right_knee.position - right_dir.position).sqrMagnitude <= 0.25)
        {
            right_knee_forward = false;

            //if foot under directional controller (original), stop moving foot forward
            if (Mathf.Abs(right_control.position.x - right_dir_org.x) <= 0.25 && Mathf.Abs(right_control.position.z - right_dir_org.z) <= 0.25)
            {
                right_foot_forward = false;

                //set foot down when leg is on top of directional controller (original)
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

        //if setting foot down
        if (right_foot_down)
        {
            // when foot on ground stop forward movement and reset original positions 
            //and change right foot to be in front
            if (right_control.position.y <= 0.5)
            {
                right_foot_down = false;
                right_dir_org = right_dir.position;
                right_leg_in_front = true;
                is_moving_right_leg = false;
                is_moving_forward = false;
                //Debug.Log("End Foot Down");
            }
        }


        move_forward_right();
    }// end right_leg_forward()

    //move_left
    void move_forward_left()
    {
        //if foot is to start setting down, don't move knee nor foot forward
        if (left_foot_down)
        {
            left_knee_forward = false;
            left_foot_forward = false;
        }

        //move knee forward
        if (left_knee_forward)
        {
            //Debug.Log("knee forward + foot");
            left_control.position += left_control.forward * speed * Time.deltaTime;
            left_control.position += left_control.up * speed * Time.deltaTime;
        }

        //move foot forward
        if (left_foot_forward)
        {
            //Debug.Log("Foot Forward only");
            left_control.position += left_control.forward * speed * Time.deltaTime;
        }

        //set foot down
        if (left_foot_down)
        {
            //Debug.Log("foot down");
            left_control.position += left_control.forward * -speed * Time.deltaTime;
            left_control.position += left_control.up * -speed * Time.deltaTime;
            left_leg.position += left_leg.forward * speed * Time.deltaTime;
            left_dir.position += left_dir.forward * speed * Time.deltaTime;

        }

    }// end move_forward()

    //left leg boolean determinate same as right leg
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
                is_moving_forward = false;
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
            //right_control.position += right_control.forward * -speed * Time.deltaTime;
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
            right_control.position += right_control.forward * -speed * Time.deltaTime;
            //right_foot.position = right_control.position;
        }

        if (right_control.position.y <= 0.5)
        {
            is_moving_right_leg = false;
            right_leg_in_front = false;
            right_control.position = right_foot.position;
            right_foot_org = right_control.position;
            is_moving_backwards = false;
        }
    }// end right_leg_backwards()

    //move left leg backwards
    void left_leg_backwards()
    {
        float dist = (left_foot.position - left_foot_org).magnitude;
        //Debug.Log("dist: " + dist);
        //Debug.Log("knee dist: " + knee_foot_dist);

        //when foot reaches right height to step back
        if (dist > knee_foot_dist / 3.0f)
        {
            left_control.position += left_control.up * -speed * Time.deltaTime;
            //left_control.position += left_control.forward * -speed * Time.deltaTime;
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
            left_control.position += left_control.forward * -speed * Time.deltaTime;
            //left_foot.position = left_control.position;
        }

        //set boolean controls and org positions
        if(left_control.position.y <= 0.5)
        {
            is_moving_left_leg = false;
            right_leg_in_front = true;
            left_control.position = left_foot.position;
            left_foot_org = left_control.position;
            is_moving_backwards = false;
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

    //complete left turn
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
        float angle = Mathf.Abs(Vector3.SignedAngle(right_leg.position - left_leg.position, left_leg.forward, Vector3.up));

        Debug.Log("left angle " + angle);

        right_knee.parent = null;
        right_foot.parent = null;

        if (angle >= 89.8f && angle <= 90.2f)
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
            //swing right leg around left
            right_leg.RotateAround(left_leg.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_knee.RotateAround(left_knee.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_foot.RotateAround(left_foot.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_control.RotateAround(left_control.position, Vector3.up, rotate_speed * Time.deltaTime);
            right_dir.RotateAround(left_leg.position, Vector3.up, rotate_speed * Time.deltaTime);
            pelvis.RotateAround(left_leg.position, Vector3.up, rotate_speed * Time.deltaTime);
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

        right_leg.Rotate(Vector3.up, -rotate_speed * Time.deltaTime);
        //right_leg.Rotate(Vector3.up, rotate_speed * Time.deltaTime);

    }

    //complete right turn, like left
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
        float angle = Mathf.Abs(Vector3.SignedAngle(left_leg.position - right_leg.position, right_leg.forward, Vector3.up));

        Debug.Log("right angle " + angle);
        Debug.Log("abs angle " + Mathf.Abs(angle));
        left_knee.parent = null;
        left_foot.parent = null;

        if (angle >= 89.8f && angle <= 90.2f)
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
            pelvis.RotateAround(right_leg.position, Vector3.up, -rotate_speed * Time.deltaTime);
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
