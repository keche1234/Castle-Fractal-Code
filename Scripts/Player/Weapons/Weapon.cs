using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //[Header("Basic Attributes")]
    //[SerializeField] protected float power;
    //[SerializeField] protected int maxDurability;
    protected bool heavy; //Player cannot move and use heavy weapons at the same time
    protected bool melee;

    [Header("Player + Cam")]
    protected ActionState state;
    [SerializeField] protected PlayerController owner;
    [SerializeField] protected Camera cam;
    protected float sigSlowdown;

    protected List<GameObject> connected;

    [Header("Hitboxes")]
    [SerializeField] protected List<Hitbox> mainAttack;
    [SerializeField] protected List<Hitbox> sigAttack;
    protected List<float> sigMods;

    [Header("Initial Transform")]
    [SerializeField] protected Vector3 initPos;
    [SerializeField] protected Vector3 initAng;
    [SerializeField] protected Vector3 initScale;

    [SerializeField] protected RoomManager roomManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        state = ActionState.Inactive;
        owner = transform.parent.GetComponent<PlayerController>();
        sigSlowdown = 4;
        sigMods = new List<float>();

        for (int i = 0; i < sigAttack.Count; i++) sigMods.Add(sigAttack[i].GetDamageMod());

        Camera[] possibleCams = FindObjectsOfType<Camera>();
        foreach (Camera c in possibleCams)
        {
            if (c.name == "UI Camera")
                cam = c;
        }

        RoomManager[] possibleRMs = FindObjectsOfType<RoomManager>();
        foreach (RoomManager rm in possibleRMs)
        {
            if (rm.name == "RoomManager")
                roomManager = rm;
        }

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public abstract IEnumerator Attack();
    public abstract IEnumerator Signature();

    public void Abort()
    {
        StopAllCoroutines();
        state = ActionState.Inactive;
        //owner.SetFixDirection(false);
        InitializeTransform();
    }

    public void InitializeTransform()
    {
        transform.localPosition = initPos;
        transform.localEulerAngles = initAng;
        transform.localScale = initScale;
    }

    protected enum ActionState
    {
        Inactive = 0b_0001,
        Startup = 0b_0010,
        Active = 0b_0100,
        Cooldown = 0b_1000,
        Attacking = Startup | Active | Cooldown
    }

    public bool IsInactive()
    {
        return state == ActionState.Inactive;
    }

    public bool IsStarting()
    {
        return state == ActionState.Startup;
    }

    public bool IsActive()
    {
        return state == ActionState.Active;
    }

    public bool IsCoolingDown()
    {
        return state == ActionState.Cooldown;
    }

    public bool IsAttacking()
    {
        return (state & ActionState.Attacking) != 0;
    }

    public void SetActivity(bool b)
    {
        if (b) state = ActionState.Active;
        else state = ActionState.Inactive;
    }

    public bool IsHeavy()
    {
        return heavy;
    }

    public bool IsMelee()
    {
        return melee;
    }

    public static float GetBasePower()
    {
        return 0;
    }

    public static float GetBaseDurability()
    {
        return 0;
    }

    public void SetState(int s)
    {
        if (s == 0)
            state = ActionState.Inactive;
        else if (s == 1)
            state = ActionState.Startup;
        else if (s == 2)
            state = ActionState.Active;
        else if (s == 3)
            state = ActionState.Cooldown;
    }

    protected float CalculateRate()
    {
        CustomWeapon current = owner.GetCustomWeapon();
        if (current != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "AttackRateUp");
            if (current.GetAbilities().Contains(place))
                return 1 + GetComponents<Ability>()[place].GetModifier();
        }
        return 1;
    }

    protected float CalculateRange()
    {
        CustomWeapon current = owner.GetCustomWeapon();
        if (current != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "AttackRangeUp");
            if (current.GetAbilities().Contains(place))
                return 1 + GetComponents<Ability>()[place].GetModifier();
        }
        return 1;
    }

    protected float CalculateSignatureDamage()
    {
        CustomWeapon current = owner.GetCustomWeapon();
        if (current != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "SignatureDamageUp");
            if (current.GetAbilities().Contains(place))
                return 1 + GetComponents<Ability>()[place].GetModifier();
        }
        return 1;
    }

    protected float CalculateSignatureDuration()
    {
        CustomWeapon current = owner.GetCustomWeapon();
        if (current != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "SignatureDurationUp");
            if (current.GetAbilities().Contains(place))
                return 1 + GetComponents<Ability>()[place].GetModifier();
        }
        return 1;
    }

    protected GameObject AutoAim()
    {
        //Auto Aiming
        if (owner.GetMeleeAuto())
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            if (enemies.Length > 0)
            {
                // Find closest enemy
                int closestIndex = 0;
                float closestDistance = new Vector3(owner.transform.position.x - enemies[0].transform.position.x, 0,
                                                    owner.transform.position.z - enemies[0].transform.position.z).magnitude;
                for (int i = 1; i < enemies.Length; i++)
                {
                    float thisDistance = new Vector3(owner.transform.position.x - enemies[i].transform.position.x, 0,
                                                    owner.transform.position.z - enemies[i].transform.position.z).magnitude;
                    if (thisDistance < closestDistance)
                    {
                        closestIndex = i;
                        closestDistance = thisDistance;
                    }
                }
                LookAtTarget(enemies[closestIndex].gameObject);
                return enemies[closestIndex].gameObject;
            }
        }
        return null;
    }

    protected void LookAtTarget(GameObject obj)
    {
        Vector3 direction = new Vector3(obj.transform.position.x - owner.transform.position.x, 0,
                                                obj.transform.position.z - owner.transform.position.z).normalized;
        owner.transform.rotation = Quaternion.LookRotation(direction);
    }
}
