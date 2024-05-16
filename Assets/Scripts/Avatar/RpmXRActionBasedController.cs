using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace ReadyPlayerMe.XR
{
    public class RpmXRActionBasedController : ActionBasedController
    {
        [Header("Head properties")] [SerializeField]
        private Transform headTransform;

        [SerializeField] private InputActionProperty trackedHeadPosition;

        private InputAction cachedPositionAction;
        private InputAction cachedRotationAction;

        protected override void OnEnable()
        {
            base.OnEnable();
            CacheInputActions();
        }

        private void CacheInputActions()
        {
            cachedPositionAction = positionAction.action;
            cachedRotationAction = rotationAction.action;
        }

        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            base.UpdateTrackingInput(controllerState);
            if (controllerState == null)
            {
                return;
            }

            if (cachedPositionAction != null && (controllerState.inputTrackingState & InputTrackingState.Position) != 0)
            {
                UpdatePosition(controllerState);
            }

            if (cachedRotationAction != null && (controllerState.inputTrackingState & InputTrackingState.Rotation) != 0)
            {
                UpdateRotation(controllerState);
            }
        }

        private void UpdatePosition(XRControllerState controllerState)
        {
            var headSetPosition = trackedHeadPosition.action.ReadValue<Vector3>();
            var headOffset = headSetPosition - headTransform.localPosition;
            var controllerPos = cachedPositionAction.ReadValue<Vector3>();
            controllerState.position = controllerPos - headOffset;
        }

        private void UpdateRotation(XRControllerState controllerState)
        {
            controllerState.rotation = cachedRotationAction.ReadValue<Quaternion>();
        }
    }
}
