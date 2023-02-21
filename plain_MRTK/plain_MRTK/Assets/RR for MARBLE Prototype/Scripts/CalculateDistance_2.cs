using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateDistance_2 : MonoBehaviour
{
    public Transform Object;
    public Transform Parent;

    private Vector3 origiScale;

    private void Start()
    {
        Parent = this.Parent.transform;
        origiScale = transform.localScale;
    }


    // Update is called once per frame
    void Update()
    {

        this.transform.position = Object.position + (Vector3.Normalize(Parent.position - Object.position) * (Mathf.Sqrt(Object.localScale.x * Object.localScale.x + Object.localScale.y * Object.localScale.y + Object.localScale.z * Object.localScale.z)/2));
        this.transform.LookAt(Parent.position);

        this.transform.localScale = origiScale * (2 * Vector3.Distance(Parent.position, this.transform.position) *Mathf.Tan(43.0f*Mathf.PI/180));
    }
}
