using System.Collections;
using System.Collections.Generic;
using System;
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
    public bool selected = false;

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

    public void MakeTarget()
    {
        redGene = GeneCombiner.targetGene();
        blueGene = GeneCombiner.targetGene();
        greenGene = GeneCombiner.targetGene();
        heightGene = GeneCombiner.targetGene();
        petalGene = GeneCombiner.targetGene();
    }

    public void CrossoverFrom(Flowerer p, Flowerer q)
    {
        redGene = GeneCombiner.cross(p.redGene, q.redGene);
        blueGene = GeneCombiner.cross(p.blueGene, q.blueGene);
        greenGene = GeneCombiner.cross(p.greenGene, q.greenGene);
        heightGene = GeneCombiner.cross(p.heightGene, q.heightGene);
        petalGene = GeneCombiner.cross(p.petalGene, q.petalGene);
    }
    
    public void MutateFrom(Flowerer p)
    {
        CopyFrom(p);
        
        System.Random rng = new System.Random();
        int changedGene = rng.Next(0, 5);
        switch (changedGene) {
            case 0:
                redGene = GeneCombiner.mutate(p.redGene);
                break;
            case 1:
                blueGene = GeneCombiner.mutate(p.blueGene);
                break;
            case 2:
                greenGene = GeneCombiner.mutate(p.greenGene);
                break;
            case 3:
                heightGene = GeneCombiner.mutate(p.heightGene);
                break;
            case 4:
                petalGene = GeneCombiner.mutate(p.petalGene);
                break;
        }
    }

    public void CopyFrom(Flowerer original) 
    {
        redGene = original.redGene;
        blueGene = original.blueGene;
        greenGene = original.greenGene;
        heightGene = original.heightGene;
        petalGene = original.petalGene;
    }

    public void Transparent(bool setTransparent) 
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            Color c = r.material.color;
            r.material.color = new Color(c.r, c.g, c.b, setTransparent ? 0.05f : 1f );
        }       
    }

    public void Reset()
    {
        Transparent(false);
        selected = false;
    }
    
    const float HEIGHT_TRESH = 0.2f;
    const int PETAL_TRESH = 0;
    const float COLOR_TRESH = 0.08f;
    public bool SimilarTo (Flowerer other) 
    {
        float heightDist = Math.Abs(getHeight() - other.getHeight());
        int petalDist = Math.Abs(getPetalNum() - other.getPetalNum());
        float colorDist = Math.Max(Math.Abs(getColor().r - other.getColor().r),
                           Math.Max(Math.Abs(getColor().b - other.getColor().b),
                                    Math.Abs(getColor().g - other.getColor().g)));
        return (heightDist <= HEIGHT_TRESH) && (petalDist <= PETAL_TRESH) && (colorDist <= COLOR_TRESH);
    }
    
    public float DistanceFrom (Flowerer other)
    {
        float heightDist = Math.Abs(getHeight() - other.getHeight()) / (2.5f - 1f);
        float petalDist = Math.Abs(getPetalNum() - other.getPetalNum()) / 12f;
        float colorDist = Math.Max(Math.Abs(getColor().r - other.getColor().r),
                           Math.Max(Math.Abs(getColor().b - other.getColor().b),
                                    Math.Abs(getColor().g - other.getColor().g)));
        return heightDist + petalDist + 5 * colorDist;
    }
}
