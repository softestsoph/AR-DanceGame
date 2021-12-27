# AR Dance Game
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


## License

All our code and modifications are licensed under the attached MIT License. 

We use some code and assets from:
- [This fork](https://github.com/Aviscii/azure-kinect-dk-unity) of the [azure-kinect-dk-unity repository](https://github.com/curiosity-inc/azure-kinect-dk-unity) (MIT License).
- [NativeWebSocket](https://github.com/endel/NativeWebSocket) (Apache-2.0 License). 
- [SMPL](https://smpl.is.tue.mpg.de/) (Creative Commons Attribution 4.0 International License). 
- [Space Robot Kyle](https://assetstore.unity.com/packages/3d/characters/robots/space-robot-kyle-4696) (Unity Extension Asset License). 
- [Lightweight human pose estimation](https://github.com/Daniil-Osokin/lightweight-human-pose-estimation-3d-demo.pytorch) (Apache-2.0 License). 
- [Official Sample Unity Body Tracking Application](https://github.com/microsoft/Azure-Kinect-Samples/tree/master/body-tracking-samples/sample_unity_bodytracking) (MIT License)

