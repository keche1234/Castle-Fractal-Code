using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsInfoBoxUI : InfoBoxUI
{
    [SerializeField] protected List<GameObject> slides;
    [SerializeField] protected Button leftButton;
    [SerializeField] protected Button rightButton;

    protected int slideIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementSlideIndex()
    {
        if (slideIndex < slides.Count - 1)
        {
            slideIndex++;
            DrawDescription();
        }
    }

    public void DecrementSlideIndex()
    {
        if (slideIndex > 0)
        {
            slideIndex--;
            DrawDescription();
        }
    }

    public void SetSlideIndex(int i)
    {
        if (i >= 0 && i < slides.Count)
        {
            slideIndex = i;
            DrawDescription();
        }
    }

    public override void ClearDescription()
    {
        foreach (GameObject slide in slides)
            slide.SetActive(false);
    }

    public override void DrawDescription()
    {
        ClearDescription();
        if (slideIndex >= 0 && slideIndex < slides.Count)
            slides[slideIndex].SetActive(true);
        CorrectArrows();
    }

    protected void CorrectArrows()
    {
        if (slideIndex > 0)
        {
            //enable left button
            leftButton.enabled = true;
            leftButton.gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(true);
            leftButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            //disable left button
            leftButton.gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(false);
            leftButton.GetComponent<Image>().color = emptyColor;
            leftButton.enabled = false;
        }

        if (slideIndex < slides.Count - 1)
        {
            //enable right button
            rightButton.enabled = true;
            rightButton.gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(true);
            rightButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            //disable right button
            rightButton.gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(false);
            rightButton.GetComponent<Image>().color = emptyColor;
            rightButton.enabled = false;
        }
    }
}
