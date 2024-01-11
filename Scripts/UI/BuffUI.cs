using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    [SerializeField] protected BarUI bar;
    [SerializeField] protected Image icon;
    [SerializeField] protected Text value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIcon(Sprite i)
    {
        icon.sprite = i;
    }

    public void SetFill(float percentage, Color fillColor)
    {
        bar.SetMax(1);
        bar.SetValue(percentage, 0);
        bar.SetFillColor(fillColor);
    }

    public void SetValue(float val)
    {
        if (value != null) value.text = val + "";
    }

    public void SetValue(string str)
    {
        if (value != null) value.text = str;
    }

    public void SetValueColor(Color32 textColor)
    {
        if (value != null) value.color = textColor;
    }
}
