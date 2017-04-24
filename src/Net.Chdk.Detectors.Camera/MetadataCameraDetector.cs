using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;
using System;

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

            var camera = GetValue(cardInfo);
            if (camera == null)
                return null;

            if (!Validate(camera.Version))
                return null;

            if (!Validate(camera.Base))
                return null;

            if (!Validate(camera.Canon))
                return null;

            return camera;
        }

        protected override string FileName => "CAMERA.JSN";

        private static bool Validate(Version version)
        {
            if (version == null)
                return false;

            if (version.Major < 1 || version.Minor < 0)
                return false;

            return true;
        }

        private static bool Validate(BaseInfo @base)
        {
            if (@base == null)
                return false;

            if (string.IsNullOrEmpty(@base.Make))
                return false;

            if (string.IsNullOrEmpty(@base.Model))
                return false;

            return true;
        }

        private static bool Validate(CanonInfo canon)
        {
            if (canon == null)
                return false;

            if (canon.ModelId < 0x100000)
                return false;

            if (canon.FirmwareRevision < 0x1000000)
                return false;

            if ((canon.FirmwareRevision & 0xffff) < 0x100)
                return false;

            return true;
        }
    }
}
