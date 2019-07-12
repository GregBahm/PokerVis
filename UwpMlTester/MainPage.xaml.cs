using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.AI.MachineLearning;
using Windows.Storage;
using Windows.Media;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using System.Timers;

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

        private async Task LoadModelAsync()
        {
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{_ourOnnxFileName}"));
            _model = new CustomVision.ObjectDetection(new string[] { "AceOfHearts" });
            await _model.Init(modelFile);
        }

        private async void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            ButtonRun.IsEnabled = false;
            
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".bmp");
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");
            fileOpenPicker.FileTypeFilter.Add(".gif");
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            StorageFile selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();

            SoftwareBitmap softwareBitmap;
            using (IRandomAccessStream stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
            {

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                
                softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }


            SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
            await imageSource.SetBitmapAsync(softwareBitmap);
            viewModel.UIPreviewImage = imageSource;
            VideoFrame inputImage = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
            await EvaluateVideoFrameAsync(inputImage);

            ButtonRun.IsEnabled = true;
        }

        private async Task<IList<CustomVision.PredictionModel>> EvaluateVideoFrameAsync(VideoFrame frame)
        {

            if (_model == null)
            {
                await LoadModelAsync();
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