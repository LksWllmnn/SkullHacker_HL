using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateDistance : MonoBehaviour
{
    public Transform Object;
    public Transform Cam;

    [HideInInspector]public float ScaleFactor = 1;

    public Vector3 origiScale;

    private bool _isChecking = false;

    public Vector3 CamPos;

    public bool DualSystem;
    private float _scaleVar = 7.9f;

    private bool _closeToObject;
    private Transform _originParent;
    private Vector3 _originCamPos = new Vector3(0, 0, 7f);
    private Vector3 _originCamRot = new Vector3(0, -180, 0);

    private void Start()
    {
        Cam = this.Cam.transform;
        origiScale = transform.localScale;

        _originParent = this.transform.parent;
    }


    // Update is called once per frame
    void Update()
    {
        if(DualSystem)
        {
            Vector3 activeModelScaling = Object.localScale;

            if(Vector3.Distance(Cam.position, Object.position) < Mathf.Max(activeModelScaling.x, activeModelScaling.z))_closeToObject = true;
            else _closeToObject = false;

            if(_closeToObject)
            {
                this.transform.parent = Cam;
                this.transform.localPosition = _originCamPos;
                this.transform.localEulerAngles = _originCamRot;
                this.transform.localScale = origiScale * _scaleVar * Mathf.Tan(Mathf.Deg2Rad * 70.0f / 2);
            } else
            {
                this.transform.parent = _originParent;
                //Plane wird noch hinter die ausmaße der Objekts gesetzt, da das Objekt in 3d sonst immer so erscheint, als ob es leicht vor der tatsächlichen Position erscheint (Mathematik_und_Simulation_Kapitel-1.pdf, S.9/75)
                //if (!_isChecking) StartCoroutine(setNewOrientation());

                CamPos = new Vector3(Mathf.Floor(Cam.position.x * 100) / 100, Mathf.Floor(Cam.position.y * 1000) / 1000, Mathf.Floor(Cam.position.z * 1000) / 1000);
                this.transform.position = Object.position + (Vector3.Normalize(Object.position - Cam.position) * (Mathf.Sqrt(Object.localScale.x * Object.localScale.x + Object.localScale.y * Object.localScale.y + Object.localScale.z * Object.localScale.z) / 2));
                this.transform.LookAt(CamPos);

                //größe der plane anpassen anhand des abstands zum Objekt mithilfe der Kathete https://de.wikipedia.org/wiki/Rechtwinkliges_Dreieck und dann mal 2 da 2x rechtwinkliges dreieck ein gleichschenkliges ergeben
                this.transform.localScale = origiScale * ScaleFactor * Mathf.Tan(Mathf.Deg2Rad * 70.0f / 2);
                //this.transform.localScale = origiScale * ScaleFactor * Mathf.Tan(43.0f * Mathf.PI / 180);
            }
        } else
        {
            //Plane wird noch hinter die ausmaße der Objekts gesetzt, da das Objekt in 3d sonst immer so erscheint, als ob es leicht vor der tatsächlichen Position erscheint (Mathematik_und_Simulation_Kapitel-1.pdf, S.9/75)
            //if (!_isChecking) StartCoroutine(setNewOrientation());

            CamPos = new Vector3(Mathf.Floor(Cam.position.x * 100) / 100, Mathf.Floor(Cam.position.y * 1000) / 1000, Mathf.Floor(Cam.position.z * 1000) / 1000);
            this.transform.position = Object.position + (Vector3.Normalize(Object.position - Cam.position) * (Mathf.Sqrt(Object.localScale.x * Object.localScale.x + Object.localScale.y * Object.localScale.y + Object.localScale.z * Object.localScale.z) / 2));
            this.transform.LookAt(CamPos);

            //größe der plane anpassen anhand des abstands zum Objekt mithilfe der Kathete https://de.wikipedia.org/wiki/Rechtwinkliges_Dreieck und dann mal 2 da 2x rechtwinkliges dreieck ein gleichschenkliges ergeben
            this.transform.localScale = origiScale * ScaleFactor * Mathf.Tan(43.0f * Mathf.PI / 180);
        }
        
    }

    IEnumerator setNewOrientation()
    {
        _isChecking = true;
        yield return new WaitForSeconds(0.00f);
        CamPos = new Vector3(Mathf.Floor(Cam.position.x*100)/100, Mathf.Floor(Cam.position.y * 1000) / 1000, Mathf.Floor(Cam.position.z * 1000) / 1000);


        this.transform.position = Object.position + (Vector3.Normalize(Object.position - Cam.position) * (Mathf.Sqrt(Object.localScale.x * Object.localScale.x + Object.localScale.y * Object.localScale.y + Object.localScale.z * Object.localScale.z) / 2));
        this.transform.LookAt(CamPos);

        _isChecking = false;
    }
}
