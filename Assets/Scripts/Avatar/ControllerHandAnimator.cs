using UnityEngine;
using UnityEngine.InputSystem;

namespace ReadyPlayerMe.XR
{
    public class ControllerHandAnimator : MonoBehaviour
    {
        private const float GRIP_THRESHOLD = 0.02f;

        private static readonly int leftHandGripHash = Animator.StringToHash("L_Hand_Grip");
        private static readonly int rightHandGripHash = Animator.StringToHash("R_Hand_Grip");
        private static readonly int leftHandTriggerHash = Animator.StringToHash("L_Hand_Trigger");
        private static readonly int rightHandTriggerHash = Animator.StringToHash("R_Hand_Trigger");

        [SerializeField] private bool isLeftHand;
        [SerializeField] private Animator animator;
        [SerializeField] private InputActionProperty trigger;
        [SerializeField] private InputActionProperty grip;

        private float lastGripValue;
        private float lastTriggerValue;

        private void Update()
        {
            var triggerValue = trigger.action.ReadValue<float>();
            var gripValue = grip.action.ReadValue<float>();

            if (Mathf.Abs(lastGripValue - gripValue) > GRIP_THRESHOLD)
            {
                animator.SetFloat(isLeftHand ? leftHandGripHash : rightHandGripHash, gripValue);
                lastGripValue = gripValue;
            }

            if (Mathf.Abs(lastTriggerValue - triggerValue) > GRIP_THRESHOLD)
            {
                animator.SetFloat(isLeftHand ? leftHandTriggerHash : rightHandTriggerHash, triggerValue);
                lastTriggerValue = triggerValue;
            }
        }
    }
}