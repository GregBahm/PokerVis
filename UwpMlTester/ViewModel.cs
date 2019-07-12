using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Media;

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
    public class ViewModel : INotifyPropertyChanged
    {
        private ImageSource _uIPreviewImage;

        public ImageSource UIPreviewImage
        {
            get { return _uIPreviewImage; }
            set
            {
                _uIPreviewImage = value;
                OnPropertyChanged(nameof(UIPreviewImage));
            }
        }

        private IEnumerable<ViewModelBox> _items;
        public IEnumerable<ViewModelBox> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}