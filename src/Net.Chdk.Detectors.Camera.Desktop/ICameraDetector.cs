using Net.Chdk.Model.Camera;

namespace Net.Chdk.Detectors.Camera
{
    public interface ICameraDetector
    {
        CameraInfo GetCamera(string driveLetter);
    }
}