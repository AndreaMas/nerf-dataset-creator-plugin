# Create synthetic datasets for NeRF with this Unity plugin!

A Unity plugin for easy creation of synthetic datasets for NeRF is presented. The plugin spawns cameras around a target, capturing images from them. A simple GUI is provided to help the user choose: which game object to target; how many cameras are spawned and in what configuration; how many images are wanted for training testing and validation. The output file contains the captured images, the ground-truth position and orientation of such cameras, depth and normal maps for final NeRF evaluation purposes. The output dataset file is identical to the Blender data-sets used in the original NeRF paper. 

![UnityCapture](https://user-images.githubusercontent.com/32450751/148696613-df457232-7c66-43be-a7bb-fe1f0ea95f48.png)

![Dataset Structure](https://user-images.githubusercontent.com/32450751/148696633-4c8b630e-e9a4-4aec-937e-7df52003a325.png)
