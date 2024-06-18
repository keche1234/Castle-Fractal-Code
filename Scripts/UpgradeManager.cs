using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    protected bool upgrading = false;
    [SerializeField] protected PlayerController player;

    [Header("Upgrades")]
    [SerializeField] protected RankUpButton healthUpgradeButton;
    [SerializeField] protected RankUpButton inventoryUpgradeButton;
    [SerializeField] protected RankUpButton signatureUpgradeButton;
    [SerializeField] protected RankUpInfoBoxUI infoBox;
    protected int rank;

    // Visual Elements
    protected float buttonAppearanceInterval = 0.5f;
    protected float buttonAppearanceDelay = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartUpgradeSequence()
    {
        upgrading = true;
        // TODO: Remove Player Control

        // TODO: Wait until player is not attacking:
        
        // TODO: Shrink player
        // TODO: Move player to stage center
        // TODO: Grow player

        // TODO: Reveal Health Upgrade Button, Grow, Disable Component
        // TODO: Reveal Inventory Upgrade Button, Grow, Disable Component
        // TODO: Reveal Signature Upgrade Button, Grow, Disable Component

        // TODO: Enable Buttons Components, Reveal Info Box

        yield return null;
    }

    public void EndUpgradeSequenceFunction()
    {
        StartCoroutine(EndUpgradeSequence());
    }

    public IEnumerator EndUpgradeSequence()
    {
        // TODO: Disable Button Components, linger for interval + delay

        // TODO: Hide Info Box, Shrink Buttons and hide them

        // TODO: Restore player movement

        upgrading = false;
        yield return null;
    }

    public bool IsUpgrading()
    {
        return upgrading;
    }
}
