using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

namespace ReadyPlayerMe.XR
{
    public class RpmHandDriver : XRHandSkeletonDriver
    {
        [SerializeField] private new Vector3 rootOffset = new(0, 0.5f, 0);
        [SerializeField] private Transform handBone;

        [Header("Head properties")] [SerializeField]
        private Transform trueHeadPosition;

        [SerializeField] private InputActionProperty headPosition;

        private readonly List<KeyValuePair<Transform, Pose>> updateablePoses = new();

        private void Awake()
        {
            ApplyRootPoseOffset(rootOffset);
        }

        private void Start()
        {
            handTrackingEvents.trackingLost.AddListener(() =>
            {
                AvatarComponentReferences.Instance.Vrik.GetIKSolver().OnPostUpdate -= UpdateBones;
            });
            handTrackingEvents.trackingAcquired.AddListener(() =>
            {
                AvatarComponentReferences.Instance.Vrik.GetIKSolver().OnPostUpdate += UpdateBones;
            });
        }

        private void UpdateBones()
        {
            foreach (var poses in updateablePoses)
            {
                poses.Key.SetLocalPose(poses.Value);
            }
        }

        [ContextMenu("Set Bone References")]
        public void SetBoneReferences()
        {
            if (handBone == null)
            {
                Debug.LogError("Hand bone is not assigned.");
                return;
            }

            var handMap = handBone.name.Contains("LeftHand")
                ? HandBoneMap.JOINT_ID_BONE_PATH_MAP_LEFT
                : HandBoneMap.JOINT_ID_BONE_PATH_MAP_RIGHT;
            m_JointTransformReferences = new List<JointToTransformReference>();
            var handJoint = new JointToTransformReference
            {
                xrHandJointID = XRHandJointID.BeginMarker,
                jointTransform = handBone
            };
            m_JointTransformReferences.Add(handJoint);
            foreach (var kvp in handMap)
            {
                var joint = handBone.Find(kvp.Value);
                if (joint == null)
                {
                    Debug.LogWarning($"Joint transform not found: {kvp.Value}");
                    continue;
                }

                var jointRef = new JointToTransformReference
                {
                    xrHandJointID = kvp.Key,
                    jointTransform = joint
                };

                m_JointTransformReferences.Add(jointRef);
            }
        }

        /// <summary>
        ///     Update the <see cref="rootTransform" />'s local position and rotation with the hand's root pose.
        /// </summary>
        /// <param name="rootPose">The root pose of the hand.</param>
        /// <remarks>
        ///     Override this method to change how to the root pose is applied to the skeleton.
        /// </remarks>
        protected override void OnRootPoseUpdated(Pose rootPose)
        {
            if (!m_HasRootTransform || !trueHeadPosition || headPosition.action == null)
            {
                return;
            }

            var headSetPosition = headPosition.action.ReadValue<Vector3>();
            var headOffset = headSetPosition - trueHeadPosition.localPosition;
            var newCalculatedPosition = rootPose.position - headOffset;

            if (hasRootOffset)
            {
                rootTransform.localPosition = newCalculatedPosition + rootOffset;
            }
            else
            {
                rootTransform.localPosition = newCalculatedPosition;
            }

            rootTransform.localRotation = rootPose.rotation;
        }

        protected override void ApplyUpdatedTransformPoses()
        {
            updateablePoses.Clear();

            for (var i = 0; i < m_JointTransforms.Length; i++)
            {
                if (!m_HasJointTransformMask[i])
                {
                    continue;
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (m_JointTransforms[i] == null)
                {
                    Debug.LogError(
                        "XR Hand Skeleton has detected that a joint transform has been destroyed after it was initialized." +
                        " After removing or modifying transform joint references at runtime it is required to call InitializeFromSerializedReferences to update the joint transform references.",
                        this);

                    continue;
                }
#endif
                var pose = m_JointLocalPoses[i];
                updateablePoses.Add(new KeyValuePair<Transform, Pose>(m_JointTransforms[i], pose));
                m_JointTransforms[i].SetLocalPose(m_JointLocalPoses[i]);
            }
        }
    }
}