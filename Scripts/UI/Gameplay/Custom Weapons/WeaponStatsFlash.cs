using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatsFlash : MonoBehaviour
{
    [Header("Appearing and Fading")]
    [SerializeField] protected float appearTime;
    [SerializeField] protected float fadeTime;
    protected float appearTimeRemaining = 0;
    protected float fadeTimeRemaining = 0;

    [Header("Information")]
    [SerializeField] protected Text powerText;
    [SerializeField] protected Image powerIcon;
    [SerializeField] protected Text durabilityText;
    [SerializeField] protected Image durabilityIcon;
    [SerializeField] protected List<Sprite> abilitySprites;
    [SerializeField] protected List<Image> abilityImages;
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
                powerText.gameObject.SetActive(false);
                powerIcon.gameObject.SetActive(false);
                durabilityText.gameObject.SetActive(false);
                durabilityIcon.gameObject.SetActive(false);

                foreach (Image ability in abilityImages)
                    ability.gameObject.SetActive(false);
            }
            else if (fadeTimeRemaining > 0)
            {
                fadeTimeRemaining -= Time.deltaTime;

                Color colorWhite = new Color(255, 255, 255, fadeTimeRemaining / fadeTime);
                powerText.color = colorWhite;
                powerIcon.color = colorWhite;
                durabilityText.color = colorWhite;
                durabilityIcon.color = colorWhite;

                foreach (Image ability in abilityImages)
                    ability.color = colorWhite;
            }
        }
    }

    public void DrawWeaponStatsFlash(CustomWeapon weapon)
    {
        if (weapon)
        {
            Color colorWhite = new Color(255, 255, 255, 1);
            powerText.gameObject.SetActive(true);
            powerText.color = colorWhite;
            powerIcon.gameObject.SetActive(true);
            powerIcon.color = colorWhite;
            durabilityText.gameObject.SetActive(true);
            durabilityText.color = colorWhite;
            durabilityIcon.gameObject.SetActive(true);
            durabilityIcon.color = colorWhite;

            foreach (Image ability in abilityImages)
            {
                ability.gameObject.SetActive(true);
                ability.color = colorWhite;
            }

            if (weapon.GetPower() - Mathf.Floor(weapon.GetPower()) >= 0.1f)
                powerText.text = weapon.GetPower().ToString("0.0");
            else
                powerText.text = weapon.GetPower().ToString("0");
            durabilityText.text = weapon.DecrementDurability(0).ToString();

            List<int> abilityList = weapon.GetAbilities();
            int drawCount = Mathf.Min(abilityImages.Count, abilityList.Count);
            for (int i = 0; i < drawCount; i++)
                abilityImages[i].sprite = abilitySprites[abilityList[i]];

            appearTimeRemaining = appearTime;
            fadeTimeRemaining = fadeTime;
        }
        else
        {
            powerText.gameObject.SetActive(false);
            powerIcon.gameObject.SetActive(false);
            durabilityText.gameObject.SetActive(false);
            durabilityIcon.gameObject.SetActive(false);

            foreach (Image ability in abilityImages)
                ability.gameObject.SetActive(false);
        }
    }
}
