using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    [RequireComponent(typeof(AssetSelectionElement))]
    public class AssetSelectionUI : MonoBehaviour
    {
        private AssetSelectionElement assetSelectionElement;
        private OutfitGender lastGender = OutfitGender.None;

        public AssetSelectionElement AssetSelectionElement =>
            assetSelectionElement ??= GetComponent<AssetSelectionElement>();

        public OutfitGender Gender { get; set; } = OutfitGender.None;

        private void OnEnable()
        {
            if (Gender == OutfitGender.None || Gender == lastGender)
            {
                return;
            }

            lastGender = Gender;

            GetAssets();
        }

        private async void GetAssets()
        {
            await AssetSelectionElement.LoadAndCreateButtons(Gender);
        }
    }
}