using Net.Chdk.Model.Camera;
using System.IO;

namespace Net.Chdk.Detectors.Camera
{
    sealed class MetadataCameraDetector : ICameraDetector
    {
        public CameraInfo GetCamera(string driveLetter)
        {
            var metadataPath = Path.Combine(driveLetter, "METADATA");
            var cameraPath = Path.Combine(metadataPath, "CAMERA.JSN");
            if (!File.Exists(cameraPath))
                return null;

            using (var stream = File.OpenRead(cameraPath))
            {
                return JsonObject.Deserialize<CameraInfo>(stream);
            }
        }
    }
}
