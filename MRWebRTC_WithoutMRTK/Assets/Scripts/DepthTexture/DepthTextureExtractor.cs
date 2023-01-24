using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class DepthTextureExtractor : MonoBehaviour
{
    [Tooltip("The Camera from which you want the Depth Informations")]
    public Camera Cam;
    [Tooltip("The Material with the DepthInfo Shader")]
    public Material Mat;
    [HideInInspector]public RenderTexture RTex;
    private Texture2D _sendTex;
    [HideInInspector]public byte[] JpgSample;
    [HideInInspector] public bool ShouldRenderDepth = false;

    private bool _isSettingDepth = false;

    private Texture2D _rTex2Tex2D;

    void Start()
    {
        Cam.depthTextureMode = DepthTextureMode.Depth;
        RTex = new RenderTexture(Cam.scaledPixelWidth, Cam.scaledPixelHeight, 0);
    }

    void Update()
    {
        if(ShouldRenderDepth)
        {
            if (!_isSettingDepth) StartCoroutine(GetDepthAsTexture());
            /*_sendTex = ToTexture2D(RTex);
            JpgSample = _sendTex.EncodeToJPG();
            _isSettingDepth = false;*/
        }
        
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, RTex, Mat);
        Graphics.Blit(source, destination);
    }

    Texture2D ToTexture2D(RenderTexture rTex)
    {
        Destroy(_rTex2Tex2D);
        _rTex2Tex2D = new Texture2D(Cam.scaledPixelWidth, Cam.scaledPixelHeight, TextureFormat.R8, false);
        RenderTexture.active = rTex;
        _rTex2Tex2D.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        _rTex2Tex2D.Apply();
        return _rTex2Tex2D;
    }

    IEnumerator GetDepthAsTexture()
    {
        _isSettingDepth = true;
        yield return new WaitForSeconds(0.00f);
        _sendTex = ToTexture2D(RTex);
        JpgSample = _sendTex.EncodeToJPG();
        _isSettingDepth = false;
    }
}