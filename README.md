# Create synthetic datasets for NeRF with this Unity plugin!

![UnityCapture](https://user-images.githubusercontent.com/32450751/148696613-df457232-7c66-43be-a7bb-fe1f0ea95f48.png)

A Unity plugin for easy creation of synthetic datasets for NeRF is presented (Neural Radiance Fields: https://www.matthewtancik.com/nerf). 

The plugin simply spawns cameras around a target, capturing images from them (& additional data). The output dataset folder is identical to the Blender datasets used in the original NeRF paper. 

A simple GUI is provided to help the user choose: 
- which game object to target,
- how many cameras are spawned and in what configuration,
- how many images are wanted for training testing and validation. 

The output file contains:
- the captured images, 
- the extrinsics (position and orientation) and field-of-view of such cameras, 
- depth and normal maps for final NeRF evaluation purposes. 

To use, drag and drop the "nerf-plugin" folder into your Unity's project Assets folder. A new Unity window to create datasets can then be found in the Unity's Window tab (top-left). Be sure to create a new tag in unity called "ProCam" before use. A much more detailed guide and explanation of the inner workings can be found in the latex paper in this repository. 

Results that can be expected by NeRF using this tool: [NeRF result video](https://user-images.githubusercontent.com/32450751/134493567-9afd8f72-4be1-47af-a3c8-c2239ce79641.mp4)

Youtube tutorial: https://youtu.be/iZZh4I_UEBg

![Dataset Structure](https://user-images.githubusercontent.com/32450751/148696633-4c8b630e-e9a4-4aec-937e-7df52003a325.png)


