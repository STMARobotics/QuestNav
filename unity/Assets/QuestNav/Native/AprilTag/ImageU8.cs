using System;
using QuestNav.Utils;

namespace QuestNav.Native.AprilTag
{
    /// <summary>
    /// Represents an image frame that has been converted into a greyscale integer frame
    /// </summary>
    public unsafe class ImageU8 : IDisposable
    {
        /// <summary>
        /// The handle of the ImageU8 frame
        /// </summary>
        internal ImageU8Native* Handle { get; private set; }
        
        /// <summary>
        /// Whether the ImageU8 frame has been disposed or not
        /// </summary>
        private bool disposed;
        
        /// <summary>
        /// Creates a new ImageU8 frame
        /// </summary>
        /// <param name="img">The image_u8 handle</param>
        private ImageU8(ImageU8Native* img)
        {
            Handle = img;
        }

        /// <summary>
        /// Internal method to create ImageU8 from a native handle
        /// </summary>
        /// <param name="pjpegHandle">The native pjpeg handle</param>
        /// <returns>A new ImageU8 object, or null if conversion failed</returns>
        internal static ImageU8 FromPjpegHandle(PjpegNative* pjpegHandle)
        {
            var img = AprilTagNatives.pjpeg_to_u8_baseline(pjpegHandle);
            
            if (img == null)
            {
                QueuedLogger.LogError("Failed to convert PJpeg to ImageU8");
                return null;
            }

            return new ImageU8(img);
        }

        /// <summary>
        /// Frees the memory in native code taken by the ImageU8 frame
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;

            if (Handle != null)
            {
                AprilTagNatives.image_u8_destroy(Handle);
                Handle = null;
            }
            
            disposed = true;
        }
    }
}