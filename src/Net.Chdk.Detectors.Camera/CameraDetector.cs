using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;
using System.Collections.Generic;
using System.Linq;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class CameraDetector : ICameraDetector
    {
        private ILoggerFactory LoggerFactory { get; }
        private IEnumerable<ICameraDetector> CameraDetectors { get; }

        public CameraDetector(IFileCameraDetector fileCameraDetector, ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            CameraDetectors = new ICameraDetector[]
            {
                new MetadataCameraDetector(LoggerFactory),
                new FileSystemCameraDetector(fileCameraDetector)
            };
        }

        public CameraInfo GetCamera(CardInfo cardInfo)
        {
            return CameraDetectors
                .Select(d => d.GetCamera(cardInfo))
                .FirstOrDefault(c => c != null);
        }
    }
}
