using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponIndexInfoBoxUI : InventoryIndexInfoBoxUI
{
    [SerializeField] protected TextMeshProUGUI power;
    [SerializeField] protected TextMeshProUGUI durability;
    [SerializeField] protected TextMeshProUGUI signature;
    [SerializeField] protected Image signatureTitleIcon;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ClearDescription()
    {
        base.ClearDescription();
    }

    public void DrawDescription(WeaponIndexButtonUI weapon)
    {
        base.DrawDescription();
        SetIcon(weapon);
        SetTitles(weapon);
        SetDescriptions(weapon);

        power.text = weapon.GetBasePower();
        durability.text = weapon.GetBaseDurability();
        signature.text = weapon.GetSignatureCapacity();

        bool valid = weapon.IsValidWeapon();
        signatureTitleIcon.gameObject.SetActive(valid);
        titles[1].gameObject.SetActive(valid);
        descriptions[1].gameObject.SetActive(valid);
    }
}
