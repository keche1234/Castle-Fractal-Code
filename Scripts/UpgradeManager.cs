using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UpgradeManager : MonoBehaviour
{
    protected bool upgrading = false;
    protected PlayerController player;

    [Header("Upgrades")]
    [Tooltip("Should have Upgrade Buttons as children")]
    [SerializeField] protected GameObject upgradeHolder;
    [SerializeField] protected List<RankUpButton> upgradeButtons;
    [SerializeField] protected RankUpInfoBoxUI infoBox;
    protected int rank;

    [Header("Visual Elements")]
    [SerializeField] protected Image blackFade;
    [SerializeField] protected float baseButtonScale;
    protected const float PLAYER_SCALE_TIME = 0.25f;
    protected const float BUTTON_APPEARANCE_DELAY = 0.25f;
    protected const float BUTTON_APPEARANCE_INTERVAL = 0.5f;
    [SerializeField] protected EventSystem eventSystem;

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

        player = FindObjectOfType<PlayerController>();
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
        Cursor.visible = true;

        // Remove Player Control
        player.SetControllable(false);
        player.OverrideInvincibility(10);
        player.GetComponent<Rigidbody>().velocity *= 0;

        foreach (RankUpButton button in upgradeButtons)
            button.gameObject.transform.localScale = Vector3.zero;

        // Wait until player is not attacking:
        yield return new WaitUntil(() => player.GetAttackState() == 0 && player.GetCurrentWeaponAttackState() <= 0);
        player.SetControllable(false);

        player.FullyRestoreHealth();
        // Shrink player, move them to room center, and regrow
        for (float t = 0; t < PLAYER_SCALE_TIME; t += Time.deltaTime)
        {
            player.GetComponent<Rigidbody>().velocity *= 0;
            player.transform.localScale = Vector3.one * (1 - (t / PLAYER_SCALE_TIME));
            player.OverrideInvincibility(Time.deltaTime * 2);
            yield return null;
        }
        player.transform.localScale = Vector3.zero;
        yield return null;

        blackFade.enabled = true;
        player.transform.position = new Vector3(0, player.transform.position.y, -4f);
        player.transform.rotation = Quaternion.Euler(0, 180, 0);
        for (float t = 0; t < PLAYER_SCALE_TIME; t += Time.deltaTime)
        {
            player.GetComponent<Rigidbody>().velocity *= 0;
            player.transform.localScale = Vector3.one * (t / PLAYER_SCALE_TIME);
            yield return null;
        }
        player.transform.localScale = Vector3.one;
        player.ResetCurrentWeapon();
        yield return null;

        // Grow Buttons to reveal them
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            player.OverrideInvincibility(BUTTON_APPEARANCE_DELAY * 2);
            yield return new WaitForSeconds(BUTTON_APPEARANCE_DELAY);
            upgradeButtons[i].gameObject.GetComponent<Button>().enabled = false;
            upgradeButtons[i].gameObject.GetComponent<ButtonColorManipulation>().Select(false);
            upgradeButtons[i].gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(false);
            for (float t = 0; t < BUTTON_APPEARANCE_INTERVAL; t += Time.deltaTime)
            {
                upgradeButtons[i].gameObject.transform.localScale = Vector3.one * baseButtonScale * (t / BUTTON_APPEARANCE_INTERVAL);
                yield return null;
            }
            upgradeButtons[i].gameObject.transform.localScale = Vector3.one * baseButtonScale;
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
        infoBox.transform.parent.gameObject.SetActive(true);
        if (player.GetActionInputDevice("main attack") == Keyboard.current)
        {
            int i = 0;
            while (i < upgradeButtons.Count)
            {
                Debug.Log(upgradeButtons[i].gameObject.GetComponent<Button>().enabled);
                if (upgradeButtons[i].gameObject.GetComponent<Button>().enabled)
                {
                    Debug.Log(upgradeButtons[i]);
                    eventSystem.SetSelectedGameObject(upgradeButtons[i].gameObject);
                    upgradeButtons[i].GetComponent<ButtonColorManipulation>().whenHover();
                    i = upgradeButtons.Count;
                }
                i++;
            }
        }

        player.OverrideInvincibility(0);
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
        {
            upgradeButtons[i].gameObject.GetComponent<Button>().enabled = false;
            if (upgradeButtons[i] != rankUpButton)
                upgradeButtons[i].gameObject.GetComponent<ButtonColorManipulation>().ActivateColorManipulation(false);
        }

        yield return new WaitForSeconds((BUTTON_APPEARANCE_DELAY + BUTTON_APPEARANCE_INTERVAL) * 2);

        // Hide Info Box, Shrink Buttons and hide them
        infoBox.transform.parent.gameObject.SetActive(false);
        for (float t = 0; t < BUTTON_APPEARANCE_INTERVAL; t += Time.deltaTime)
        {
            foreach (RankUpButton button in upgradeButtons)
                button.gameObject.transform.localScale = Vector3.one * baseButtonScale * (1 - (t / BUTTON_APPEARANCE_INTERVAL));
            yield return null;
        }
        foreach (RankUpButton button in upgradeButtons)
            button.gameObject.transform.localScale = Vector3.zero;

        if (player.IsAttributeMaxRank(rankUpButton.GetAttributeIndex()))
        {
            rankUpButton.EmptyButton(); // Button color is darkened
        }

        // Restore player control
        player.SetControllable(true);
        upgradeHolder.SetActive(false);
        Cursor.visible = false;
        blackFade.enabled = false;
        yield return null;
    }

    public bool IsUpgrading()
    {
        return upgrading;
    }
}
