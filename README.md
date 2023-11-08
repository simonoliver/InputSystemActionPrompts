
# Input System Action Prompts

![Unity Figma Bridge](/Docs/input_system_action_prompts_example.gif)

**[WebGL Demo Here](https://simonoliver.itch.io/input-system-action-prompts)**

**[Example Unity Project Here](https://github.com/simonoliver/InputSystemActionPromptsExample)**

Automatically set controller prompts in text and icons in your Unity game UI. This package is an 
add-on for [Unity's Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/index.html),
working with TextMeshPro and Unity's UI system.

The package includes prompts for:

* Mouse & Keyboard
* PlayStation DualSense (PS5)
* PlayStation DualShock (PS4)
* Nintendo Switch Pro Controller
* Xbox Series X Controller
* Xbox One Controller
* Steam Deck

The prompts are exported as Sprite Sheets from the [Figma file here](https://www.figma.com/community/file/1228728155074049847),
which in turn are derived from the original great resource [Xelu's Free Controller Prompts](https://thoseawesomeguys.com/prompts/)

**Warning** This is a very early release, so there's a lot that is missing, and probably a lot of bugs - feel free to
send over any PRs or drop me a message on 🐤[Twitter](https://twitter.com/simonoliveruk) or
🐘[Mastodon](https://mastodon.gamedev.place/@simonoliver).

Please note - currently only supports **Unity 2021.3** and later!

## Setting up

![Add Package](/Docs/AddPackage.png)

* Open the Package Manager window (Window → Package Manager) and and then select the Add Package icon in the top left of
  the window to add a git package.
* Enter ```https://github.com/simonoliver/InputSystemActionPrompts.git```
* Create your settings file Window → Input System Action Prompts → Create Settings

Note, the settings file will be placed in your Resources folder, and will automatically add all the included device prompts, 
as every InputActionAsset found in the project.

## Usage

The simplest way is to use one of the built-in Behaviours:

### PromptText

Add this onto any **TextMeshPro - Text (UI)** component. It will automatically replace tags referencing your actions
with a corresponding prompt. For example put in ```To jump, press [Player/Jump]``` and will insert the correct prompt
for the active platform as a Text Sprite, eg ```To jump, press <sprite="PS5_Prompts" sprite="ps5_button_cross">```

### PromptIcon

Add this onto any **Image** component and specify the action you want to display, eg ```Player/Jump```.

### Manual Usage

You can use ```InputDevicePromptSystem.InsertPromptSprites(string inputText)``` to insert the correct prompt sprites in 
your text (for example after retrieving localisation key). For example ```text=InputDevicePromptSystem.InsertPromptSprites(To jump, press [Player/Jump])```

Or you can use GetActionPathBindingSprite(string inputTag) to get the first matching sprite, eg ```sprite=InputDevicePromptSystem.GetActionPathBindingSprite("Player/Jump")```

## Settings

### PromptSpriteFormatter

Formatter used to add additional [Rich Text formatting](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.2/manual/RichText.html) to the returned string from `InputDevicePromptSystem.InsertPromptSprites` and in turn `PromptText`.

By default `PromptSpriteFormatter` is set to `{SPRITE}` which will return the prompt sprite unedited (e.g. `<sprite="PS5_Prompts" sprite="ps5_button_cross">`).

To add additional formatting, you might edit `{SPRITE}` as follows `<size=200%><voffset=-3px>{SPRITE}</voffset></size>`. This will double the sprite size and modify it's vertical position (e.g. `<size=200%><voffset=-3px><sprite="PS5_Prompts" sprite="ps5_button_cross"></voffset></size>`).

## Adding your own prompts

You can add new device prompts in your project by creating a new InputDevicePromptDataFile
(Assets → Create → Input System Action Prompts → Input Device Prompt Data File), and add to your settings file under "Device Prompt Assets".

Make sure that the TextMeshPro Sprite asset containing your prompt icons is in the **"Resources/Sprite Assets/"** folder, or it wont be dynamically loaded. 

At the top of the asset, specify the name of the device that will be used - follow the examples of the included assets.

## How it works

On initial startup, it iterates through the InputActionAssets in the project to find all actions (eg "Player/Jump") and builds
a list of all bindings to that path (eg "<Gamepad>/buttonSouth" or "<Keyboard>/space").

When a tag is requested by a component, it will find the Input Device Prompt Data file for the active device, and see if
any of the bindings match any of the valid paths for that action (eg ```<Gamepad>/buttonSouth```). If one or more is found, it will
repace the tag with the matchinging prompt icon as a sprite (eg `````<sprite="PS5_Prompts" sprite="ps5_button_cross">`````).

It also tracks the last button press from connected devices and if a switch occurs (eg from keyboard to gamepad), it will
notify all components and change accordingly.

There is also a "default" device choice. On startup, no buttons have been pressed, so initial prompts must be selected. The settings
file will choose which device to prioritise before any button is pressed. If the active device is disconnected, it will attempt
to find a default device again.




