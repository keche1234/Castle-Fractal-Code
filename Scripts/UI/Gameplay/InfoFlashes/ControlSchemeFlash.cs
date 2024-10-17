using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlSchemeFlash : InfoFlash
{
    [Header("Information")]
    [SerializeField] protected TextMeshProUGUI controlSchemeText;
    protected string controlSchemeName;
    protected int index;

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
                controlSchemeText.gameObject.SetActive(false);
            }
            else if (fadeTimeRemaining > 0)
            {
                fadeTimeRemaining -= Time.deltaTime;

                Color colorWhite = new Color(255, 255, 255, fadeTimeRemaining / fadeTime);
                controlSchemeText.color = colorWhite;
            }
        }
    }

    public void SetControlName(string s, int i)
    {
        controlSchemeName = s;
        index = i;
    }

    public override void DrawInfoFlash()
    {
        if (!string.IsNullOrEmpty(controlSchemeName))
        {
            Color colorWhite = new Color(255, 255, 255, 1);
            controlSchemeText.gameObject.SetActive(true);
            controlSchemeText.color = colorWhite;
            controlSchemeText.text = "Controls set to \"" + controlSchemeName + "\" (" + (index + 1) + ")";

            appearTimeRemaining = appearTime;
            fadeTimeRemaining = fadeTime;

            OverrideOtherFlashes();
        }
        else
            controlSchemeText.gameObject.SetActive(false);
    }
}
