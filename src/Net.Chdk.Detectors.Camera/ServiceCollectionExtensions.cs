using Microsoft.Extensions.DependencyInjection;

namespace Net.Chdk.Detectors.Camera
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCameraDetector(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<ICameraDetector, CameraDetector>()
                .AddSingleton<IInnerCameraDetector, MetadataCameraDetector>()
                .AddSingleton<IInnerCameraDetector, FileSystemCameraDetector>();
        }

        public static IServiceCollection AddFileCameraDetector(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IFileCameraDetector, FileCameraDetector>();
        }
    }
}
