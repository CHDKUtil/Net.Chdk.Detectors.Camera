using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;
using Net.Chdk.Validators;
using System;

namespace Net.Chdk.Detectors.Camera
{
    [Obsolete]
    sealed class MetadataCameraDetector : MetadataDetector<MetadataCameraDetector, CameraInfo>, IInnerCameraDetector
    {
        public MetadataCameraDetector(IValidator<CameraInfo> validator, ILoggerFactory loggerFactory)
            : base(validator, loggerFactory)
        {
        }

        public CameraInfo GetCamera(CardInfo cardInfo, IProgress<double> progress)
        {
            Logger.LogTrace("Detecting camera from {0} metadata", cardInfo.DriveLetter);

            return GetValue(cardInfo, progress);
        }

        protected override string FileName => Files.Metadata.Camera;
    }
}
