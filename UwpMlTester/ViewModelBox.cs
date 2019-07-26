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
    public class ViewModelBox
    {
        const int CanvasWidth = 800;
        const int CanvasHeight = 600;

        private readonly CustomVision.PredictionModel model;

        public float Left
        {
            get
            {
                // Something's wrong with the positioning. No clue what.
                float pos = model.BoundingBox.Left - .5f;
                pos *= .75f;
                pos += .5f;
                return pos * CanvasWidth;
            }
        }
        public float Top { get { return model.BoundingBox.Top * CanvasHeight; } }
        public float Width { get { return model.BoundingBox.Width * CanvasWidth; } }
        public float Height { get { return model.BoundingBox.Height * CanvasHeight; } }

        public float Opacity { get { return model.Probability; } }

        public ViewModelBox(CustomVision.PredictionModel model)
        {
            this.model = model;
        }
    }
}