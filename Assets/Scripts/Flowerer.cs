using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowerer : MonoBehaviour
{
    public Color color;
    public int petalNum = 3;
    public float height = 1;
    [SerializeField]
    private GameObject petal;
    [SerializeField]
    private GameObject stem;
    public bool adult { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        // add petals
        float angle = 360 / petalNum;
        float width = 3f / petalNum;
        Transform petals = transform.Find("petals");        
        for (int i = 0; i < petalNum; i++) {
            GameObject p = Instantiate(petal,petals);
            PetalProps properties = p.GetComponent<PetalProps>();
            properties.petalColor = color;
            properties.angle = new Vector3(0, 0, angle*i);
            properties.width = width;
        }
        petals.localScale = (new Vector3(1, 1, 1)) * height / 3;

        // add stem
        GameObject s = Instantiate(stem, transform);
        s.GetComponent<StemProps>().height = height;
        transform.localPosition += transform.up * (height+(height*transform.parent.localScale.y-height)/2);

    }

    public void Randomize() {
        color = new Color(Random.Range(0,255)/255f, Random.Range(0, 255)/255f, Random.Range(0, 255)/255f);
        height = Random.Range(8, 20) / 8f;
        petalNum = Random.Range(3, 9);
    }

    
}
