# temp-unity-connection

Project example of how to add the GestureRecognitionPackage to a Unity project

## Obtaining the package

### Install TensorFlow Lite for Unity

* Without using OpenUPM-CLI 
  * Download the package from [OpenUPM](https://openupm.com/packages/com.github.asus4.tflite/)
  * With the project opened, click on the downloaded .unitypackage file. It will automatically open the import confirmation on Unity
  * Select "Import"
  
* Using OpenUPM-CLI
  * Install the Command Line Interface for OpenUPM on the terminal via npm:
  ```
  npm install -g openupm-cli
  ```
  Or via yarn:
  ```
  yarn global add openupm-cli
  ```
  * Go to your Unity project's directory and install the pacakge:
  ```
  openupm add com.github.asus4.tflite
  ```
  
### Install the Gesture Recognition Package
  * Download the latest version of the package from the [releases tab](https://github.com/danilosobral/temp-unity-connection/releases)
  * With the project opened, click on the downloaded .unitypackage file. It will automatically open the import confirmation on Unity
  * Select "Import"
  
## Using the package

To use this package, you will need to add three empty `GameObjects` to your scene.

### Image Classification 
Assign the script `ImageClassificationScript` to the first one. This will be responsible for connecting to the device's camera and running the inference, which will allow you to detect open or closed hands.

If you want to render the image captured inside your game, assign a `RawImage` to its `Camera Display` attribute.

By default, the video capture uses the parameters of `Frames to Record = 60` and ` Initial Frame = 120`, which means that the capture will start at the 120th game frame, smapling 60 frames. You are free to change these on the Inspector.

### Api Controller

Assign the script `ApiController` to the second `GameObject` you have added to your scene. It will be responsible for connecting to your API.
Assig the `Image Classification Game Object` that you have set up in the previous step to the `Api Controller`'s `ImageClassGameObject` attribute.
Finally, set the URL's to your custom endpoints for login and image uploading (video capturing).

### Events Manager
Assign the script `EventsManager` to the third `GameObject` you have added to your scene. This is responsible for signalling your game that the player's hand is opened or closed, as well as activating the `ApiController`, so it sends the requests for Login and imageUpload, when the game triggers it.

#### Events triggered by the game
* **Login**:
When your game needs to login to the back-end server, trigger the Login event, by using the following command, where `login` and `password` are `strings` and `remeber_login` is `bool`
```
EventsManager.instance.OnLoginTrigger(gameObject.GetInstanceID(), login, password, remember_login);
```

* **Upload Images**:
 When your game needs to send the video to the back-end server, trigger the uplaodImages event, by using the following command
```
EventsManager.instance.OnUploadImagesTrigger(gameObject.GetInstanceID());
```

#### Events triggered by the package
* **Hand movement detected**:
 When the package detects that the player has the hand opened or closed, it will trigger the MoveHand event. To detect it, add the following line to the `Start()` function.
```
EventsManager.instance.MoveHandTrigger += HandMovementDetected;
```

`HandMoveDetected` is a function on the same file as the previous line and will execute when the event gets triggered. An example implementation of it is given below.
```cs
private void HandMovementDetected(int id, Boolean isOpenHand) 
{
    if (isOpenHand)
    {
        // Do something when hand is opened
    } else
    {
        // Do something when hand is closed
    }
}
```

## Building the package

Because of the TensorFlow Lite Plugin's limitations, this pacakge is only suitable for Androids with ARM64 Architecture.

In order to build the project for Android, follow the steps:
* Select 'File' > 'Build Settings...'
* Select the Android Platform
* Press 'Player Settings...'
* This will open the 'Project Settings' Tab. With 'Player' selected, scroll until the 'Other Settings' panel and expand it
* In this panel, ensure that the 'Scripting Backend' property is set to 'IL2CPP'
* In this panel, ensure that the 'ARM64' target architecture is checked and 'ARMv7' is unchecked
* Close the 'Project Settings' Tab
* Press 'Switch Platform'
* Press 'Build and Run'
