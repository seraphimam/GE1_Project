# **Simulation of humanoid walking**

**Name:** Ming Chung Poon

**Student Number:** C18748391

**Class Group:** DT228 / TU856

## Description of the project: 
Form a humanoid using cubes and circles, 
then making it to walk like a normal human would,
maybe use inverse kinematics.

## Instructions for use:
~old: \
the user only needs to press WASD (possibly QE) to move the humanoid

~update: \
W move forward\
S move backwards\
A turn left\
D turn right\


## How it works:
~old: \
when the user press w, the humanoid will swing one of their hands forward 
along with their leg on the different side simulating a person's movement, 
when the user press A or D (QE included), the humanoid will strafe left/right, 
Q or E to turn right or left, then D to move backwards slowly.

~update: \
Q, E not included\
A, D for turning instead of strafing\

## List of classes/assets in the project and whether made yourself or modified or if its from a source, please give the reference
~old:\
Humanoid.cs - self written\
Movement.cs - written using inverse kinematics (current plan - using youtube videos and unity docs)

~updated: \
Draw_left_upper_arm.cs - self writtern with unity forum help\
Draw_left_lower_arm.cs - self writtern with unity forum help\
Draw_left_upper_leg.cs - self writtern with unity forum help\
Draw_left_lower_leg.cs - self writtern with unity forum help\
\
Draw_right_upper_arm.cs - self writtern with unity forum help\
Draw_right_lower_arm.cs - self writtern with unity forum help\
Draw_right_upper_leg.cs - self writtern with unity forum help\
Draw_right_lower_leg.cs - self writtern with unity forum help\
\
Left_Leg_IK.cs - uses inverse kinematics (youtube ref + unity docs)\
Right_Leg_IK.cs - uses inverse kinematics (youtube ref + unity docs)\
\
move_leg.cs - self writter\
\
Maintain_leg_distance.cs - uses simplified IK\
\
Camera_follow.cs - self-written + unity forum help + unity docs\


## References:
[Unity Docs](https://docs.unity3d.com/Manual/InverseKinematics.html)\
[YouTube](https://www.youtube.com/watch?v=qqOAzn05fvk)
[YouTube](https://www.youtube.com/watch?v=JOm1Cr2p_cI)
[Unity Forum](https://answers.unity.com/questions/1482210/how-to-make-an-object-always-in-front-of-the-ovrpl.html)
[Unity Forum](https://answers.unity.com/questions/48934/how-to-scale-and-move-a-cuboid-so-that-it-fits-bet.html)


## What I am most proud of in the assignment:
~update: \
Just making it work is hard\
inverse kinematics is not easily understood nor applied\
coordinated movement caused a lot of bugs to be fixed\
turn angle creates visual errors when over 30 degrees (fix by locked)\
incomplete movements causes boolean control misbehaviours (fix by locked)\
