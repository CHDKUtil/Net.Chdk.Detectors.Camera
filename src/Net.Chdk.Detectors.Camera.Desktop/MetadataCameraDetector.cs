using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;

namespace Net.Chdk.Detectors.Camera
{
    sealed class MetadataCameraDetector : MetadataDetector<MetadataCameraDetector, CameraInfo>, ICameraDetector
    {
        public MetadataCameraDetector(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        public CameraInfo GetCamera(string driveLetter)
        {
            return GetValue(driveLetter);
        }

        protected override string FileName => "CAMERA.JSN";
    }
}
