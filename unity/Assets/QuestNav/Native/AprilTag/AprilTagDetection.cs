using System;
using System.Runtime.InteropServices;

namespace QuestNav.Native.AprilTag
{
    /// <summary>
    /// Represents an AprilTag detection returned by a detector
    /// </summary>
    public unsafe class AprilTagDetection : IDisposable
    {
        /// <summary>
        /// The native detector pointer
        /// </summary>
        internal AprilTagDetectionNative* Handle { get; private set; }

        /// <summary>
        /// Tracks if the native structure has been disposed
        /// </summary>
        private bool disposed;

        public AprilTagDetection(AprilTagDetectionNative* handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            Handle = handle;
        }

        /// <summary>
        /// Gets the decoded ID of the detected tag
        /// </summary>
        public int Id
        {
            get
            {
                ThrowIfDisposed();
                return Handle->id;
            }
        }

        /// <summary>
        /// Gets the number of error bits that were corrected during decoding.
        /// Higher values indicate lower confidence. Values > 2 greatly increase false positive rates.
        /// </summary>
        public int Hamming
        {
            get
            {
                ThrowIfDisposed();
                return Handle->hamming;
            }
        }

        /// <summary>
        /// Gets the decision margin - a measure of decoding quality.
        /// Higher numbers indicate better decodes. Most reliable for small tags.
        /// </summary>
        public float DecisionMargin
        {
            get
            {
                ThrowIfDisposed();
                return Handle->decision_margin;
            }
        }

        /// <summary>
        /// Gets the center point of the detection in image pixel coordinates
        /// </summary>
        public Point2D Center
        {
            get
            {
                ThrowIfDisposed();
                return Handle->center;
            }
        }

        /// <summary>
        /// Gets the bottom left corner of the tag in image pixel coordinates.
        /// Corners wrap counter-clockwise around the tag.
        /// </summary>
        public Point2D CornerBottomLeft0
        {
            get
            {
                ThrowIfDisposed();
                return Handle->corner0;
            }
        }

        /// <summary>
        /// Gets the bottom right corner of the tag in image pixel coordinates.
        /// Corners wrap counter-clockwise around the tag.
        /// </summary>
        public Point2D CornerBottomRight1
        {
            get
            {
                ThrowIfDisposed();
                return Handle->corner1;
            }
        }

        /// <summary>
        /// Gets the upper corner right of the tag in image pixel coordinates.
        /// Corners wrap counter-clockwise around the tag.
        /// </summary>
        public Point2D CornerUpperRight2
        {
            get
            {
                ThrowIfDisposed();
                return Handle->corner2;
            }
        }

        /// <summary>
        /// Gets the upper left corner of the tag in image pixel coordinates.
        /// Corners wrap counter-clockwise around the tag.
        /// </summary>
        public Point2D CornerUpperLeft3
        {
            get
            {
                ThrowIfDisposed();
                return Handle->corner3;
            }
        }

        /// <summary>
        /// Gets all four corners as an array.
        /// Corners wrap counter-clockwise around the tag.
        /// </summary>
        public Point2D[] GetCorners()
        {
            ThrowIfDisposed();
            return new Point2D[]
            {
                CornerBottomLeft0,
                CornerBottomRight1,
                CornerUpperRight2,
                CornerUpperLeft3,
            };
        }

        /// <summary>
        /// Gets the name of the tag family this detection belongs to
        /// </summary>
        public string FamilyName
        {
            get
            {
                ThrowIfDisposed();
                if (Handle->family == IntPtr.Zero)
                    return null;

                var family = (AprilTagFamilyNative*)Handle->family;
                return Marshal.PtrToStringAnsi(family->name);
            }
        }

        /// <summary>
        /// Throws ObjectDisposedException if the detection has been disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(AprilTagDetection));
        }

        /// <summary>
        /// Formats data as string
        /// </summary>
        /// <returns>A friendly string with data formatted</returns>
        public override string ToString()
        {
            return $"Id: {Id}\n"
                + $"Family: {FamilyName}\n"
                + $"Hamming: {Hamming}\n"
                + $"DecisionMargin: {DecisionMargin}\n"
                + $"Center: X: {Center.x}, Y: {Center.y}\n"
                + $"Corners: BL: {CornerBottomLeft0}, BR: {CornerBottomRight1}, UR: {CornerUpperRight2}, UL: {CornerUpperLeft3}";
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (Handle != null)
                {
                    AprilTagNatives.apriltag_detection_destroy(Handle);
                    Handle = null;
                }
                disposed = true;
            }
        }
    }
}
