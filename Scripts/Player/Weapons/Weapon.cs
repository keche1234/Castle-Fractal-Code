using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
    //[Header("Basic Attributes")]
    //[SerializeField] protected float power;
    //[SerializeField] protected int maxDurability;
    protected bool heavy; //Player cannot move and use heavy weapons at the same time
    protected bool melee;
    protected Enemy autoTarget = null;

    [Header("Player + Cam")]
    protected ActionState state;
    [SerializeField] protected PlayerController owner;
    [SerializeField] protected Camera cam;
    [SerializeField] protected Canvas reticle;
    [SerializeField] protected Canvas subReticle;
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

    /************
     * Parameters
     ************/
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

    public static float GetSignatureCapacity()
    {
        return 0;
    }

    /************
     * Attacking
     ************/
    public abstract IEnumerator Attack(InputDevice device);
    public abstract IEnumerator Signature(InputDevice device);

   /******************
    * Aim Mechanics
    ******************/
    protected void FindAutoTarget()
    {
        if (IsInactive())
        {
            // Find AutoTarget
            if (melee && owner.GetMeleeAuto() || owner.GetRangedAssist() > 0)
            {
                /*****************************************************
                 * Determine what the player might be trying to aim at
                 *****************************************************/
                RaycastHit hit;
                Vector3 aimVector = Vector3.zero;
                if (owner.GetActionInputDevice("main attack") == Mouse.current)
                {
                    // Wherever the mouse hovers, treat that as your forward
                    aimVector = reticle.transform.position - owner.transform.position;
                    aimVector = new Vector3(aimVector.x, 0, aimVector.z).normalized;
                }
                else if (owner.GetActionInputDevice("main attack") == Keyboard.current)
                {
                    aimVector = owner.transform.forward;
                }

                /****************
                 * Set the target
                 ****************/
                if (owner.GetRangedAssist() > ControlPresetSettings.GetMaxRangedAssist())
                {
                    // Overwrite the current target if the player faces/aims at an enemy
                    if (Physics.Raycast(owner.transform.position, aimVector, out hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
                        autoTarget = hit.collider.gameObject.GetComponent<Enemy>();
                }
                else
                {
                    float angle = melee ? 360 : ControlPresetSettings.GetRangeAssistBase() * owner.GetRangedAssist();
                    Debug.DrawRay(owner.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * aimVector * 30f, Color.red);
                    Debug.DrawRay(owner.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * aimVector * 30f, Color.red);
                    autoTarget = FindClosestTarget(aimVector, ControlPresetSettings.GetRangeAssistBase() * owner.GetRangedAssist());
                }
            }
            else
                autoTarget = null;
        }
    }

    protected void RenderReticles(bool signatureCheck = false)
    {
        // Determine whether to render the reticle
        if (owner.GetActionInputDevice("main attack") == Mouse.current
            || (signatureCheck && owner.GetActionInputDevice("signature attack") == Mouse.current && owner.IsSigning()))
        {
            reticle.gameObject.SetActive(true);
            float depth = Mathf.Min(cam.transform.position.y - 3, new Vector3(0, cam.transform.position.y, cam.transform.position.z).magnitude);
            reticle.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
            reticle.transform.rotation = Quaternion.Euler(70, 0, 0);
        }
        else
            reticle.gameObject.SetActive(false);

        RenderSubReticle(autoTarget ? autoTarget.gameObject : null);

        ///**********************************
        // * Cinematic Angle
        // * TODO: DELETE ME WHEN NOT NEEDED
        // **********************************/
        //reticle.gameObject.SetActive(false);
    }

    protected void RenderSubReticle(GameObject target)
    {
        if (target)
        {
            subReticle.gameObject.SetActive(true);
            subReticle.transform.position = new Vector3(target.transform.position.x, 0.1f, target.transform.position.z);

            subReticle.transform.localScale = Vector3.one * 0.0075f;
            if (target.gameObject.GetComponent<Boss>()) subReticle.transform.localScale *= 2.5f;

            subReticle.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
            subReticle.gameObject.SetActive(false);
    }

    public Vector3 DetermineAttackDirection(InputDevice device)
    {
        Vector3 worldPoint;
        Vector3 depthVec;
        float cameraDistance;
        Vector3 dir;

        if (autoTarget)
        {
            dir = new Vector3(autoTarget.gameObject.transform.position.x - owner.transform.position.x, 0,
                                autoTarget.gameObject.transform.position.z - owner.transform.position.z).normalized;
        }
        else
        {
            if (device == Mouse.current)
            {
                worldPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
                depthVec = (worldPoint - cam.transform.position).normalized;
                cameraDistance = (cam.transform.position.y - 1f) / (depthVec.y != 0 ? Mathf.Abs(depthVec.y) : 1);
                worldPoint = cam.transform.position + (depthVec * cameraDistance);
                dir = new Vector3(worldPoint.x - owner.transform.position.x, 0, worldPoint.z - owner.gameObject.transform.position.z).normalized;
            }
            else
                dir = owner.transform.forward;
        }
        return dir;
    }

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

    /************************
     * Action State Handling
     ************************/
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

    /*****************************
     * Ability Modifier Mechanics
     *****************************/
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

    /*************
     * Targetting
     *************/
    protected Enemy FindClosestTarget()
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
            return enemies[closestIndex];
        }
        return null;
    }

    /************************************************************************
     * Find the closest target within a field of view of the owner
     * (fovDegrees means fovDegrees to the left, and fovDegrees to the right)
     ************************************************************************/
    protected Enemy FindClosestTarget(Vector3 myAim, float fovDegrees)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if (enemies.Length > 0)
        {
            // Find closest enemy
            int closestIndex = -1;
            float closestDistance = 9999999f;
            for (int i = 0; i < enemies.Length; i++)
            {
                Vector3 thisDistanceVector = new Vector3(enemies[i].transform.position.x - owner.transform.position.x, 0,
                                                enemies[i].transform.position.z - owner.transform.position.z);
                if (closestIndex == -1 || thisDistanceVector.magnitude < closestDistance)
                {
                    // Get angle between owner.transform.forward and thisDistanceVector
                    if (Vector3.Angle(myAim, thisDistanceVector) <= fovDegrees)
                    {
                        closestIndex = i;
                        closestDistance = thisDistanceVector.magnitude;
                        Debug.DrawRay(owner.transform.position, thisDistanceVector);
                    }
                }
            }
            if (closestIndex >= 0)
                return enemies[closestIndex];
            return null;
        }
        return null;
    }

    protected GameObject MeleeAutoAim()
    {
        if (!melee)
        {
            Debug.LogError("Cannot use this method for ranged weapons! You may be looking for `FindClosestTarget`.");
            return null;
        }

        if (owner.GetMeleeAuto())
        {
            Enemy closest = FindClosestTarget();
            if (closest != null)
            {
                LookAtTarget(closest.gameObject);
                return closest.gameObject;
            }
        }
        return null;
    }

    protected bool LookAtTarget(GameObject obj)
    {
        if (obj)
        {
            Vector3 direction = new Vector3(obj.transform.position.x - owner.transform.position.x, 0,
                                                    obj.transform.position.z - owner.transform.position.z).normalized;
            owner.transform.rotation = Quaternion.LookRotation(direction);
            return true;
        }

        return false;
    }

    public void OnDisable()
    {
        reticle.gameObject.SetActive(false);
    }
}
