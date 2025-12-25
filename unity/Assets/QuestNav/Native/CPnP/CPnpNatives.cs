using System;
using System.Runtime.InteropServices;

namespace QuestNav.QuestNav.Native.CPnP
{
    public static class CPnpNatives
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct CPnPResult
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public double[] qvec;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] tvec;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public double[] qvec_GN;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] tvec_GN;
        }

        [DllImport("cpnp", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cpnp_solve(
            double[] points_2d,
            double[] points_3d,
            int num_points,
            double[] camera_params,
            ref CPnPResult result
        );

        [DllImport("cpnp", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cpnp_get_last_error();

        [DllImport("cpnp", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cpnp_cleanup();

        public static string GetLastError()
        {
            IntPtr ptr = cpnp_get_last_error();
            return Marshal.PtrToStringAnsi(ptr) ?? string.Empty;
        }
    }
}
