using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    public Camera Cam;
    public Transform Parent;

    private Vector3 _originalParentScale;
    private float _largestBounds;

    private void Start()
    {
        _originalParentScale = Parent.localScale; 
    }

    private float TallestInVector3(Vector3 vec)
    {
        return Mathf.Max(vec.x, vec.y, vec.z);
    }



    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(Cam.transform);

        if(_originalParentScale != Parent.localScale)
        {
            _largestBounds = TallestInVector3(Parent.localScale);
            this.transform.localScale = _originalParentScale*_largestBounds;
        }
    }
}
