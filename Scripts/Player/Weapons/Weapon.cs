using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Basic Attributes")]
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
        Inactive,
        Startup,
        Active,
        Cooldown
    }

    public bool IsActive()
    {
        return state == ActionState.Active;
    }

    public bool IsInactive()
    {
        return state == ActionState.Inactive;
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
}
