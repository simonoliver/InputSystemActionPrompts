using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InputSystemActionPrompts
{
   
   /// <summary>
   /// A single entry for an action and the corresponding sprite to use
   /// </summary>
   ///
   [Serializable]
   public class ActionBindingPromptEntry
   {
      /// <summary>
      /// The Action Binding path, eg "<Gamepad>/leftStick"
      /// As described here - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/ActionBindings.html
      /// </summary>
      public string ActionBindingPath;
      /// <summary>
      /// The name of the corresponding sprite. We assume the TMP Sprite asset and source sprite are
      /// synced up and have the same name 
      /// </summary>
      public Sprite PromptSprite;
   }


    /// <summary>
    /// Custom sprite entry for when you want to use different image based on the device (e.g. for the controller Image)
    /// </summary>
    [Serializable]
    public class DeviceSpriteEntry
    {
        public string SpriteName;
        public Sprite Sprite;
    }
   
   /// <summary>
   /// The data for a single input device such as PlayStation 4 Controller
   /// </summary>
   [CreateAssetMenu(fileName = "InputDevicePromptData", menuName = "InputDevicePromptData", order = 1)]
   public class InputDevicePromptData : ScriptableObject
   {
      /// <summary>
      /// The types of devices supported by this asset (can be multiple eg mouse/keyboard)
      /// </summary>
      public List<InputDeviceType> DeviceTypes = new List<InputDeviceType>();
      /// <summary>
      /// Device names that can be used to identify this device
      /// </summary>
      public string[] DeviceNames;
      /// <summary>
      /// Description of the device (for reference)
      /// </summary>
      public string DeviceDescription;
      /// <summary>
      /// The TextMeshPro Sprite Asset which contains prompt icons for this device
      /// </summary>
      public TMP_SpriteAsset SpriteAsset;
      /// <summary>
      /// A list of all the action bindings and their corresponding prompt icons
      /// </summary>
      public List<ActionBindingPromptEntry> ActionBindingPromptEntries;
      /// <summary>
      /// A list of optional custom sprites and their corresponding names
      /// </summary>
      public List<DeviceSpriteEntry> DeviceSpriteEntries;
   }
}
