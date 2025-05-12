using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class RearLaserSensor : MonoBehaviour
{

    private ROSConnection rosConnection;
    public string topicName = "/rear_laser";

    public GameObject laserLink;
    public bool debugVisualization = false;
    public int updateRate = 12;
    public int samples = 541;
    public float angleMin = -2.2465f;
    public float angleMax = 2.2465f;
    public float rangeMin = 0.1f;
    public float rangeMax = 30.0f;

    private float angleIncrement;
    private float scanTime;
    private Quaternion[] rayRotations;
    private float[] ranges;
    private Vector3 rayStartPosition;
    private Vector3 rayStartForward;

    private LineRenderer[] lineRenderers;

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.RegisterPublisher<LaserScanMsg>(topicName);

        angleIncrement = (angleMax - angleMin) / (samples - 1);
        scanTime = 1f / updateRate;

        rayRotations = new Quaternion[samples];
        ranges = new float[samples];
        lineRenderers = new LineRenderer[samples];

        for (int i = 0; i < samples; i++)
        {
            float angle = angleMin + i * angleIncrement;
            rayRotations[i] = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);

            GameObject lrObj = new GameObject("LaserRay_" + i);
            lrObj.transform.parent = this.transform;
            lineRenderers[i] = lrObj.AddComponent<LineRenderer>();
            lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            lineRenderers[i].widthMultiplier = 0.01f;
            lineRenderers[i].positionCount = 2;
            lineRenderers[i].startColor = Color.red;
            lineRenderers[i].endColor = Color.red;
        }

        InvokeRepeating(nameof(ScanAndPublish), 1f, scanTime);
    }

    void ScanAndPublish()
    {
        rayStartPosition = laserLink.transform.position;
        rayStartForward = laserLink.transform.forward;

        for (int i = 0; i < samples; i++)
        {
            Vector3 dir = rayRotations[i] * rayStartForward;
            bool hit = Physics.Raycast(rayStartPosition, dir, out RaycastHit hitInfo, rangeMax);

            float distance = hit && !hitInfo.collider.isTrigger && hitInfo.distance >= rangeMin
                ? hitInfo.distance
                : rangeMax;

            ranges[i] = distance;

            if (debugVisualization)
                Debug.DrawRay(rayStartPosition, dir * distance, Color.red, scanTime);

            lineRenderers[i].SetPosition(0, rayStartPosition);
            lineRenderers[i].SetPosition(1, rayStartPosition + dir * distance);
            lineRenderers[i].enabled = true;
        }

        PublishLaserScan();
    }

    void PublishLaserScan()
    {
        TimeMsg timeStamp = new TimeMsg
        {
            sec = (uint)Time.time,
            nanosec = (uint)((Time.time - (int)Time.time) * 1000000000)
        };

        HeaderMsg header = new HeaderMsg
        {
            stamp = timeStamp,
            frame_id = "robot_rear_base_laser_link"
        };

        LaserScanMsg scan = new LaserScanMsg
        {
            header = header,
            angle_min = angleMin,
            angle_max = angleMax,
            angle_increment = angleIncrement,
            time_increment = 0.0f,
            scan_time = scanTime,
            range_min = rangeMin,
            range_max = rangeMax,
            ranges = ranges,
            intensities = new float[ranges.Length] 
        };

        rosConnection.Publish(topicName, scan);
    }

    public float[] GetCurrentScanRanges()
    {
        return ranges;
    }
}


