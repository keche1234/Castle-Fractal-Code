using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A wrapper for various InventoryIndex menu functions
 */
public class InventoryIndexMainMenu : MonoBehaviour
{
    [SerializeField] protected WeaponIndexInfoBoxUI weaponInfo;
    [SerializeField] protected AbilityIndexInfoBoxUI abilityInfo;
    [SerializeField] protected PotionIndexInfoBoxUI potionInfo;

    [SerializeField] protected float clearDelay; // To prevent a "flickering" effect that occurs due to spaced out buttons.
    protected bool needToClear = false;
    protected float clearTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (needToClear)
        {
            clearTimer -= Time.deltaTime;
            if (clearTimer <= 0)
                TrueClearDescription();
        }
    }

    public void ClearDescription()
    {
        needToClear = true;
        clearTimer = clearDelay;
    }

    protected void TrueClearDescription()
    {
        weaponInfo.ClearDescription();
        abilityInfo.ClearDescription();
        potionInfo.ClearDescription();
        needToClear = false;
    }

    public void DrawDescription(WeaponIndexButtonUI weapon)
    {
        needToClear = false;
        abilityInfo.ClearDescription();
        potionInfo.ClearDescription();

        weaponInfo.DrawDescription(weapon);
        //weaponInfo.gameObject.SetActive(true);
    }

    public void DrawDescription(AbilityIndexButtonUI ability)
    {
        needToClear = false;
        weaponInfo.ClearDescription();
        potionInfo.ClearDescription();

        abilityInfo.DrawDescription(ability);
        //abilityInfo.gameObject.SetActive(true);
    }

    public void DrawDescription(PotionIndexButtonUI potion)
    {
        needToClear = false;
        weaponInfo.ClearDescription();
        abilityInfo.ClearDescription();

        potionInfo.DrawDescription(potion);
        //potionInfo.gameObject.SetActive(true);
    }
}
