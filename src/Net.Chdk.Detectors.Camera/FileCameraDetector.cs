using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using System;
using System.IO;
using System.Linq;

#if PCL
using MetadataCollection = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using MetadataCollection = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace Net.Chdk.Detectors.Camera
{
    public sealed class FileCameraDetector : IFileCameraDetector
    {
        private static Version Version => new Version("1.0");

        private ILogger Logger { get; }

        public FileCameraDetector(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<FileCameraDetector>();
        }

#if !PCL
        public CameraInfo GetCamera(string filePath)
        {
            Logger.LogInformation("Reading {0}", filePath);

            using (var stream = File.OpenRead(filePath))
            {
                return GetCamera(stream);
            }
        }
#endif

        public CameraInfo GetCamera(Stream stream)
        {
            try
            {
                var metadata = ImageMetadataReader.ReadMetadata(stream);
                if (metadata.Count == 0)
                    return null;

                return new CameraInfo
                {
                    Version = Version,
                    Base = GetBase(metadata),
                    Canon = GetCanon(metadata),
                };
            }
            catch (MetadataException ex)
            {
                Logger.LogError(0, ex, "Error reading metadata");
                throw;
            }
        }

        private BaseInfo GetBase(MetadataCollection metadata)
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

        private CanonInfo GetCanon(MetadataCollection metadata)
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
