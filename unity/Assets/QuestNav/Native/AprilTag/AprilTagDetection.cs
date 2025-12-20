using System;
using System.Runtime.InteropServices;

namespace QuestNav.Native.AprilTag
{
    /// <summary>
    /// Represents an AprilTag detection returned by a detector
    /// </summary>
    public class AprilTagDetection
    {
        /// <summary>
        /// The ID of the detected tag
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// How many error bits were corrected
        /// </summary>
        public int Hamming { get; set; }
        /// <summary>
        /// The confidence of the detection (higher = better)
        /// </summary>
        public float DecisionMargin { get; set; }
        /// <summary>
        /// The center of the tag in image coordinates
        /// </summary>
        public (double X, double Y) Center { get; set; }
        /// <summary>
        /// The corners of the tag in image coordinates
        /// </summary>
        public (double X, double Y)[] Corners { get; set; }

        /// <summary>
        /// Converts a native AprilTagDetection into a managed one
        /// </summary>
        /// <param name="native">The native struct to use</param>
        /// <returns></returns>
        public unsafe static AprilTagDetection FromNative(AprilTagDetectionNative native)
        {
            return new AprilTagDetection
            {
                Center = (native.c[0], native.c[1]),
                Corners = new[]
                {
                    (native.p[0], native.p[1]),
                    (native.p[2], native.p[3]),
                    (native.p[4], native.p[5]),
                    (native.p[6], native.p[7]),
                },
            };
        }
    }
}
