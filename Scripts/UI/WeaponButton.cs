using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponButton : MonoBehaviour
{
    [SerializeField] protected int weaponNumber;
    [SerializeField] protected PlayerController player;

    [Header("Abilities")]
    [SerializeField] protected List<Sprite> abilityIconSprites;
    [SerializeField] protected List<Image> myAbilityIcons;

    [Header("Icon")]
    [SerializeField] protected List<Sprite> weaponIconSprites;
    [SerializeField] protected Image myWeaponIcon;

    [Header("Bars")]
    [SerializeField] protected BarUI durability;
    [SerializeField] protected BarUI signature;

    [Header("The Button")]
    [SerializeField] protected Button button;
    [SerializeField] protected Color filledColor; //if there is a weapon
    [SerializeField] protected Color emptyColor; //if there is no weapon

    [Header("Navigation")] //left weapon card only
    [SerializeField] protected Button upArrow;
    [SerializeField] protected Button downArrow;
    [SerializeField] protected Text pages;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Update Signature
        CustomWeapon weapon = player.GetCustomWeapon(weaponNumber);
        if (weapon != null)
        {
            signature.SetMax(weapon.GetSignatureCap());
            signature.SetValue(weapon.GetSignatureGauge());
        }
    }

    public void DrawButton()
    {
        CustomWeapon weapon = player.GetCustomWeapon(weaponNumber);
        if (weapon != null) //weaponNumber is in range of player's inventory
        {
            //draw the specific weapon
            foreach (Image ability in myAbilityIcons)
                ability.gameObject.SetActive(true);
            myWeaponIcon.gameObject.SetActive(true);
            durability.gameObject.SetActive(true);
            signature.gameObject.SetActive(true);

            //Show the correct abilities
            for (int i = 0; i < myAbilityIcons.Count; i++)
                myAbilityIcons[i].sprite = abilityIconSprites[weapon.GetAbilities()[i]];

            //Show the correct weapon
            myWeaponIcon.sprite = weaponIconSprites[weapon.GetWeaponType()];

            //Update Durability
            durability.SetMax(weapon.GetMaxDurability());
            durability.SetValue(weapon.DecrementDurability(0));

            //Update Signature
            signature.SetMax(weapon.GetSignatureCap());
            signature.SetValue(weapon.GetSignatureGauge());

            //Select if activate weapon
            if (button.GetComponent<ButtonManipulation>() != null)
            {
                if (weaponNumber == player.GetCustomIndex())
                    button.GetComponent<ButtonManipulation>().Select(true);
                else
                    button.GetComponent<ButtonManipulation>().Select(false);
            }

            //Change weapon if player is not attacking
            if (player.GetAttackState() == 0)
            {
                button.gameObject.GetComponent<Button>().enabled = true;
                if (button.gameObject.GetComponent<ButtonManipulation>() != null) //show enable colors
                {
                    button.gameObject.GetComponent<ButtonManipulation>().enabled = true;
                    button.gameObject.GetComponent<ButtonManipulation>().Activate(true);
                }
            }
            else
            {
                button.gameObject.GetComponent<Button>().enabled = false;
                if (button.gameObject.GetComponent<ButtonManipulation>() != null)
                {
                    button.gameObject.GetComponent<ButtonManipulation>().Activate(false);
                    button.gameObject.GetComponent<ButtonManipulation>().enabled = false;
                }
            }
            button.GetComponent<Image>().color = filledColor;

            /*********************************************************************
             * Left Card Only- enable/disable arrows and show page based on index
             *********************************************************************/
            if (upArrow != null && downArrow != null)
            {
                if (weaponNumber == 0) //beginning of the inventory
                {
                    upArrow.gameObject.GetComponent<ButtonManipulation>().Activate(false);
                    upArrow.GetComponent<Image>().color = emptyColor;
                    upArrow.gameObject.GetComponent<Button>().enabled = false;
                }
                else
                {
                    upArrow.gameObject.GetComponent<Button>().enabled = true;
                    upArrow.gameObject.GetComponent<ButtonManipulation>().Activate(true);
                    upArrow.GetComponent<Image>().color = Color.white;
                }

                if (weaponNumber == player.InvCount() - 1 - ((player.InvCount() + 1) % 2)) //last (odd inventory) or second-to-last (even inventory)
                {
                    downArrow.gameObject.GetComponent<ButtonManipulation>().Activate(false);
                    downArrow.GetComponent<Image>().color = emptyColor;
                    downArrow.gameObject.GetComponent<Button>().enabled = false;
                }
                else
                {
                    downArrow.gameObject.GetComponent<Button>().enabled = true;
                    downArrow.gameObject.GetComponent<ButtonManipulation>().Activate(true);
                    downArrow.GetComponent<Image>().color = Color.white;
                }
            }

            if (pages != null) //update page counter
            {
                pages.text = string.Format("{0:D2}", (weaponNumber / 2) + 1) + "/" + string.Format("{0:D2}", ((player.InvCount() + 1)/ 2));
            }

        }
        else
        {
            //disable all UI
            foreach (Image ability in myAbilityIcons)
                ability.gameObject.SetActive(false);
            myWeaponIcon.gameObject.SetActive(false);
            durability.gameObject.SetActive(false);
            signature.gameObject.SetActive(false);

            button.gameObject.GetComponent<Button>().enabled = false;
            if (button.GetComponent<ButtonManipulation>() != null) //show disable colors
            {
                button.gameObject.GetComponent<ButtonManipulation>().Select(false);
                button.GetComponent<Image>().color = emptyColor;
                button.gameObject.GetComponent<ButtonManipulation>().Activate(false);
                button.gameObject.GetComponent<ButtonManipulation>().enabled = false;
            }

            /*********************************************************************
             * Left Card Only- enable/disable arrows and show page based on index
             *********************************************************************/
            if (upArrow != null && downArrow != null)
            {
                upArrow.gameObject.GetComponent<ButtonManipulation>().Activate(false);
                upArrow.GetComponent<Image>().color = emptyColor;
                upArrow.gameObject.GetComponent<Button>().enabled = false;

                downArrow.gameObject.GetComponent<ButtonManipulation>().Activate(false);
                downArrow.GetComponent<Image>().color = emptyColor;
                downArrow.gameObject.GetComponent<Button>().enabled = false;
            }

            if (pages != null) //update page counter
            {
                pages.text = "--/--";
            }
        }
    }

    public int GetWeaponNumber()
    {
        return weaponNumber;
    }

    public void IncrementWeaponNumber(int i)
    {
        if (weaponNumber + i >= 0 && weaponNumber + i < player.InvCount() + (player.InvCount() % 2))
            weaponNumber += i;
    }

    public void SetWeaponNumber(int i)
    {
        weaponNumber = i;
        if (weaponNumber == player.GetCustomIndex() && button.GetComponent<ButtonManipulation>() != null)
        {
            button.GetComponent<ButtonManipulation>().Select(true);
        }
    }

    void OnEnable()
    {
        DrawButton();
    }

    public List<Image> GetAbilityIcons()
    {
        List<Image> icons = new List<Image>();
        foreach (Image i in myAbilityIcons)
            icons.Add(i);
        return icons;
    }
}
