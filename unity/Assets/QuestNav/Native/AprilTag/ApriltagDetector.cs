using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestNav.Native.AprilTag
{
    public class ApriltagDetector : IDisposable
    {
        /// <summary>
        /// The handle/IntPtr of the detector
        /// </summary>
        private IntPtr detectorHandle = AprilTagNatives.apriltag_detector_create();

        /// <summary>
        /// A list of handles for all tag families added to the detector
        /// </summary>
        private readonly List<IntPtr> tagFamilies = new List<IntPtr>();

        /// <summary>
        /// Tracks if the native structure has been disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Disposes of tag detector
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                if (!detectorHandle.Equals(IntPtr.Zero))
                {
                    //TODO: IMPLEMENT FULL DECONSTRUCTION ON ALL OBJECTS
                    AprilTagNatives.apriltag_detector_destroy(detectorHandle);
                    detectorHandle = IntPtr.Zero;
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Adds a family to the detector
        /// </summary>
        /// <param name="family"></param>
        public void AddFamily(AprilTagFamily family)
        {
            ThrowIfDisposed();
            AprilTagNatives.apriltag_detector_add_family(detectorHandle, family.Handle);
            tagFamilies.Add(family.Handle);
        }

        /// <summary>
        /// Removes a specific family from the detector
        /// </summary>
        /// <param name="family"></param>
        public void RemoveFamily(AprilTagFamily family)
        {
            ThrowIfDisposed();
            AprilTagNatives.apriltag_detector_remove_family(detectorHandle, family.Handle);
            tagFamilies.Remove(family.Handle);
        }

        /// <summary>
        /// Removes all families from the detector
        /// </summary>
        public void RemoveAllFamilies()
        {
            ThrowIfDisposed();
            AprilTagNatives.apriltag_detector_clear_families(detectorHandle);
            tagFamilies.Clear();
        }

        /// <summary>
        /// Runs the detector on the input image
        /// </summary>
        public void Detect()
        {
            
        }

        /// <summary>
        /// Throws if the detector native object has already been disposed
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new InvalidOperationException("36h11 Family has already been disposed");
        }
    }
}
