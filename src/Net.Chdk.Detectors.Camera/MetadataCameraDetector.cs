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
            return GetValue(cardInfo);
        }

        protected override string FileName => "CAMERA.JSN";
    }
}
