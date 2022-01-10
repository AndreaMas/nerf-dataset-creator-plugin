using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;


public class NerfDatasetCreator : EditorWindow
{
    public bool useProCam = false;
    public bool transparentBkgd = true;

    public int numCameras;

    public float camHeight = 1f;
    public float camGroundDistance = 4f;
    public float pitch = 0f;

    public bool setSnapResAsScreen = false;
    public int snapResWidth = 800;
    public int snapResHeight = 800;

    public bool isValData = false;
    public bool isTestData = false;

    public int numViewsTrain = 100;
    public int numViewsVal = 100;
    public int numViewsTest = 500;


    public float cameraAngle = 0.0f;
    public float cameraRadious = 0.0f;
    public float cameraAngleIncrement = 0.0f;

    public GameObject cameraTemp;
    public GameObject target;

    public List<GameObject> cameraList = new List<GameObject>();

    public string[] options = new string[] { "Halo", "Sphere", "Semisphere" };
    public int indexTrain = 3;
    public int indexVal = 3;
    public int indexTest = 0;

    public float p1 = 1f;
    public float p2 = 1f;
    public float p3 = 0f;




    // EDITOR //


    [MenuItem("Window/NeRF Dataset Creator")]
    public static void ShowWindow()
    {
        GetWindow<NerfDatasetCreator>("NeRF Dataset Creator");
    }

    private void OnGUI()
    {
        // PART: Spawn camera
        EditorGUILayout.Space();
        GUILayout.Label("Spawn camera pointing to target.", EditorStyles.boldLabel);
        target = EditorGUILayout.ObjectField("target: ", target, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Spawn dummy camera"))
        {
            OneProCam();
        }

        // PART: Tweak position
        EditorGUILayout.Space();
        GUILayout.Label("Choose cam position, tweak its parameters in inspector.", EditorStyles.boldLabel);
        camGroundDistance = EditorGUILayout.FloatField("distance: ", camGroundDistance);
        camHeight = EditorGUILayout.FloatField("height: ", camHeight);
        //pitch = EditorGUILayout.FloatField("pitch: ", pitch);
        if (GUILayout.Button("Update camera position"))
        {
            CamPositionRotationFocus();
        }

        //PART: Save cam as prefab
        EditorGUILayout.Space();
        GUILayout.Label("Save in /Resources a prefab of this camera.", EditorStyles.boldLabel);

        if (GUILayout.Button("Save camera as prefab"))
        {
            MakeCameraPrefab(cameraTemp);
            UpdateCameraAngleAndRadious();
        }

        // PART: choose camera resolution
        EditorGUILayout.Space();
        GUILayout.Label("Snapshot resolution. ", EditorStyles.boldLabel);
        
        snapResWidth = EditorGUILayout.IntField("width (800 suggested): ", snapResWidth);
        snapResHeight = EditorGUILayout.IntField("height (800 suggested): ", snapResHeight);
        //setSnapResAsScreen = EditorGUILayout.Toggle("Use the screen resolution: ", setSnapResAsScreen);

        // PART: Test spawn positioning of cameras
        EditorGUILayout.Space();
        GUILayout.Label("Test various camera spawn configurations.", EditorStyles.boldLabel);
        
        numCameras = EditorGUILayout.IntField("Num of views: ", numCameras);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Halo"))
        {
            SpawnCameraArray();
        }
        if (GUILayout.Button("Sphere"))
        {
            SpawnCameraSphere();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Semisphere"))
        {
            SpawnCameraSemiSphere();
        }

        //PART: Advanced Semisphere parameters
        EditorGUILayout.Space();
        GUILayout.Label("Advanced semisphere parameters.", EditorStyles.boldLabel);
        p1 = EditorGUILayout.FloatField("semisphere top 1-0: ", p1);
        p2 = EditorGUILayout.FloatField("semisphere bottom 1-0: ", p2);
        p3 = EditorGUILayout.FloatField("semisphere ratio (0=gold): ", p3);

        // PART 6: Automatically create dataset
        EditorGUILayout.Space();
        GUILayout.Label("Generate dataset.", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        numViewsTrain = EditorGUILayout.IntField("Num views Train: ", numViewsTrain);
        indexTrain = EditorGUILayout.Popup(indexTrain, options);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        numViewsVal = EditorGUILayout.IntField("Num views Val: ", numViewsVal);
        indexVal = EditorGUILayout.Popup(indexVal, options);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        numViewsTest = EditorGUILayout.IntField("Num views Test: ", numViewsTest);
        indexTest = EditorGUILayout.Popup(indexTest, options);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Create dataset"))
        {
            CreateWholeDataset();
        }

    }



    // FUNCTIONS //

    // Create a new camera, or if prefab exists spawn that
    private void OneProCam()
    {
        DestroyPreviousCameras();
        if (target == null) { Debug.Log("Set target first !"); return; }

        if (System.IO.File.Exists(Application.dataPath + "/Resources/ProCamera.prefab"))
        {
            cameraTemp = (GameObject)PrefabUtility.InstantiatePrefab(LoadPrefabFromFile("ProCamera"));
        }
        else
        {
            cameraTemp = CreateProCamera();
        }

        CamPositionRotationFocus();
    }


    // Create a new camera gameobject (add procam components)
    private GameObject CreateProCamera()
    {
        GameObject go = new GameObject();
        go.name = "ProCamera";
        go.gameObject.tag = "ProCam";
        go.AddComponent<Camera>();
        if (transparentBkgd)
        {
            Camera goCam = go.GetComponent<Camera>();
            goCam.clearFlags = CameraClearFlags.SolidColor;
            goCam.backgroundColor = Color.clear;
            goCam.farClipPlane = 50;
        }
        return go;
    }


    // Handle camera spawn position and rotation (and focus if procam)
    private void CamPositionRotationFocus()
    {
        Vector3 pos = target.transform.position + new Vector3(camGroundDistance, camHeight, 0f);
        cameraTemp.transform.position = pos;
        cameraTemp.transform.LookAt(target.transform.position + Vector3.up * pitch);
    }


    // Save as prefab the input gameobject
    private void MakeCameraPrefab(GameObject cam)
    {
        // Create prefab folder
        if(!System.IO.Directory.Exists(Application.dataPath + "/Resources")) {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        PrefabUtility.SaveAsPrefabAssetAndConnect(
                    cam,
                    "Assets/Resources/" +
                    cam.name + ".prefab",
                    InteractionMode.AutomatedAction);
    }

    
    private void UpdateCameraAngleAndRadious()
    {
        cameraRadious = Mathf.Sqrt(Mathf.Pow(camHeight - pitch,2) + Mathf.Pow(camGroundDistance,2));
        cameraAngle = 0;
        if (cameraRadious == 0) { Debug.Log("WARNING:Radious betw target and camera is = 0"); return; }
        cameraAngle = Mathf.Acos(camGroundDistance / cameraRadious);
        Debug.Log("Camera Radious is: " + cameraRadious + "; Camera angle is: " + cameraAngle);
    }


    // Destroy previous cameras
    private void DestroyPreviousCameras()
    {
        if (cameraList.Count != 0)
        {
            cameraList.Clear();
        }
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("ProCam");
        foreach (GameObject proCam in taggedObjects)
        {
            DestroyImmediate(proCam);
        }
    }


    // Load camera Prefab from file
    private GameObject LoadPrefabFromFile(string filename)
    {
        GameObject loadedObject = (GameObject)Resources.Load(filename);
        if (loadedObject == null)
        {
            Debug.Log("... no prefab found ...");
            return null;
        }
        return loadedObject;
    }


    private void CreateWholeDataset()
    {
        // Create Training data
        numCameras = numViewsTrain;
        isValData = false;
        isTestData = false;
        switch (indexTrain)
        {
            case 0:
                SpawnCameraArray();
                break;
            case 1:
                SpawnCameraSphere();
                break;
            case 2:
                SpawnCameraSemiSphere();
                break;
            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
        WriteCamPositionsTxtJson(cameraList, cameraAngle);
        TakeSnapshot();

        // Create Validation data
        numCameras = numViewsVal;
        isValData = true;
        isTestData = false;
        switch (indexVal)
        {
            case 0:
                SpawnCameraArray();
                break;
            case 1:
                SpawnCameraSphere();
                break;
            case 2:
                SpawnCameraSemiSphere();
                break;
            default:
                Debug.LogError("Unrecognized Option");
                break;
        };
        WriteCamPositionsTxtJson(cameraList, cameraAngle);
        TakeSnapshot();

        // Create Test data
        numCameras = numViewsTest;
        isValData = false;
        isTestData = true;
        switch (indexTrain)
        {
            case 0:
                SpawnCameraArray();
                break;
            case 1:
                SpawnCameraSphere();
                break;
            case 2:
                SpawnCameraSemiSphere();
                break;
            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
        WriteCamPositionsTxtJson(cameraList, cameraAngle);
        TakeSnapshot();
    }


    // Spawn cameras on a halo around target
    private void SpawnCameraArray()
    {
        DestroyPreviousCameras();
        float arcRad = 2f * Mathf.PI / numCameras;
        GameObject cam;
        for (int i = 0; i < numCameras; i++)
        {
            //Instantiate camera prefab
            GameObject loadedPrefabResource = LoadPrefabFromFile("ProCamera");
            Vector3 pos = target.transform.position + new Vector3(camGroundDistance, camHeight, 0f);
            cam = Instantiate(loadedPrefabResource, pos, Quaternion.identity);

            // Move camera around target
            float currentYawn = i * arcRad;
            currentYawn = currentYawn * 180 / Mathf.PI;
            cam.transform.RotateAround(target.transform.position, Vector3.up, currentYawn);
            cam.transform.LookAt(target.transform.position + Vector3.up * pitch);

            // Add to list
            cameraList.Add(cam);
        }
    }


    // Spawn cameras on a sphere around target
    private void SpawnCameraSphere()
    {
        DestroyPreviousCameras();
        GameObject cam;

        for (int i = 0; i < numCameras; i++)
        {
            //Instantiate camera prefab
            GameObject loadedPrefabResource = LoadPrefabFromFile("ProCamera");
            Vector3 initialPos = target.transform.position + Vector3.up * pitch;
            cam = Instantiate(loadedPrefabResource, initialPos, Quaternion.identity);

            // Place camera on sphere around target
            Vector3[] positions = PointsInSphere();
            cam.transform.position += positions[i];
            cam.transform.LookAt(target.transform.position + Vector3.up * pitch);

            // Add to list
            cameraList.Add(cam);
        }
    }


    // Spawn cameras on a semi-sphere around target
    private void SpawnCameraSemiSphere()
    {
        DestroyPreviousCameras();
        GameObject cam;

        for (int i = 0; i < numCameras; i++)
        {
            //Instantiate camera prefab
            GameObject loadedPrefabResource = LoadPrefabFromFile("ProCamera");
            Vector3 initialPos = target.transform.position + Vector3.up * pitch;
            cam = Instantiate(loadedPrefabResource, initialPos, Quaternion.identity);

            // Place camera on sphere around target
            Vector3[] positions = PointsInSemiSphere();
            cam.transform.position += positions[i];
            cam.transform.LookAt(target.transform.position + Vector3.up * pitch);

            // Add to list
            cameraList.Add(cam);
        }
    }


    private Vector3[] PointsInSphere()
    {
        Vector3[] positions = new Vector3[numCameras];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numCameras; i++)
        {
            float t = (float)i / numCameras;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            //z and y have been inverted
            float x = camGroundDistance * Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = camGroundDistance * Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = camGroundDistance * Mathf.Cos(inclination);
            positions[i] = new Vector3(x, y, z);
        }
        return positions;
    }


    private Vector3[] PointsInSemiSphere()
    {
        Vector3[] positions = new Vector3[numCameras];
        float ratio = p3;
        if (ratio == 0)
        {
            ratio = (1 + Mathf.Sqrt(5)) / 2; //golden ratio
        }
        float angleIncrement = Mathf.PI * 2 * ratio;
        cameraAngleIncrement = angleIncrement;

        for (int i = 0; i < numCameras; i++)
        {
            float t = (float)i / numCameras;
            // Standard half sphere
            //float inclination = Mathf.Acos(1 - t);

            // Mix betw halo & half sphere
            float inclination = Mathf.Acos(p1 - p2 * t);
            float azimuth = angleIncrement * i;

            float x = camGroundDistance * Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float z = camGroundDistance * Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float y = camGroundDistance * Mathf.Cos(inclination);
            // note z and y inverted
            positions[i] = new Vector3(x, y, z);
        }
        return positions;
    }


    // store on txt/json file camera positions (as transform matrices)
    private void WriteCamPositionsTxtJson(List<GameObject> _cameraList, float _rotation)
    {
        CreateDatabaseFolders();
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        string trainTestOrVal = "";
        string fileName = "";
        if (isValData == true)
        {
            trainTestOrVal = "val";
            fileName = "/transforms_val.json";
        } else if ( isTestData == true)
        {
            trainTestOrVal = "test";
            fileName = "/transforms_test.json";
        } else
        {
            trainTestOrVal = "train";
            fileName = "/transforms_train.json";
        }

        string text;
        Camera camComp = _cameraList[0].GetComponent<Camera>();
        float cameraFoV = camComp.fieldOfView * Mathf.PI / 180f;
        text = String.Format(culture, "{{ \"camera_angle_x\": {0}, \"frames\": [", cameraFoV);

        for (int i = 0; i < _cameraList.Count; i++)
        {
            camComp = _cameraList[i].GetComponent<Camera>();
            Matrix4x4 m = camComp.cameraToWorldMatrix;
            text += String.Format(culture, " {{ \"file_path\": \"./{0}/r_{1}\", \"rotation\": {2}, \"transform_matrix\": ", trainTestOrVal, i, cameraAngleIncrement);
            text += String.Format(culture,
                "[ [ {0:##0.0################}, {1:##0.0################}, {2:##0.0################}, {3:##0.0################} ], " +
                "[ {4:##0.0################}, {5:##0.0################}, {6:##0.0################}, {7:##0.0################} ], " +
                "[ {8:##0.0################}, {9:##0.0################}, {10:##0.0################}, {11:##0.0################} ], " +
                "[ {12:##0.0################}, {13:##0.0################}, {14:##0.0################}, {15:##0.0################} ] ] }}",
                m.GetRow(0).x, m.GetRow(0).y, m.GetRow(0).z, m.GetRow(0).w,
                m.GetRow(2).x, m.GetRow(2).y, m.GetRow(2).z, m.GetRow(2).w,
                m.GetRow(1).x, m.GetRow(1).y, m.GetRow(1).z, m.GetRow(1).w,
                m.GetRow(3).x, m.GetRow(3).y, m.GetRow(3).z, m.GetRow(3).w);

            //"[ [ {0}, {1}, {2}, {3} ], [ {4}, {5}, {6}, {7} ], [ {8}, {9}, {10}, {11} ], [ {12}, {13}, {14}, {15} ] ] }}",
            //:##0.0################

            if (i != _cameraList.Count - 1)
            {
                text += ", ";
            } else {
                text += " ] }";
            }
        }
        File.AppendAllText(Application.dataPath + "/Dataset" + fileName, text);
    }


    private void CreateDatabaseFolders()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/Dataset"))
        {
            AssetDatabase.CreateFolder("Assets", "Dataset");
        }
        if (!System.IO.Directory.Exists(Application.dataPath + "/Dataset/train"))
        {
            AssetDatabase.CreateFolder("Assets/Dataset", "train");
        }
        if (!System.IO.Directory.Exists(Application.dataPath + "/Dataset/val"))
        {
            AssetDatabase.CreateFolder("Assets/Dataset", "val");
        }
        if (!System.IO.Directory.Exists(Application.dataPath + "/Dataset/test"))
        {
            AssetDatabase.CreateFolder("Assets/Dataset", "test");
        }

    }

    // Iterates over camera array/sphere/semisphere and calls snapshot
    private void TakeSnapshot()
    {

        if (setSnapResAsScreen)
        {
            snapResWidth = Screen.width;
            snapResHeight = Screen.height;
        }

        for (int i = 0; i < cameraList.Count; i++)
        {
            string snapshotName = SnapshotName(i, 0);
            ActualSnapshot(cameraList[i], snapshotName);
            
            if (isTestData)
            {
                DepthNormalSnapshot(cameraList[i], i);
            }
        }
        Debug.Log("Snapshots taken!");
    }


    private void DepthNormalSnapshot(GameObject go, int i)
    {
        
        Camera cam = go.GetComponent<Camera>();
        go.AddComponent<ScreenDepthNormal>();
        ScreenDepthNormal dn = go.GetComponent<ScreenDepthNormal>();
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
        string snapshotName;

        dn.mat = new Material(Shader.Find("Hidden/Shader_Depth"));
        snapshotName = SnapshotName(i, 1);
        ActualSnapshot(go, snapshotName);

        dn.mat = new Material(Shader.Find("Hidden/Shader_Normal"));
        snapshotName = SnapshotName(i, 2);
        ActualSnapshot(go, snapshotName);

        DestroyImmediate(dn);
        cam.depthTextureMode = DepthTextureMode.None;
    }


    private void ActualSnapshot(GameObject go, string snapshotName)
    {
        Camera cam = go.GetComponent<Camera>();

        // Swap if you dont need transparent background
        //cam.targetTexture = new RenderTexture(snapResWidth, snapResHeight, 24);
        cam.targetTexture = new RenderTexture(snapResWidth, snapResHeight, 32);

        // 1. crerate a texture 2d, to hold image info
        // Swap if you dont need transparent background
        //Texture2D snapshot = new Texture2D(snapResWidth, snapResHeight, TextureFormat.RGB24, false);
        Texture2D snapshot = new Texture2D(snapResWidth, snapResHeight, TextureFormat.ARGB32, false);

        // 2. tell unity that we want to render from our snapshot camera
        cam.Render();
        RenderTexture.active = cam.targetTexture;

        // 3. take the pixels from the snapshot camera and put them in the created texture
        snapshot.ReadPixels(new Rect(0, 0, snapResWidth, snapResHeight), 0, 0);

        // 4. put the texture in a png file that we can store somewhere in our pc. PNG stores info in bites
        byte[] bytes = snapshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(snapshotName, bytes);
    }


    private string SnapshotName(int i, int nameType)
    {
        string s = "";
        if (!isTestData && !isValData)
        {
            s = string.Format("{0}/Dataset/Train/r_{1}.png", Application.dataPath, i);
        }
        if (isValData)
        {
            s = string.Format("{0}/Dataset/Val/r_{1}.png", Application.dataPath, i);
        }
        if (isTestData)
        {
            if (nameType == 0)
            {
                s = string.Format("{0}/Dataset/Test/r_{1}.png", Application.dataPath, i);
            }
            if (nameType == 1)
            {
                s = string.Format("{0}/Dataset/Test/r_{1}_depth_0001.png", Application.dataPath, i);
            }
            if (nameType == 2)
            {
                s = string.Format("{0}/Dataset/Test/r_{1}_normal_0001.png", Application.dataPath, i);
            }
        }

        return s;
    }

}
