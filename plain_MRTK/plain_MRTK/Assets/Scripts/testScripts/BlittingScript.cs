using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BlittingScript : MonoBehaviour
{
    public Renderer vidRend_Left;
    public Renderer vidRend_Right;

    private Material vidMatLeft;
    private Material vidMatRight;

    private bool activateBlit = false;
    private RenderTexture stationLeft;

    [ExecuteInEditMode]

    private void Start()
    {
        vidMatLeft = vidRend_Left.material;
        vidMatRight = vidRend_Right.material;
    }

    public void activateBlitting()
    {
        activateBlit = true;
        Debug.Log("start blitting");
    }
    

    // Update is called once per frame
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        
        if(activateBlit)
        {
            stationLeft = new RenderTexture(src.width, src.height, 0);
            Graphics.Blit(src, stationLeft, vidMatLeft);
            Graphics.Blit(stationLeft, dest, vidMatRight);

            
        } else
        {
            Graphics.Blit(src, dest);
        }
        

    }
}
