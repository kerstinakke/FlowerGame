using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowerer : MonoBehaviour
{
    public Color color;
    public int petal_num=3;
    [SerializeField]
    private GameObject petal;

    // Start is called before the first frame update
    void Start()
    {
        float angle = 360 / petal_num;
        float width = 3f / petal_num;
        List<GameObject> petals = new List<GameObject>();
        for (int i = 0; i < petal_num; i++) {
            GameObject p = Instantiate(petal,transform);
            Petal_props properties = p.GetComponent<Petal_props>();
            properties.petal_color = color;
            properties.angle = new Vector3(0, 0, angle*i);
            properties.width = width;
            petals.Add(p);
        }
    }


}
