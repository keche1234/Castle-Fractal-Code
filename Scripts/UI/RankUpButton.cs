using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUpButton : MonoBehaviour
{
    [SerializeField] protected Text attributeText; // Tells you
    [SerializeField] protected Text helpText;
    [SerializeField] protected int attributeIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetAttributeText()
    {
        return attributeText.text;
    }

    public string GetHelpText()
    {
        return helpText.text;
    }

    public int GetAttributeIndex()
    {
        return attributeIndex;
    }
}
