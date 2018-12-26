using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowerer : MonoBehaviour
{
    public Color color;
    public int petal_num=3;
    public float height=1;
    [SerializeField]
    private GameObject petal;
    [SerializeField]
    private GameObject stem;

    // Start is called before the first frame update
    void Start()
    {
        // add petals
        float angle = 360 / petal_num;
        float width = 3f / petal_num;
        Transform petals = transform.Find("petals");        
        for (int i = 0; i < petal_num; i++) {
            GameObject p = Instantiate(petal,petals);
            Petal_props properties = p.GetComponent<Petal_props>();
            properties.petal_color = color;
            properties.angle = new Vector3(0, 0, angle*i);
            properties.width = width;
        }
        petals.localScale = (new Vector3(1, 1, 1)) * height / 3;

        // add stem
        GameObject s = Instantiate(stem, transform);
        s.GetComponent<Stem_props>().height = height;
        transform.position += transform.up * height;
    }


}
