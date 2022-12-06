using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    public Camera cam;
    public Transform parent;

    private Vector3 originalParentScale;
    private float largestBounds;

    private void Start()
    {
        originalParentScale = parent.localScale; 
    }

    private float TallestInVector3(Vector3 vec)
    {
        return Mathf.Max(vec.x, vec.y, vec.z);
    }



    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(cam.transform);

        if(originalParentScale != parent.localScale)
        {
            largestBounds = TallestInVector3(parent.localScale);
            this.transform.localScale = originalParentScale*largestBounds;
        }
    }
}
