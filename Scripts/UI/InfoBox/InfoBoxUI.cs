using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class InfoBoxUI : MonoBehaviour
{
    [Header("The Box")]
    [SerializeField] protected Image box;
    [SerializeField] protected Image border;
    [SerializeField] protected Color filledColor;
    [SerializeField] protected Color emptyColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void ClearDescription();
    public abstract void DrawDescription();
}
