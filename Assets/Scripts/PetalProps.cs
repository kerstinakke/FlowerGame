using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalProps : MonoBehaviour
{   
    public Color petalColor;
    public Vector3 angle;
    public float width;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.color = petalColor;
        transform.localScale = new Vector3(width,1,0.05f);
        transform.localRotation = Quaternion.Euler(angle);
        transform.localPosition -= transform.up*0.5f;
    }

}
