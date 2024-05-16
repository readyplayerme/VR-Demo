using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.XR
{
    public class AvatarSelectorUI : MonoBehaviour
    {
        [SerializeField] private SimpleAvatarCreator simpleAvatarCreator;
        [SerializeField] private Transform buttonPanel;
        [SerializeField] private GameObject avatarCreator;
        [SerializeField] private GameObject noNetwork;
        [SerializeField] private PanelManager panelManager;

        private Button[] buttons;

        private void Start()
        {
            buttons = buttonPanel.GetComponentsInChildren<Button>();
        }

        public void OpenAvatarCreator()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                panelManager.ShowPanel(noNetwork);
                return;
            }

            panelManager.ShowPanel(avatarCreator);
            simpleAvatarCreator.LoadAvatarCreatorElements();
        }

        public void OnCalibrate()
        {
            AvatarComponentReferences.Instance.HeightCalibrator.CalibrateHeight();
        }
    }
}