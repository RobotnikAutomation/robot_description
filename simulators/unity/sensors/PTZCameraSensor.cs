using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using System;
using RosMessageTypes.BuiltinInterfaces;

public class PTZCameraSensor : MonoBehaviour
{

    public string ptzTopic = "/camera/ptz";
    public string imageTopic = "/thermal/image";

    public float panSpeed = 20f;
    public float tiltSpeed = 20f;
    public float zoomSpeed = 10f;

    public Transform panLink;
    public Transform tiltLink;
    public Camera thermalCamera;

    public RenderTexture renderTexture;

    private float targetPan = 0f;
    private float targetTilt = 0f;
    private float targetZoom = 60f;

    private ROSConnection rosConnection;
    private Texture2D tex2D;

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<TwistMsg>(ptzTopic, controlPTZ);
        rosConnection.RegisterPublisher<ImageMsg>(imageTopic);

        tex2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
    }

    void Update()
    {

        float currentPan = transformAngle(panLink.localEulerAngles.y);
        float currentTilt = transformAngle(tiltLink.localEulerAngles.x);
        float currentZoom = thermalCamera.fieldOfView;

        float newPan = Mathf.MoveTowards(currentPan, targetPan, panSpeed * Time.deltaTime);
        float newTilt = Mathf.MoveTowards(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
        float newZoom = Mathf.MoveTowards(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);

        panLink.localRotation = Quaternion.Euler(0f, newPan, 0f);
        tiltLink.localRotation = Quaternion.Euler(newTilt, 0f, 0f);
        thermalCamera.fieldOfView = newZoom;

        PublishThermalImage();
    }

    private void controlPTZ(TwistMsg msg)
    {
        targetPan += (float)msg.linear.x;
        targetTilt += (float)msg.angular.y;
        targetZoom -= (float)msg.linear.z;

        targetPan = Mathf.Clamp(targetPan, -170f, 170f);
        targetTilt = Mathf.Clamp(targetTilt, -45f, 80f);
        targetZoom = Mathf.Clamp(targetZoom, 30f, 90f);
    }

    private float transformAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private void PublishThermalImage()
    {
        RenderTexture.active = renderTexture;
        tex2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex2D.Apply();
        RenderTexture.active = null;

        byte[] imageData = tex2D.GetRawTextureData();

        ImageMsg imgMsg = new ImageMsg
        {
            header = new HeaderMsg
            {
                stamp = new TimeMsg
                {
                    sec = (uint)(Time.timeSinceLevelLoad),
                    nanosec = (uint)((Time.timeSinceLevelLoad % 1000) * 1000000)
                },
                frame_id = "robot_top_ptz_camera_base_link"
            },
            height = (uint)tex2D.height,
            width = (uint)tex2D.width,
            encoding = "rgb8",
            is_bigendian = 0,
            step = (uint)(tex2D.width * 3),
            data = imageData
        };

        rosConnection.Publish(imageTopic, imgMsg);
    }
}

