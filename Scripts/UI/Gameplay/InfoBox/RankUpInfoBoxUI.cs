using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUpInfoBoxUI : InfoBoxUI
{
    [SerializeField] protected Text attributeText;
    [SerializeField] protected Text helpText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ClearDescription()
    {
        box.color = emptyColor;
        border.color = emptyColor;

        attributeText.gameObject.SetActive(false);
        helpText.gameObject.SetActive(false);
    }

    public override void DrawDescription()
    {
        throw new System.NotImplementedException();
    }

    public void DrawDescription(RankUpButton rbButton)
    {
        box.color = filledColor;
        border.color = filledColor;

        attributeText.text = rbButton.GetAttributeText();
        attributeText.gameObject.SetActive(true);

        helpText.text = rbButton.GetHelpText();
        helpText.gameObject.SetActive(true);

    }
}
