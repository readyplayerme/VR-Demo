using ReadyPlayerMe.Core;
using RootMotion.FinalIK;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public static class VRIKHelper
    {
        /// <summary>
        ///     Configures the VRIK component on the provided GameObject, setting up its animator, avatar data, and bone
        ///     references.
        ///     It also adds and configures a TwistRelaxer component if it does not exist already
        /// </summary>
        /// <param name="vrik">The VRIK component to be configured.</param>
        public static void Setup(VRIK vrik)
        {
            var gameObject = vrik.gameObject;
            var animator = vrik.GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
            }

            var avatarData = gameObject.GetComponent<AvatarData>();
            animator.avatar = AvatarAnimationHelper.GetAnimationAvatar(avatarData.AvatarMetadata.OutfitGender,
                avatarData.AvatarMetadata.BodyType);

            if (vrik.references.root == null)
            {
                vrik.references.root = vrik.transform;
            }

            SetVRIKReferences(vrik, animator);
            var twistRelaxer = vrik.gameObject.GetComponent<TwistRelaxer>();
            if (twistRelaxer == null)
            {
                twistRelaxer = vrik.gameObject.AddComponent<TwistRelaxer>();
            }

            twistRelaxer.ik = vrik;
            SetupTwistRelaxer(vrik.references, twistRelaxer);
        }

        /// <summary>
        ///     Sets up the bone references for the VRIK component based on the provided Animator's bone structure.
        ///     This method maps all necessary human body bones from the Animator to the VRIK references, facilitating IK
        ///     adjustments.
        /// </summary>
        /// <param name="vrik">The VRIK component whose references are to be set.</param>
        /// <param name="animator">The Animator component from which bone transforms are retrieved.</param>
        public static void SetVRIKReferences(VRIK vrik, Animator animator)
        {
            vrik.references.pelvis = animator.GetBoneTransform(HumanBodyBones.Hips);
            vrik.references.spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            vrik.references.chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            vrik.references.neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            vrik.references.head = animator.GetBoneTransform(HumanBodyBones.Head);
            vrik.references.leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            vrik.references.leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            vrik.references.leftForearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            vrik.references.leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            vrik.references.rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            vrik.references.rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            vrik.references.rightForearm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            vrik.references.rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            vrik.references.leftThigh = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            vrik.references.leftCalf = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            vrik.references.leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            vrik.references.leftToes = animator.GetBoneTransform(HumanBodyBones.LeftToes);
            vrik.references.rightThigh = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            vrik.references.rightCalf = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            vrik.references.rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            vrik.references.rightToes = animator.GetBoneTransform(HumanBodyBones.RightToes);
        }

        /// <summary>
        ///     Configures a TwistRelaxer component with twist solvers for both left and right hands to mitigate unwanted twisting
        ///     effects during animations.
        ///     This method initializes and configures solvers for each hand and their respective parent bones, ensuring smooth
        ///     transitions and rotations during the animation.
        /// </summary>
        /// <param name="references">The VRIK.References object containing transform references for the character's body parts.</param>
        /// <param name="twistRelaxer">The TwistRelaxer component to be configured with solvers for left and right hands.</param>
        public static void SetupTwistRelaxer(VRIK.References references, TwistRelaxer twistRelaxer)
        {
            twistRelaxer.twistSolvers = new TwistSolver[4];

            var leftForeArmTwist = references.leftHand.transform.parent;
            twistRelaxer.twistSolvers[0] =
                CreateDefaultSolver(leftForeArmTwist);
            twistRelaxer.twistSolvers[1] = CreateDefaultSolver(leftForeArmTwist.parent.parent); // Left arm twist

            var rightForeArmTwist = references.rightHand.transform.parent;
            twistRelaxer.twistSolvers[2] =
                CreateDefaultSolver(rightForeArmTwist);
            twistRelaxer.twistSolvers[3] = CreateDefaultSolver(rightForeArmTwist.parent.parent); // Right arm twist
        }

        /// <summary>
        ///     Creates a TwistSolver configured for a specified target transform. This solver is set with default properties to
        ///     effectively manage and reduce twisting artifacts in animations.
        /// </summary>
        /// <param name="target">
        ///     The transform of the bone that the TwistSolver will control, typically a joint such as a hand or
        ///     its parent.
        /// </param>
        /// <returns>A configured TwistSolver with specified properties set to handle twist corrections.</returns>
        private static TwistSolver CreateDefaultSolver(Transform target)
        {
            var twistSolver = new TwistSolver();
            twistSolver.transform = target;
            twistSolver.weight = 1;
            twistSolver.parentChildCrossfade = 0.5f;
            return twistSolver;
        }
    }
}