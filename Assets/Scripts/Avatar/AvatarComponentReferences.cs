using ReadyPlayerMe.Core;
using RootMotion.FinalIK;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace ReadyPlayerMe.XR
{
    public class AvatarComponentReferences : MonoBehaviour
    {
        private const float MALE_DEFAULT_HEIGHT = 1.7f;
        private const float FEMALE_DEFAULT_HEIGHT = 1.6f;

        [field: SerializeField] public HeightCalibrator HeightCalibrator { get; private set; }
        [field: SerializeField] public AvatarData AvatarData { get; private set; }
        [field: SerializeField] public XROrigin XROrigin { get; private set; }
        [field: SerializeField] public VRIK Vrik { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public TrackedPoseDriver TrackedPoseDriver { get; private set; }

        public float AvatarDefaultHeight => AvatarData.AvatarMetadata.OutfitGender == OutfitGender.Masculine
            ? MALE_DEFAULT_HEIGHT
            : FEMALE_DEFAULT_HEIGHT;

        public static AvatarComponentReferences Instance { get; private set; }

        private void Awake()
        {
            AvatarData.AvatarMetadata.OutfitGender = OutfitGender.Masculine;
            AvatarData.AvatarMetadata.BodyType = BodyType.FullBodyXR;

            if (Instance != null && Instance != this)
            {
                Debug.LogError("Another instance of AvatarComponentReferences exists! Destroying this instance.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}