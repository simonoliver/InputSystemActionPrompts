using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace InputSystemActionPrompts
{
    /// <summary>
    /// Settings for Input Device Prompt system
    /// </summary>
   
    public class InputSystemDevicePromptSettings : ScriptableObject
    {
        /// <summary>
        /// List of all actions to consider for text replacement
        /// </summary>
        public List<InputActionAsset> InputActionAssets;

        /// <summary>
        /// List of all device prompt assets to apply
        /// </summary>
        public List<InputDevicePromptData> DevicePromptAssets;
        
        /// <summary>
        /// The priority for prompt display before a button is pressed 
        /// </summary>
        public List<InputDeviceType> DefaultDevicePriority = new List<InputDeviceType>
        {
            InputDeviceType.GamePad,
            InputDeviceType.Keyboard,
            InputDeviceType.Mouse
        };
        
        public char OpenTag = '[';
        public char CloseTag = ']';
        
        /// <summary>
        /// The amount a gamepad stick must be moved to be considered a device detection event. 
        /// </summary>
        [Range(0,1)]
        [Tooltip("The amount a gamepad stick must be moved to be considered a device detection event.")]
        public float gamepadStickDeviceDetectionThreshold = 0.1f;
        
        public const string SettingsDataFile = "InputSystemDevicePromptSettings";
        
        public static InputSystemDevicePromptSettings GetSettings()
        {
            var settings = Resources.Load<InputSystemDevicePromptSettings>(SettingsDataFile);
            if (settings == null)
            {
                Debug.LogWarning($"Could not find InputSystemDevicePromptSettings at path '{SettingsDataFile} - Create using Window->Input System Device Prompts->Create Settings'");
            }
            return settings;
        }

    }
}