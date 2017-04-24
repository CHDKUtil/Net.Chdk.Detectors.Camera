using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Camera;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace Net.Chdk.Detectors.Camera
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                WriteUsage();
                return;
            }

            var loggerFactory = new LoggerFactory();

            string inputPath = null;
            string outputPath = null;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-i":
                        settings.Formatting = Formatting.Indented;
                        break;
                    default:
                        if (inputPath == null)
                        {
                            inputPath = args[i];
                        }
                        else if (outputPath == null)
                        {
                            outputPath = args[i];
                        }
                        else
                        {
                            WriteUsage();
                            return;
                        }
                        break;
                }
            }
            try
            {
                var cameraInfo = GetCamera(inputPath, loggerFactory);
                cameraInfo.Version = new Version("1.0");
                Serialize(outputPath, cameraInfo, settings);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static CameraInfo GetCamera(string path, ILoggerFactory loggerFactory)
        {
            var detector = new FileCameraDetector(loggerFactory);
            return detector.GetCamera(path);
        }

        private static void Serialize(string path, CameraInfo cameraInfo, JsonSerializerSettings settings)
        {
            using (var stream = File.OpenWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                JsonSerializer.CreateDefault(settings).Serialize(writer, cameraInfo);
            }
        }

        private static void WriteUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.WriteLine("generate <input-path> <output-path> [-i]");
            Console.WriteLine();
            Console.WriteLine("\t<input-path>  Input JPEG file path");
            Console.WriteLine("\t<output-path> Output camera metadata file path");
            Console.WriteLine("\t-i            Indented output");
        }
    }
}