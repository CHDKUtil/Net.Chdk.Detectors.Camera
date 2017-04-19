using Net.Chdk.Model.Camera;
using System.IO;
using System.Linq;

namespace Net.Chdk.Detectors.Camera
{
    sealed class FileSystemCameraDetector : ICameraDetector
    {
        private static readonly string[] Patterns = new[] { "IMG_????.JPG", "_MG_????.JPG", "MVI_????.THM" };

        private FileCameraDetector FileCameraDetector { get; }

        public FileSystemCameraDetector(FileCameraDetector fileCameraDetector)
        {
            FileCameraDetector = fileCameraDetector;
        }

        public CameraInfo GetCamera(string driveLetter)
        {
            var path = Path.Combine(driveLetter, "DCIM");
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
