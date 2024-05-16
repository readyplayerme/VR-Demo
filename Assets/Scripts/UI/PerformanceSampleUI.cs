using ReadyPlayerMe.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.XR
{
    public class PerformanceSampleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI avatarCount;
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [SerializeField] private GameObject fpsError;

        [SerializeField] private UnityEvent<Lod> OnAvatarQualityChanged;

        private void Start()
        {
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        private void OnQualityChanged(int value)
        {
            switch (value)
            {
                case 0:
                    OnAvatarQualityChanged?.Invoke(Lod.High);
                    break;
                case 1:
                    OnAvatarQualityChanged?.Invoke(Lod.Medium);
                    break;
                case 2:
                    OnAvatarQualityChanged?.Invoke(Lod.Low);
                    break;
            }
        }

        public void SetAvatars(int number)
        {
            avatarCount.text = number.ToString();
        }

        public void ShowFPSError()
        {
            fpsError.SetActive(true);
        }

        public void HideFPSError()
        {
            fpsError.SetActive(false);
        }
    }
}