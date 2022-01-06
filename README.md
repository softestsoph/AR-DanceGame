# AR Dance Game
## Explanation of Separate Submission
This repository is a copy of the [original repository](https://github.com/softestsoph/motion-instructor) that was being worked on with the whole group, with some additional work done as final polishing. The decision to split from the group was made after the other team members repeatedly did not deliver the work they promised, with the first half of the semester being almost exclusively worked on by myself, and some of them stating after the project presentation that they would no longer work on the project. The report has a section on the distribution of tasks, but you can also see the pull request history on the old repository to confirm my statements.

## Environment
As of December, 2021, this repository has been tested under the following environment:
- Windows 10 Education (10.0.19042 Build 19042)
- [All tools required to develop on the HoloLens](https://docs.microsoft.com/en-us/windows/mixed-reality/install-the-tools)
- Unity 2020.3.19f1

 A dedicated CUDA compatible graphics card is necessary, NVIDIA GEFORCE GTX 1070 or better. For more information consult the 
 [official BT SDK hardware requirements](https://docs.microsoft.com/en-us/azure/kinect-dk/system-requirements). 
 
## Project Structure
All Scenes can be found directly in the Asset Folder, and any relevant code can be found in the Scripts folder. Inside the Scripts folder, the files are separated by application. There is also a folder called Obsolete containing code from the old project that was not reused.
The main script for the dance scene is the DanceManager, and can be found directly inside the Scripts folder.
#### Calibration
The script used for the initial Scene Setup is found inside the Calibration folder.
#### Dance Editor
Anything to do with the Dance Editor and Recording Scene are found in the DanceEditor folder.
#### Data
Any data object related scripts such as the definitions for Dance Data and ScriptableObjects can be found in the Data Folder.
#### Display
Scripts used for visualization, such as moving Avatars or Displaying Scores are found in the Display folder.
#### Menus
Menu related helper scripts are found inside the Menus folder.
#### Pose Getting
Anything related to aquiring Body Pose Data, be it from a Kinect or a File, is located inside the PoseGetting Folder. The filtering for KinectData is also found here.
#### Scoring
The main Scoring Script is ScoringManager inside the Scoring Folder, but some functions for computation are located inside ScoringUtils for better readability.

## Get Started
1. Clone this repository.
2. Open the `unity` folder as a Unity project, with `Universal Windows Platform` as the build platform. It might take a while to fetch all packages.
3. Use the Mixed Reality Feature Tool to install:
    - Mixed Reality Toolkit Examples v2.7.2
    - Mixed Reality Toolkit Extensions v2.7.2
    - Mixed Reality Toolkit Foundation v2.7.2
    - Mixed Reality Toolkit Standard Assets v2.7.2
    - Mixed Reality Toolkit Tools v2.7.2
    - Mixed Reality OpenXR Plugin v1.1.1
5. Setup the Azure Kinect Libraries: (same as [Sample Unity Body Tracking Application](https://github.com/microsoft/Azure-Kinect-Samples/tree/master/body-tracking-samples/sample_unity_bodytracking))
    1. Upgrade Microsoft.Azure.Kinect.Sensor Package to the newest Version (v1.4.1)
    2. Manually install the Azure Kinect Body Tracking SDK v1.1.0 [Link](https://docs.microsoft.com/en-us/azure/kinect-dk/body-sdk-download)
    4. Get the NuGet packages of libraries:
        - Open the Visual Studio Solution (.sln) associated with this project. You can create one by opening a csharp file in the Unity Editor.
        - In Visual Studio open: Tools->NuGet Package Manager-> Package Manager Console
        - When Prompted to get missing packages, click confirm
    5. Move libraries to correct folders:
        - Execute the file `unity/MoveLibraryFile.bat`. You should now have library files in `unity/` and in the newly created `unity/Assets/Plugins`.
    6. Add these libraries to the root directory (contains the assets folder)
       From Azure Kinect Body Tracking SDK\tools\
        - cudnn64_8.dll
        - cudnn64_cnn_infer64_8.dll
        - cudnn64_ops_infer64_8.dll
        - cudart64_110.dll
        - cublas64_11.dll
        - cublasLt64_11.dll
        - onxxruntime.dll
        - dnn_model_2_0_op11.onnx
6. Open `Assets/StartMenu` in the Unity Editor.
7. When prompted to import TextMesh Pro, select `Import TMP Essentials`. You will need to reopen the scene to fix visual glitches.
8. (Optional) Connect to the HoloLens with [Holographic Remoting](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Tools/HolographicRemoting.html#connecting-to-the-hololens-with-wi-fi) using the `Holographic Remoting for Play Mode` in Unity.
Otherwise the scene will only play in the editor.
9. Click play inside the Unity editor.


## How to use
Use the UI to navigate in the application. This can also be done in the editor, consult the 
[MRTK In-Editor Input Simulation](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSimulation/InputSimulationService.html)
page to see how.

## Demo Video Explanation
Two demo videos can be found here: [Demo Videos](https://drive.google.com/drive/folders/1dV66g14-gO3e55sY8MSQsrKDcKEhm505?usp=sharing)
Unfortunately, due to not having access to the devices, I cannot provide a demo video of an entire walkthrough of the application. These were recorded by a teammate, but they did not deliver on the requested video. I will explain here what can be seen in the demo videos and what is missing from them.
#### SceneSetup
The first video shows how the scene can be set up. The player takes the three cubes, red for teacher, green for kinect and blue for player, and places them where they should be in the room. The assignment is explained in the hand menu, where there is also an option to add more teacher avatars. Unfortunately this feature is not showcased in the video. What would happen is an additional red cube would spawn at the players hand, and they could then place it. They can also remove teachers, with the latest added being the first removed, and one teacher being required. Once calibration is done, the player can confirm in the hand menu and return to the main menu.
#### Dance Scene
Unfortunately, the scene setup in this video does not correspond to the one in the previous video. The dance scene consists of the teacher avatar, the score display and the video cube. With a correct scene setup, the video cube and score display show up overtop the kinect camera. In this video however they were not placed correctly, so they are harder to see and not in the field of view a lot of the time. Nevertheless, you can still see the teacher avatar showing the dance and the score being displayed. At the end of the dance, the final score is displayed in an end screen.
#### Things missing from the videos
Apart from the things already mentioned like correct scene setup and the multiple teacher support, the main dance game is shown almost from start to finish through these two videos. What is not shown are the tools, those being the recording scene and the editor for dances. The recording scene works similar to the dance scene, but it only has a video cube and an avatar reflecting the users movement. The editor was not functional when I split from the group so I cannot say what it looks like.

## License

All our code and modifications are licensed under the attached MIT License. 

We use some code and assets from:
- [This fork](https://github.com/Aviscii/azure-kinect-dk-unity) of the [azure-kinect-dk-unity repository](https://github.com/curiosity-inc/azure-kinect-dk-unity) (MIT License).
- [NativeWebSocket](https://github.com/endel/NativeWebSocket) (Apache-2.0 License). 
- [SMPL](https://smpl.is.tue.mpg.de/) (Creative Commons Attribution 4.0 International License). 
- [Space Robot Kyle](https://assetstore.unity.com/packages/3d/characters/robots/space-robot-kyle-4696) (Unity Extension Asset License). 
- [Lightweight human pose estimation](https://github.com/Daniil-Osokin/lightweight-human-pose-estimation-3d-demo.pytorch) (Apache-2.0 License). 
- [Official Sample Unity Body Tracking Application](https://github.com/microsoft/Azure-Kinect-Samples/tree/master/body-tracking-samples/sample_unity_bodytracking) (MIT License)

