﻿using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using Net.Chdk.Model.Camera;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class CameraDetector
    {
        private CameraInfo GetCamera(Stream stream)
        {
            var metadata = ImageMetadataReader.ReadMetadata(stream);
            if (metadata.Count == 0)
                return null;

            return new CameraInfo
            {
                Base = GetBase(metadata),
                Canon = GetCanon(metadata),
            };
        }

        private BaseInfo GetBase(IList<Directory> metadata)
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

        private CanonInfo GetCanon(IList<Directory> metadata)
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
