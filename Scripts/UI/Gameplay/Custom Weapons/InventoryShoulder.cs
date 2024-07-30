using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class will have its functions called and updated by other functions
public class InventoryShoulder : MonoBehaviour
{
    [SerializeField] List<Sprite> iconSpriteList; //the sprites to display
    [SerializeField] List<Image> iconImageList; //my actual objects (not including select border)
    [SerializeField] Image selectBorder;
    [SerializeField] Text indexText;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Image icon in iconImageList)
            icon.gameObject.SetActive(false);
        selectBorder.gameObject.SetActive(false);
        indexText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawShoulder(int selected, ref List<CustomWeapon> inventory)
    {
        // Determine which slot to draw
        if (inventory == null || inventory.Count == 0)
        {
            // Blank out all icons and selectBorder
            foreach (Image icon in iconImageList)
                icon.gameObject.SetActive(false);
            selectBorder.gameObject.SetActive(false);
            indexText.gameObject.SetActive(false);
            return;
        }

        if (selected < 0 || selected >= inventory.Count)
        {
            Debug.Log("Selected weapon is out of bounds! (" + selected + "/" + inventory.Count + ")");
            return;
        }

        int iconsToDraw = Mathf.Min(iconImageList.Count, inventory.Count);
        if (selected <= 1) // Render selected on left
        {
            for (int i = 0; i < iconsToDraw; i++)
            {
                iconImageList[i].gameObject.SetActive(true);
                iconImageList[i].sprite = iconSpriteList[inventory[i].GetWeaponType()];
            }
            selectBorder.rectTransform.anchoredPosition = iconImageList[selected].rectTransform.anchoredPosition;
        }
        else if (selected < inventory.Count - 2) // Render selected in the center (I assert that inventory.Count >= 3)
        {
            for (int i = 0; i < iconsToDraw; i++)
            {
                iconImageList[i].gameObject.SetActive(true);
                iconImageList[i].sprite = iconSpriteList[inventory[selected - (2 - i)].GetWeaponType()];
            }
            selectBorder.rectTransform.anchoredPosition = iconImageList[iconImageList.Count / 2].rectTransform.anchoredPosition;
        }
        else // selected >= inventory.Count - 2 && select < inventory.Count
        {
            int shoulderIndex = iconsToDraw - (inventory.Count - selected);
            for (int i = 0; i < iconsToDraw; i++)
            {
                iconImageList[i].gameObject.SetActive(true);
                iconImageList[i].sprite = iconSpriteList[inventory[inventory.Count - (iconsToDraw - i)].GetWeaponType()];
            }
            selectBorder.rectTransform.anchoredPosition = iconImageList[shoulderIndex].rectTransform.anchoredPosition;
        }

        selectBorder.gameObject.SetActive(true);
        for (int i = iconsToDraw; i < iconImageList.Count; i++)
            iconImageList[i].gameObject.SetActive(false);
        indexText.gameObject.SetActive(true);
        indexText.text = "(" + (selected + 1).ToString() + "/" + inventory.Count + ")";
    }
}
