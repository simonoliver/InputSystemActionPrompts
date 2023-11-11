using UnityEngine;

namespace InputSystemActionPrompts
{
    /// <summary>
    /// Helps us our <see cref="InputDevicePromptSystem"/> listen to various mono behavior events.
    /// </summary>
    public class InputDevicePromptSystemHelper : MonoBehaviour
    {
        private void OnDestroy()
        {
            InputDevicePromptSystem.Terminate();
        }
    }
}