using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe.XR
{
    public class HoverButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image clickBlur;
        [SerializeField] private Image hoverBlur;
        [SerializeField] private bool scaleOnHover;

        private readonly float hoverScale = 1.06f;

        private bool isClicked;
        private bool isHovering;
        private bool isPointerDown;

        private ButtonState state = new Normal();

        public void OnPointerDown(PointerEventData eventData)
        {
            state.OnPointerDown(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            state.OnMouseEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            state.OnMouseExit(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            state.OnPointerUp(this);
        }

        private void SetState(ButtonState state)
        {
            this.state = state;
            this.state.Init(this);
        }

        private abstract class ButtonState
        {
            protected static readonly ButtonState normal = new Normal();
            protected static readonly ButtonState hover = new Hover();
            protected static readonly ButtonState click = new Click();

            public virtual void Init(HoverButtonController button)
            {
            }

            public virtual void OnPointerDown(HoverButtonController button)
            {
            }

            public virtual void OnPointerUp(HoverButtonController button)
            {
            }

            public virtual void OnMouseEnter(HoverButtonController button)
            {
            }

            public virtual void OnMouseExit(HoverButtonController button)
            {
            }
        }

        private sealed class Normal : ButtonState
        {
            public override void Init(HoverButtonController button)
            {
                if (button.scaleOnHover)
                {
                    button.gameObject.transform.localScale = Vector3.one;
                }

                if (button.clickBlur)
                {
                    button.clickBlur.gameObject.SetActive(false);
                }

                if (button.hoverBlur)
                {
                    button.hoverBlur.gameObject.SetActive(false);
                }
            }

            public override void OnMouseEnter(HoverButtonController button)
            {
                button.SetState(hover);
            }
        }

        private sealed class Hover : ButtonState
        {
            public override void Init(HoverButtonController button)
            {
                if (button.scaleOnHover)
                {
                    button.gameObject.transform.localScale =
                        new Vector3(button.hoverScale, button.hoverScale, button.hoverScale);
                }

                if (button.hoverBlur)
                {
                    button.hoverBlur.gameObject.SetActive(true);
                }

                if (AudioManager.Instance)
                {
                    AudioManager.Instance.PlayAudio(AudioManager.Audio.Hover);
                }
            }

            public override void OnPointerDown(HoverButtonController button)
            {
                button.SetState(click);
            }

            public override void OnMouseExit(HoverButtonController button)
            {
                button.SetState(normal);
            }
        }

        private sealed class Click : ButtonState
        {
            public override void Init(HoverButtonController button)
            {
                if (button.scaleOnHover)
                {
                    button.gameObject.transform.localScale = Vector3.one;
                }

                if (button.clickBlur)
                {
                    button.hoverBlur.gameObject.SetActive(true);
                }

                if (AudioManager.Instance)
                {
                    AudioManager.Instance.PlayAudio(AudioManager.Audio.Click);
                }
            }

            public override void OnPointerUp(HoverButtonController button)
            {
                button.SetState(normal);
            }
        }
    }
}