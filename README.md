# robotnik_description

This package includes the description for Robotnik robots.

Available robots and models are:
 - RB-1
 - RB-Ficus
 - RB-Kairos / RB-Kairos+
 - RB-Robout / RB-Robout+
 - RB-Summit
 - RB-Summit-Steel
 - RB-Theron / RB-Theron+
 - RB-Vogui / RB-Vogui+
 - RB-Vogui-XL
 - RB-Watcher


## Quick Usage

```
ros2 launch robotnik_description robot_description.launch.py robot:=ROBOT [robot_model:=ROBOT_MODEL]
```

Launches the description of a ROBOT, specifically ROBOT_MODEL.

## Contributing

All files must be formatted using [Prettier](https://prettier.io/) with the configuration found in [.github/.prettierrc.json](.github/.prettierrc.json).

Install npm, prettier and prettier/plugin-xml:

```bash
sudo apt install npm
sudo npm install -g prettier @prettier/plugin-xml
```

To format the files, run the following command inside the package folder:

```bash
prettier --plugin=$(npm root -g)/@prettier/plugin-xml/src/plugin.js --config .github/.prettierrc.json --write "urdf/**/*.{xml,xacro,urdf}"
```

## Structure

The description of a robot is divided into three parts:

- **robot**: The whole robot, including all its parts and customizations.
- **base**: The basic unit for each robot, composed of chassis + wheels.
- **body**: The chassis of each robot.

![image](/img/robot_urdf.png)

This repository contains three high-level folders:

- **robots**: Contains the URDF for complete robots, including the mobile base, arms, sensors, etc.
- **urdf**: Contains the URDF files of components that compose a robot.
- **meshes**: Contains the 3D meshes for each individual component of the robots.

### Robots

The `robots` folder contains a folder for each robot type:

 - rb1
 - rbfiqus
 - rbkairos
 - rbrobout
 - rbsummit
 - rbsummit_steel
 - rbtheron
 - rbvogui
 - rbvogui_xl
 - rbwatcher

Inside each robot type folder, there may exist several versions.

### URDF

The `urdf` folder contains a folder for each main component:

- **arms**: Contains the different types of robot arms.
- **bases**: Body of robots, including wheels, structures, and arms.
- **bodies**: Includes chassis.
- **structures**: Other structures included in a robot (e.g., columns, protection, elevator, support, etc.).
- **wheels**: Contains the different types of wheels.

## Launch

The launch files in this package run the `robot_state_publisher` node, publishing the topic `robot_description`.

### Nodes

- **robot_state_publisher** (`robot_state_publisher/robot_state_publisher`)

  Standard `robot_state_publisher` node from [robot_state_publisher](https://github.com/ros/robot_state_publisher).

### Topics

#### Input Topics

- **joint_states** (`sensor_msgs/msg/JointState`)

  The joint state updates to the robot poses.

#### Output Topics

- **~/robot_description** (`std_msgs/msg/String`)

  The description of the robot URDF as a string.

- **tf** (`std_msgs/msg/String`)

  The transforms corresponding to the movable joints of the robot.

- **tf_static** (`std_msgs/msg/String`)

  The transforms corresponding to the static joints of the robot.

### Arguments

- **namespace** (string, default: `robot`)

  Adds a namespace to this launch.

- **robot** (string, default: ``)

  Specifies which robot to select from the [robots](#robots) folder.

- **robot_model** (string, default: same as `robot`)

  Specifies the model to use from the [robots](#robots) folder.

- **robot_xacro_path** (string, default: `none`)

  Absolute path to specify a custom URDF/Xacro robot description, discarding the `robot` and `robot_model` arguments.

- **gazebo_ignition** (boolean, default: `False`)

  Boolean to set if simulating in Gazebo Ignition.

## Usage

```
ros2 launch robotnik_description robot_description.launch.py robot:=rbkairos
```

Launches the description for the RB-Kairos mobile base.

```
ros2 launch robotnik_description robot_description.launch.py robot:=rbkairos robot_model:=rbkairos_plus
```

Launches the description for the RB-Kairos with a UR arm.

```
ros2 launch robotnik_description robot_description.launch.py robot:=rbkairos robot_model:=rbkairos_plus namespace:=robot_b
```

Launches the description for the RB-Kairos with a UR arm, under the namespace `robot_b`.
