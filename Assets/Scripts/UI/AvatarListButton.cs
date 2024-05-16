using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ReadyPlayerMe.XR
{
    [RequireComponent(typeof(Button))]
    public class AvatarListButton : MonoBehaviour
    {
        [SerializeField] private GameObject avatar;
        [SerializeField] private UnityEvent<GameObject> OnAvatarSelected;

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectAvatar);
        }

        private void SelectAvatar()
        {
            OnAvatarSelected?.Invoke(avatar);
        }
    }
}