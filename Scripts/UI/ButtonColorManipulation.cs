using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorManipulation : MonoBehaviour
{
    [SerializeField] protected Button button;
    [SerializeField] protected List<Image> images; //what images need to change color when hovered
    [SerializeField] protected List<Text> texts; //what text needs to change color when hovered
    [SerializeField] protected Color isoColorImage; //isolated
    [SerializeField] protected Color isoColorText; //isolated
    [SerializeField] protected Color hoverColor;
    [SerializeField] protected Color selectColor;

    [Header("Properties")]
    [SerializeField] protected bool selectable;
    [SerializeField] protected bool activated; //Button Color Manipulations will occur

    [Header("Scaling")]
    [SerializeField] protected float startScale;
    [SerializeField] protected float hoverScale;
    [SerializeField] protected float clickScale;
    private bool isSelected = false;
    private ColorBlock cb;

    // Start is called before the first frame update
    void Start()
    {
        cb = button.colors;
        //foreach (Image i in images)
        //    i.color = isoColorImage;
        //foreach (Text t in texts)
        //    t.color = isoColorText;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
     * Select or deselect this button, if needed
     */
    public void Select(bool b)
    {
        isSelected = b;
        if (isSelected)
        {
            foreach (Image i in images)
                i.color = selectColor;
            foreach (Text t in texts)
                t.color = selectColor;
        }
        else
        {
            foreach (Image i in images)
                i.color = isoColorImage;
            foreach (Text t in texts)
                t.color = isoColorText;
        }

        button.transform.localScale = Vector3.one * startScale;
    }

    public void Select()
    {
        Select(!isSelected);
    }

    public void ActivateColorManipulation(bool b)
    {
        activated = b;
        if (!activated)
        {
            button.transform.localScale = Vector3.one * startScale;
            foreach (Image i in images)
                i.color = isoColorImage;
            foreach (Text t in texts)
                t.color = isoColorText;
        }
    }

    /*
     * Call this function when the button is being hovered over
     */
    public void whenHover()
    {
        if (!isSelected && activated)
        {
            button.transform.localScale = Vector3.one * hoverScale;
            foreach (Image i in images)
                i.color = hoverColor;
            foreach (Text t in texts)
                t.color = hoverColor;
        }
    }

    /*
     * Call this function when the cursor leaves the button
     */
    public void whenLeave()
    {
        if (!isSelected && activated)
        {
            button.transform.localScale = Vector3.one * startScale;
            foreach (Image i in images)
                i.color = isoColorImage;
            foreach (Text t in texts)
                t.color = isoColorText;
        }
    }

    /*
     * Call this function when the cursor clicks down
     */
    public void whenDown()
    {
        if (!isSelected && activated)
        {
            button.transform.localScale = Vector3.one * clickScale;
        }
    }

    /*
     * Call this function every frame the cursor is held
     */
    public void whenHeld()
    {
        
    }

    /*
     * Call this function when the cursor is released
     */
    public void whenUp()
    {
        button.transform.localScale = Vector3.one * startScale;
    }

    public void OnDisable()
    {
        button.transform.localScale = Vector3.one * startScale;
        foreach (Image i in images)
            i.color = isoColorImage;
        foreach (Text t in texts)
            t.color = isoColorText;

        isSelected = false;
    }
}
