using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Media;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using System.Timers;

using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

/// <summary>
/// How to CHANGE ONNX model for this sample application.
/// 1) Copy new ONNX model to "Assets" subfolder.
/// 2) Add model to "Project" under Assets folder by selecting "Add existing item"; navigate to new ONNX model and add.
///    Change properties "Build-Action" to "Content"  and  "Copy to Output Directory" to "Copy if Newer"
/// 3) Update the inialization of the variable "_ourOnnxFileName" to the name of the new model.
/// 4) In the constructor for OnnxModelOutput update the number of expected output labels.
/// </summary>

namespace SampleOnnxEvaluationApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CustomVision.ObjectDetection _model;
        private string _ourOnnxFileName = "model.onnx";
        public ViewModel viewModel;


        MediaCapture mediaCapture;
        DisplayRequest displayRequest;

        private bool ready = true;
        private Timer timer;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new ViewModel();
            DataContext = viewModel;
            displayRequest = new DisplayRequest();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(ready)
            {
                ready = false;
                EvaluateWebcam();
            }
        }

        private async void EvaluateWebcam()
        {
            VideoFrame frame = await mediaCapture.GetPreviewFrameAsync();
            var ret = await EvaluateVideoFrameAsync(frame);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => viewModel.Items = ret.Select(item => new ViewModelBox(item)).ToList());
            ready = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StartPreviewAsync();
            base.OnNavigatedTo(e);
        }

        // Doing this to test loading model from bytes which I have in Unity
        // It randomly freezes the application. Wish I understood streams.
        // I'll have to resort to the "write model file to disk then read model file from disk" idiocy.
        private async Task LoadModelFromBytes()
        {
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{_ourOnnxFileName}"));
            byte[] myBytes = await GetBytesAsync(modelFile);
            IRandomAccessStreamReference streamReference = await ConvertToRandomAccessStream(myBytes);

            _model = new CustomVision.ObjectDetection(new string[] { "AceOfHearts" });
            _model.Init(streamReference);

        }
        private static async Task<IRandomAccessStreamReference> ConvertToRandomAccessStream(byte[] arr)
        {
            MemoryStream memoryStream = new MemoryStream(arr);
            var randomAccessStream = new InMemoryRandomAccessStream();
            var outputStream = randomAccessStream.GetOutputStreamAt(0);
            await RandomAccessStream.CopyAndCloseAsync(memoryStream.AsInputStream(), outputStream);
            var result = RandomAccessStreamReference.CreateFromStream(randomAccessStream);
            return result;
        }

        private static async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes;

            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }

        private async Task LoadModelAsync()
        {
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{_ourOnnxFileName}"));
            _model = new CustomVision.ObjectDetection(new string[] { "AceOfHearts" });
            await _model.Init(modelFile);
        }
        private async Task<IList<CustomVision.PredictionModel>> EvaluateVideoFrameAsync(VideoFrame frame)
        {

            if (_model == null)
            {
                await LoadModelAsync();
                //await LoadModelFromBytes();
            }
            return await _model.PredictImageAsync(frame);
        }
        private async Task StartPreviewAsync()
        {

            mediaCapture = new MediaCapture();
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            
            IReadOnlyList<MediaCaptureVideoProfile> profiles = MediaCapture.FindAllVideoProfiles(devices[1].Id);
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
            settings.VideoDeviceId = devices[1].Id;
            await mediaCapture.InitializeAsync(settings);

            displayRequest.RequestActive();

            MyWebcam.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();


            timer = new Timer(1);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }

        private async Task CleanupCameraAsync()
        {
            if (mediaCapture != null)
            {
                await mediaCapture.StopPreviewAsync();
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MyWebcam.Source = null;
                    if (displayRequest != null)
                    {
                        displayRequest.RequestRelease();
                    }

                    mediaCapture.Dispose();
                    mediaCapture = null;
                });
            }

        }
    }
}