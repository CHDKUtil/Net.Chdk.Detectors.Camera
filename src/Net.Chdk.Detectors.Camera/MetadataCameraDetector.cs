using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;

namespace Net.Chdk.Detectors.Camera
{
    sealed class MetadataCameraDetector : MetadataDetector<MetadataCameraDetector, CameraInfo>, IInnerCameraDetector
    {
        public MetadataCameraDetector(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        public CameraInfo GetCamera(CardInfo cardInfo)
        {
            Logger.LogTrace("Detecting camera from {0} metadata", cardInfo.DriveLetter);

            return GetValue(cardInfo);
        }

        protected override string FileName => "CAMERA.JSN";
    }
}
