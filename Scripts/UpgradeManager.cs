using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    protected bool upgrading = false;
    [SerializeField] protected PlayerController player;

    [Header("Upgrades")]
    [SerializeField] protected Text titleMessage;
    [Tooltip("Should have Upgrade Buttons as children")]
    [SerializeField] protected GameObject upgradeHolder;
    [SerializeField] protected List<RankUpButton> upgradeButtons;
    [SerializeField] protected RankUpInfoBoxUI infoBox;
    protected int rank;

    [Header("Visual Elements")]
    [SerializeField] protected Image blackFade;
    protected const float PLAYER_SCALE_TIME = 0.2f;
    protected const float BUTTON_APPEARANCE_DELAY = 0.25f;
    protected const float BUTTON_APPEARANCE_INTERVAL = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Check to make sure every upgrade button is a child of the upgrade holder
        Component[] childrenArray = upgradeHolder.GetComponentsInChildren(System.Type.GetType("RankUpButton"));
        List<RankUpButton> childrenList = new List<RankUpButton>();

        for (int i = 0; i < childrenArray.Length; i++)
            childrenList.Add((RankUpButton)childrenArray[i]);

        foreach (RankUpButton button in upgradeButtons)
        {
            if (!childrenList.Contains(button))
                Debug.LogWarning("Upgrade Holder does not contain button " + button);
        }

        // TEMP CODE
        StartCoroutine(StartUpgradeSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // To be called by SpawnManager
    public IEnumerator StartUpgradeSequence()
    {
        upgrading = true;
        upgradeHolder.SetActive(true);

        // Remove Player Control
        player.SetControllable(false);

        // Wait until player is not attacking:
        yield return new WaitUntil(() => player.GetAttackState() == 0 && player.GetCurrentWeaponAttackState() <= 0);

        player.FullyRestoreHealth();
        // Shrink player, move them to room center, and regrow
        for (float t = 0; t < PLAYER_SCALE_TIME; t += Time.deltaTime)
        {
            player.transform.localScale = Vector3.one * (1 - (t / PLAYER_SCALE_TIME));
            yield return null;
        }
        player.transform.localScale = Vector3.zero;
        yield return null;

        blackFade.enabled = true;
        player.transform.position = new Vector3(0, player.transform.position.y, 0);
        for (float t = 0; t < PLAYER_SCALE_TIME; t += Time.deltaTime)
        {
            player.transform.localScale = Vector3.one * (t / PLAYER_SCALE_TIME);
            yield return null;
        }
        player.transform.localScale = Vector3.one;
        yield return null;

        // Grow Buttons to reveal them
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            yield return new WaitForSeconds(BUTTON_APPEARANCE_DELAY);
            upgradeButtons[i].gameObject.GetComponent<Button>().enabled = false;
            upgradeButtons[i].gameObject.GetComponent<ButtonColorManipulation>().Select(false);
            upgradeButtons[i].gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(false);
            for (float t = 0; t < BUTTON_APPEARANCE_INTERVAL; t += Time.deltaTime)
            {
                upgradeButtons[i].gameObject.transform.localScale = Vector3.one * (t / BUTTON_APPEARANCE_INTERVAL);
                yield return null;
            }
            upgradeButtons[i].gameObject.transform.localScale = Vector3.one;
        }
        yield return new WaitForSeconds(BUTTON_APPEARANCE_DELAY);

        // Enable Button Components, Reveal Info Box
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (!player.IsAttributeMaxRank(upgradeButtons[i].GetAttributeIndex()))
            {
                upgradeButtons[i].gameObject.GetComponent<Button>().enabled = true;
                upgradeButtons[i].gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(true);
            }
        }
        infoBox.gameObject.SetActive(true);

        yield return null;
    }

    // To be called by Button
    public void FinishUpgradeSequenceFunction(RankUpButton rankUpButton)
    {
        if (upgrading)
            StartCoroutine(FinishUpgradeSequence(rankUpButton));
    }

    protected IEnumerator FinishUpgradeSequence(RankUpButton rankUpButton)
    {
        upgrading = false;
        player.RankUp(rankUpButton.GetAttributeIndex());
        // Disable Button Components, linger for (interval + delay) * 2
        for (int i = 0; i < upgradeButtons.Count; i++)
            upgradeButtons[i].gameObject.GetComponent<Button>().enabled = false;

        yield return new WaitForSeconds((BUTTON_APPEARANCE_DELAY + BUTTON_APPEARANCE_INTERVAL) * 2);

        if (player.IsAttributeMaxRank(rankUpButton.GetAttributeIndex()))
        {
            rankUpButton.EmptyButton(); // Button color is darkened
        }

        // Hide Info Box, Shrink Buttons and hide them
        infoBox.gameObject.SetActive(false);
        for (float t = 0; t < BUTTON_APPEARANCE_INTERVAL; t += Time.deltaTime)
        {
            foreach (RankUpButton button in upgradeButtons)
                button.gameObject.transform.localScale = Vector3.one * (1 - (t / PLAYER_SCALE_TIME));
            yield return null;
        }
        foreach (RankUpButton button in upgradeButtons)
            button.gameObject.transform.localScale = Vector3.zero;

        // Restore player control
        player.SetControllable(true);

        upgradeHolder.SetActive(false);
        yield return null;
    }

    public bool IsUpgrading()
    {
        return upgrading;
    }
}
