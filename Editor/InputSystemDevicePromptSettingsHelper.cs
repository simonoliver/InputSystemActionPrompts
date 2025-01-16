using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemActionPrompts;

namespace InputSystemActionPrompts.Editor
{
    public static class InputSystemDevicePromptSettingsHelper
    {
        [MenuItem("Window/Input System Action Prompts/Create Settings")]
        public static void CreateSettings()
        {
            var settings = ScriptableObject.CreateInstance<InputSystemDevicePromptSettings>();
            // Initialise with all input action assets found in project and packages
            settings.InputActionAssets = GetAllInstances<InputActionAsset>().ToList();
            // Initialise with all prompt assets found in project and packages
            settings.DevicePromptAssets = GetAllInstances<InputDevicePromptData>().ToList();
            settings.DefaultDevicePriority = new List<InputDeviceType>
            {
                InputDeviceType.GamePad,
                InputDeviceType.Keyboard,
                InputDeviceType.Mouse
            };
            settings.OpenTag = '[';
            settings.CloseTag = ']';
            AssetDatabase.CreateAsset(settings, $"Assets/Resources/{InputSystemDevicePromptSettings.SettingsDataFile}.asset");
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// Gets all instances of a given type in asset database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            // From here https://answers.unity.com/questions/1425758/how-can-i-find-all-instances-of-a-scriptable-objec.html
            string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for(int i =0;i<guids.Length;i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
 
            return a;
 
        }
    }
}