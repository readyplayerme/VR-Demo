using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ReadyPlayerMe.XR
{
    public class HeightCalibrator : MonoBehaviour
    {
        private const float MAX_ALLOWED_HEIGHT = 2.2f;
        private const float MIN_ALLOWED_HEIGHT = 1.35f;
        [SerializeField] private InputActionProperty trackedHeadPosition;
        private float lastCalibratedHeight;
        private float scale;

        private static VRIK Vrik => AvatarComponentReferences.Instance.Vrik;

        private void Start()
        {
            lastCalibratedHeight = AvatarComponentReferences.Instance.AvatarDefaultHeight;
        }

        public void CalibrateHeight()
        {
            var headPosition = trackedHeadPosition.action.ReadValue<Vector3>();

            lastCalibratedHeight = Mathf.Min(MAX_ALLOWED_HEIGHT, Mathf.Max(MIN_ALLOWED_HEIGHT, headPosition.y));

            CalibrateBody();
        }

        public void CalibrateBody()
        {
            scale = lastCalibratedHeight / AvatarComponentReferences.Instance.AvatarDefaultHeight;
            Vrik.references.root.localScale = new Vector3(scale, scale, scale);
            CalibrateHead();
            CalibrateHands();
        }

        private void CalibrateHead()
        {
            const float scaleDivisionConstant = 2f;

            var headScale = 1f + (1f - scale) / scaleDivisionConstant;
            Vrik.references.head.localScale = new Vector3(headScale, headScale, headScale);
        }

        private void CalibrateHands()
        {
            ScaleBoneToOne(Vrik.references.leftHand);
            ScaleBoneToOne(Vrik.references.rightHand);
        }

        private void ScaleBoneToOne(Transform hand)
        {
            hand.localScale = Vector3.one;
            var lossyScale = hand.lossyScale;

            hand.localScale = new Vector3(1f / lossyScale.x,
                1f / lossyScale.y, 1f / lossyScale.z);
        }
    }
}