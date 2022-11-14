using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTextInfo : MonoBehaviour
{
    public GameObject Manipulator;


    // Update is called once per frame
    void Update()
    {
        this.GetComponent<TextMesh>().text = "Scale: " + Manipulator.transform.localScale.x + "\nRot: " + Manipulator.transform.rotation.eulerAngles.x + "|" + Manipulator.transform.rotation.eulerAngles.y + "|" + Manipulator.transform.rotation.eulerAngles.z;
    }
}
