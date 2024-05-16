using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ReadyPlayerMe.XR
{
    public class CameraAnimation : MonoBehaviour
    {
        private const float END_FOCUS_VALUE = 2f;
        private const float POST_PROCCESS_PRIORITY = 100f;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 headViewPoint = new(0f, 0f, 0f);
        [SerializeField] private Vector3 footwearViewPoint = new(0f, 1.66f, 0.85f);
        [SerializeField] private Vector3 bottomsViewPoint = new(0f, 1.4f, 2.6f);
        [SerializeField] private Vector3 bodyViewPoint = new(0f, 1.4f, 2.6f);
        [SerializeField] private Vector3 topViewPoint = new(0f, 0f, 0f);
        [SerializeField] private float defaultDuration = 1f;
        private DepthOfField depthOfField;
        private float duration;
        private float endFocus;
        private bool isTransitioning;
        private float startFocus;
        private Vector3 startPosition;

        private Vector3 targetPosition;
        private float transitionTime;

        private PostProcessVolume volume;

        private void Start()
        {
            depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
            depthOfField.enabled.Override(true);
            depthOfField.focusDistance.Override(END_FOCUS_VALUE);

            volume = PostProcessManager.instance.QuickVolume(gameObject.layer, POST_PROCCESS_PRIORITY, depthOfField);
        }

        private void LateUpdate()
        {
            if (!isTransitioning)
            {
                return;
            }

            transitionTime += Time.deltaTime;
            if (transitionTime < duration)
            {
                var t = Mathf.SmoothStep(0.0f, 1.0f, transitionTime / duration);
                cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
                depthOfField.focusDistance.value = math.lerp(startFocus, endFocus, t);
            }
            else
            {
                cameraTransform.position = targetPosition;
                isTransitioning = false;
            }
        }


        private void OnDestroy()
        {
            RuntimeUtilities.DestroyVolume(volume, true, true);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + footwearViewPoint, 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + bodyViewPoint, 0.1f);
        }

        public void FocusOnHead()
        {
            StartTransition(footwearViewPoint, defaultDuration, 2f, 0.8f);
            StartTransition(headViewPoint, defaultDuration, startFocus, endFocus);
        }

        public void FocusOnFoot()
        {
            StartTransition(footwearViewPoint, defaultDuration, 2f, 0.8f);
        }

        public void FocusOnBottoms()
        {
            StartTransition(bottomsViewPoint, defaultDuration, 2f, 1.2f);
        }

        public void FocusOnTop()
        {
            StartTransition(topViewPoint, defaultDuration, 2f, 1.2f);
        }

        public void FocusOnBody()
        {
            StartTransition(bodyViewPoint, defaultDuration, 1f, 2f);
        }

        public void StopTransition()
        {
            isTransitioning = false;
        }

        private void StartTransition(Vector3 newTargetLocalPosition, float transitionDuration, float newStartFocus,
            float newEndFocus)
        {
            startFocus = newStartFocus;
            endFocus = newEndFocus;
            startPosition = cameraTransform.position;
            targetPosition = transform.TransformPoint(newTargetLocalPosition);
            duration = transitionDuration;
            transitionTime = 0f;
            isTransitioning = true;
        }
    }
}