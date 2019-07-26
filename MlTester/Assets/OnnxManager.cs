#if ENABLE_WINMD_SUPPORT
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnnxManager : MonoBehaviour
{
    private string onnxFilePath; 

#if CAN_USE_UWP_TYPES
    private ObjectDetection _model;
#endif

    private void Start()
    {
        onnxFilePath = Application.streamingAssetsPath + "model.onnx";

#if CAN_USE_UWP_TYPES
        await LoadModelAsync();
#endif
    }

#if CAN_USE_UWP_TYPES
    private async Task LoadModelAsync()
    {
        StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileName));
        _model = new ObjectDetection(new string[] { "AceOfHearts" });
        await _model.Init(modelFile);
    }
#endif
}
