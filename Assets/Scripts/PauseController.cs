using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectsToDisable;
        private AudioSource[] audios;

        private void Awake()
        {
            audios = FindObjectsOfType<AudioSource>();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                OnResume();
                return;
            }

            OnPause();
        }

        private void OnResume()
        {
            ResumeAudio();
            foreach (var inAppObject in objectsToDisable)
            {
                inAppObject.SetActive(true);
            }

            Time.timeScale = 1f;
        }

        private void OnPause()
        {
            PauseAudio();
            foreach (var inAppObject in objectsToDisable)
            {
                inAppObject.SetActive(false);
            }

            Time.timeScale = 0f;
        }

        private void ResumeAudio()
        {
            foreach (var audioSource in audios)
            {
                audioSource.UnPause();
            }
        }

        private void PauseAudio()
        {
            foreach (var audioSource in audios)
            {
                audioSource.Pause();
            }
        }
    }
}