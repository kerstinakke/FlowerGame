using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotLabel : MonoBehaviour
{
    private TextMesh label;
    // Start is called before the first frame update
    void Start()
    {
        label = GetComponentInChildren<TextMesh>();
    }

    public void SetLabel(string text) {
        if (label == null) {
            label = GetComponentInChildren<TextMesh>();
        }
        label.text = text;
    }

    public string GetLabel()
    {
        if (label == null)
        {
            label = GetComponentInChildren<TextMesh>();
        }
        return label.text;
    }

}
