using Net.Chdk.Model.Camera;
using Net.Chdk.Model.Card;
using System.IO;
using System.Linq;

namespace Net.Chdk.Detectors.Camera
{
    public sealed class FileSystemCameraDetector : ICameraDetector
    {
        public static string[] Patterns => new[] { "IMG_????.JPG", "_MG_????.JPG", "MVI_????.THM" };

        private FileCameraDetector FileCameraDetector { get; }

        public FileSystemCameraDetector(FileCameraDetector fileCameraDetector)
        {
            FileCameraDetector = fileCameraDetector;
        }

        public CameraInfo GetCamera(CardInfo cardInfo)
        {
            var rootPath = cardInfo.GetRootPath();
            var path = Path.Combine(rootPath, "DCIM");
            if (!Directory.Exists(path))
                return null;

            return Directory.EnumerateDirectories(path)
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
            return Directory.EnumerateFiles(dir, pattern)
                .Select(GetCameraFromFile)
                .FirstOrDefault(c => c != null);
        }

        private CameraInfo GetCameraFromFile(string file)
        {
            return FileCameraDetector.GetCamera(file);
        }
    }
}
