using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class InventoryIndexInfoBoxUI : InfoBoxUI
{
    [SerializeField] protected Image icon;
    [SerializeField] protected List<TextMeshProUGUI> titles;
    [SerializeField] protected List<TextMeshProUGUI> descriptions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIcon(InventoryIndexButtonUI button)
    {
        icon.sprite = button.GetIcon();
    }

    public void SetTitles(InventoryIndexButtonUI button)
    {
        List<string> buttonTitles = button.GetTitles();
        int limit = Mathf.Min(titles.Count, buttonTitles.Count);
        for (int i = 0; i < limit; i++)
            titles[i].text = buttonTitles[i];
    }

    public void SetDescriptions(InventoryIndexButtonUI button)
    {
        List<string> buttonDescriptions = button.GetDescriptions();
        int limit = Mathf.Min(descriptions.Count, buttonDescriptions.Count);
        for (int i = 0; i < limit; i++)
            descriptions[i].text = buttonDescriptions[i];
    }

    public override void ClearDescription()
    {
        box.color = emptyColor;
        border.color = emptyColor;

        //icon.gameObject.SetActive(false);
        //foreach (TextMeshProUGUI title in titles)
        //    title.gameObject.SetActive(false);
        //foreach (TextMeshProUGUI description in descriptions)
        //    description.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    public override void DrawDescription()
    {
        box.color = filledColor;
        border.color = filledColor;
        gameObject.SetActive(true);

        //icon.gameObject.SetActive(true);
        //foreach (TextMeshProUGUI title in titles)
        //    title.gameObject.SetActive(true);
        //foreach (TextMeshProUGUI description in descriptions)
        //    description.gameObject.SetActive(true);
    }
}
