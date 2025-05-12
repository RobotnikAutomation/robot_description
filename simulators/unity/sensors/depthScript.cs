using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class depthScript : MonoBehaviour
{
    public Material material;
    public int height= 1080;
	public int width = 1080;
	private Camera cam;
	private RenderTexture rt;

    void Start()
    {
        cam = GetComponent<Camera>(); 
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage (RenderTexture source, RenderTexture dest){
		Graphics.Blit(source, dest, material);
	}

    void Update()
    {
        
    }
}
