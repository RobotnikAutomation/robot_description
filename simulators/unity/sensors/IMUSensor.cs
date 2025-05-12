using UnityEngine;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using System;

public class ImuSensor : MonoBehaviour
{
    private ROSConnection rosConnection;
    public string topicName = "/imu/data";
    public string frameId = "rbrobout_imu_link";

    public Rigidbody robotRigidBody;

    private Vector3 lastVelocity;

    public float noiseFactor = 0.02f; 
    public float accelerationScale = 1.0f;

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        if (rosConnection == null)
        {
            Debug.LogError("No hay conexión");
        }

        if (robotRigidBody == null)
        {
            Debug.LogError("No se asignó el Rigidbody del robot principal.");
        }

        rosConnection.RegisterPublisher<ImuMsg>(topicName);
        lastVelocity = robotRigidBody.velocity;
    }

    void FixedUpdate()
    {
        if (rosConnection == null || robotRigidBody == null)
            return;

        ImuMsg imuMsg = new ImuMsg();
        HeaderMsg header = new HeaderMsg();
        header.frame_id = frameId;
        double now = Time.realtimeSinceStartupAsDouble;
        header.stamp.sec = (uint)Math.Floor(now);
        header.stamp.nanosec = (uint)((now - Math.Floor(now)) * 1e9);
        imuMsg.header = header;

        Quaternion quaternion = transform.rotation;
        imuMsg.orientation.x = quaternion.x;
        imuMsg.orientation.y = quaternion.y;
        imuMsg.orientation.z = quaternion.z;
        imuMsg.orientation.w = quaternion.w;
        imuMsg.orientation_covariance[0] = 0.01 * 0.01;
        imuMsg.orientation_covariance[4] = 0.01 * 0.01;
        imuMsg.orientation_covariance[8] = 0.01 * 0.01;

        Vector3 angularVelocity = robotRigidBody.angularVelocity;
        imuMsg.angular_velocity.x = angularVelocity.x;
        imuMsg.angular_velocity.y = angularVelocity.y;
        imuMsg.angular_velocity.z = angularVelocity.z;
        imuMsg.angular_velocity_covariance[0] = 0.02 * 0.02;
        imuMsg.angular_velocity_covariance[4] = 0.02 * 0.02;
        imuMsg.angular_velocity_covariance[8] = 0.02 * 0.02;

        Vector3 currentVelocity = robotRigidBody.velocity;
        Vector3 linearAcceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = currentVelocity;

        imuMsg.linear_acceleration.x = (linearAcceleration.x / accelerationScale);
        imuMsg.linear_acceleration.y = (linearAcceleration.y / accelerationScale);
        imuMsg.linear_acceleration.z = (linearAcceleration.z / accelerationScale);
        imuMsg.linear_acceleration_covariance[0] = 0.1 * 0.1;
        imuMsg.linear_acceleration_covariance[4] = 0.1 * 0.1;
        imuMsg.linear_acceleration_covariance[8] = 0.1 * 0.1;

        rosConnection.Publish(topicName, imuMsg);
    }
}

