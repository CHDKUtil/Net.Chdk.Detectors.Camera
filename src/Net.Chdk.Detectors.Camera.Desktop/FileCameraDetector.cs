using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using System.Collections.Generic;
using System.Linq;
using Directory = MetadataExtractor.Directory;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class FileCameraDetector
    {
        private static string Version => "1.0";

        private ILogger Logger { get; }

        public FileCameraDetector(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<FileCameraDetector>();
        }

        public CameraInfo GetCamera(string filePath)
        {
            Logger.LogInformation("Reading {0}", filePath);

            var metadata = ImageMetadataReader.ReadMetadata(filePath);
            if (metadata.Count == 0)
                return null;

            return new CameraInfo
            {
                Version = Version,
                Base = GetBase(metadata),
                Canon = GetCanon(metadata),
            };
        }

        private BaseInfo GetBase(IReadOnlyList<Directory> metadata)
        {
            var ifd0 = metadata.OfType<ExifIfd0Directory>().SingleOrDefault();
            if (ifd0 == null)
                return null;

            return new BaseInfo
            {
                Make = ifd0.GetString(ExifDirectoryBase.TagMake),
                Model = ifd0.GetString(ExifDirectoryBase.TagModel),
            };
        }

        private CanonInfo GetCanon(IReadOnlyList<Directory> metadata)
        {
            var canon = metadata.OfType<CanonMakernoteDirectory>().SingleOrDefault();
            if (canon == null)
                return null;

            return new CanonInfo
            {
                ModelId = canon.GetUInt32(CanonMakernoteDirectory.TagModelId),
                FirmwareRevision = canon.GetUInt32(CanonMakernoteDirectory.TagFirmwareRevision)
            };
        }
    }
}
