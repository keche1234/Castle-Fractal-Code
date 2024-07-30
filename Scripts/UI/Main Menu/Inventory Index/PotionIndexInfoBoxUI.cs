using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionIndexInfoBoxUI : InventoryIndexInfoBoxUI
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawDescription(PotionIndexButtonUI potion)
    {
        base.DrawDescription();
        SetIcon(potion);
        SetTitles(potion);
        SetDescriptions(potion);
    }
}
