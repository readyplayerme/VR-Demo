using RootMotion.FinalIK;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    [RequireComponent(typeof(LookAtController))]
    public class SocialAvatar : MonoBehaviour
    {
        private const string XR_TAG = "XR";

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Transform playerHead;

        private Transform defaultLookat;
        private LookAtController lookAtController;

        private void Start()
        {
            lookAtController = GetComponent<LookAtController>();
            defaultLookat = lookAtController.target;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!HasXRTag(other.gameObject))
            {
                return;
            }

            audioSource.Play();
            lookAtController.target = playerHead;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!HasXRTag(other.gameObject))
            {
                return;
            }

            audioSource.Stop();
            lookAtController.target = defaultLookat;
        }

        private bool HasXRTag(GameObject targetObject)
        {
            return targetObject.name.Contains(XR_TAG);
        }
    }
}