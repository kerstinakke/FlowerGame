using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowerer : MonoBehaviour
{
    private GeneCombiner.Gene redGene, blueGene, greenGene, petalGene, heightGene;
    public Color getColor() 
    {
        return new Color (redGene.popCount() / 12f, blueGene.popCount() / 12f, greenGene.popCount() / 12f);
    }
    
    public int getPetalNum()
    {
        return 3 + petalGene.popCount();
    }
    
    public float getHeight()
    {
        return (8 + heightGene.popCount()) / 8f;
    }
    
    [SerializeField]
    private GameObject petal;
    [SerializeField]
    private GameObject stem;
    public bool adult { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        // add petals
        float angle = 360 / getPetalNum();
        float width = 3f / getPetalNum();
        Transform petals = transform.Find("petals");        
        for (int i = 0; i < getPetalNum(); i++) {
            GameObject p = Instantiate(petal,petals);
            PetalProps properties = p.GetComponent<PetalProps>();
            properties.petalColor = getColor();
            properties.angle = new Vector3(0, 0, angle*i);
            properties.width = width;
        }
        petals.localScale = (new Vector3(1, 1, 1)) * getHeight() / 3;

        // add stem
        GameObject s = Instantiate(stem, transform);
        s.GetComponent<StemProps>().height = getHeight();
        transform.localPosition += transform.up * (getHeight()+(getHeight()*transform.parent.localScale.y-getHeight())/2);

    }

    public void Randomize() 
    {
        redGene = GeneCombiner.randomGene();
        blueGene = GeneCombiner.randomGene();
        greenGene = GeneCombiner.randomGene();
        heightGene = GeneCombiner.randomGene();
        petalGene = GeneCombiner.randomGene();
    }

    public void crossoverFrom(Flowerer p, Flowerer q)
    {
        redGene = GeneCombiner.cross(p.redGene, q.redGene);
        blueGene = GeneCombiner.cross(p.blueGene, q.blueGene);
        greenGene = GeneCombiner.cross(p.greenGene, q.greenGene);
        heightGene = GeneCombiner.cross(p.heightGene, q.heightGene);
        petalGene = GeneCombiner.cross(p.petalGene, q.petalGene);
    }
    
    public void mutateFrom(Flowerer p)
    {
        redGene = GeneCombiner.mutate(p.redGene);
        blueGene = GeneCombiner.mutate(p.blueGene);
        greenGene = GeneCombiner.mutate(p.greenGene);
        heightGene = GeneCombiner.mutate(p.heightGene);
        petalGene = GeneCombiner.mutate(p.petalGene);
    }
}
