using System;
using System.Runtime.InteropServices;

namespace QuestNav.Native.AprilTag
{
    public abstract class AprilTagFamily
    {
        /// <summary>
        /// Gets the nice name of the family
        /// </summary>
        /// <returns>The nice name of the family</returns>
        /// <exception cref="InvalidOperationException">If the family has already been disposed</exception>
        public string Name()
        {
            ThrowIfDisposed();

            var native = Marshal.PtrToStructure<ApriltagFamilyNative>(Handle);
            return native.name;
        }

        /// <summary>
        /// The handle/IntPtr of the family
        /// </summary>
        public abstract IntPtr Handle { get; set;  }

        /// <summary>
        /// Tracks if the native structure has been disposed
        /// </summary>
        public bool Disposed { get; private protected set; }
        
        /// <summary>
        /// Throws if the family native object has already been disposed
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ThrowIfDisposed()
        {
            if (Disposed)
                throw new InvalidOperationException("Family has already been disposed");
        }
    }
    
    /// <summary>
    /// Creates a new tag36h11 family
    /// </summary>
    public class Tag36H11 : AprilTagFamily, IDisposable
    {
        /// <inheritdoc/>
        public override IntPtr Handle { get; set; } = AprilTagNatives.tag36h11_create();
        
        /// <summary>
        /// Disposes of tag family
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                if (!Handle.Equals(IntPtr.Zero))
                {
                    AprilTagNatives.tag36h11_destroy(Handle);
                    Handle = IntPtr.Zero;
                }
                Disposed = true;
            }
        }
    }
    
}
