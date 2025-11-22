using System;
using System.Reflection;
using UnityEngine;

namespace QuestNav.WebServer
{
    /// <summary>
    /// Provides VR pose reset functionality for the configuration system.
    /// Handles recentering VR tracking to origin (0,0,0) with identity rotation.
    /// Uses reflection to access PoseResetCommand directly, avoiding circular assembly dependencies.
    /// </summary>
    public class PoseResetProvider : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// Flag indicating pose reset was requested from background thread
        /// </summary>
        private bool poseResetRequested = false;

        /// <summary>
        /// Cached PoseResetCommand instance (accessed via reflection)
        /// </summary>
        private object poseResetCommand;

        /// <summary>
        /// Cached Execute method from PoseResetCommand
        /// </summary>
        private MethodInfo executeMethod;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initializes PoseResetCommand via reflection.
        /// Finds QuestNav instance and creates PoseResetCommand with required transforms.
        /// </summary>
        void Start()
        {
            // Find QuestNav instance via reflection
            var questNavType = Type.GetType("QuestNav.Core.QuestNav, QuestNav");
            if (questNavType == null)
            {
                Debug.LogError("[PoseResetProvider] QuestNav.Core.QuestNav type not found");
                return;
            }

            var questNav = FindFirstObjectByType(questNavType);
            if (questNav == null)
            {
                Debug.LogError("[PoseResetProvider] QuestNav instance not found in scene");
                return;
            }

            // Get VR camera transforms via reflection
            var vrCameraField = questNavType.GetField(
                "vrCamera",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            var vrCameraRootField = questNavType.GetField(
                "vrCameraRoot",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            var resetTransformField = questNavType.GetField(
                "resetTransform",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (vrCameraField == null || vrCameraRootField == null || resetTransformField == null)
            {
                Debug.LogError("[PoseResetProvider] Failed to find VR camera transform fields");
                return;
            }

            Transform vrCamera = vrCameraField.GetValue(questNav) as Transform;
            Transform vrCameraRoot = vrCameraRootField.GetValue(questNav) as Transform;
            Transform resetTransform = resetTransformField.GetValue(questNav) as Transform;

            if (vrCamera == null || vrCameraRoot == null || resetTransform == null)
            {
                Debug.LogError("[PoseResetProvider] VR camera transforms are null");
                return;
            }

            // Get PoseResetCommand type via reflection
            var poseResetCommandType = Type.GetType(
                "QuestNav.Commands.Commands.PoseResetCommand, QuestNav"
            );
            if (poseResetCommandType == null)
            {
                Debug.LogError("[PoseResetProvider] PoseResetCommand type not found");
                return;
            }

            // Create PoseResetCommand instance
            // Pass null for networkTableConnection since web-initiated resets don't use NetworkTables
            poseResetCommand = Activator.CreateInstance(
                poseResetCommandType,
                new object[] { null, vrCamera, vrCameraRoot, resetTransform }
            );

            // Cache Execute method
            executeMethod = poseResetCommandType.GetMethod("Execute");

            if (poseResetCommand != null && executeMethod != null)
            {
                Debug.Log("[PoseResetProvider] Initialized with PoseResetCommand via reflection");
            }
            else
            {
                Debug.LogError("[PoseResetProvider] Failed to initialize PoseResetCommand");
            }
        }

        /// <summary>
        /// Checks for pending pose reset requests on main thread.
        /// Executes reset when flag is set.
        /// </summary>
        void Update()
        {
            if (poseResetRequested)
            {
                poseResetRequested = false;
                ExecutePoseReset();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Requests pose reset. Can be called from any thread.
        /// Sets flag that will be checked on main thread in Update().
        /// </summary>
        public void RequestPoseReset()
        {
            poseResetRequested = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Executes pose reset to origin using PoseResetCommand via reflection.
        /// Creates a command protobuf to reset to (0,0,0) with identity rotation.
        /// </summary>
        private void ExecutePoseReset()
        {
            if (poseResetCommand == null || executeMethod == null)
            {
                Debug.LogError("[PoseResetProvider] PoseResetCommand not initialized");
                return;
            }

            Debug.Log("[PoseResetProvider] Executing pose reset to origin via PoseResetCommand");

            try
            {
                // Create command protobuf to reset to origin via reflection
                var commandType = Type.GetType(
                    "QuestNav.Protos.Generated.ProtobufQuestNavCommand, QuestNav"
                );
                var commandTypeEnum = Type.GetType(
                    "QuestNav.Protos.Generated.QuestNavCommandType, QuestNav"
                );
                var payloadType = Type.GetType(
                    "QuestNav.Protos.Generated.ProtobufPoseResetPayload, QuestNav"
                );
                var pose3dType = Type.GetType("QuestNav.Protos.Generated.ProtobufPose3d, QuestNav");
                var translation3dType = Type.GetType(
                    "QuestNav.Protos.Generated.ProtobufTranslation3d, QuestNav"
                );
                var rotation3dType = Type.GetType(
                    "QuestNav.Protos.Generated.ProtobufRotation3d, QuestNav"
                );
                var quaternionType = Type.GetType(
                    "QuestNav.Protos.Generated.ProtobufQuaternion, QuestNav"
                );

                if (
                    commandType == null
                    || commandTypeEnum == null
                    || payloadType == null
                    || pose3dType == null
                    || translation3dType == null
                    || rotation3dType == null
                    || quaternionType == null
                )
                {
                    Debug.LogError("[PoseResetProvider] Failed to find protobuf types");
                    return;
                }

                // Create command instance
                var command = Activator.CreateInstance(commandType);

                // Set CommandId
                commandType.GetProperty("CommandId")?.SetValue(command, Guid.NewGuid().ToString());

                // Set Type = PoseReset
                var poseResetValue = Enum.Parse(commandTypeEnum, "PoseReset");
                commandType.GetProperty("Type")?.SetValue(command, poseResetValue);

                // Create payload
                var payload = Activator.CreateInstance(payloadType);

                // Create pose (0,0,0 with identity rotation)
                var pose = Activator.CreateInstance(pose3dType);

                // Create translation (0,0,0)
                var translation = Activator.CreateInstance(translation3dType);
                translation3dType.GetProperty("X")?.SetValue(translation, 0.0);
                translation3dType.GetProperty("Y")?.SetValue(translation, 0.0);
                translation3dType.GetProperty("Z")?.SetValue(translation, 0.0);

                // Create rotation (identity quaternion: w=1, x=0, y=0, z=0)
                var rotation = Activator.CreateInstance(rotation3dType);
                var quaternion = Activator.CreateInstance(quaternionType);
                quaternionType.GetProperty("W")?.SetValue(quaternion, 1.0);
                quaternionType.GetProperty("X")?.SetValue(quaternion, 0.0);
                quaternionType.GetProperty("Y")?.SetValue(quaternion, 0.0);
                quaternionType.GetProperty("Z")?.SetValue(quaternion, 0.0);

                rotation3dType.GetProperty("Q")?.SetValue(rotation, quaternion);

                // Assemble pose
                pose3dType.GetProperty("Translation")?.SetValue(pose, translation);
                pose3dType.GetProperty("Rotation")?.SetValue(pose, rotation);

                // Set payload target pose
                payloadType.GetProperty("TargetPose")?.SetValue(payload, pose);

                // Set command payload
                commandType.GetProperty("PoseResetPayload")?.SetValue(command, payload);

                // Execute command via PoseResetCommand.Execute()
                executeMethod.Invoke(poseResetCommand, new object[] { command });

                Debug.Log("[PoseResetProvider] Pose reset completed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PoseResetProvider] Failed to execute pose reset: {ex.Message}");
                Debug.LogError($"[PoseResetProvider] Stack trace: {ex.StackTrace}");
            }
        }
        #endregion
    }
}
