using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStateController : MonoBehaviour
{
    [SerializeField]
    private Button[] tools = new Button[2];
    [SerializeField]
    private Button stateSwitch;
    private ControlInterface ci;
    private bool crossState;

    // Start is called before the first frame update
    void Start()
    {
        ci = gameObject.GetComponent<ControlInterface>();
        crossState = true;
    }

    public void Switch() {
       foreach(Button b in tools) {
            b.gameObject.SetActive(!crossState);
       }
       if (crossState) ci.ActivateSelecter();
        else {
            ci.KeepSelected();
            ci.DeactivateTools();
        }
       crossState = !crossState;
    }

    public void SwitchAutoplay() {
        if (!ci.autoplay)
        {
            foreach (Button b in tools)
            {
                b.interactable = false;
            }
            stateSwitch.interactable = false;
            ci.DeactivateTools();
            ci.autoplay = true;
            StartCoroutine("Autoplay");
            
        }
        else {           
            StopCoroutine("Autoplay");
            ci.autoplay = false;
            foreach (Button b in tools)
            {
                b.interactable = true;
            }
            stateSwitch.interactable = true;
        }
    }
    IEnumerator Autoplay()
    {
        List<int> flowerpots = new List<int>();
        for (int i = 0; i < ci.n + ci.m; i++) {
            flowerpots.Add(i);
        }
        while(true)
        {
            if (crossState)
            {
                List<int> bestFlowers = new List<int>();
                for (int i = 0; i < ci.n; i++) 
                {
                    bestFlowers.Add(i);
                }
                bestFlowers = bestFlowers.OrderBy(x => ci.GetFlower(x, true).DistanceFrom(ci.target)).ToList();
                    
                int chooseFromBest = ci.n;
                int free = ci.GetEmptySmalls();
                for (int k = 0; k < free; k++)
                {
                    if (Random.value < 0.5) 
                    {
                        int p1 = Random.Range(0, chooseFromBest);
                        int p2 = p1;
                        while (p2 == p1) 
                        {
                            p2 = Random.Range(0, chooseFromBest);
                        }
                        
                        ci.MakeCrossover(ci.GetFlower(p1, true), ci.GetFlower(p2, true));
                    } 
                    else 
                    {
                        int p1 = Random.Range(0, chooseFromBest);
                        ci.MakeMutant(ci.GetFlower(p1, true));
                    }
                    yield return new WaitForSeconds(0.2f);
                    
                }
            }
            else
            {
                List<int> bestFlowers = new List<int>();
                for (int i = 0; i < ci.n + ci.m; i++) 
                {
                    if (ci.GetFlower(i, false) != null)
                    {
                        bestFlowers.Add(i);
                    }
                }
                bestFlowers = bestFlowers.OrderBy(x => ci.GetFlower(x, false).DistanceFrom(ci.target)).ToList();

                for (int i = 0; i < ci.n - ci.selected; i++)
                {
                    int index = bestFlowers[i];
                    Flowerer flower = ci.GetFlower(index);
                    flower.selected=true;
                    flower.Transparent(true);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            Switch();
            yield return new WaitForSeconds(0.01f);
        }
    }
}
