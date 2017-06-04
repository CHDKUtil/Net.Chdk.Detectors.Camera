using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class FileSystemCameraDetector : IInnerCameraDetector
    {
        public static string[] Patterns => new[]
        {
            "IMG_????.JPG",
            "_MG_????.JPG",
            "MVI_????.THM",
            "IMG_????.CRW",
            "_MG_????.CRW",
            "IMG_????.CR2",
            "_MG_????.CR2",
        };

        private ILogger Logger { get; }
        private IFileCameraDetector FileCameraDetector { get; }

        public FileSystemCameraDetector(IFileCameraDetector fileCameraDetector, ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<FileSystemCameraDetector>();
            FileCameraDetector = fileCameraDetector;
        }

        public CameraInfo GetCamera(CardInfo cardInfo, IProgress<double> progress, CancellationToken token)
        {
            Logger.LogTrace("Detecting camera from {0} file system", cardInfo.DriveLetter);

            var rootPath = cardInfo.GetRootPath();
            var path = Path.Combine(rootPath, Directories.Images);
            if (!Directory.Exists(path))
                return null;

            token.ThrowIfCancellationRequested();

            var dirs = Directory.EnumerateDirectories(path)
                .Reverse();
            var count = progress != null
                ? dirs.Count()
                : 0;
            var index = 0;

            foreach (var dir in dirs)
            {
                token.ThrowIfCancellationRequested();

                var camera = GetCameraFromDirectory(dir);
                if (camera != null)
                    return camera;

                if (progress != null)
                    progress.Report((double)(++index) / count);
            }

            return null;
        }

        private CameraInfo GetCameraFromDirectory(string dir)
        {
            return Patterns
                .Select(p => GetCameraFromDirectory(dir, p))
                .FirstOrDefault(c => c != null);
        }

        private CameraInfo GetCameraFromDirectory(string dir, string pattern)
        {
            return Directory.EnumerateFiles(dir, pattern)
                .Reverse()
                .Select(GetCameraFromFile)
                .FirstOrDefault(c => c != null);
        }

        private CameraInfo GetCameraFromFile(string file)
        {
            try
            {
                return FileCameraDetector.GetCamera(file);
            }
            catch (CameraDetectionException)
            {
                return null;
            }
        }
    }
}
