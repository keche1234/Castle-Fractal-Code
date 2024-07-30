using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIndexButtonUI : MonoBehaviour
{
    [SerializeField] protected Image icon;
    [SerializeField] protected List<string> titles;
    [SerializeField] protected List<string> descriptions;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetIcon()
    {
        return icon.sprite;
    }

    public List<string> GetTitles()
    {
        return new List<string>(titles);
    }

    public List<string> GetDescriptions()
    {
        return new List<string>(descriptions);
    }
}
