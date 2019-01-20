using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlInterface : MonoBehaviour
{
    public Flowerer target;
    public int n = 2; // Survivor limit
    public int m = 3; // Offspring limit
    public bool autoplay = false;
    [SerializeField]
    private GameObject flowerPrefab;
    [SerializeField]
    private GameObject potPrefab;
    [SerializeField]
    private Texture2D crosserCursor;
    [SerializeField]
    private Texture2D crosserCursor2;
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
    private Text messages;
    

    // Start is called before the first frame update
    void Start()
    {
        activeTool = Tools.None;
        cam = GetComponent<Camera>();
        messages = GameObject.FindGameObjectWithTag("MessageScreen").GetComponent<Text>();
        messages.text = "";

        // create and fill n big pots
        float z = cam.nearClipPlane +5;
        Vector3 rightBound = cam.ViewportToWorldPoint(new Vector3(0.9f, 0.5f, z));
        Vector3 leftBound = cam.ViewportToWorldPoint(new Vector3(0.25f, 0.5f, z));
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

        // create the target
        GameObject pot = Instantiate(potPrefab, cam.ViewportToWorldPoint(new Vector3(0.08f, 0.5f, z)), Quaternion.identity);
        pot.name = "Target";
        pot.GetComponent<PotLabel>().SetLabel("Target");
        target = Instantiate(flowerPrefab, pot.transform, false).GetComponent<Flowerer>();
        target.MakeTarget();

    }

    // Update is called once per frame
    void Update()
    {
        // Handles interaction with flowers 
        if (Input.GetMouseButtonDown(0)) {
            if (autoplay) return;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                messages.text = "";
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
                    if (empty.Count == 0) messages.text = "No more room";
                    else if (!flower.adult) messages.text = "This flower can not give offspring";
                    else MakeMutant(flower);
                }

                // create crossover offspring
                else if (activeTool == Tools.Crosser)
                {
                    if (empty.Count == 0) messages.text = "No more room";
                    else if (!flower.adult) messages.text = "This flower can not give offspring";
                    else if (firstCrossover == null)
                    {
                        firstCrossover = flower;
                        print("selected flower in " + flower.transform.parent.name);
                        Cursor.SetCursor(crosserCursor2, new Vector2(0, 0), CursorMode.ForceSoftware);
                    }
                    else if (!firstCrossover.Equals(flower)) // can't cross with self
                    {
                        MakeCrossover(firstCrossover, flower);
                        firstCrossover = null;
                        Cursor.SetCursor(crosserCursor, new Vector2(0, 0), CursorMode.ForceSoftware);
                    }
                    else messages.text = "Can't cross flower with itself";
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
                    else messages.text = "Selection limit reached";
                }
            }
        }
    }

    public void MakeCrossover(Flowerer parent1, Flowerer parent2)
    {
        if (parent1 == parent2) return;
        int freePot = empty.Dequeue();
        GameObject offspring = Instantiate(flowerPrefab, smallPots[freePot].transform, false);
        print("crossing flowers in " + parent1.transform.parent.name + " and " + parent2.transform.parent.name);
        offspring.GetComponent<Flowerer>().CrossoverFrom(parent1,parent2);
        string newLabel = parent1.transform.parent.name + "+" + parent2.transform.parent.name;
        smallPots[freePot].SetLabel(newLabel);
    }

    public void MakeMutant(Flowerer parent)
    {
        int freePot = empty.Dequeue();
        GameObject offspring = Instantiate(flowerPrefab, smallPots[freePot].transform, false);
        print("mutating flower in " + parent.transform.parent.name);
        offspring.GetComponent<Flowerer>().MutateFrom(parent);
        smallPots[freePot].SetLabel(parent.transform.parent.name + "*");
    }

    public Flowerer GetFlower(int pot, bool assertAdult=false) {
        if (pot < n)
        {
            return bigPots[pot].GetComponentInChildren<Flowerer>();
        }
        else if (pot < m + n && !assertAdult)
        {
            return smallPots[pot - n].GetComponentInChildren<Flowerer>();
        }
        else throw new UnityException("invalid pot index");
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
        Cursor.SetCursor(crosserCursor,new Vector2(0,0), CursorMode.ForceSoftware);
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

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void KeepSelected()
    {
        int free = 0;       
        for (int i = 0; i < m; i++) // empty all small pots
        {
            if (!empty.Contains(i))
            {
                Flowerer flower = smallPots[i].GetComponentInChildren<Flowerer>();
                if (flower.selected)
                {
                    // switch selected small flower with an unselected flower in big pot
                    while (free < n && bigPots[free].GetComponentInChildren<Flowerer>().selected)
                    {
                        free += 1;
                    }
                    Destroy(bigPots[free].GetComponentInChildren<Flowerer>().gameObject);
                    GameObject repotted = Instantiate(flowerPrefab, bigPots[free].transform, false);
                    repotted.GetComponent<Flowerer>().CopyFrom(flower);
                    repotted.GetComponent<Flowerer>().adult = true;
                    free += 1;
                }
                Destroy(flower.gameObject);
                smallPots[i].SetLabel("");
                empty.Enqueue(i);
            }
        }
        for (int i = 0; i < n; i++) {
            bigPots[i].GetComponentInChildren<Flowerer>().Reset();
        }
        
        // check if we have a winner
        for (int i = 0; i < n; i++) {
            if (bigPots[i].GetComponentInChildren<Flowerer>().SimilarTo(target)) {
                messages.text = "You win! Flower pot " + (i + 1) + " has reached the target!";
                if (autoplay) {
                    gameObject.GetComponent<GameStateController>().SwitchAutoplay();
                }
            }
        }
    }

}
