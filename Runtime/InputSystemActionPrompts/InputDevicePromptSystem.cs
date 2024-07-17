using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace InputSystemActionPrompts
{

    
    /// <summary>
    /// Enumeration of device type
    /// TODO - Remove and use Input system types more effectively
    /// </summary>
    public enum InputDeviceType {
        Mouse,
        Keyboard,
        GamePad,
        Touchscreen
    }
    
    /// <summary>
    /// Encapsulates a binding map entry
    /// </summary>
    public class ActionBindingMapEntry
    {
        public string BindingPath;
        public bool IsComposite;
        public bool IsPartOfComposite;
    }
    
    public static class InputDevicePromptSystem
    {
        
        /// <summary>
        /// Map of action paths (eg "Player/Move" to binding map entries eg "Gamepad/leftStick")
        /// </summary>
        private static Dictionary<string,List<ActionBindingMapEntry>> s_ActionBindingMap = new Dictionary<string, List<ActionBindingMapEntry>>();
        
        /// <summary>
        /// Map of device names (eg "DualShockGamepadHID") to device prompt data (list of action bindings and sprites)
        /// </summary>
        private static Dictionary<string,InputDevicePromptData> s_DeviceDataBindingMap = new Dictionary<string, InputDevicePromptData>();
        
        /// <summary>
        /// Currently initialised
        /// </summary>
        private static bool s_Initialised = false;
        
        /// <summary>
        /// The settings file
        /// </summary>
        private static InputSystemDevicePromptSettings s_Settings;
        
        /// <summary>
        /// Currently active device
        /// </summary>
        private static InputDevice s_ActiveDevice;
        
        /// <summary>
        /// Delegate for when the active device changes
        /// </summary>
        public static Action<InputDevice> OnActiveDeviceChanged = delegate {  };
        
        /// <summary>
        /// Event listener for button presses on input system
        /// </summary>
        private static IDisposable s_EventListener;

        private static InputDevicePromptData s_PlatformDeviceOverride;

        public static bool GetPlatformDeviceOverride(out InputDevicePromptData inputDevice)
        {
            if (s_PlatformDeviceOverride != null)
            {
                inputDevice = s_PlatformDeviceOverride;
                return true;
            }

            // get the current platform
            var platform = Application.platform;
            // check if we have a platform override
            foreach (var platformOverride in s_Settings.RuntimePlatformsOverride)
            {
                if (platformOverride.Platform == platform) { 
                    inputDevice = platformOverride.DevicePromptData;
                    return true;
                }
            }

            inputDevice = null;
            return false;
        }
        
        /// <summary>
        /// Initialises data structures and load settings, called on first use
        /// </summary>
        private static void Initialise()
        {
            Debug.Log("Initialising InputDevicePromptSystem");
            s_Settings =InputSystemDevicePromptSettings.GetSettings();
            
            if (s_Settings == null)
            {
                Debug.LogWarning("InputSystemDevicePromptSettings missing");
                return;
            }

            if (!s_Settings.PromptSpriteFormatter.Contains(InputSystemDevicePromptSettings.PromptSpriteFormatterSpritePlaceholder))
            {
                Debug.LogError($"{nameof(InputSystemDevicePromptSettings.PromptSpriteFormatter)} must include {InputSystemDevicePromptSettings.PromptSpriteFormatterSpritePlaceholder} or no sprites will be shown.");
            }
            
            // We'll want to listen to buttons being pressed on any device
            // in order to dynamically switch device prompts (From description in InputSystem.cs)
            s_EventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
            
            // Listen to device change. If the active device is disconnected, switch to default
            InputSystem.onDeviceChange += OnDeviceChange;
            
            BuildBindingMaps();
            FindDefaultDevice();

            GetPlatformDeviceOverride(out s_PlatformDeviceOverride);

            s_Initialised = true;
        }

        /// <summary>
        /// Called on device change
        /// </summary>
        /// <param name="device"></param>
        /// <param name="change"></param>
        private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            // If the active device has been disconnected, revert to default device
            if (device != s_ActiveDevice) return;
            
            if ((change == InputDeviceChange.Disconnected) || (change == InputDeviceChange.Removed))
            {
                FindDefaultDevice();
                // Notify change
                OnActiveDeviceChanged.Invoke(s_ActiveDevice);
            }
        }

        /// <summary>
        /// Replace tags in a given string with TMPPro strings to insert device prompt sprites
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static string InsertPromptSprites(string inputText)
        {
            if (!s_Initialised) Initialise();
            if (!s_Initialised) return "InputSystemDevicePrompt Settings missing - please create using menu item 'Window/Input System Device Prompts/Create Settings'";

            var foundTags = GetTagList(inputText);
            var replacedText = inputText;
            foreach (var tag in foundTags)
            {
                var replacementTagText = GetActionPathBindingTextSpriteTags(tag);
                
                //if PromptSpriteFormatter is empty for some reason return the text as if formatter was {SPRITE} (normally)
                var promptSpriteFormatter = s_Settings.PromptSpriteFormatter == "" ? InputSystemDevicePromptSettings.PromptSpriteFormatterSpritePlaceholder : s_Settings.PromptSpriteFormatter;
                //PromptSpriteFormatter in settings uses {SPRITE} as a placeholder for the sprite, convert it to {0} for string.Format
                promptSpriteFormatter = promptSpriteFormatter.Replace( InputSystemDevicePromptSettings.PromptSpriteFormatterSpritePlaceholder, "{0}");
                replacementTagText = string.Format(promptSpriteFormatter, replacementTagText);
                
                replacedText = replacedText.Replace($"{s_Settings.OpenTag}{tag}{s_Settings.CloseTag}", replacementTagText);
            }

            return replacedText;
        }
        
        /// <summary>
        /// Gets the first matching sprite (eg DualShock Cross Button Sprite) for the given input tag (eg "Player/Jump")
        /// Currently only supports one sprite, not composite (eg WASD)
        /// </summary>
        /// <param name="inputTag"></param>
        /// <returns></returns>
        public static Sprite GetActionPathBindingSprite(string inputTag)
        {
            if (!s_Initialised) Initialise();
            var (_, matchingPrompt) = GetActionPathBindingPromptEntries(inputTag);
            return matchingPrompt != null && matchingPrompt.Count>0 ? matchingPrompt[0].PromptSprite : null;
        }

        /// <summary>
        /// Gets the current active device matching sprite in DeviceSpriteEntries list for the given sprite name
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>

        public static Sprite GetDeviceSprite(string spriteName)
        {
            if (!s_Initialised) Initialise();

            InputDevicePromptData validDevice;

            if (s_PlatformDeviceOverride != null)
            {
                validDevice = s_PlatformDeviceOverride;
            }
            else
            {
                if (s_ActiveDevice == null) return null;

                var activeDeviceName = s_ActiveDevice.name;

                if (!s_DeviceDataBindingMap.ContainsKey(activeDeviceName))
                {
                    Debug.LogError($"MISSING_DEVICE_ENTRIES '{activeDeviceName}'");
                    return null;
                }

                //// search for key in dictionary s_DeviceDataBindingMap that starts with activeDeviceName
                //var matchingDevice = s_DeviceDataBindingMap.FirstOrDefault(x => x.Key.StartsWith(activeDeviceName)).Value;

                validDevice = s_DeviceDataBindingMap[activeDeviceName];
            }
            

            var matchingSprite = validDevice.DeviceSpriteEntries.FirstOrDefault((sprite) =>
                           String.Equals(sprite.SpriteName, spriteName,
                                              StringComparison.CurrentCultureIgnoreCase));

            if (matchingSprite != null)
            {
                return matchingSprite.Sprite;
            }

            return null;
        }

        /// <summary>
        /// Creates a TextMeshPro formatted string for all matching sprites for a given tag
        /// Supports composite tags, eg WASD by returning all matches for active device (observing order)
        /// </summary>
        /// <param name="inputTag"></param>
        /// <returns></returns>
        private static string GetActionPathBindingTextSpriteTags(string inputTag)
        {
            if (s_PlatformDeviceOverride == null) // not platform override
            {
                if (s_ActiveDevice == null) return "NO_ACTIVE_DEVICE";
                var activeDeviceName = s_ActiveDevice.name;

                if (!s_DeviceDataBindingMap.ContainsKey(activeDeviceName))
                {
                    return $"MISSING_DEVICE_ENTRIES '{activeDeviceName}'";
                }
            }

            var lowerCaseTag = inputTag.ToLower();

            if (!s_ActionBindingMap.ContainsKey(lowerCaseTag))
            {
                return $"MISSING_ACTION {lowerCaseTag}";
            }

            var (validDevice, matchingPrompt) = GetActionPathBindingPromptEntries(inputTag);
           
            if (matchingPrompt==null || matchingPrompt.Count==0)
            {
                return $"MISSING_PROMPT '{inputTag}'";
            }
            // Return each
            var outputText = string.Empty;
            foreach (var prompt in matchingPrompt)
            {
                outputText += $"<sprite=\"{validDevice.SpriteAsset.name}\" name=\"{prompt.PromptSprite.name}\" {s_Settings.RichTextTags}>";
            }
            return outputText;
        }

        /// <summary>
        /// Gets all matching prompt entries for a given tag (eg "Player/Jump")
        /// </summary>
        /// <param name="inputTag"></param>
        /// <returns></returns>
        private static (InputDevicePromptData,List<ActionBindingPromptEntry>) GetActionPathBindingPromptEntries(string inputTag)
        {
            InputDevicePromptData validDevice;

            var lowerCaseTag = inputTag.ToLower();
            if (!s_ActionBindingMap.ContainsKey(lowerCaseTag)) return (null, null);

            if (s_PlatformDeviceOverride != null)
            {
                validDevice = s_PlatformDeviceOverride;
            }
            else
            {
                if (s_ActiveDevice == null) return (null, null);
                if (!s_DeviceDataBindingMap.ContainsKey(s_ActiveDevice.name)) return (null, null);

                validDevice = s_DeviceDataBindingMap[s_ActiveDevice.name];
            }

            var validEntries = new List<ActionBindingPromptEntry>();
            var actionBindings=s_ActionBindingMap[lowerCaseTag];
            
            foreach (var actionBinding in actionBindings)
            {
                //Debug.Log($"Checking binding '{actionBinding}' on device {validDevice.name}");
                var usage = GetUsageFromBindingPath(actionBinding.BindingPath);
                if (string.IsNullOrEmpty(usage))
                {
                    var matchingPrompt = validDevice.ActionBindingPromptEntries.FirstOrDefault((prompt) =>
                        String.Equals(prompt.ActionBindingPath, actionBinding.BindingPath,
                            StringComparison.CurrentCultureIgnoreCase));
                    if (matchingPrompt != null)
                    {
                        //Debug.Log($"Found matching prompt {matchingPrompt.ActionBindingPath} for {inputTag}");
                        validEntries.Add(matchingPrompt);
                    }
                }
                else
                {
                    // This is a usage, eg "Submit" or "Cancel", in the format "*/{Submit}"
                    
                    // Its possible in some control schemes (eg mouse keyboard) that active device
                    // Doesnt have a given usage (eg submit), so will want to find an alternative

                    var matchingUsageFound = false;
                    var deviceList = new List<InputDevice>(InputSystem.devices);
                    // Move active device to front of queue
                    deviceList.Remove(s_ActiveDevice);
                    deviceList.Insert(0, s_ActiveDevice);

                    for (var i = 0; i < deviceList.Count && !matchingUsageFound; i++)
                    {
                        var testDevice=deviceList[i];
                        foreach (var control in testDevice.allControls)
                        {
                            foreach (var controlUsage in control.usages)
                            {
                                if (controlUsage.ToLower() == usage.ToLower())
                                {
                                    // Match! Search for prompt entry with same extension (ignore first part eg "gamepad")
                                    var matchingPrompt = validDevice.ActionBindingPromptEntries.FirstOrDefault((prompt) =>
                                        String.Equals(prompt.ActionBindingPath.Split('/').Last(), control.name,
                                            StringComparison.CurrentCultureIgnoreCase));
                                    if (matchingPrompt != null)
                                    {
                                        validEntries.Add(matchingPrompt);
                                        matchingUsageFound = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return (validDevice, validEntries);
        }
        
        /// <summary>
        /// Extract the usage from a binding path, eg "*/{Submit}" returns "Submit"
        /// </summary>
        /// <param name="actionBinding"></param>
        /// <returns></returns>
        private static string GetUsageFromBindingPath(string actionBinding)
        {
            return actionBinding.Contains("*/{") ? actionBinding.Substring(3, actionBinding.Length - 4) : String.Empty;
        } 
        
        
        
        /// <summary>
        /// Extracts all tags from a given string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static List<string> GetTagList(string input)
        {
            var outputTags = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == s_Settings.OpenTag)
                {
                    var start = i + 1;
                    var end = input.IndexOf(s_Settings.CloseTag, i + 1);
                    var foundTag = input.Substring(start, end - start);
                    outputTags.Add(foundTag);
                }
            }

            return outputTags;
        }

       
        /// <summary>
        /// Finds default device based on current settings priorities
        /// </summary>
        private static void FindDefaultDevice()
        {
            // When we start up there have been no button presses, so we want to pick the first device
            // that matches the priorities in the settings file
            
            foreach (var deviceType in s_Settings.DefaultDevicePriority)
            {
                foreach (var device in InputSystem.devices.Where(device => DeviceMatchesType(device, deviceType)))
                {
                    s_ActiveDevice = device;
                    return;
                }
            }
        }

        private static bool DeviceMatchesType(InputDevice device, InputDeviceType type)
        {
            return type switch
            {
                InputDeviceType.Mouse => device is Mouse,
                InputDeviceType.Keyboard => device is Keyboard,
                InputDeviceType.GamePad => device is Gamepad,
                InputDeviceType.Touchscreen => device is Touchscreen,
                _ => false
            };
        }
        

        /// <summary>
        /// Builds internal map of all actions (eg "Player/Jump" to available binding paths (eg "Gamepad/ButtonSouth")
        /// </summary>
        private static void BuildBindingMaps()
        {
            s_ActionBindingMap = new Dictionary<string, List<ActionBindingMapEntry>>();
            
            // Build a map of all controls and associated bindings
            foreach (var inputActionAsset in s_Settings.InputActionAssets)
            {
                var allActionMaps = inputActionAsset.actionMaps;
                foreach (var actionMap in allActionMaps)
                {
                    foreach (var binding in actionMap.bindings)
                    {
                        var bindingPath = $"{actionMap.name}/{binding.action}";
                        var bindingPathLower = bindingPath.ToLower();
                        
                        //Debug.Log($"Binding {bindingPathLower} to path {binding.path}");
                        var entry = new ActionBindingMapEntry
                        {
                            BindingPath = binding.effectivePath,
                            IsComposite = binding.isComposite,
                            IsPartOfComposite = binding.isPartOfComposite
                        };
                        if (s_ActionBindingMap.TryGetValue(bindingPathLower, out var value))
                        {
                            value.Add(entry);
                        }
                        else
                        {
                            s_ActionBindingMap.Add(bindingPathLower, new List<ActionBindingMapEntry> { entry});
                        }
                    }
                }
            }


            // Build a map of device name to device data
            foreach (var devicePromptData in s_Settings.DevicePromptAssets)
            {
                foreach (var deviceName in devicePromptData.DeviceNames)
                {
                    if (s_DeviceDataBindingMap.ContainsKey(deviceName))
                    {
                        Debug.LogWarning(
                            $"Duplicate device name found in InputSystemDevicePromptSettings: {deviceName}. Check your entries");
                    }
                    else
                    {
                        s_DeviceDataBindingMap.Add(deviceName, devicePromptData);
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when a button is pressed on any device
        /// </summary>
        /// <param name="button"></param>
        private static void OnButtonPressed(InputControl button)
        {
            if (s_ActiveDevice==button.device) return;
            s_ActiveDevice = button.device;
            OnActiveDeviceChanged.Invoke(s_ActiveDevice);
        }
        
    }
}
