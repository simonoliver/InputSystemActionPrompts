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
        /// {SPRITE} = "<sprite="PS5_Prompts" sprite="ps5_button_cross">" (unformatted).
        /// ]]><br/>
        /// <![CDATA[
        /// <size=200%>{SPRITE}</size> = "<size=200%><sprite="PS5_Prompts" sprite="ps5_button_cross"></size>" (formatted output double size).
        /// ]]>
        /// </example>
        /// </summary>
        [Tooltip("Formatter used to add additional Rich Text formatting to all text return from InputDevicePromptSystem.InsertPromptSprites and in turn PromptText. Example <size=200%>{SPRITE}</size>")]
        public string PromptSpriteFormatter = PromptSpriteFormatterSpritePlaceholder;
        
        /// <summary>
        /// Tags used for custom rich text formatting. 
        /// </summary>
        /// <remarks>
        /// This field can be utilized to define additional rich text tags that can be used in conjunction with the PromptSpriteFormatter.
        /// </remarks>
        [Tooltip("Additional rich text tags to be used in conjunction with PromptSpriteFormatter.")]
        public string RichTextTags = "";
        
        /// <summary>
        /// Placeholder used to denote where a sprite should be inserted in the <see cref="InputSystemDevicePromptSettings.PromptSpriteFormatter"/>
        /// </summary>
        public const string PromptSpriteFormatterSpritePlaceholder = "{SPRITE}";
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

        [Space]
        [Tooltip("Optional overrides for platform specific device prompts. Instead of being dynamic, on these platform will be used only the specified input settings.")]
        public List<PlatformInputOverride> RuntimePlatformsOverride = new List<PlatformInputOverride>();

        [System.Serializable]
        public class PlatformInputOverride
        {
            public RuntimePlatform Platform;
            public InputDevicePromptData DevicePromptData;
        }

    }
}