using UnityEngine;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class Laser3DSensor : MonoBehaviour
{
    public string topicName = "/laser3D/points";
    public GameObject laserLink;
    public bool debugVisualization = true;

    public int horizontalSamples = 180;
    public float angleMin = -Mathf.PI;
    public float angleMax = Mathf.PI;

    public int verticalSamples = 15;
    public float verticalAngleMin = -0.2618f;
    public float verticalAngleMax = 0.2618f;

    public float rangeMin = 0.1f;
    public float rangeMax = 10f;

    public int updateRate = 10;
    private float scanTime;

    private ROSConnection rosConnection;
    private float timeSinceLastScan = 0f;

    private LineRenderer[,] lineRenderers;
    public int visualizationStep = 5;

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.RegisterPublisher<PointCloud2Msg>(topicName);

        scanTime = 1f / updateRate;

        lineRenderers = new LineRenderer[verticalSamples, horizontalSamples];
        for (int v = 0; v < verticalSamples; v += visualizationStep)
        {
            for (int h = 0; h < horizontalSamples; h += visualizationStep)
            {
                GameObject lrObj = new GameObject($"LaserRay_{v}_{h}");
                lrObj.transform.parent = this.transform;
                LineRenderer lr = lrObj.AddComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.widthMultiplier = 0.01f;
                lr.positionCount = 2;
                lr.startColor = Color.green;
                lr.endColor = Color.green;
                lineRenderers[v, h] = lr;
            }
        }
    }

    void Update()
    {
        timeSinceLastScan += Time.deltaTime;
        if (timeSinceLastScan >= scanTime)
        {
            timeSinceLastScan = 0f;
            PublishLaser3D();
        }
    }

    void PublishLaser3D()
    {
        List<byte> data = new List<byte>();
        Vector3 origin = laserLink.transform.position;
        Vector3 forward = laserLink.transform.forward;

        float hStep = (angleMax - angleMin) / (horizontalSamples - 1);
        float vStep = (verticalAngleMax - verticalAngleMin) / (verticalSamples - 1);

        for (int v = 0; v < verticalSamples; v++)
        {
            float vAngle = verticalAngleMin + v * vStep;
            for (int h = 0; h < horizontalSamples; h++)
            {
                float hAngle = angleMin + h * hStep;
                Quaternion rot = Quaternion.Euler(vAngle * Mathf.Rad2Deg, hAngle * Mathf.Rad2Deg, 0f);
                Vector3 vector = rot * forward;

                Vector3 endPoint;
                if (Physics.Raycast(origin, vector, out RaycastHit hit, rangeMax) && hit.distance >= rangeMin && !hit.collider.isTrigger)
                {
                    endPoint = hit.point;
                }
                else
                {
                    endPoint = origin + vector * rangeMax;
                }

                data.AddRange(System.BitConverter.GetBytes(endPoint.x));
                data.AddRange(System.BitConverter.GetBytes(endPoint.y));
                data.AddRange(System.BitConverter.GetBytes(endPoint.z));

                if (debugVisualization && v % visualizationStep == 0 && h % visualizationStep == 0)
                {
                    lineRenderers[v, h].SetPosition(0, origin);
                    lineRenderers[v, h].SetPosition(1, endPoint);
                }
            }
        }

        uint pointCount = (uint)(data.Count / 12);

        List<PointFieldMsg> fields = new List<PointFieldMsg>
        {
            new PointFieldMsg("x", 0, PointFieldMsg.FLOAT32, 1),
            new PointFieldMsg("y", 4, PointFieldMsg.FLOAT32, 1),
            new PointFieldMsg("z", 8, PointFieldMsg.FLOAT32, 1)
        };

        HeaderMsg header = new HeaderMsg
        {
            frame_id = "robot_top_3d_laserbase_link",
            stamp = new TimeMsg
            {
                sec = (uint)System.DateTimeOffset.Now.ToUnixTimeSeconds(),
                nanosec = (uint)((System.DateTimeOffset.Now.ToUnixTimeMilliseconds() % 1000) * 1000000)
            }
        };

        PointCloud2Msg cloudMsg = new PointCloud2Msg
        {
            header = header,
            height = 1,
            width = pointCount,
            is_bigendian = false,
            point_step = 12,
            row_step = (uint)(pointCount * 12),
            is_dense = true,
            fields = fields.ToArray(),
            data = data.ToArray()
        };

        rosConnection.Publish(topicName, cloudMsg);
    }
}

