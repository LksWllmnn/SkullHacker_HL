using UnityEngine;
using UnityEditor.Media;
using System.Collections;
using Unity.Collections;

//[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class dt2 : MonoBehaviour
{
    public Camera _cam;
    public Material mat;

    public float DepthLevel = 3.0F;

    public RenderTexture _rTex;
    public Texture2D sendTex;

    public byte[] jpgSample;

    private byte[] m_byteCache = null;

    void Start()
    {
        _cam.depthTextureMode = DepthTextureMode.Depth;
        _rTex = new RenderTexture(_cam.scaledPixelWidth, _cam.scaledPixelHeight, 0);
    }

    void Update()
    {
        sendTex = toTexture2D(_rTex);
        jpgSample = sendTex.EncodeToJPG();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mat.SetFloat("_DepthLevel", DepthLevel);
        Graphics.Blit(source, _rTex, mat);
        Graphics.Blit(source, destination);
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(_cam.scaledPixelWidth, _cam.scaledPixelHeight, TextureFormat.R8, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    /*public byte[] GetRawTextureData(Texture2D texture)
    {
        NativeArray<byte> nativeByteArray = texture.GetRawTextureData<byte>();

        if (m_byteCache?.Length != nativeByteArray.Length)
        {
            m_byteCache = new byte[nativeByteArray.Length];
        }
        nativeByteArray.CopyTo(m_byteCache);

        return m_byteCache;
    }*/

    /*public byte[] deleteColums(byte[] originSize) //from 1920x1080 to 960x1080
    {
        byte[] result = new byte[originSize.Length/2];
        for (int i = 0; i < originSize.Length; i++)
        {
            if (i % 2 == 0)
            {
                if( i == 0)
                {
                    result[0] = originSize[i];
                } else
                {
                    result[i/2] = originSize[i];
                }
                
            }
        }
        return result;
    }

    public byte[] deleteRows(byte[] reducedOne) //from 960x1080 to 960x540
    {
        byte[] result = new byte[reducedOne.Length/2];
        for (int i = 0; i < reducedOne.Length; i++)
        {
            if (i % 1920 > 959)
            {
                if (i == 0)
                {
                    result[0] = reducedOne[i];
                }
                else
                {
                    result[i / 2] = reducedOne[i];
                }
            }
        }
        return result;
    }*/
}