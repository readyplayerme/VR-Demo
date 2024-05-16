using UnityEngine;
using UnityEngine.InputSystem;

namespace ReadyPlayerMe.XR
{
    /// <summary>
    ///     Class, that resets the grip actions.
    /// </summary>
    /// <remarks>
    ///     This is used together with XRInputModalityManager class to reset actions
    ///     when hands are swapped to controllers. With this the grip values of hands
    ///     are not affecting animations of the controllers.
    /// </remarks>
    public class ControllerHandAnimatorEventResetter : MonoBehaviour
    {
        [SerializeField] private InputActionProperty leftHandGrip;
        [SerializeField] private InputActionProperty rightHandGrip;

        public void ResetActions()
        {
            leftHandGrip.action.Reset();
            rightHandGrip.action.Reset();
        }
    }
}