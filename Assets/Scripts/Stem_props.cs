using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stem_props : MonoBehaviour
{
    public float height=1;
    // Start is called before the first frame update
    void Start()
    {
        height /= 2f;
        transform.localScale = (new Vector3(0.1f, 1, 0.1f))*height;
        transform.position -= transform.up * (GetComponent<Renderer>().bounds.size.y / 2f);
        transform.position += transform.forward * GetComponent<Renderer>().bounds.size.x;
    }


}
