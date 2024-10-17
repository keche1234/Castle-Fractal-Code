using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeneralTextFlash : InfoFlash
{
    [SerializeField] protected TextMeshProUGUI myText;
    protected string myMessage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (appearTimeRemaining > 0)
        {
            appearTimeRemaining -= Time.deltaTime;
        }
        else
        {
            appearTimeRemaining = 0;
            if (fadeTime <= 0)
            {
                myText.gameObject.SetActive(false);
            }
            else if (fadeTimeRemaining > 0)
            {
                fadeTimeRemaining -= Time.deltaTime;

                Color colorWhite = new Color(255, 255, 255, fadeTimeRemaining / fadeTime);
                myText.color = colorWhite;
            }
        }
    }

    public void SetMessage(string s, bool drawFlash = false)
    {
        myMessage = s;
        if (drawFlash)
            DrawInfoFlash();
    }

    public override void DrawInfoFlash()
    {
        Color colorWhite = new Color(255, 255, 255, 1);
        myText.gameObject.SetActive(true);
        myText.color = colorWhite;
        myText.text = myMessage;

        appearTimeRemaining = appearTime;
        fadeTimeRemaining = fadeTime;

        OverrideOtherFlashes();
    }
}
