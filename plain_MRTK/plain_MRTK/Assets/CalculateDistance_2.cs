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
        //Plane wird noch hinter die ausmaße der Objekts gesetzt, da das Objekt in 3d sonst immer so erscheint, als ob es leicht vor der tatsächlichen Position erscheint (Mathematik_und_Simulation_Kapitel-1.pdf, S.9/75)
        this.transform.position = Object.position + (Vector3.Normalize(Parent.position - Object.position) * (Mathf.Sqrt(Object.localScale.x * Object.localScale.x + Object.localScale.y * Object.localScale.y + Object.localScale.z * Object.localScale.z)/2));
        this.transform.LookAt(Parent.position);
        //größe der plane anpassen anhand des abstands zum Objekt mithilfe der Kathete https://de.wikipedia.org/wiki/Rechtwinkliges_Dreieck und dann mal 2 da 2x rechtwinkliges dreieck ein gleichschenkliges ergeben
        this.transform.localScale = origiScale * (2 * Vector3.Distance(Parent.position, this.transform.position) *Mathf.Tan(43.0f*Mathf.PI/180));
    }
}
