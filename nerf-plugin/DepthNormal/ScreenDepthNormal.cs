using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ScreenDepthNormal : MonoBehaviour
{
    public Camera cam;
    public Material mat;
    public bool chooseDoverN = true; //D = true, N = false

    // Start is called before the first frame update
    private void Awake()
    {
        //cam = this.GetComponent<Camera>();
        //cam.depthTextureMode = DepthTextureMode.DepthNormals;
        //if (chooseDoverN)
        //{
        //    mat = new Material(Shader.Find("Hidden/Shader_Depth"));
        //}
        //else
        //{
        //    mat = new Material(Shader.Find("Hidden/Shader_Normal"));
        //}
    }


    private void OnPreRender()
    {
        //we pass the camera matrix before render. Matrix passed as global value.
        Shader.SetGlobalMatrix(Shader.PropertyToID("UNITY_MATRIX_IV"), cam.cameraToWorldMatrix);
        //Shader.SetGlobalMatrix("UNITY_MATRIX_IV", cam.cameraToWorldMatrix);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }

}
