using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        /// Formatter used to add additional Rich Text formatting to the returned string from <see cref="InputDevicePromptSystem.InsertPromptSprites"/>
        /// <example>
        /// <![CDATA[
        /// {0} = "<sprite="PS5_Prompts" sprite="ps5_button_cross">" (unformatted).
        /// ]]><br/>
        /// <![CDATA[
        /// <size=200%>{0}</size> = "<size=200%><sprite="PS5_Prompts" sprite="ps5_button_cross"></size>" (formatted output double size).
        /// ]]>
        /// </example>
        /// </summary>
        [Tooltip("Formatter used to add additional Rich Text formatting to all text return from InputDevicePromptSystem.InsertPromptSprites and in turn PromptText. Example <size=200%>{0}</size>")]
        public string PromptSpriteFormatter = "{0}";
        
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