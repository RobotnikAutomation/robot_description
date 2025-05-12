using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class GpsSensor : MonoBehaviour
{
    public string topicName = "/gps";                  
    public float publishRateHz = 1.0f;                 
    
    public ArticulationBody frontLeftWheel;            
    public ArticulationBody frontRightWheel;           
    public ArticulationBody backLeftWheel;            
    public ArticulationBody backRightWheel;            
    
    public float wheelRadius = 0.127f;                 
    public float robotLength = 0.573f;                 

    private float frontLeftWheelRotation = 0f;         
    private float frontRightWheelRotation = 0f;        
    private float backLeftWheelRotation = 0f;          
    private float backRightWheelRotation = 0f;         

    private float totalX = 0f;                          
    private float totalZ = 0f;                          
    private float orientation = 0f;                     

    private ROSConnection rosConnection;                
    private float timeElapsed = 0f;                     


    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.RegisterPublisher<NavSatFixMsg>(topicName);  
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;


        if (timeElapsed >= 1f / publishRateHz)
        {
            Odometry();    
            Position();    
            timeElapsed = 0f;
        }
    }

    void Odometry()
    {
        
        float distanceFrontLeft = frontLeftWheel.jointPosition[0] - frontLeftWheelRotation;
        float distanceFrontRight = frontRightWheel.jointPosition[0] - frontRightWheelRotation;
        float distanceBackLeft = backLeftWheel.jointPosition[0] - backLeftWheelRotation;
        float distanceBackRight = backRightWheel.jointPosition[0] - backRightWheelRotation;

        frontLeftWheelRotation = frontLeftWheel.jointPosition[0];
        frontRightWheelRotation = frontRightWheel.jointPosition[0];
        backLeftWheelRotation = backLeftWheel.jointPosition[0];
        backRightWheelRotation = backRightWheel.jointPosition[0];

        float distanceFrontLeftWheel = distanceFrontLeft * wheelRadius;
        float distanceFrontRightWheel = distanceFrontRight * wheelRadius;
        float distanceBackLeftWheel = distanceBackLeft * wheelRadius;
        float distanceBackRightWheel = distanceBackRight * wheelRadius;

        float distanceCenter = (distanceFrontLeftWheel + distanceFrontRightWheel + distanceBackLeftWheel + distanceBackRightWheel) / 4f;

        float distance = (distanceFrontRightWheel - distanceFrontLeftWheel + distanceBackRightWheel - distanceBackLeftWheel) / (4 * robotLength);
        orientation += distance;

        totalX += distanceCenter * Mathf.Sin(orientation);
        totalZ += distanceCenter * Mathf.Cos(orientation);
    }

    void Position()
    {

        NavSatFixMsg gpsMsg = new NavSatFixMsg
        {
            header = new HeaderMsg
            {
                frame_id = "robot_gps_base_link",
                stamp = GetCurrentRosTime()
            },
            latitude = totalZ,  
            longitude = totalX, 
            altitude = 0.0,
            status = new NavSatStatusMsg
            {
                status = NavSatStatusMsg.STATUS_FIX,
                service = NavSatStatusMsg.SERVICE_GPS
            },
            position_covariance = new double[]
            {
                0.5, 0, 0,
                0, 0.5, 0,
                0, 0, 1
            },
            position_covariance_type = NavSatFixMsg.COVARIANCE_TYPE_DIAGONAL_KNOWN
        };

        rosConnection.Publish(topicName, gpsMsg);
    }

    TimeMsg GetCurrentRosTime()
    {
        double rosTime = Time.realtimeSinceStartupAsDouble;
        uint secs = (uint)rosTime;
        uint nsecs = (uint)((rosTime - secs) * 1e9);
        return new TimeMsg(secs, nsecs);
    }
}


