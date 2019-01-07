using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateController : MonoBehaviour
{
    [SerializeField]
    private Button[] tools = new Button[2];
    [SerializeField]
    private Button switchState;
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

    
}
