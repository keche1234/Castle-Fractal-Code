using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUpButton : MonoBehaviour
{
    [Header("The Button")]
    [SerializeField] protected Button button;
    [SerializeField] protected Color filledColor; //if this attribute can be upgraded
    [SerializeField] protected Color emptyColor; //if this attribute cannot be upgraded
    [SerializeField] protected Text plusOne;
    [SerializeField] protected Image icon;
    [SerializeField] protected Sprite filledIconSprite; // If you *can* rank up this Attribute
    [SerializeField] protected Sprite emptyIconSprite; // If you *can't* rank up this Attribute

    [Header("Info Text")]
    [SerializeField] protected Text attributeText; // Tells you the attribute being upgraded and by how much
    [SerializeField] protected Text helpText; // Tells you what the attribute being upgraded will do
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

    /********************************************************
     * Darkens the button color to indicate it being disabled
     ********************************************************/
    public void EmptyButton()
    {
        button.gameObject.GetComponent<ButtonColorManipulation>().Select(false);
        button.gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(false);
        button.gameObject.GetComponent<Button>().enabled = false;
        button.image.color = emptyColor;
        icon.sprite = emptyIconSprite;
        plusOne.color = emptyColor;
    }
}
