using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionButton : MonoBehaviour
{
    [SerializeField] protected int potionNumber;
    [SerializeField] protected PlayerController player;

    [Header("Icon")]
    [SerializeField] protected List<Sprite> potionIconSprites;
    [SerializeField] protected Image myPotionIcon;

    [Header("The Button")]
    [SerializeField] protected Button button;
    [SerializeField] protected Color filledColor; //if there is a weapon
    [SerializeField] protected Color emptyColor; //if there is no weapon

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawButton()
    {
        List<int> potionList = player.GetPotions();
        if (potionNumber >= 0 && potionNumber < potionList.Count)
        {
            myPotionIcon.gameObject.SetActive(true);
            myPotionIcon.sprite = potionIconSprites[potionList[potionNumber] - 1];
            button.gameObject.GetComponent<Button>().enabled = true;
            if (button.GetComponent<ButtonColorManipulation>() != null) //show enable colors
            {
                button.GetComponent<Image>().color = filledColor;
                button.gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(true);
                button.gameObject.GetComponent<ButtonColorManipulation>().enabled = true;
            }
        }
        else
        {
            myPotionIcon.gameObject.SetActive(false);
            if (button.GetComponent<ButtonColorManipulation>() != null) //show disable colors
            {
                button.gameObject.GetComponent<ButtonColorManipulation>().Select(false);
                button.GetComponent<Image>().color = emptyColor;
                button.gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(false);
                button.gameObject.GetComponent<ButtonColorManipulation>().enabled = false;
            }
            button.gameObject.GetComponent<Button>().enabled = false;
        }
    }

    public void SetNumber(int num)
    {
        potionNumber = num;
    }

    public int GetNumber()
    {
        return potionNumber;
    }

    void OnEnable()
    {
        DrawButton();
    }
}
