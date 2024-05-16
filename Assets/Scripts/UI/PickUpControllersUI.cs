using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace ReadyPlayerMe.XR
{
    public class PickUpControllersUI : MonoBehaviour
    {
        [SerializeField] private GameObject pickupControllersUI;

        private void OnEnable()
        {
            XRInputModalityManager.currentInputMode.Subscribe(OnInputModeChanged);
            OnInputModeChanged(XRInputModalityManager.currentInputMode.Value);
        }

        private void OnDisable()
        {
            XRInputModalityManager.currentInputMode.Unsubscribe(OnInputModeChanged);
        }

        private void OnInputModeChanged(XRInputModalityManager.InputMode mode)
        {
            pickupControllersUI.SetActive(mode != XRInputModalityManager.InputMode.MotionController);
        }
    }
}