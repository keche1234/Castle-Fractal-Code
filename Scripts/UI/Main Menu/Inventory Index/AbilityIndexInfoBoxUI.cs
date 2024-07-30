using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityIndexInfoBoxUI : InventoryIndexInfoBoxUI
{
    [SerializeField] protected TextMeshProUGUI abilityRangeDescription;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawDescription(AbilityIndexButtonUI ability)
    {
        base.DrawDescription();
        SetIcon(ability);
        SetTitles(ability);
        SetDescriptions(ability);

        abilityRangeDescription.gameObject.SetActive(ability.IsNumeric());
        abilityRangeDescription.text = "Range: " + ability.GetMinValue() + "-" + ability.GetMaxValue();
    }
}
