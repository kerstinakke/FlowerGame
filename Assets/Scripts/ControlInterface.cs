using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlInterface : MonoBehaviour
{
    public int n = 2; // Survivor limit
    public int m = 3; // Offspring limit
    [SerializeField]
    private GameObject flowerPrefab;
    [SerializeField]
    private GameObject potPrefab;
    [SerializeField]
    private Texture2D crosserCursor;
    [SerializeField]
    private Texture2D mutaterSprite;

    private enum Tools { None, Mutater, Crosser, Selecter };
    [SerializeField]
    private Tools activeTool = Tools.None;
    private Camera cam;
    private List<PotLabel> bigPots = new List<PotLabel>();
    private List<PotLabel> smallPots = new List<PotLabel>();
    private Queue<int> empty = new Queue<int>();
    private Flowerer firstCrossover;
    private int selected = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        activeTool = Tools.None;
        cam = GetComponent<Camera>();

        // create and fill n big pots
        float z = cam.nearClipPlane +5;
        Vector3 rightBound = cam.ViewportToWorldPoint(new Vector3(0.9f, 0.6f, z));
        Vector3 leftBound = cam.ViewportToWorldPoint(new Vector3(0.25f, 0.6f, z));
        float step = (rightBound.x - leftBound.x) / (n-1);
        for (int i = 0; i < n; i++) {
            GameObject flowerpot = Instantiate(potPrefab, leftBound + new Vector3(step * i, 0),Quaternion.identity);
            flowerpot.name = ""+(i+1);
            flowerpot.GetComponent<PotLabel>().SetLabel("" + (i + 1));
            bigPots.Add(flowerpot.GetComponent<PotLabel>());
            GameObject flower = Instantiate(flowerPrefab,flowerpot.transform,false);
            Flowerer flowerScript = flower.GetComponent<Flowerer>();
            flowerScript.Randomize();
            flowerScript.adult = true;
            
            
        }

        // create m empty small pots
        leftBound = cam.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, z));
        step = (rightBound.x - leftBound.x) / (m - 1);
        for (int i = 0; i < m; i++)
        {
            GameObject flowerpot = Instantiate(potPrefab, leftBound + new Vector3(step * i, 0), Quaternion.identity);
            flowerpot.name = "tinyPot" + (i + 1);
            flowerpot.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            smallPots.Add(flowerpot.GetComponent<PotLabel>());
            empty.Enqueue(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Handles interaction with flowers 
        if (Input.GetMouseButtonDown(0)) {  
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (activeTool == Tools.None) {
                    print("no tool selected");
                    return;
                }
                print(hit.transform.name);
                Flowerer flower = hit.transform.GetComponentInParent<Flowerer>();
                if (flower == null) {
                    print("no flower clicked");
                    return;
                }

                // create mutated offspring
                if (activeTool == Tools.Mutater)
                {
                    if (empty.Count == 0 || flower.adult == false) print("can't mutate");
                    else
                    {
                        int freePot = empty.Dequeue();
                        GameObject offspring = Instantiate(flowerPrefab, smallPots[freePot].transform, false);
                        print("mutating flower in " + flower.transform.parent.name);                       
                        offspring.GetComponent<Flowerer>().MutateFrom(flower);
                        smallPots[freePot].SetLabel(flower.transform.parent.name+"*");                        
                    }
                }

                // create crossover offspring
                else if (activeTool == Tools.Crosser)
                {
                    if (empty.Count == 0 || !flower.adult) print("can't crossover");
                    else if (firstCrossover == null)
                    {
                        firstCrossover = flower;
                        print("selected flower in " + flower.transform.parent.name);
                    }
                    else if (!firstCrossover.Equals(flower)) // can't cross with self
                    {
                        int freePot = empty.Dequeue();
                        GameObject offspring = Instantiate(flowerPrefab, smallPots[freePot].transform, false);
                        print("crossing flowers in " + flower.transform.parent.name + " and " + firstCrossover.transform.parent.name);
                        offspring.GetComponent<Flowerer>().CrossoverFrom(flower, firstCrossover);
                        string newLabel = firstCrossover.transform.parent.name + "+" + flower.transform.parent.name;
                        smallPots[freePot].SetLabel(newLabel);
                        firstCrossover = null;
                    }
                    else print("can't cross flower with itself");
                }
                else if (activeTool == Tools.Selecter) {
                    if (selected < n && !flower.selected)
                    {
                        flower.selected = true;
                        selected += 1;
                        flower.Transparent(true);
                    } else if (flower.selected)
                    {
                        flower.selected = false;
                        selected -= 1;
                        flower.Transparent(false);
                    }
                    else print("selection limit reached or flower already selected");
                }
            }
        }
    }

    public void ActivateMutater() {
        activeTool = Tools.Mutater;
        print("mutating tool active");
        Cursor.SetCursor(mutaterSprite, new Vector2(0.1f, 0), CursorMode.Auto);
    }

    public void ActivateCrosser()
    {
        firstCrossover = null;
        activeTool = Tools.Crosser;
        print("crossover tool active");
        Cursor.SetCursor(crosserCursor,new Vector2(0,0),CursorMode.Auto);
    }

    public void ActivateSelecter()
    {
        activeTool = Tools.Selecter;
        print("survivor selection tool active");
        Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        selected = 0;
    }

    public void DeactivateTools()
    {
        activeTool = Tools.None;
    }

    public void KeepSelected()
    {
        int free = 0;
        int lastSmall = 0;
        while (selected > 0 && lastSmall < m)
        {
            if (bigPots[free].GetComponentInChildren<Flowerer>().selected)
            {
                bigPots[free].GetComponentInChildren<Flowerer>().Reset();
                free += 1;
                selected -= 1;
            }
            else {
                Flowerer flower = smallPots[lastSmall].GetComponentInChildren<Flowerer>(); 
                while ((flower==null  || !flower.selected) && lastSmall < m-1){
                    print(lastSmall);
                    lastSmall += 1;
                    flower = smallPots[lastSmall].GetComponentInChildren<Flowerer>();
                }
                if (flower == null) break;
                // switch an unselected flower in big pot with a selected small flower
                Destroy(bigPots[free].GetComponentInChildren<Flowerer>().gameObject);
                GameObject repotted = Instantiate(flowerPrefab, bigPots[free].transform, false);
                repotted.GetComponent<Flowerer>().CopyFrom(flower);
                repotted.GetComponent<Flowerer>().adult = true;
                lastSmall += 1;
                free += 1;
                selected -= 1;
            }
            
        }
        for (int i = free; i < n; i++) { // reset all remaining big flowers
            bigPots[i].GetComponentInChildren<Flowerer>().Reset();
        }
        for (int i = 0; i < m; i++) // empty all small pots
        {
            if (!empty.Contains(i))
            {
                Destroy(smallPots[i].GetComponentInChildren<Flowerer>().gameObject);
                smallPots[i].SetLabel("");
                empty.Enqueue(i);
            }
        }
    }
}
