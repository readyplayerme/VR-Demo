using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace ReadyPlayerMe.XR
{
    public class SeatedExperienceController : MonoBehaviour
    {
        private const float CAMERA_ROTATION_OFFSET_IN_DEGREES = -90f;
        private const float ORIGIN_Y_OFFSET = 1.7f;

        [SerializeField] private Transform leftLegTarget;
        [SerializeField] private Transform rightLegTarget;
        private readonly Vector3 sittingAvatarCameraPosition = new(0, -0.5f, 0);

        private TrackedPoseDriver poseDriver => AvatarComponentReferences.Instance.TrackedPoseDriver;
        private Animator animator => AvatarComponentReferences.Instance.Animator;
        private Transform origin => AvatarComponentReferences.Instance.XROrigin.transform;
        private VRIK vrik => AvatarComponentReferences.Instance.Vrik;

        public void DelayedSitdown()
        {
            StartCoroutine(SitDownDelayed());
        }

        private IEnumerator SitDownDelayed()
        {
            yield return null;
            SitDown();
        }

        [ContextMenu("Sit down")]
        public void SitDown()
        {
            var rotation = origin.rotation;
            animator.speed = 0;

            origin.position = new Vector3(transform.position.x,
                ORIGIN_Y_OFFSET, transform.position.z);

            rotation = Quaternion.Euler(rotation.x, CAMERA_ROTATION_OFFSET_IN_DEGREES,
                rotation.z);
            origin.rotation = rotation;

            SetLegWeights(1);

            poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
            poseDriver.transform.localPosition = sittingAvatarCameraPosition;

            StartCoroutine(CheckIfStanding());
        }

        private IEnumerator CheckIfStanding()
        {
            var headFollowPos = poseDriver.transform.position;
            while (Vector3.Distance(headFollowPos, poseDriver.transform.position) < 0.1f)
            {
                yield return null;
            }

            StandUp();
        }

        private void StandUp()
        {
            animator.speed = 1;
            SetLegWeights(0);

            poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        }

        private void SetLegWeights(float value)
        {
            if (value != 0)
            {
                vrik.solver.leftLeg.target = leftLegTarget;
                vrik.solver.rightLeg.target = rightLegTarget;
            }
            else
            {
                vrik.solver.leftLeg.target = null;
                vrik.solver.rightLeg.target = null;
            }

            vrik.solver.leftLeg.positionWeight = value;
            vrik.solver.rightLeg.positionWeight = value;
        }
    }
}