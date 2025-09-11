This is a Work in Progress document.

## How to add a new robot

So in that case, the specific model and variation of the robot depends only in the robot.urdf.xacro that can be found in [robots folder](/robots/)

To create a new robot that it's not defined in robotnik_description we will use the [mobile base macro files](/urdf/bases/). This macro files includes the body and wheels, all the basics of the robots, all the modifications are over this macro.

1. Start by including the macro files needed ([robot base macro](/urdf/bases/), [sensors](https://github.com/RobotnikAutomation/robotnik_sensors/) and all the needed [structures](/urdf/structures/)).

```xml
<!-- Import all posible elements defined in the urdf.xacro files. All these elements are defined as macro:xacros -->

  <xacro:include
    filename="$(find robotnik_description)/urdf/bases/rbvogui/rbvogui_base.urdf.xacro" />
  <xacro:include
    filename="$(find robotnik_sensors)/urdf/all_sensors.urdf.xacro" />

```

2. Define arguments of the urdf.

```xml

  <!-- ARGUMENTS -->

  <xacro:arg name="namespace" default="robot"/>
  <xacro:arg name="prefix" default="robot_"/>
  <xacro:arg name="gazebo_ignition" default="false"/>
```

3. Call the macro of the robot base.

```xml

  <!-- Robot -->
  <xacro:rbvogui prefix="$(arg prefix)" hq="${hq}"/>
```

4. Call the macros of the sensors.

```xml
<!-- Sensors -->

  <xacro:sensor_sick_s300 prefix="rbkairos_front_laser" parent="rbkairos_base_link" prefix_topic="front_laser" gpu="true">
    <origin xyz="1.0 2.0 3.0" rpy="0 ${-pi} ${3/4*pi}" />
  </xacro:sensor_sick_s300>
```

5. Add arm if the robot has it.

```xml
  <xacro:include
    filename="$(find robotnik_description)/urdf/arms/ur_macro.urdf.xacro" />

  <!-- Arm -->
  <xacro:arm_ur
    ur_type="$(arg ur_type)"
    frame_prefix="$(arg prefix)arm_"
    parent="$(arg prefix)top_cover"
    gazebo_ignition="$(arg gazebo_ignition)">
    <origin
      xyz="0 0 0"
      rpy="0 0 0" />
  </xacro:arm_ur>

```

### Robots

To understand the structure of the robots definition let's see an example, in this case the [rbkairos.urdf.xacro](/robots/rbkairos/rbkairos.urdf.xacro).

First, it is included the macro file of the base robot (body + wheels) and the sensors macro that are in the package [robotnik_sensors](https://github.com/RobotnikAutomation/robotnik_sensors/).

```xml
  <xacro:include
    filename="$(find robotnik_description)/urdf/bases/rbkairos/rbkairos_base.urdf.xacro" />
  <xacro:include
    filename="$(find robotnik_sensors)/urdf/all_sensors.urdf.xacro" />
```

Then, set default arguments to xacro:

```xml

  <!-- ARGUMENTS -->
  <xacro:arg name="namespace" default="robot" />
  <xacro:arg name="prefix" default="" />
  <xacro:arg name="gazebo_ignition" default="false" />

```

Next, the macro of the robot is called.

```xml
  <xacro:rbkairos
    prefix="$(arg prefix)"
    gazebo_ignition="$(arg gazebo_ignition)"
    namespace="$(arg namespace)" />
```

And finally the sensors are called, including the position declared in the properties.

```xml
  <!-- IMU -->
  <xacro:sensor_vectornav
    frame_prefix="$(arg prefix)imu_"
    parent="$(arg prefix)base_link"
    node_namespace="$(arg namespace)"
    gazebo_ignition="$(arg gazebo_ignition)">
    <origin
      xyz="0.127 -0.129 0.212"
      rpy="0 0 0" />
  </xacro:sensor_vectornav>

  <!-- LASERS -->
  <xacro:sensor_sick_s300
    frame_prefix="$(arg prefix)front_laser_"
    parent="$(arg prefix)base_link"
    node_namespace="$(arg namespace)"
    node_name="front_laser"
    gazebo_ignition="$(arg gazebo_ignition)">
    <origin
      xyz="0.2865 -0.20894 0.2973"
      rpy="0 ${-pi} ${3/4*pi}" />
  </xacro:sensor_sick_s300>
```

Let's see now the macro file definition.

### MACROS

Moving to the macro file included in the previous robot file [rbkairos_base.urdf.xacro](urdf/bases/rbkairos/rbkairos_base.urdf.xacro).

As in the previous robot file, first it is included the robot body macro file and the wheels macro file.

```xml
  <xacro:include
    filename="$(find robotnik_description)/urdf/bodies/rbkairos/rbkairos_body.urdf.xacro" />
  <xacro:include
    filename="$(find robotnik_description)/urdf/wheels/mecanum_wheel/rbkairos_mecanum_wheel.urdf.xacro" />
  <xacro:include
    filename="$(find robotnik_description)/simulators/gazebo_ignition/rbkairos/rbkairos_control.urdf.xacro" />
```

Then, it is defined the properties of the position of the wheels.

```xml

  <!-- WHEELS PROPERTIES -->
  <xacro:property name="wheel_offset_x" value="0.21528" />
  <xacro:property name="wheel_offset_y" value="0.2590" />
  <xacro:property name="wheel_offset_z" value="0.0" />

```

And finally, the robot macro definition which includes calling the macro body (chassis + logos) and the wheels and control macros.

```xml

  <xacro:macro
    name="rbkairos"
    params="
      prefix:=robot_
      gazebo_ignition:=false
      namespace:=''
      ">
    <xacro:property
      name="hq"
      value="true" />
    <xacro:rbkairos_body prefix="${prefix}" />

    <!-- WHEELS -->
    <xacro:rbkairos_mecanum_wheel
      prefix="${prefix}front_right_"
      parent="${prefix}base_link"
      reflect="false"
      hq="true"
      gazebo_ignition="${gazebo_ignition}"
      xyz="${wheel_offset_x} -${wheel_offset_y} ${wheel_offset_z}" />
    <xacro:rbkairos_mecanum_wheel
      prefix="${prefix}front_left_"
      parent="${prefix}base_link"
      reflect="true"
      hq="true"
      gazebo_ignition="${gazebo_ignition}"
      xyz="${wheel_offset_x} ${wheel_offset_y} ${wheel_offset_z}" />
    <xacro:rbkairos_mecanum_wheel
      prefix="${prefix}back_left_"
      parent="${prefix}base_link"
      reflect="false"
      hq="true"
      gazebo_ignition="${gazebo_ignition}"
      xyz="-${wheel_offset_x} ${wheel_offset_y} ${wheel_offset_z}" />
    <xacro:rbkairos_mecanum_wheel
      prefix="${prefix}back_right_"
      parent="${prefix}base_link"
      reflect="true"
      hq="true"
      gazebo_ignition="${gazebo_ignition}"
      xyz="-${wheel_offset_x} -${wheel_offset_y} ${wheel_offset_z}" />

    <xacro:if value="$(arg gazebo_ignition)">
      <xacro:rbkairos_gz_ignition_control
        namespace="${namespace}"
        prefix="${prefix}" />
    </xacro:if>
  </xacro:macro>
```


Inner called macros:

- The [macro body file](urdf/bodies/rbkairos/rbkairos_body.urdf.xacro) includes the links and joints definition of the base_link, base_footprint, etc.

- The [wheels macro file](urdf/wheels/omni_wheel/omni_wheel.urdf.xacro) includes the links and joints of the mecanum wheel.
