using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    public Slider slider;
    protected float internalValue;
    [SerializeField] protected RectTransform mainFill;
    [SerializeField] protected RectTransform rollFill;

    [Header("Text")]
    [SerializeField] protected Text amount;
    [SerializeField] protected Text rank;

    [Header("Rolling Info")]
    [SerializeField] protected bool rolling;
    protected float rollSpeed;
    protected int direction; //1 is increasing, -1 is decreasing
    protected float delayTime;
    protected float delay;

    public void Update()
    {
        if (rolling)
        {
            if (delayTime < delay) //don't start rolling immediately
                delayTime += Time.deltaTime;
            else
            {
                if (direction == 1)
                {
                    if (internalValue > slider.value)
                    {
                        slider.value += rollSpeed * Time.deltaTime;
                    }
                    else
                    {
                        direction = 0;
                        slider.value = internalValue;
                    }
                }
                else if (direction == -1)
                {
                    if (internalValue < slider.value)
                    {
                        slider.value -= rollSpeed * Time.deltaTime;
                    }
                    else
                    {
                        direction = 0;
                        slider.value = internalValue;
                    }
                }
            }
        }
    }

    /*
     * value, delay for the roll, and roll speed
     */
    public void SetValue(float val, float d = 0.5f, float rs = 4)
    {
        float temp = slider.value;
        if (val > slider.value) //increase
        {
            internalValue = val;
            slider.fillRect = rollFill;
            slider.value = val;

            slider.fillRect = mainFill;
            slider.value = temp;
            if (!rolling)
                slider.value = val;

            direction = 1;
        }
        else
        {
            internalValue = val;
            slider.fillRect = mainFill;
            slider.value = val;

            slider.fillRect = rollFill;
            slider.value = temp;
            if (!rolling)
                slider.value = val;

            direction = -1;
        }
        delayTime = 0;
        delay = d;
        rollSpeed = rs;
    }

    public void SetMax(float val)
    {
        slider.fillRect = rollFill;
        slider.maxValue = val;
        slider.fillRect = mainFill;
        slider.maxValue = val;
    }

    public float GetMax()
    {
        return slider.maxValue;
    }

    public void UpdateAmountTxt(string str)
    {
        amount.text = str;
    }

    public void UpdateRankTxt(string str)
    {
        rank.text = str;
    }

    public void SetFillColor(Color col)
    {
        mainFill.gameObject.GetComponent<Image>().color = col;
    }

    public void SetRollColor(Color col)
    {
        rollFill.gameObject.GetComponent<Image>().color = col;
    }
}
