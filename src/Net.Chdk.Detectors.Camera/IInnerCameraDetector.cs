using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;
using System;

namespace Net.Chdk.Detectors.Camera
{
    public interface IInnerCameraDetector
    {
        CameraInfo GetCamera(CardInfo cardInfo, IProgress<double> progress);
    }
}
