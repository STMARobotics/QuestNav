using UnityEngine;

namespace QuestNav.QuestNav.Native.CPnP
{
    public static class TestPNP
    {
        public static void TestSimpleCube()
        {
            Debug.Log("Test 1: Simple Cube Corners");
            // Camera intrinsics: fx, fy, cx, cy
            // Simulating a 640x480 camera with focal length ~500
            double[] cameraParams = { 500.0, 500.0, 320.0, 240.0 };

            // 3D points: 8 corners of a cube centered at (0, 0, 5)
            // The cube has side length 2, so corners range from -1 to 1 in x,y
            // and from 4 to 6 in z (in front of camera)
            double[] points3D = {
                -1.0, -1.0, 4.0,  // Point 0
                1.0, -1.0, 4.0,  // Point 1
                1.0,  1.0, 4.0,  // Point 2
                -1.0,  1.0, 4.0,  // Point 3
                -1.0, -1.0, 6.0,  // Point 4
                1.0, -1.0, 6.0,  // Point 5
                1.0,  1.0, 6.0,  // Point 6
                -1.0,  1.0, 6.0   // Point 7
            };

            // 2D points: projections of the 3D points
            // Calculated as: u = fx * X/Z + cx, v = fy * Y/Z + cy
            double[] points2D = {
                195.0, 115.0,   // Point 0: 500*(-1)/4 + 320, 500*(-1)/4 + 240
                445.0, 115.0,   // Point 1: 500*(1)/4 + 320, 500*(-1)/4 + 240
                445.0, 365.0,   // Point 2: 500*(1)/4 + 320, 500*(1)/4 + 240
                195.0, 365.0,   // Point 3: 500*(-1)/4 + 320, 500*(1)/4 + 240
                236.67, 156.67, // Point 4: 500*(-1)/6 + 320, 500*(-1)/6 + 240
                403.33, 156.67, // Point 5: 500*(1)/6 + 320, 500*(-1)/6 + 240
                403.33, 323.33, // Point 6: 500*(1)/6 + 320, 500*(1)/6 + 240
                236.67, 323.33  // Point 7: 500*(-1)/6 + 320, 500*(1)/6 + 240
            };

            int numPoints = 8;

            RunTest(points2D, points3D, numPoints, cameraParams);
        }
        
        static void RunTest(double[] points2D, double[] points3D, int numPoints, double[] cameraParams)
        {
            Debug.Log($"Number of points: {numPoints}");
            Debug.Log($"Camera params: fx={cameraParams[0]}, fy={cameraParams[1]}, cx={cameraParams[2]}, cy={cameraParams[3]}");

            // Initialize result structure
            CPnpNatives.CPnPResult result = new CPnpNatives.CPnPResult
            {
                qvec = new double[4],
                tvec = new double[3],
                qvec_GN = new double[4],
                tvec_GN = new double[3],
            };

            // Call the solver
            int success = CPnpNatives.cpnp_solve(
                points2D,
                points3D,
                numPoints,
                cameraParams,
                ref result
            );

            if (success == 1)
            {
                Debug.Log("\nResult (Initial Estimate):");
                Debug.Log($"  Quaternion (w,x,y,z): [{result.qvec[0]:F6}, {result.qvec[1]:F6}, {result.qvec[2]:F6}, {result.qvec[3]:F6}]");
                Debug.Log($"  Translation (x,y,z): [{result.tvec[0]:F6}, {result.tvec[1]:F6}, {result.tvec[2]:F6}]");

                Debug.Log("\nResult (Gauss-Newton Refined):");
                Debug.Log($"  Quaternion (w,x,y,z): [{result.qvec_GN[0]:F6}, {result.qvec_GN[1]:F6}, {result.qvec_GN[2]:F6}, {result.qvec_GN[3]:F6}]");
                Debug.Log($"  Translation (x,y,z): [{result.tvec_GN[0]:F6}, {result.tvec_GN[1]:F6}, {result.tvec_GN[2]:F6}]");
                
            }
            else
            {
                Debug.Log($"FAILED! Error: {CPnpNatives.GetLastError()}");
            }
        }
    }
}
