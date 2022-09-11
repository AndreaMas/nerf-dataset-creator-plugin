# Create synthetic datasets for NeRF with this Unity plugin!

![UnityCapture](https://user-images.githubusercontent.com/32450751/148696613-df457232-7c66-43be-a7bb-fe1f0ea95f48.png)


## What is this?

A Unity Plugin for easy creation of synthetic datasets for NeRF is presented (Neural Radiance Fields: https://www.matthewtancik.com/nerf). 

The plugin simply spawns cameras around a target, capturing images from them (& additional data). The output dataset folder is identical to the Blender datasets used in the original NeRF paper. 

A simple GUI is provided to help the user choose: 
- which game object to target,
- how many cameras are spawned and in what configuration,
- how many images are wanted for training testing and validation. 

The output file contains:
- the captured images, 
- the extrinsics (position and orientation) and field-of-view of such cameras, 
- depth and normal maps for final NeRF evaluation purposes. 

## How to use

Youtube tutorial: https://youtu.be/iZZh4I_UEBg

To use, drag and drop the "nerf-plugin" folder into your Unity's project Assets folder. A new Unity window to create datasets can then be found in the Unity's Window tab (top-left). Be sure to create a new tag in unity called "ProCam" before use. A much more detailed guide and explanation of the inner workings can be found in the latex paper in this repository. 

## Expected Results

Results that can be expected by NeRF using this tool:

![expected-result-nerf-plugin](https://user-images.githubusercontent.com/32450751/189521972-1de0d74b-0453-4d62-822e-007c3e457688.gif)

Example of generated images and blender-like file structure:
![Dataset Structure](https://user-images.githubusercontent.com/32450751/148696633-4c8b630e-e9a4-4aec-937e-7df52003a325.png)


