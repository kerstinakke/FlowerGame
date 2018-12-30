using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StemProps : MonoBehaviour
{
    public float height=1;
    // Start is called before the first frame update
    void Start()
    {
        height /= 2f;
        transform.localScale = (new Vector3(0.1f, 1, 0.1f))*height;
        transform.localPosition -= transform.up * (GetComponent<Renderer>().bounds.size.y / 2f);
        transform.localPosition += transform.forward * GetComponent<Renderer>().bounds.size.x / 2f;
    }


}
