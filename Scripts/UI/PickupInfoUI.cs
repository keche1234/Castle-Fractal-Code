using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupInfoUI : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] PickupCW pickup;
    protected float infoRange = 2;
    protected float scaleCap;
    protected int infoLayerMask;

    [Header("The Box")]
    [SerializeField] protected GameObject group;
    protected float spawnScaleSpeed;

    [Header("UI Info")] // Update these based on the pickup
    [SerializeField] protected Text power;
    [SerializeField] protected Text durability;
    [SerializeField] protected Text mightPoints;
    [SerializeField] protected List<Image> myAbilityIcons; //For my two abilities

    [Header("All Ability Icons")]
    [SerializeField] protected List<Sprite> abilityIconSprites; //For every ability

    PlayerController player;
    UIAttach uiAttach;

    // Start is called before the first frame update
    void Start()
    {
        if (pickup == null) Debug.LogError("Need a weapon pickup attached!");

        if (group == null)
        {
            Debug.LogError("Need a group full of info attached!");
            scaleCap = 0;
        }
        else
        {
            scaleCap = (group.transform.localScale.x + group.transform.localScale.y + group.transform.localScale.z) / 3;
            group.transform.localScale *= 0;
        }

        if (power == null) Debug.LogError("Need power text attached!");
        else power.text = "P: ";

        if (durability == null) Debug.LogError("Need durability text attached!");
        else durability.text = "D: ";

        if (mightPoints == null) Debug.LogError("Need might points text attached!");
        else mightPoints.text = "M: ";
        spawnScaleSpeed = scaleCap * 12;

        player = FindObjectOfType<PlayerController>();
        uiAttach = group.GetComponent<UIAttach>();
        uiAttach.NewCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
    }

    // Update is called once per frame
    void Update()
    {
        power.text = "P: " + pickup.GetPower();
        durability.text = "D: " + pickup.GetDurability();
        mightPoints.text = "M: " + pickup.CalculateMightPoints();

        List<int> abilities = pickup.GetAbilities();
        for (int i = 0; i < myAbilityIcons.Count; i++)
            myAbilityIcons[i].sprite = abilityIconSprites[abilities[i]];
        if (player != null)
        {
            if ((player.gameObject.transform.position - transform.position).magnitude <= infoRange)
            {
                Debug.Log(player.gameObject.transform.position.z + " vs " + transform.position.z);
                if (player.gameObject.transform.position.z > transform.position.z)
                    uiAttach.NewOffset(new Vector2(0, -120));
                else
                    uiAttach.NewOffset(new Vector2(0, 120));
                Vector3 scale = group.transform.localScale;
                group.transform.localScale = new Vector3(Mathf.Clamp(scale.x + (spawnScaleSpeed * Time.deltaTime), 0, scaleCap),
                                                        Mathf.Clamp(scale.y + (spawnScaleSpeed * Time.deltaTime), 0, scaleCap),
                                                        Mathf.Clamp(scale.z + (spawnScaleSpeed * Time.deltaTime), 0, scaleCap));
            }
            else
            {
                Vector3 scale = group.transform.localScale;
                group.transform.localScale = new Vector3(Mathf.Clamp(scale.x - (spawnScaleSpeed * Time.deltaTime), 0, scaleCap),
                                                        Mathf.Clamp(scale.y - (spawnScaleSpeed * Time.deltaTime), 0, scaleCap),
                                                        Mathf.Clamp(scale.z - (spawnScaleSpeed * Time.deltaTime), 0, scaleCap));
                
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //Show the box, update the values
        if (other.gameObject.CompareTag("Player"))
        {
            group.SetActive(true);
            power.text = "P: " + pickup.GetPower();
            durability.text = "D: " + pickup.GetDurability();
            mightPoints.text = "M: " + pickup.CalculateMightPoints();

            List<int> abilities = pickup.GetAbilities();
            for (int i = 0; i < myAbilityIcons.Count; i++)
                myAbilityIcons[i].sprite = abilityIconSprites[abilities[i]];
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Hide the box
        if (other.gameObject.CompareTag("Player"))
        {
            group.SetActive(false);
        }
    }
}
