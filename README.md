# robot_description

This package includes all the URDF description for the real robots.

## 1. Launch

The launch that can be found in this package run the robot state publisher node, publishing the topic robot_description. The launch file includes the following arguments:

|  argument | default  | definition  |
|---|---|---|
| namespace  |  robot | add namespace to the node and topic (/robot/robot_state_publisher and /robot/robot_description)  |
|  robot | rbvogui  | which robot select from folder robots  |
|  robot_model | robot argument value  | this argument is used to select the specific version of the robot. Example, on rbkairos folder there are 2 versions, rbkarios and rbkairos_ur. To use this argument correctly, the previous argument has to be used also.  |
|  robot_xacro_path | path to the urdf file to use | In case of using a custom robot that it is not in robot_description, select the path to the file in this argument|
|  gazebo_classic | False | Boolean to set if simulating in gazebo classic |

## 2. Examples

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

## 3. How to define a new robot for a project?

The structure behind the robot_description is as the following image.

![image](/img/robot_description.matrioska.png)

So in that case, the specific model and variation of the robot depends only in the robot.urdf.xacro that can be found in [robots folder](/robots/)

To create a new robot that it's not defined in robot_description we will use the [mobile base macro files](/urdf/bases/). This macro files includes the body and wheels, all the basics of the robots, all the modifications are over this macro.

You can base in the [robot_template.urdf.xacro](/robots/robot_template.urdf.xacro) file.

1. Start by including the macro files needed ([robot base macro](/macros/), [sensors](https://github.com/RobotnikAutomation/robotnik_sensors/tree/humble-devel) and all the needed [structures](/urdf/structures/)).

```xml
<!-- Import all posible elements defined in the urdf.xacro files. All these elements are defined as macro:xacros -->

  <xacro:include filename="$(find robot_description)/urdf/bases/rbvogui/rbvogui_base.urdf.xacro" />
  <xacro:include filename="$(find robotnik_sensors)/urdf/all_sensors.urdf.xacro" />

```

2. Define arguments of the urdf.

```xml

  <!-- Args -->

  <xacro:arg name="robot" default="rbvogui"/>
  <xacro:arg name="namespace" default="robot"/>
  <xacro:arg name="prefix" default="robot_"/>
  <xacro:arg name="has_arm" default="false"/>
  <xacro:arg name="gazebo_classic" default="false"/>
```

3. Define parameters of the urdf.

```xml

  <!-- Properties -->

  <xacro:property name="hq" value="true"/>

  <xacro:property name="front_laser_offset_x" value="0.2865"/>
  <xacro:property name="front_laser_offset_y" value="-0.20894"/>
  <xacro:property name="front_laser_offset_z" value="0.2973"/>
```

4. Call the macro of the robot base.

```xml

  <!-- Robot -->
  <xacro:rbvogui prefix="$(arg prefix)" hq="${hq}"/>
```

5. Call the macros of the sensors.

```xml
<!-- Sensors -->

  <xacro:sensor_sick_s300 prefix="rbkairos_front_laser" parent="rbkairos_base_link" prefix_topic="front_laser" gpu="true">
    <origin xyz="${front_laser_offset_x} ${front_laser_offset_y} ${front_laser_offset_z}" rpy="0 ${-PI} ${3/4*PI}" />
  </xacro:sensor_sick_s300>
```

6. Add arm if the robot has it.

```xml

  <!-- Arm -->

  <xacro:if value="$(arg has_arm)">
    <xacro:arg name="ur_type" default="ur5e"/>
    <xacro:arg name="ur_name" default="$(arg ur_type)"/>
    <xacro:arg name="joint_limit_params" default="$(find ur_description)/config/$(arg ur_type)/joint_limits.yaml"/>
    <xacro:arg name="kinematics_params" default="$(find ur_description)/config/$(arg ur_type)/default_kinematics.yaml"/>
    <xacro:arg name="physical_params" default="$(find ur_description)/config/$(arg ur_type)/physical_parameters.yaml"/>
    <xacro:arg name="visual_params" default="$(find ur_description)/config/$(arg ur_type)/visual_parameters.yaml"/>
    <xacro:arg name="use_fake_hardware" default="false" />
    <xacro:arg name="fake_sensor_commands" default="false" />
    <xacro:arg name="sim_gazebo" default="true" />
    <xacro:arg name="sim_ignition" default="false" />

    <xacro:include filename="$(find robot_description)/urdf/arms/ur/ur.urdf.xacro"/>
  </xacro:if>
```

7. Add the control plugin for Gazebo, if it is going to be used in simulation

```xml

  <!-- Control -->
  <xacro:include filename="$(find robot_description)/simulators/gazebo_classic/rbvogui/rbvogui_control.urdf.xacro" />

  <xacro:if value="$(arg gazebo_classic)">
    <xacro:rbvogui_gz_classic_control namespace="$(arg namespace)" prefix="$(arg prefix)"/>
    <xacro:ros_planar_move_gazebo_classic/>
  </xacro:if>
```

## 4. Structure

The package is divided in 4 folders:

- robots: This folder contains the URDF files that includes mobile base + arms + sensors. This URDF are the default robots models from Robotnik.
- bases: This folder contains the URDF files that includes the body of the robot + wheels. This URDF are the macros to generate a standard robot.
- urdf: This folder contains the remaining URDF files that compound a robot:
  - bodies: Includes all the chassis, logos, leds, etc.
  - arms: Includes the description of arms.
  - wheels: Includes all kind of wheels that are in the robot models:
    - caster_wheels
    - fixed_wheels
    - rubber_wheels
    - steering_wheels
    - omni_wheels
  - structures: Includes any other structure that could be included in a robot (columns, protection, elevator, support, etc.)
- meshes: The folder includes all the 3D model of the robots:
  - bases
  - structures
  - wheels

## 5. Robot Structure

### robots

To understand the structure of the robots definition let's see an example, in this case the [rbkairos.urdf.xacro](/robots/rbkairos/rbkairos.urdf.xacro).

First, it is included the macro file of the base robot (body + wheels) and the sensors macro that are in the package [robotnik_sensors](https://github.com/RobotnikAutomation/robotnik_sensors/tree/humble-devel).

```xml
  <xacro:include filename="$(find robot_description)/urdf/bases/rbkairos/rbkairos_base.urdf.xacro" />

  <!-- Import all available sensors -->
  <xacro:include filename="$(find robotnik_sensors)/urdf/all_sensors.urdf.xacro" />
```

Then, there are some properties declared to define the position of the sensors.

```xml
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

```xml
  <xacro:rbkairos/>
```

And finally the sensors are called, including the position declared in the properties.

```xml
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

Moving to the macro file included in the previous robot file [rbkairos_base.urdf.xacro](urdf/bases/rbkairos/rbkairos_base.urdf.xacro).

As in the previous robot file, first it is included the robot body macro file and the wheels macro file.

```

	<xacro:include filename="$(find robot_description)/urdf/bodies/rbkairos/rbkairos_plus_body.urdf.xacro" />

  <xacro:include filename="$(find robot_description)/urdf/wheels/omni_wheel/omni_wheel.urdf.xacro" />
```

Then, it is defined the properties of the position of the wheels.

```xml

  	<xacro:property name="PI" value="3.1415926535897931"/>

  	<!-- Wheel parameters -->
  	<xacro:property name="wheel_offset_x" value="0.21528" />    <!-- x,y,z in translation from base_link to the center of the wheel -->
  	<xacro:property name="wheel_offset_y" value="0.2590" />
  	<xacro:property name="wheel_offset_z" value="0.0" />

```

And finally, the robot macro definition which includes calling the macro body (chassis + logos) and the wheels macros.

```xml

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

