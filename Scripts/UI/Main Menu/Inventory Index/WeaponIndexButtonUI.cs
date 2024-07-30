using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIndexButtonUI : InventoryIndexButtonUI
{
    [SerializeField] protected string weaponType;
    protected string basePower;
    protected string baseDurability;
    protected string signatureCapacity;
    protected bool validWeapon = true;
    // Start is called before the first frame update
    void Start()
    {
        switch (weaponType.ToLower())
        {
            case "sword":
                basePower = Sword.GetBasePower().ToString("0");
                baseDurability = Sword.GetBaseDurability().ToString("0");
                signatureCapacity = Sword.GetSignatureCapacity().ToString();
                break;
            case "axe":
                basePower = Axe.GetBasePower().ToString("0");
                baseDurability = Axe.GetBaseDurability().ToString("0");
                signatureCapacity = Axe.GetSignatureCapacity().ToString();
                break;
            case "spear":
                basePower = Spear.GetBasePower().ToString("0");
                baseDurability = Spear.GetBaseDurability().ToString("0");
                signatureCapacity = Spear.GetSignatureCapacity().ToString();
                break;
            case "crossbow":
                basePower = Crossbow.GetBasePower().ToString("0");
                baseDurability = Crossbow.GetBaseDurability().ToString("0");
                signatureCapacity = Crossbow.GetSignatureCapacity().ToString();
                break;
            case "tome":
                basePower = Tome.GetBasePower().ToString("0");
                baseDurability = Tome.GetBaseDurability().ToString("0");
                signatureCapacity = Tome.GetSignatureCapacity().ToString();
                break;
            default:
                basePower = "--";
                baseDurability = "--";
                signatureCapacity = "---";
                validWeapon = false;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetBasePower()
    {
        return basePower;
    }

    public string GetBaseDurability()
    {
        return baseDurability;
    }

    public string GetSignatureCapacity()
    {
        return signatureCapacity;
    }

    public bool IsValidWeapon()
    {
        return validWeapon;
    }
}
