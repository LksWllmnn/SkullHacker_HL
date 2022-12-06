using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class depthTexture : MonoBehaviour
{
    /*private CommandBuffer _commandBuffer;
    public Camera cam;
    private RenderBuffer depthBuffer;

    // Start is called before the first frame update
    void Start()
    {
        //https://forum.unity.com/threads/depth-buffer-and-rendertextures.485675/
        RenderTexture depthTexture = new RenderTexture(cam.scaledPixelWidth, cam.scaledPixelHeight, 24, RenderTextureFormat.Depth);

        RenderTexture renderTexture = new RenderTexture(cam.scaledPixelWidth, cam.scaledPixelHeight, 0);
        cam.depthTextureMode = DepthTextureMode.Depth;
        cam.SetTargetBuffers(renderTexture.colorBuffer, depthTexture.depthBuffer);

        Debug.Log(depthTexture.depthBuffer.GetNativeRenderBufferPtr());

        //_commandBuffer = new CommandBuffer();
        //_commandBuffer.RequestAsyncReadback()
         //depthBuffer = Graphics.activeDepthBuffer;

        //RenderTexture renTex = new RenderTexture()
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(depthBuffer.GetNativeRenderBufferPtr());
    }*/

    public Camera cam;
    public RenderTexture targetColorBuffer;
    public RenderTexture targetDepthBuffer;
    public Texture2D tex2D;

    public void Start()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;
        cam.clearFlags = CameraClearFlags.Skybox;

        if (targetColorBuffer == false)
        {
            targetColorBuffer = new RenderTexture(1024, 1024, 32, RenderTextureFormat.ARGB32);
            targetColorBuffer.Create();
        }
        if (targetDepthBuffer == false)
        {
            targetDepthBuffer = new RenderTexture(1024, 1024, 32, RenderTextureFormat.Depth);
            targetDepthBuffer.Create();
        }

        cam.SetTargetBuffers(targetColorBuffer.colorBuffer, targetDepthBuffer.depthBuffer);
        cam.Render();
        cam.targetTexture = null;

        if (tex2D == false)
        {
            tex2D = new Texture2D(1024, 1024, TextureFormat.RFloat, false);
        }

        tex2D.LoadRawTextureData(targetDepthBuffer.depthBuffer.GetNativeRenderBufferPtr(), 1024 * 1024 * 4);
        tex2D.Apply();

        
    }
}
