using UnityEngine;

namespace QuestNav.Config
{
    public static class Tunables
    {
        [Config(
            DisplayName = "Tag Size (meters)",
            Description = "Physical size of AprilTag markers in meters",
            Category = "AprilTag Detection",
            Min = 0.01f,
            Max = 1.0f,
            Step = 0.01f,
            ControlType = "slider",
            Order = 1
        )]
        public static float tagSizeMeters = 0.08f;

        [Config(
            DisplayName = "Decimation",
            Description = "Downscale factor for detection (higher = faster, less accurate)",
            Category = "AprilTag Detection",
            Min = 1,
            Max = 8,
            Step = 1,
            ControlType = "slider",
            Order = 2
        )]
        public static int decimation = 2;

        [Config(
            DisplayName = "Max Detections Per Second",
            Description = "Maximum detection rate to maintain performance",
            Category = "AprilTag Detection",
            Min = 1f,
            Max = 60f,
            Step = 1f,
            ControlType = "slider",
            Order = 3
        )]
        public static float maxDetectionsPerSecond = 15f;

        [Config(
            DisplayName = "Enable Debug Logging",
            Description = "Enable verbose debug output for detection system",
            Category = "AprilTag Detection",
            ControlType = "checkbox",
            Order = 4
        )]
        public static bool enableDebugLogging = false;

        [Config(
            DisplayName = "Server Port",
            Description = "HTTP server port for configuration UI",
            Category = "Network",
            Min = 1024,
            Max = 65535,
            ControlType = "input",
            RequiresRestart = true,
            Order = 40
        )]
        public static int serverPort = 18080;

        [Config(
            DisplayName = "Enable CORS Dev Mode",
            Description = "Allow localhost connections for development",
            Category = "Network",
            ControlType = "checkbox",
            Order = 41
        )]
        public static bool enableCORSDevMode = false;
    }
}

