using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.XR
{
    public class SimpleAvatarCreator : MonoBehaviour
    {
        [SerializeField] private AvatarConfig avatarConfig;
        [SerializeField] private GameObject loading;

        [SerializeField] private GameObject panelTemplateSelection;
        [SerializeField] private GameObject panelElements;
        [SerializeField] private TemplateSelectionElement templateSelectionElement;

        [SerializeField] private PanelManager mainPanelManager;
        [SerializeField] private List<AssetSelectionUI> assetSelectionElementUis;

        [SerializeField] private UnityEvent<AvatarProperties> onTemplateSelected;


        private readonly BodyType bodyType = BodyType.FullBodyXR;
        private AvatarManager avatarManager;

        private OutfitGender gender = OutfitGender.None;

        private void Start()
        {
            assetSelectionElementUis.ForEach(element =>
                element.AssetSelectionElement.OnAssetSelected.AddListener(OnAssetSelection));
        }

        public void LoadAvatarCreatorElements()
        {
            mainPanelManager.ShowPanel(panelTemplateSelection);

            avatarManager = new AvatarManager(avatarConfig);

            templateSelectionElement.OnAssetSelected.AddListener(assetData =>
                TemplateSelected((AvatarTemplateData)assetData));
            templateSelectionElement.LoadAndCreateButtons();
        }

        private void SetGender(OutfitGender gender)
        {
            if (gender == this.gender)
            {
                return;
            }

            this.gender = gender;
            assetSelectionElementUis.ForEach(elementUI => elementUI.Gender = gender);
        }

        public void LoadCachedAvatar(GameObject cachedAvatar)
        {
            var loadedAvatar = Instantiate(cachedAvatar);
            var avatarData = loadedAvatar.GetComponent<AvatarData>();
            TransferAvatarData(avatarData.AvatarId, avatarData.AvatarMetadata);
            UpdateAvatar(loadedAvatar);
        }

        public async void OnAssetSelection(IAssetData assetData)
        {
            loading.SetActive(true);
            var updatedAvatar = await avatarManager.UpdateAsset(assetData.AssetType, bodyType, assetData.Id);
            UpdateAvatar(updatedAvatar);
            loading.SetActive(false);
        }

        private async void TemplateSelected(AvatarTemplateData assetData)
        {
            loading.SetActive(true);
            var avatarProperties = await GetAvatar(assetData);

            SetGender(avatarProperties.Gender);
            onTemplateSelected?.Invoke(avatarProperties);

            mainPanelManager.ShowPanel(panelElements);
            loading.SetActive(false);
        }

        private async Task<AvatarProperties> GetAvatar(AvatarTemplateData avatarTemplate)
        {
            var templateAvatarProps = await avatarManager.CreateAvatarFromTemplateAsync(avatarTemplate.Id, bodyType);
            var avatarProperties = templateAvatarProps.Properties;

            TransferAvatarData(avatarProperties.Id, new AvatarMetadata
            {
                OutfitGender = avatarProperties.Gender,
                BodyType = avatarProperties.BodyType
            });

            UpdateAvatar(templateAvatarProps.AvatarObject);
            return avatarProperties;
        }

        private void UpdateAvatar(GameObject newAvatar)
        {
            AvatarMeshHelper.TransferMesh(newAvatar, AvatarComponentReferences.Instance.Vrik.gameObject);
            AvatarComponentReferences.Instance.HeightCalibrator.CalibrateBody();
            Destroy(newAvatar);
        }

        private void TransferAvatarData(string id, AvatarMetadata metadata)
        {
            var avatarData = AvatarComponentReferences.Instance.AvatarData;
            avatarData.AvatarId = id;
            avatarData.AvatarMetadata = metadata;
        }
    }
}