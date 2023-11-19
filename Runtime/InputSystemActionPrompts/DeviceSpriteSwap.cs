using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InputSystemActionPrompts
{
    [RequireComponent(typeof(Image))]
    public class DeviceSpriteSwap : MonoBehaviour
    {

        /// <summary>
        /// The image to apply the prompt sprite to
        /// </summary>
        private Image m_Image;

        /// <summary>
        /// The name of the custom sprite to use
        /// </summary>
        [SerializeField] private string customSpriteName = "";

        [SerializeField] private bool _setNativeSize = true;

        void Start()
        {
            m_Image = GetComponent<Image>();
            if (m_Image == null) return;
            RefreshSprite();
            // Listen to device changing
            InputDevicePromptSystem.OnActiveDeviceChanged += DeviceChanged;
        }

        private void OnDestroy()
        {
            // Remove listener
            InputDevicePromptSystem.OnActiveDeviceChanged -= DeviceChanged;
        }

        /// <summary>
        /// Called when active input device changed
        /// </summary>
        /// <param name="obj"></param>
        private void DeviceChanged(InputDevice device)
        {
            RefreshSprite();
        }

        /// <summary>
        /// Sets the icon for the current action
        /// </summary>
        private void RefreshSprite()
        {
            var sourceSprite = InputDevicePromptSystem.GetDeviceSprite(customSpriteName);
            if (sourceSprite == null) return;

            m_Image.sprite = sourceSprite;
            if (_setNativeSize)
                m_Image.SetNativeSize();
        }

    }
}
