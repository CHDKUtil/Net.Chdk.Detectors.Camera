using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;
using Net.Chdk.Model.Software;
using System.Collections.Generic;
using System.Linq;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class CameraDetector : ICameraDetector
    {
        private ILogger Logger { get; }
        private IEnumerable<IInnerCameraDetector> CameraDetectors { get; }

        public CameraDetector(IEnumerable<IInnerCameraDetector> cameraDetectors, IFileCameraDetector fileCameraDetector, ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<CameraDetector>();
            CameraDetectors = cameraDetectors;
        }

        public CameraInfo GetCamera(CardInfo cardInfo, SoftwareInfo softwareInfo)
        {
            Logger.LogTrace("Detecting camera from {0}", cardInfo.DriveLetter);

            return CameraDetectors
                .Select(d => d.GetCamera(cardInfo, softwareInfo))
                .FirstOrDefault(c => c != null);
        }
    }
}
