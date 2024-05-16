using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

namespace ReadyPlayerMe.XR
{
    public enum XRHandAnimation
    {
        None = 0,
        Hold = 1,
        Pinch = 2,
        Grab = 3
    }

    public class XRGrabAttach : MonoBehaviour
    {
        private const string DEFAULT_LAYER = "Default";
        private const string IGNORE_RAYCAST_LAYER = "Ignore Raycast";
        private const string RIGHT_HAND_TAG = "RightHand";

        private static readonly int leftHandPoseHash = Animator.StringToHash("L_Hand_Pose");
        private static readonly int rightHandPoseHash = Animator.StringToHash("R_Hand_Pose");

        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private XRHandAnimation handAnimation;
        private int defaultLayerMask;

        private XRGrabInteractable grabInteractable;
        private int ignoreRaycastMask;
        private IXRInteractable interactable;
        private XRInteractorLineVisual interactorLineVisual;

        private XRPokeInteractor pokeInteractor;
        private float positionMultiplier;
        private Transform trackedHand;
        private VRIK vrik => AvatarComponentReferences.Instance.Vrik;
        private Animator playerAnimator => AvatarComponentReferences.Instance.Animator;

        private void Start()
        {
            defaultLayerMask = LayerMask.GetMask(DEFAULT_LAYER);
            ignoreRaycastMask = LayerMask.GetMask(IGNORE_RAYCAST_LAYER);
            vrik.GetIKSolver().OnPostUpdate += OnPostUpdate;
        }

        private void OnEnable()
        {
            Debug.Log("XRGrabAttach enabled");

            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }

        private void OnDisable()
        {
            Debug.Log("XRGrabAttach disabled");

            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }

        private void OnPostUpdate()
        {
            if (interactable == null)
            {
                return;
            }

            UpdateInteractableTransform();
        }

        private void UpdateInteractableTransform()
        {
            var rotation = trackedHand.rotation;

            interactable.transform.position = trackedHand.position + rotation *
                new Vector3(positionOffset.x * positionMultiplier, positionOffset.y, positionOffset.z);
            interactable.transform.rotation = rotation *
                                              Quaternion.Euler(rotationOffset.x, rotationOffset.y * positionMultiplier,
                                                  rotationOffset.z * positionMultiplier);
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            interactable = null;
            var releaseInteractor = args.interactorObject;

            pokeInteractor.gameObject.SetActive(true);
            if (interactorLineVisual != null)
            {
                interactorLineVisual.enabled = true;
            }

            grabInteractable.interactionLayerMask = defaultLayerMask;

            var handedness = releaseInteractor.transform.parent.name.ToLower().Contains("right")
                ? Handedness.Right
                : Handedness.Left;
            playerAnimator.SetInteger(handedness == Handedness.Right ? rightHandPoseHash : leftHandPoseHash, 0);
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            interactable = args.interactableObject;
            var interactor = args.interactorObject.transform;

            pokeInteractor = interactor.parent.GetComponentInChildren<XRPokeInteractor>();
            interactorLineVisual = interactor.GetComponentInChildren<XRInteractorLineVisual>();

            pokeInteractor.gameObject.SetActive(false);
            if (interactorLineVisual != null)
            {
                interactorLineVisual.enabled = false;
            }

            grabInteractable.interactionLayerMask = ignoreRaycastMask;

            var handedness = interactor.parent.name.ToLower().Contains("right")
                ? Handedness.Right
                : Handedness.Left;
            trackedHand = handedness == Handedness.Right ? vrik.references.rightHand : vrik.references.leftHand;
            positionMultiplier = handedness == Handedness.Right ? 1 : -1;
            playerAnimator.SetInteger(handedness == Handedness.Right ? rightHandPoseHash : leftHandPoseHash,
                (int)handAnimation);
        }
    }
}