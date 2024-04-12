using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCW : MonoBehaviour
{
    [SerializeField] protected int weapon; //type: 0 --> 4 are Sword, Axe, Spear, Crossbow, Tome
    [Header("Basic Attributes")]
    [SerializeField] protected float power;
    [SerializeField] protected float currentDurability;
    [SerializeField] protected float maxDurability; //Non-positive => Infinite Durability
    [SerializeField] protected bool canPickup;
    [SerializeField] protected int signatureGauge;

    [Header("Abilities (type and potency)")]
    [SerializeField] protected List<int> abilities;
    [SerializeField] protected List<float> mods; //parallel array

    [Header("Visual")]
    [SerializeField] protected List<GameObject> models;
    protected Vector3 startPos;
    [SerializeField] protected float hoverOffset;
    [SerializeField] protected float hoverSpeed;

    //[SerializeField] protected Text mightPoints;
    private float hoverDist = 0;
    private int direction = 1; //+1 is up, -1 is down

    [Header("Game Management")]
    [SerializeField] protected RoomManager roomManager;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < models.Count; i++)
            models[i].gameObject.SetActive(i == weapon);

        startPos = models[weapon].transform.localPosition;
        canPickup = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hoverDist > hoverOffset) direction = -1;
        else if (hoverDist < -hoverOffset) direction = 1;

        hoverDist += hoverSpeed * Time.deltaTime * direction;
        models[weapon].transform.localPosition = Vector3.Lerp(startPos - new Vector3(0, hoverOffset, 0), startPos + new Vector3(0, hoverDist, 0), (hoverDist + hoverOffset)/ (hoverOffset * 2));

    }

    public void Initialize(int w, float pow, float cDur, float mDur, int sg, List<int> a, List<float> m)
    {
        weapon = w;
        power = pow;
        currentDurability = cDur;
        maxDurability = mDur;
        signatureGauge = sg;
        abilities = new List<int>();
        mods = new List<float>();

        if (a != null)
            for (int i = 0; i < a.Count; i++)
                abilities.Add(a[i]);

        if (m != null)
            for (int i = 0; i < m.Count; i++)
            {
                mods.Add(m[i]);
                //GameObject empty = new GameObject();
                //empty.AddComponent(System.Type.GetType(Ability.names[abilities[i]]));
                //Ability ab = empty.GetComponent<Ability>();
                //ab.SetModifier(m[i]);
                //mightMod += ab.GetMightMult();
            }
    }

    public int CalculateMightPoints()
    {
        float mightMod = 1;

        for (int i = 0; i < abilities.Count; i++)
        {
            GameObject empty = new GameObject();
            empty.AddComponent(System.Type.GetType(Ability.GetNames()[abilities[i]]));
            Ability ab = empty.GetComponent<Ability>();
            ab.SetModifier(mods[i]);
            mightMod += ab.GetMightMult();
            Destroy(empty);
        }

        switch (weapon)
        {
            case 0: //Sword
                mightMod *= power / Sword.GetBasePower();
                mightMod *= maxDurability / Sword.GetBaseDurability();
                break;
            case 1: //Axe
                mightMod *= power / Axe.GetBasePower();
                mightMod *= maxDurability / Axe.GetBaseDurability();
                break;
            case 2: //Spear
                mightMod *= power / Spear.GetBasePower();
                mightMod *= maxDurability / Spear.GetBaseDurability();
                break;
            case 3: //Crossbow
                mightMod *= power / Crossbow.GetBasePower();
                mightMod *= maxDurability / Crossbow.GetBaseDurability();
                break;
            case 4: //Tome
                mightMod *= power / Tome.GetBasePower();
                mightMod *= maxDurability / Tome.GetBaseDurability();
                break;
            default:
                break;
        }

        return (int) (100 * mightMod);
    }

    public void OnTriggerEnter(Collider targetCollider)
    {
        GameObject target = targetCollider.gameObject;
        if (canPickup && target.CompareTag("Player")) // && target.GetComponent<PlayerController>().RemainingMP() >= mp)
        {
            //Create the custom weapon
            CustomWeapon w = (CustomWeapon)ScriptableObject.CreateInstance("CustomWeapon");

            //Initialize
            w.Setup(weapon, power, currentDurability, maxDurability, signatureGauge, abilities, mods);
            if (target.GetComponent<PlayerController>().RemainingMP() >= CalculateMightPoints())
            {
                //Give
                PlayerController pc = targetCollider.gameObject.GetComponent<PlayerController>();
                pc.GiveCustomWeapon(w);
                if (pc.InvCount() == 1) pc.SetCustomWeapon(0);

                //Destroy
                Destroy(gameObject);
            }
        }

        if (target.CompareTag("Wall")) //assume a bounce
        {
            gameObject.GetComponent<Rigidbody>().velocity *= -1;
        }
    }

    public void SetPickup(bool b)
    {
        canPickup = b;
    }

    public float GetPower()
    {
        return power;
    }

    public float GetDurability()
    {
        return currentDurability;
    }

    public List<int> GetAbilities()
    {
        List<int> a = new List<int>();
        for (int i = 0; i < abilities.Count; i++) a.Add(abilities[i]);
        return a;
    }
}
