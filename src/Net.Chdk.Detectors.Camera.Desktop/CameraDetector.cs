using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using Net.Chdk.Model.Camera;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Directory = MetadataExtractor.Directory;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class CameraDetector
    {
        private static readonly string[] Patterns = new[] { "IMG_????.JPG", "_MG_????.JPG", "MVI_????.THM" };

        public CameraInfo GetCamera(string driveLetter)
        {
            return GetCameraFromMetadata(driveLetter)
                ?? GetCameraFromFileSystem(driveLetter);
        }

        public CameraInfo GetCamera(Stream stream)
        {
            var metadata = ImageMetadataReader.ReadMetadata(stream);
            if (metadata.Count == 0)
                return null;

            return new CameraInfo
            {
                Version = GetVersion(),
                Base = GetBase(metadata),
                Canon = GetCanon(metadata),
            };
        }

        private CameraInfo GetCameraFromMetadata(string driveLetter)
        {
            var metadataPath = Path.Combine(driveLetter, "METADATA");
            var cameraPath = Path.Combine(metadataPath, "CAMERA.JSN");
            if (!File.Exists(cameraPath))
                return null;

            using (var stream = File.OpenRead(cameraPath))
            {
                return JsonObject.Deserialize<CameraInfo>(stream);
            }
        }

        private string GetVersion()
        {
            return "1.0";
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

        private CameraInfo GetCameraFromFileSystem(string driveLetter)
        {
            var path = Path.Combine(driveLetter, "DCIM");
            if (!System.IO.Directory.Exists(path))
                return null;
            return System.IO.Directory.EnumerateDirectories(path)
                .Select(GetCameraFromDirectory)
                .FirstOrDefault(c => c != null);
        }

        private CameraInfo GetCameraFromDirectory(string dir)
        {
            return Patterns
                .Select(p => GetCameraFromDirectory(dir, p))
                .FirstOrDefault(c => c != null);
        }

        private CameraInfo GetCameraFromDirectory(string dir, string pattern)
        {
            return System.IO.Directory.EnumerateFiles(dir, pattern)
                .Select(GetCameraFromFile)
                .FirstOrDefault(c => c != null);
        }

        private CameraInfo GetCameraFromFile(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                return GetCamera(stream);
            }
        }
    }
}
