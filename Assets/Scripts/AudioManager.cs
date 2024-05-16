using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public class AudioManager : MonoBehaviour
    {
        public enum Audio
        {
            Click,
            Hover
        }

        [SerializeField] private AudioClip click;
        [SerializeField] private AudioClip hover;
        [SerializeField] private AudioSource audioSource;

        public static AudioManager Instance { get; private set; }


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("Another instance of AudioManager exists! Destroying this instance.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void PlayAudio(Audio audio)
        {
            var clip = audio == Audio.Click ? click : hover;

            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}