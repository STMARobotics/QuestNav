// PJpeg.cs
using System;
using QuestNav.Utils;

namespace QuestNav.Native.AprilTag
{
    /// <summary>
    /// Represents a JPEG frame that has been converted into a native frame
    /// </summary>
    public unsafe class PJpeg : IDisposable
    {
        /// <summary>
        /// The handle of the Pjpeg frame
        /// </summary>
        internal PjpegNative* Handle { get; private set; }
        
        /// <summary>
        /// Whether the Pjpeg frame has been disposed or not
        /// </summary>
        private bool disposed;
        
        /// <summary>
        /// Creates a new Pjpeg frame
        /// </summary>
        /// <param name="pj"></param>
        private  PJpeg(PjpegNative* pj)
        {
            Handle = pj;
        }

        /// <summary>
        /// Creates a Pjpeg frame from a raw JPEG
        /// </summary>
        /// <param name="rawJpeg">The byte[] containing the JPEG encoded data</param>
        /// <returns>A new Pjpeg object, or null if creation failed</returns>
        public unsafe static PJpeg FromRawJpeg(byte[] rawJpeg)
        {
            if (rawJpeg == null || rawJpeg.Length == 0)
            {
                QueuedLogger.LogError("Cannot create PJpeg from null or empty byte array");
                return null;
            }

            fixed (byte* ptr = rawJpeg)
            {
                var pjpeg = AprilTagNatives.pjpeg_create_from_buffer(
                    (IntPtr) ptr,
                    rawJpeg.Length,
                    0, // default flags
                    out var error
                );

                if (error != AprilTagNatives.PjpegError.PJPEG_OKAY)
                {
                    QueuedLogger.LogError($"Error in native JPEG conversion: {error}");
                    return null;
                }

                return new PJpeg(pjpeg);
            }
        }

        /// <summary>
        /// Converts this PJpeg to an ImageU8 greyscale frame
        /// </summary>
        /// <returns>A new ImageU8 object, or null if conversion failed</returns>
        public ImageU8 ToImageU8()
        {
            if (disposed || Handle == null)
            {
                QueuedLogger.LogError("Cannot convert disposed or invalid PJpeg to ImageU8");
                return null;
            }

            return ImageU8.FromPjpegHandle(Handle);
        }

        /// <summary>
        /// Frees the memory in native code taken by the Pjpeg frame
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;

            if (Handle != null)
            {
                AprilTagNatives.pjpeg_destroy(Handle);
                Handle = null;
            }
            
            disposed = true;
        }
    }
}