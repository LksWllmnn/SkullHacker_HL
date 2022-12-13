using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateDistance : MonoBehaviour
{
    public Transform Object;
    public Transform Parent;

    [HideInInspector]public float ScaleFactor = 1;

    private Vector3 origiScale;

    private bool _isChecking = false;

    public Vector3 ParentPos;

    private void Start()
    {
        Parent = this.Parent.transform;
        origiScale = transform.localScale;
    }


    // Update is called once per frame
    void Update()
    {
        //Plane wird noch hinter die ausmaße der Objekts gesetzt, da das Objekt in 3d sonst immer so erscheint, als ob es leicht vor der tatsächlichen Position erscheint (Mathematik_und_Simulation_Kapitel-1.pdf, S.9/75)
        if (!_isChecking) StartCoroutine(setNewOrientation());

        //größe der plane anpassen anhand des abstands zum Objekt mithilfe der Kathete https://de.wikipedia.org/wiki/Rechtwinkliges_Dreieck und dann mal 2 da 2x rechtwinkliges dreieck ein gleichschenkliges ergeben
        this.transform.localScale = origiScale * ScaleFactor * Mathf.Tan(43.0f * Mathf.PI / 180);
    }

    IEnumerator setNewOrientation()
    {
        _isChecking = true;
        yield return new WaitForSeconds(0.01f);
        ParentPos = new Vector3(Mathf.Floor(Parent.position.x*100)/100, Mathf.Floor(Parent.position.y * 100) / 100, Mathf.Floor(Parent.position.z * 100) / 100);


        this.transform.position = Object.position + (Vector3.Normalize(Object.position - Parent.position) * (Mathf.Sqrt(Object.localScale.x * Object.localScale.x + Object.localScale.y * Object.localScale.y + Object.localScale.z * Object.localScale.z) / 2));
        this.transform.LookAt(ParentPos);

        _isChecking = false;
    }
}
