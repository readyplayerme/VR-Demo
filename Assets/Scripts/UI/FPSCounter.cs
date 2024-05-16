using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.XR
{
    public class FPSCounter : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";
        [SerializeField] private Text fpsText;
        private float deltaTime;
        public float FPS { get; private set; }
        private bool isStopped = true;

        private void Update()
        {
            if(isStopped) return;
                
            deltaTime += (Time.deltaTime  - deltaTime) * 0.1f;
            FPS = 1.0f / deltaTime;
            fpsText.text = $"FPS\n{Mathf.Ceil(FPS)}";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Contains(PLAYER_TAG))
                return;
            isStopped = false;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.tag.Contains(PLAYER_TAG))
                return;
            isStopped = true;
        }
    }
}