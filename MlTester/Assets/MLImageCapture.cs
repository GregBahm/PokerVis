using SpectatorView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLImageCapture : MonoBehaviour
{
    private HoloLensCamera pvCamera;
    bool startCamera;

#if CAN_USE_UWP_TYPES
    public int FrameIndex;
    public bool GrabFrame;
    public VideoFrame VideoFrame;
#endif

    void Start()
    {
        pvCamera = new HoloLensCamera(CaptureMode.SingleLowLatency);
        pvCamera.OnCameraInitialized += OnCameraInitialized;
        pvCamera.OnFrameCaptured += OnFrameCaptured;
        pvCamera.KeepSoftwareBitmap = true;
        pvCamera.Initialize();
    }

    private void OnFrameCaptured(HoloLensCamera sender, CameraFrame frame)
    {
        Debug.Log($"Frame captured:\n  Resolution: {frame.Resolution.Width}x{frame.Resolution.Height}\n  PixelFormat: {frame.PixelFormat}\n  Exposure: {frame.Exposure}\n  Gain: {frame.Gain}\n  Frame Time: {frame.FrameTime}\n\n");

#if CAN_USE_UWP_TYPES
        // this will save the image to the CameraRoll folder, so it needs to have the appropriate priviledges.
        //string filename = Windows.Storage.KnownFolders.CameraRoll.Path + "\\image." + System.DateTime.Now.ToString("yy.MM.dd.HH.mm.ss.f") + ".jpg";
        //frame.Save(filename);


        if (GrabFrame)
        {
            GrabFrame = false;

            SoftwareBitmap softwareBitmap = frame.SoftwareBitmap;

            if (softwareBitmap != null)
            {
                VideoFrame = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
                ++frameIndex;
            }
        }

#endif
        frame.Release();
    }

    private void OnCameraInitialized(HoloLensCamera sender, bool initializeSuccessful)
    {
        // choose the first video with 720p resolution
        StreamDescription streamDesc = pvCamera.StreamSelector.Select(StreamCompare.EqualTo, 1280, 720).StreamDescriptions[0];

        Debug.Log($"Selecting {streamDesc.Resolution.Width}x{streamDesc.Resolution.Height}@{streamDesc.Resolution.Framerate}fps");

        // initialized, but not running
        pvCamera.Start(streamDesc);

        Debug.Log("Starting camera");
        startCamera = true;
    }

    void Update()
    {
        if (startCamera)
        {
            startCamera = false;
            pvCamera.StartContinuousCapture();
        }
    }
}
