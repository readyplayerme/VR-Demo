using System;
using RootMotion.FinalIK;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public class ArmStretcher : MonoBehaviour
    {
        [SerializeField] private float differenceMultiplier = 1.8f;

        [SerializeField] private HandTransformInputAction leftHand;
        [SerializeField] private HandTransformInputAction rightHand;

        private IKSolverVR solver;

        private void Start()
        {
            solver = AvatarComponentReferences.Instance.Vrik.solver;
            solver.OnPostUpdate += OnPostUpdate;
        }

        private void OnDestroy()
        {
            solver.OnPostUpdate -= OnPostUpdate;
        }

        private void OnPostUpdate()
        {
            StretchArm(leftHand, solver.leftArm);
            StretchArm(rightHand, solver.rightArm);
        }

        private void StretchArm(HandTransformInputAction hand, IKSolverVR.Arm arm)
        {
            var difference = Vector3.Distance(hand.handTransform.position, hand.trueHandPosition.position);
            arm.armLengthMlp = difference > 0.001f ? 1 + difference * differenceMultiplier : 1;
        }
    }

    [Serializable]
    public struct HandTransformInputAction
    {
        public Transform handTransform;
        public Transform trueHandPosition;
    }
}