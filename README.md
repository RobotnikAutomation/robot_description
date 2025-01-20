# robot_description

This package includes all the URDF description for the real robots.

## 1. Structure

The structure of the robot description files is as follows:

```
robot_description/
├── robots/
│   ├── rbkairos/
│   │   ├── rbkairos.urdf.xacro
│   │   └── rbkairos_ur.urdf.xacro
│   ├── rbvogui/
│   │   ├── rbvogui.urdf.xacro
│   │   └── rbvogui_cart.urdf.xacro
│   ├── rbtheron/
│   │   └── rbtheron.urdf.xacro
│   ├── rbsummit/
│   │   └── rbsummit.urdf.xacro
│   └── rbsteel/
│       └── rbsteel.urdf.xacro
├── bases/
│   ├── rbkairos/
│   │   └── rbkairos_base.urdf.xacro
│   ├── rbvogui/
│   │   └── rbvogui_base.urdf.xacro
│   ├── rbtheron/
│   │   └── rbtheron_base.urdf.xacro
│   ├── rbsummit/
│   │   └── rbsummit_base.urdf.xacro
│   ├── rbsteel/
│   │   └── rbsteel_base.urdf.xacro
├── urdf/
│   ├── arms/
│   │   ├── ur
│   │   └── kinova
│   ├── bodies/
│   │   ├── rbkairos/
│   │   │   ├── rbkairos_body.urdf.xacro
│   │   │   └── rbkairos_plus_body.urdf.xacro
│   │   ├── rbvogui/
│   │   │   ├── rbvogui_body.urdf.xacro
│   │   │   └── rbvogui_6w_body.urdf.xacro
│   │   ├── rbtheron/
│   │   │   └── rbtheron_body.urdf.xacro
│   │   ├── rbsummit/
│   │   │   └── rbsummit_body.urdf.xacro
│   │   └── rbsteel/
│   │       └── rbsteel_body.urdf.xacro
│   ├── structures/
│   └── wheels/
│       ├── caster_wheel/
│       ├── fixed_wheel/
│       ├── omni_wheel/
│       ├── rubber_wheel/
│       └── steering_wheel/
└── meshes/
    ├── bases/
    │   ├── rbkairos/
    │   ├── rbvogui/
    │   ├── rbtheron/
    │   ├── rbsummit/
    │   └── rbsteel/
    ├── structures/
    └── wheels/
        ├── caster_wheel/
        ├── fixed_wheel/
        ├── omni_wheel/
        ├── rubber_wheel/
        └── steering_wheel/

```

The package is divided in 4 folders:

- robots: Inside this folder, there is a folder per robot and each one contains a .urdf.xacro per robot model (macro + sensors + arm).
- bases: Inside this folder, there is a urdf.xacro with the macro of the base robot model (body + wheels) divided by each robot folder.
- urdf: This folder contains all the urdf files that define the parts of the mobile base:
  - bodies: divided by each robot folder, includes all the chassis, logos, leds, etc.
  - arms: includes the description of arms, at the moment it is only the ur description
  - wheels: this includes all kind of wheels that are in the robot models:
    - caster_wheels
    - fixed_wheels
    - rubber_wheels
    - steering_wheels
    - omni_wheels
  - structures: any other structure that could be included in a robot (columns, protection, elevator, support, etc.)
- meshes: The folder is divided by:
  - bases: a folder per robot including all the files for the base.
  - structures: including all the structures meshes for the structures urdf.
  - wheels: meshes for the wheels urdf.

## 2. Robot Structure

### robots

To understand the structure of the robots definition let's see an example, in this case the [rbkairos.urdf.xacro](/robots/rbkairos/rbkairos.urdf.xacro).

First, it is included the macro file of the base robot (body + wheels) and the sensors macro that are in the package [robotnik_sensors](https://github.com/RobotnikAutomation/robotnik_sensors/tree/humble-devel).

```
  <xacro:include filename="$(find robot_description)/bases/rbkairos/rbkairos_base.urdf.xacro" />

  <!-- Import all available sensors -->
  <xacro:include filename="$(find robotnik_sensors)/urdf/all_sensors.urdf.xacro" />
```

Then, there are some properties declared to define the position of the sensors.

```
  <xacro:property name="front_laser_offset_x" value="0.2865"/>
  <xacro:property name="front_laser_offset_y" value="-0.20894"/>
  <xacro:property name="front_laser_offset_z" value="0.2973"/>

  <xacro:property name="rear_laser_offset_x" value="-0.2865"/>
  <xacro:property name="rear_laser_offset_y" value="0.20894"/>
  <xacro:property name="rear_laser_offset_z" value="0.2973"/>

  <xacro:property name="imu_offset_x" value="0.127"/>
  <xacro:property name="imu_offset_y" value="-0.129"/>
  <xacro:property name="imu_offset_z" value="0.212"/>
```

Next, the macro of the robot is called.

```
  <xacro:rbkairos/>
```

And finally the sensors are called, including the position declared in the properties.

```
  <!-- IMU -->
	<xacro:sensor_vectornav prefix="rbkairos_" parent="rbkairos_base_link" topic="imu/data">
    <origin xyz="${imu_offset_x} ${imu_offset_y} ${imu_offset_z}" rpy="0 0 0"/>
  </xacro:sensor_vectornav>


  <!-- SENSORS -->
  <xacro:sensor_sick_s300 prefix="rbkairos_front_laser" parent="rbkairos_base_link" prefix_topic="front_laser" gpu="true">
    <origin xyz="${front_laser_offset_x} ${front_laser_offset_y} ${front_laser_offset_z}" rpy="0 ${-PI} ${3/4*PI}" />
  </xacro:sensor_sick_s300>
  <xacro:sensor_sick_s300 prefix="rbkairos_rear_laser" parent="rbkairos_base_link" prefix_topic="rear_laser" gpu="true">
    <origin xyz="${rear_laser_offset_x} ${rear_laser_offset_y} ${rear_laser_offset_z}" rpy="0 ${-PI} ${-1/4*PI}" />
  </xacro:sensor_sick_s300>
```

Let's see now the macro file definition.

### macro

Moving to the macro file included in the previous robot file [rbkairos_base.urdf.xacro](bases/rbkairos/rbkairos_base.urdf.xacro).

As in the previous robot file, first it is included the robot body macro file and the wheels macro file.

```

	<xacro:include filename="$(find robot_description)/urdf/bodies/rbkairos/rbkairos_plus_body.urdf.xacro" />

  <xacro:include filename="$(find robot_description)/urdf/wheels/omni_wheel/omni_wheel.urdf.xacro" />
```

Then, it is defined the properties of the position of the wheels.

```

  	<xacro:property name="PI" value="3.1415926535897931"/>

  	<!-- Wheel parameters -->
  	<xacro:property name="wheel_offset_x" value="0.21528" />    <!-- x,y,z in translation from base_link to the center of the wheel -->
  	<xacro:property name="wheel_offset_y" value="0.2590" />
  	<xacro:property name="wheel_offset_z" value="0.0" />

```

And finally, the robot macro definition which includes calling the macro body (chassis + logos) and the wheels macros.

```

    <xacro:macro name="rbkairos">

  		<!-- *************** -->
  		<!-- Robots Elements -->
  		<!-- *************** -->

  		<!-- Here we create the robot elements using the xacro:macros imported at the beggining of this file -->

  		<xacro:rbkairos_plus_body />

  		<xacro:omni_wheel robot_id="rbkairos_front_right" parent="rbkairos_base_link" reflect="false" hq="true">
  			<origin xyz="${wheel_offset_x} -${wheel_offset_y} ${wheel_offset_z}" rpy="0 0 0"/>
  		</xacro:omni_wheel>

  		<xacro:omni_wheel robot_id="rbkairos_front_left" parent="rbkairos_base_link" reflect="true" hq="true">
  			<origin xyz="${wheel_offset_x} ${wheel_offset_y} ${wheel_offset_z}" rpy="0 0 0"/>
  		</xacro:omni_wheel>

  		<xacro:omni_wheel robot_id="rbkairos_back_left" parent="rbkairos_base_link" reflect="true" hq="true">
  			<origin xyz="-${wheel_offset_x} ${wheel_offset_y} ${wheel_offset_z}" rpy="0 0 0"/>
  		</xacro:omni_wheel>

  		<xacro:omni_wheel robot_id="rbkairos_back_right" parent="rbkairos_base_link" reflect="false" hq="true">
  			<origin xyz="-${wheel_offset_x} -${wheel_offset_y} ${wheel_offset_z}" rpy="0 0 0"/>
  		</xacro:omni_wheel>

     </xacro:macro>
```

The [macro body file](urdf/bodies/rbkairos/rbkairos_plus_body.urdf.xacro) includes the links and joints definition of the base_link, base_footprint, etc.

The [wheels macro file](urdf/wheels/omni_wheel/omni_wheel.urdf.xacro) includes the links and joints of the mecanum wheel.

## 3. How to define a new robot for a project?

The structure behind the robot_description is as the following image.

![image](/img/robot_description.matrioska.png)

So in that case, the specific model and variation of the robot depends only in the robot.urdf.xacro that can be found in [robots folder](/robots/)

In case of need to use a new robot that it's not defined in robot_description the idea is to use the [base macro files](/bases/). As explained before, this macro files includes the body and wheels, all the basics of the robots, all the modifications are over this macro.

Taking the example of the robot structure of the previous point, the idea to define a new robot is:

1. Start by including the macro files needed ([robot base macro](/macros/), [sensors](https://github.com/RobotnikAutomation/robotnik_sensors/tree/humble-devel) and all the needed [structures](/urdf/structures/)).
2. Define properties of position.
3. Define any extra argument needed for the launch of the robot_state_publisher.
4. Call the macro of the robot base.
5. Call the macros of the sensors.
6. Add arm if the robot has it.

## 4. Launch

The launch that can be found in this package run the robot state publisher node, publishing the topic robot_description. The launch file includes the following arguments:

|  argument | default  | definition  |
|---|---|---|
| namespace  |  robot | add namespace to the node and topic (/robot/robot_state_publisher and /robot/robot_description)  |
|  robot | rbvogui  | which robot select from folder robots  |
|  robot_model | robot argument value  | this argument is used to select the specific version of the robot. Example, on rbkairos folder there are 2 versions, rbkarios and rbkairos_ur. To use this argument correctly, the previous argument has to be used also.  |
|  robot_xacro_path | path to the urdf file to use | In case of using a custom robot that it is not in robot_description, select the path to the file in this argument|

## 5. Examples

Here there are some examples of the robot_description launch.

- With this launch, the rbsummit description will be published.
```
ros2 launch robot_description robot_description.launch.py robot:=rbsummit
```

- With this launch, the rbkairos_ur description will be published.
```
ros2 launch robot_description robot_description.launch.py robot:=rbkairos robot_model:=rbkairos_ur
```

- With this launch, a second rbkairos_ur description will be published with namespace robot_b.
```
ros2 launch robot_description robot_description.launch.py robot:=rbkairos robot_model:=rbkairos_ur namespace:=robot_b
```