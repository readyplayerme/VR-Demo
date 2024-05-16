using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.XR
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField] private List<ButtonElementLink> buttonElementLinks;

        private void Awake()
        {
            foreach (var elementButtonLink in buttonElementLinks)
            {
                if (elementButtonLink.button)
                {
                    elementButtonLink.button.onClick.AddListener(() => { ShowPanel(elementButtonLink.element); });
                }
            }
        }

        public void ShowPanel(GameObject element)
        {
            buttonElementLinks.ForEach(elementSection =>
                elementSection.element.SetActive(elementSection.element == element));
        }
    }

    [Serializable]
    public struct ButtonElementLink
    {
        public Button button;
        public GameObject element;
    }
}