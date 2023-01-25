using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BlittingScript : MonoBehaviour
{
    public Renderer VidRendLeft;
    public Renderer VidRendRight;

    private Material _vidMatLeft;
    private Material _vidMatRight;

    private bool _activateBlit = false;
    private RenderTexture _stationLeft;

    [ExecuteInEditMode]

    private void Start()
    {
        _vidMatLeft = VidRendLeft.material;
        _vidMatRight = VidRendRight.material;
    }

    public void ActivateBlitting()
    {
        _activateBlit = true;
    }

    // Update is called once per frame
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(_activateBlit)
        {
            _stationLeft = new RenderTexture(src.width, src.height, 0);
            Graphics.Blit(src, _stationLeft, _vidMatLeft);
            Graphics.Blit(_stationLeft, dest, _vidMatRight);
        } else
        {
            Graphics.Blit(src, dest);
        }
    }
}
