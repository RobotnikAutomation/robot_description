# robot_description

This package includes the description for Robotnik robots.

Available robots are:
 - RB-Kairos
 - RB-Theron
 - RB-Vogui
 - RB-Robout
 - RB-Summit

## Structure

The description of a robot is divided in three parts:

- **robot**: the whole robot, including all its parts and customizations.
- **base**: the basic unit for each robot, composed of chassis + wheels.
- **body**: the chassis of each robot.


![image](/img/robot_urdf.png)


This repostory contains 3 high level folders:

- **robots**: contains the URDF for complete robots, that include the mobile base, arms, sensors, etc. 
- **urdf**: contains the URDF files of components that compose a robot.
- **meshes**: contains the 3D meshes for each individual component of the robots.

### robots

The robots folder contains a folder for each robot type:

 - rbkairos
 - rbrobout
 - rbsummit
 - rbvogui
 - rbtheron

 Inside each robot type folder, there may exist several versions.
 
### urdf

The urdf folder contains a folder for each main component:

  - bases: body of robots + wheels + structures + arms.
  - bodies: include chassis.
  - structures: other structures that are included in a robot (columns, protection, elevator, support, etc.)
  - wheels: with the different type of wheels.


## Launch

The launch that can be found in this package run the robot state publisher node, publishing the topic robot_description.

### Nodes

- **robot_state_publisher** (robot_state_publisher/robot_state_publisher)

Standard robot_state_publisher node from [robot_state_publisher](https://github.com/ros/robot_state_publisher)

### Topics

#### Input topics

- **joint_states** (sensor_msgs/msg/JointState)

The joint state updates to the robot poses

#### Output topics

- **~/robot_description** (std_msgs/msg/String)

The description of the robot URDF as a string.

- **tf** (std_msgs/msg/String)

The transforms corresponding to the movable joints of the robot.

- **tf_static** (std_msgs/msg/String)

The transforms corresponding to the static joints of the robot.

### Arguments

- **namespace** (string, default: robot)

Adds a namespace to this launch.

- **robot** (string, default: *rbvogui*)

Which robot select from folder [robots](#robots).

- **robot_model** (string, default: *same as robot*)

Specify the model to use, from  [robots](/robots/)

- **robot_xacro_path** (string, default: *none*)

Absolute path to specify a custom urdf/xacro robot description, discarding robot and robot_model arguments.

- **gazebo_classic** (boolean, default: False)

Boolean to set if simulating in gazebo classic. To be deprecated.

## Usage

```
ros2 launch robot_description robot_description.launch.py
```

Launches the description for the RB-Vogui, the default robot.

```
ros2 launch robot_description robot_description.launch.py robot:=rbkairos
```

Launches the description for the RB-Kairos mobile base.

```
ros2 launch robot_description robot_description.launch.py robot:=rbkairos robot_model:=rbkairos_ur
```

Launches the description for the RB-Kairos with a UR arm.

```
ros2 launch robot_description robot_description.launch.py robot:=rbkairos robot_model:=rbkairos_ur namespace:=robot_b
```

Launches the description for the RB-Kairos with a UR arm, under the namespace robot_b


