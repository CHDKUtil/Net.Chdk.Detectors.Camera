using Net.Chdk.Model.Camera;
using System.Collections.Generic;
using System.Linq;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class CameraDetector : ICameraDetector
    {
        private IEnumerable<ICameraDetector> CameraDetectors { get; }

        public CameraDetector()
            : this(new FileCameraDetector())
        {
        }

        public CameraDetector(FileCameraDetector fileCameraDetector)
        {
            CameraDetectors = new ICameraDetector[]
            {
                new MetadataCameraDetector(),
                new FileSystemCameraDetector(fileCameraDetector)
            };
        }

        public CameraInfo GetCamera(string driveLetter)
        {
            return CameraDetectors
                .Select(d => d.GetCamera(driveLetter))
                .FirstOrDefault(c => c != null);
        }
    }
}
