# Contributing to Input System Action Prompts

## Steps

1. **Fork the Repository**
    - Navigate to the GitHub repository and click **Fork** to create a copy in your GitHub account.
    - Clone your forked repository locally using:
      ```bash
      git clone https://github.com/your-username/InputSystemActionPrompts.git
      ```
    - Replace `your-username` with your GitHub username.


2. **Create or Use an Existing Unity Project**
    - If you don’t already have a Unity project:
        - Open Unity Hub and create a new project.


3. **Add the Package to Your Unity Project**
    - Locate your Unity project folder.
    - Open the `Packages/manifest.json` file in a text editor.
    - Add a reference to the cloned package repository by specifying its relative file path. For example:
      ```json
      {
        "dependencies": {
          "com.simonoliver.inputsystemactionprompts": "file:../../../InputSystemActionPrompts"
        }
      }
      ```
    - Adjust the relative path (`../../../InputSystemActionPrompts`) to point to the location of the cloned repository on your machine.


4. **Open the Project in Unity**
    - Open the Unity project in Unity Editor.
    - The package should appear under the **Packages** section in the Project window.
    - Test the package to ensure it integrates correctly.


5. **Make Your Changes**
    - Edit scripts, assets, or other files within the package directory in your Unity project.
    - Test your changes thoroughly to ensure they work as expected.


6. **Commit and Push Your Changes**
    - In your cloned repository folder, stage and commit your changes:
      ```bash
      git add .
      git commit -m "Describe your changes here"
      ```
    - Push your changes to your forked repository:
      ```bash
      git push origin main
      ```


7. **Open a Pull Request (PR)**
    - Go to your forked repository on GitHub.
    - Click **Compare & Pull Request**.
    - Provide a clear description of your changes and why they are necessary.
    - Submit the pull request for review.

