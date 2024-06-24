using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : Character
{
    private Rigidbody playerRb;

    //private float horizontalInput;
    //private float verticalInput;
    protected Vector2 moveInputVector;
    protected float scrollDelta;

    //character states
    private bool mobile;
    private AttackState playerAttack;
    private DodgeState playerDodge;
    private LifeState playerLife;
    private bool stunned;
    private bool controllable = true;
    private bool signing = false;
    //private bool fixDirection; //Are you looking in one direction while moving

    [SerializeField] protected BarUI miniDurabilityBar;
    [SerializeField] protected BarUI miniDodgeBar;
    [SerializeField] protected BarUI healthBar;
    [SerializeField] protected BarUI inventoryBar;
    [SerializeField] protected BarUI signatureBar;

    [Header("Camera")]
    [SerializeField] protected Camera cam;

    [Header("Controls")]
    [SerializeField] protected PlayerInputActions inputActions;
    protected List<string> controlSchemes;
    [SerializeField] protected List<ControlPresetSettings> controlSettings;

    // TODO: Program Targeting Reticle Settings
    //[SerializeField] protected List<string> actions;
    //[SerializeField] protected ControlManager controlManager;
    //protected Dictionary<string, List<KeyCode>> myControls;
    //protected Dictionary<string, List<int>> mouseControls;

    //[Header("Control Settings")]
    [SerializeField] protected bool signatureCombo;
    [SerializeField] protected bool meleeAuto;
    [SerializeField] protected float rangedAssist;
    [SerializeField] protected float scrollSensitivity;

    protected float invScrollPos; //inventory scroll position, ranges from 0-(inventory.Count + .99), cut off decimal for current weapon

    [SerializeField] protected float signatureMultiplier;

    [Header("Weapon Management")]
    protected int currentWeaponType = -1;
    [SerializeField] protected Weapon[] weaponTypes;
    [SerializeField] protected int equippedCustomWeapon; //current index
    [SerializeField] protected List<CustomWeapon> inventory;
    [SerializeField] protected int currentMP;
    [SerializeField] protected int maxMP;
    [SerializeField] protected PickupCW pickupPrefab; //for dropping a weapon
    [SerializeField] protected WeaponButton leftWeaponButtonUI;
    [SerializeField] protected WeaponButton rightWeaponButtonUI;

    //[Header("All Or Nothing Timers")]
    protected float aonDurabilityTimer = 0;
    protected float aonSignatureTimer = 0;

    [Header("Potion Management")]
    [SerializeField] protected List<int> potions; //Capacity: 6. 1 is Strength +2, 2 is Defense +2, 3 is Speed +20%,
                                                  //             4 is Sig Fill Rate +20%, 5 is Stun, 6 is Saving
    [SerializeField] protected int potionCapacity = 6;
    [SerializeField] protected List<bool> selectedPotions; //-1 means nothing is selected
    [SerializeField] protected float potionDuration = 20f;
    [SerializeField] protected int potionBuffInt = 3;
    [SerializeField] protected float potionBuffSpeedSig = 0.3f;
    [SerializeField] protected float potionBuffRegen = 0.06f;
    [SerializeField] protected float potionRegenTick = 4;

    [Header("Unarmed Strike")]
    [SerializeField] private Hitbox unarmedStrike;
    [SerializeField] private float startupTime;
    [SerializeField] private float activeTime;
    [SerializeField] private float cooldownTime;

    [Header("Dodging")]
    [SerializeField] protected float dodgeCool;
    //[SerializeField] protected float baseSigFillRate = 4; // This many pts per percentage of health save when dodging.
    protected TrailRenderer dodgeTrail;

    protected int[] ranks; // Health, Inventory, Signature Multiplier
    protected float[] rankGrowths = { 0.5f, 0.5f, 0.25f }; // Linear
    protected int baseHealthCap;
    protected int baseMPCap;
    protected const int MAX_RANK = 9;

    protected UpgradeManager upgradeManager;
    protected EventSystem eventSystem;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        attributesUI.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -110));
        miniDurabilityBar.gameObject.transform.parent.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        miniDurabilityBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -70));

        miniDodgeBar.gameObject.transform.parent.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        miniDodgeBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, 70));
        miniDodgeBar.gameObject.SetActive(false);

        Cursor.visible = false;
        playerRb = GetComponent<Rigidbody>();
        playerAttack = AttackState.NotAttacking;
        playerDodge = DodgeState.Available;
        playerLife = LifeState.Alive;

        baseHealthCap = 30;
        baseMPCap = 1000;

        currentHealth = baseHealthCap;
        maxHealth = baseHealthCap;
        currentMP = 0;
        maxMP = baseMPCap;
        speed = 3.5f;
        signatureMultiplier = 1;

        invScrollPos = 0;

        ranks = new int[3];

        mobile = true;
        //controllable = true;
        int abilityCount = Ability.GetNames().Length;
        for (int w = 0; w < weaponTypes.Length; w++)
        {
            weaponTypes[w].gameObject.SetActive(true);
            for (int i = 0; i < abilityCount; i++)
            {
                Ability a = (Ability)weaponTypes[w].gameObject.AddComponent(System.Type.GetType(Ability.GetNames()[i]));
                a.SetUser(this);
                a.enabled = false;
            }
        }

        equippedCustomWeapon = -1;
        currentWeaponType = -1;
        inventory = new List<CustomWeapon>();
        potions = new List<int>();
        selectedPotions = new List<bool>();
        for (int i = 0; i < potionCapacity; i++)
            selectedPotions.Add(false);

        SetCustomWeapon(-1);
        dodgeTrail = GetComponent<TrailRenderer>();

        upgradeManager = FindObjectOfType<UpgradeManager>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Awake()
    {
        // Initialize Controls
        inputActions = new PlayerInputActions();
        controlSchemes = new List<string>();
        controlSchemes.Add(inputActions.KeyboardMouseScheme.bindingGroup);
        controlSchemes.Add(inputActions.KeyboardOnlyScheme.bindingGroup);
        controlSchemes.Add(inputActions.LeftHandedScheme.bindingGroup);
        controlSchemes.Add(inputActions.RightHandedScheme.bindingGroup);
        controlSchemes.Add(inputActions.Custom1Scheme.bindingGroup);
        controlSchemes.Add(inputActions.Custom2Scheme.bindingGroup);
        controlSchemes.Add(inputActions.Custom3Scheme.bindingGroup);
        controlSchemes.Add(inputActions.Custom4Scheme.bindingGroup);
        controlSchemes.Add(inputActions.Custom5Scheme.bindingGroup);

        inputActions.Player.Enable();
        inputActions.Menus.Enable();
        SetControls(0);
    }

    private void OnEnable()
    {
        //Add Action Methods as subscribers to inputActions.Player.[Action]
        inputActions.Player.MainAttack.performed += PlayerMainAttack;
        inputActions.Player.Roll.performed += PlayerRoll;
        inputActions.Player.SignatureAttack.performed += PlayerSignatureAttack;
        inputActions.Player.ScrollInventory.performed += PlayerScrollInventory;
        inputActions.Player.DropWeapon.performed += PlayerDropWeapon;
        inputActions.Player.Inventory.performed += PlayerInventory;
        inputActions.Player.Pause.performed += PlayerPause;
    }

    private void OnDisable()
    {
        //Remove Action Methods as subscribers to inputActions.Player.[Action]
        inputActions.Player.MainAttack.performed -= PlayerMainAttack;
        inputActions.Player.Roll.performed -= PlayerRoll;
        inputActions.Player.SignatureAttack.performed -= PlayerSignatureAttack;
        inputActions.Player.ScrollInventory.performed -= PlayerScrollInventory;
        inputActions.Player.DropWeapon.performed -= PlayerDropWeapon;
        inputActions.Player.Inventory.performed -= PlayerInventory;
        inputActions.Player.Pause.performed += PlayerPause;
    }

    /****************************
     * Player Actions
     ****************************/
    public void PlayerMainAttack(InputAction.CallbackContext context)
    {
        if (playerLife != LifeState.Dead && playerDodge != DodgeState.Dodging && !stunned && !gameManager.IsPaused() && controllable)
        {
            StartCoroutine(PlayerAttackCoroutine(context.control.device));
        }
        else if ((gameManager.IsPaused() || upgradeManager.IsUpgrading()) && GetActionInputDevice("main attack") == Keyboard.current)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    public IEnumerator PlayerAttackCoroutine(InputDevice device)
    {
        yield return new WaitForSeconds(1f / 60);
        if (playerLife != LifeState.Dead && playerDodge != DodgeState.Dodging && !stunned && !gameManager.IsPaused())
        {
            if (inventory.Count > 0 && equippedCustomWeapon >= 0 && weaponTypes[currentWeaponType].IsInactive())
            {
                if (weaponTypes[currentWeaponType].gameObject.activeSelf)
                {
                    if (weaponTypes[currentWeaponType].IsHeavy()) playerRb.velocity *= 0;
                    weaponTypes[currentWeaponType].StartCoroutine(weaponTypes[currentWeaponType].Attack(device));
                }
            }
            else
            {
                if (playerAttack == AttackState.NotAttacking)
                {
                    StartCoroutine("Attack");
                }
            }
        }
        yield return null;
    }

    public void PlayerRoll(InputAction.CallbackContext context)
    {
        if (playerAttack == AttackState.NotAttacking && playerDodge == DodgeState.Available && !gameManager.IsPaused() && controllable)
        {
            StartCoroutine(Dodge());
        }
    }

    public void PlayerSignatureAttack(InputAction.CallbackContext context)
    {
        if (playerLife != LifeState.Dead && playerDodge != DodgeState.Dodging && !stunned && !gameManager.IsPaused() && controllable) //Make sure the player is alive before they try anything
        {
            if (inventory.Count > 0 && equippedCustomWeapon >= 0 && weaponTypes[currentWeaponType].IsInactive())
            {
                if (inventory[equippedCustomWeapon].SignaturePercentage() >= 1 && !signing)
                {
                    playerRb.velocity *= 0;
                    weaponTypes[currentWeaponType].StartCoroutine(weaponTypes[currentWeaponType].Signature(context.control.device));
                    inventory[equippedCustomWeapon].ResetSignature();
                    signatureBar.SetValue(0);
                    signing = true;
                    roomManager.GetCurrent().IncrementSignatureMovesUsed();
                }
            }
        }
    }

    public void PlayerScrollInventory(InputAction.CallbackContext context)
    {
        if (playerLife != LifeState.Dead && !stunned && !gameManager.IsPaused() && controllable)
        {
            //Debug.Log(weaponTypes[currentWeaponType].IsInactive());
            if (equippedCustomWeapon >= 0 && !weaponTypes[currentWeaponType].IsInactive())
                return;

            scrollDelta = context.ReadValue<float>();

            if (context.control.path.Substring(0, 6) == "/Mouse")
            {
                if (scrollDelta > 0)
                    invScrollPos += Mathf.Max(1, (int)(scrollDelta * scrollSensitivity * Time.deltaTime * 0.5f));
                else
                    invScrollPos += Mathf.Min(-1, (int)(scrollDelta * scrollSensitivity * Time.deltaTime * 0.5f));
            }
            else
            {
                invScrollPos += scrollDelta;
            }

            if (inventory.Count > 0 && scrollDelta != 0)
            {
                if (invScrollPos < 0)
                {
                    invScrollPos = 0;
                    if (equippedCustomWeapon != 0) SetCustomWeapon(0);
                }
                else if (invScrollPos >= inventory.Count)
                {
                    invScrollPos = inventory.Count - scrollSensitivity;
                    if (equippedCustomWeapon != inventory.Count - 1) SetCustomWeapon(inventory.Count - 1);
                }
                else if (equippedCustomWeapon != (int)invScrollPos) SetCustomWeapon((int)invScrollPos);
            }
            else if (inventory.Count == 0) invScrollPos = 0;
        }
    }

    public void PlayerDropWeapon(InputAction.CallbackContext context)
    {
        if (inventory.Count > 0 && weaponTypes[currentWeaponType].IsInactive()
            && !Physics.Raycast(gameObject.transform.position + new Vector3(0f, 0.5f, 0f), -transform.forward, 1.5f)
            && !gameManager.IsPaused() && controllable)
            StartCoroutine(DropCustomWeapon(inventory[equippedCustomWeapon]));
    }

    public void PlayerInventory(InputAction.CallbackContext context)
    {
        if (!gameManager.GameIsOver())
        {
            gameManager.Pause(1);
            if (!gameManager.IsPaused())
            {
                UseAllPotions();
                for (int i = 0; i < selectedPotions.Count; i++)
                    selectedPotions[i] = false;
            }
        }
    }

    public void PlayerPause(InputAction.CallbackContext context)
    {
        if (!gameManager.GameIsOver() && !upgradeManager.IsUpgrading())
        {
            gameManager.Pause(0);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        if (currentHealth <= 0 && maxHealth > 0)
        {
            playerLife = LifeState.Dead;
            playerAttack = AttackState.NotAttacking;
            playerRb.velocity *= 0;
            weaponTypes[currentWeaponType].SetActivity(false);
            weaponTypes[currentWeaponType].gameObject.SetActive(false);
            gameObject.transform.localScale *= .99f * Time.timeScale;
        }
        else
        {
            playerLife = LifeState.Alive;
        }

        // Can change controls while not attacking
        if (GetAttackState() == 0 && (equippedCustomWeapon == -1 || weaponTypes[currentWeaponType].IsInactive()))
        {
            /* Set Controls */
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetControls(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SetControls(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SetControls(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                SetControls(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                SetControls(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                SetControls(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                SetControls(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                SetControls(7);
            else if (Input.GetKeyDown(KeyCode.Alpha9))
                SetControls(8);
        }

        if (playerLife != LifeState.Dead && !stunned && !gameManager.IsPaused()) //Make sure the player is alive before they try anything
        {
            /* All or Nothing Abilities*/
            CustomWeapon current = GetCustomWeapon();
            if (current != null && !signing)
            {
                int place = System.Array.IndexOf(Ability.GetNames(), "AllOrNothingD");
                if (current.GetAbilities().Contains(place))
                {
                    while (aonDurabilityTimer >= 2)
                    {
                        current.DecrementDurability(-1);
                        miniDurabilityBar.SetValue(inventory[equippedCustomWeapon].DecrementDurability(0));
                        aonDurabilityTimer -= 2;
                    }
                    aonDurabilityTimer += Time.deltaTime;
                }

                place = System.Array.IndexOf(Ability.GetNames(), "AllOrNothingS");
                if (current.GetAbilities().Contains(place))
                {
                    while (aonSignatureTimer >= 0.25f)
                    {
                        int restoration = current.GetSignatureCap() / 100;
                        current.AddSignature((int)(restoration * signatureMultiplier * (1 + SummationBuffs(4)) * (1 + SummationDebuffs(4))));
                        signatureBar.SetValue(inventory[equippedCustomWeapon].GetSignatureGauge());
                        aonSignatureTimer -= 0.25f;
                    }
                    aonSignatureTimer += Time.deltaTime;
                }
            }
            moveInputVector = controllable ? inputActions.Player.Move.ReadValue<Vector2>() : Vector2.zero;
        }

        if (invincibilityTime > 0)
            invincibilityTime -= Time.deltaTime;
        else
            invincibilityTime = 0;
        
        if (invincibilityTime > 0)
            invincibilityCover.gameObject.SetActive(true);
        else
            invincibilityCover.gameObject.SetActive(false);

        if (!spawnManager.AllDefeated())
            ProgressBuffTime();
        UpdateAttributeUI();
        if (transform.position.y != 0.5f)
            transform.Translate(0, 0.5f - transform.position.y, 0);
    }

    public void FixedUpdate()
    {
        hitByList.Clear();
        if (playerLife != LifeState.Dead && !stunned) //Make sure the player is alive before they try anything
        {
            if (mobile && playerDodge != DodgeState.Dodging) //Make sure the player is not attacking while they have a "heavy" weapon (Axe or Spear)
            {
                if (controllable)
                {
                    if (Mathf.Abs(moveInputVector.x) >= 0.1f || Mathf.Abs(moveInputVector.y) >= 0.1f)
                    {
                        Vector3 travelVector = transform.forward;

                        // If meleeAuto and starting up or attacking, don't rotate to face movement direction
                        if (meleeAuto && FindObjectsOfType<Enemy>().Length > 0)
                        {
                            // Travel vector should be movement vector since the weapon handles the character rotation
                            if (inventory.Count > 0 && equippedCustomWeapon >= 0 && weaponTypes[currentWeaponType].IsMelee()
                                && (weaponTypes[currentWeaponType].IsStarting() || weaponTypes[currentWeaponType].IsAttacking()))
                            {
                                travelVector = new Vector3(moveInputVector.x, 0, moveInputVector.y).normalized;
                            }
                            else if (GetAttackState() == 1 || GetAttackState() == 2)
                            {
                                travelVector = new Vector3(moveInputVector.x, 0, moveInputVector.y).normalized;
                            }
                            else
                            {
                                Vector3 direction = new Vector3(moveInputVector.x, 0, moveInputVector.y).normalized;
                                transform.rotation = Quaternion.LookRotation(direction);
                            }
                        }
                        else
                        {
                            Vector3 direction = new Vector3(moveInputVector.x, 0, moveInputVector.y).normalized;
                            transform.rotation = Quaternion.LookRotation(direction);
                        }

                        // Actual movement
                        float characterSpeed = speed * Mathf.Max(-0.5f, Mathf.Min((1 + SummationBuffs(3)) * (1 + SummationDebuffs(3)), 1.99f)) * directMults[2];
                        playerRb.velocity = travelVector * characterSpeed;
                    }
                    else
                    {
                        playerRb.velocity *= 0;
                    }
                }
            }
            else if (!mobile)
            {
                playerRb.velocity *= 0;
            }
        }

        Vector3 moveVector3D = new Vector3(moveInputVector.x, 0, moveInputVector.y);
        RaycastHit hit;
        if (playerRb.velocity.magnitude > 0
            && Physics.Raycast(transform.position - (playerRb.velocity.normalized * Time.deltaTime),
                                playerRb.velocity.normalized, out hit, playerRb.velocity.magnitude * Time.deltaTime * 3, LayerMask.GetMask("Wall")))
            playerRb.velocity *= 0;

        //if (IsOOB())
        //    ReturnToInBounds();
    }

    public override void DealDamage(float val, Character target, float p, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool overrideDMG = false, bool fixKB = false)
    {
        int damage;
        float mod = ((strength + Mathf.Min(Mathf.Max(SummationBuffs(1) + SummationDebuffs(1), -9), 9)) - (target.GetDefense() + Mathf.Min(Mathf.Max(target.SummationBuffs(2) + target.SummationDebuffs(2), -9), 9))) / 10;

        CustomWeapon current = GetCustomWeapon();
        if (current != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "ArmorPierce");
            if (current.GetAbilities().Contains(place))
            {
                mod = ((strength + Mathf.Min(Mathf.Max(SummationBuffs(1) + SummationDebuffs(1), -9), 9)) - (Mathf.Min(Mathf.Max(target.GetDefense() + target.SummationBuffs(2) + target.SummationDebuffs(2), -9), 0))) / 10;
            }
        }

        if (overrideDMG)
            damage = Mathf.Max(0, (int)(Mathf.Floor((p * val * (1 + mod)) - Random.Range(0.001f, 1f) + 1.0f) * (directMults[1] / target.GetDirectMult(2))));
        else if (equippedCustomWeapon >= 0)
            damage = Mathf.Max(0, (int)(Mathf.Floor((inventory[equippedCustomWeapon].GetPower() * val * (1 + mod)) - Random.Range(0.001f, 1.000f) + 1.0f) * (directMults[1] / target.GetDirectMult(2))));
        else
            damage = Mathf.Max(0, (int)Mathf.Floor(1 + mod - Random.Range(0.001f, 1.000f) + 1.0f));
        //knockback calc, likely with a Vector3 parameter calculated by the hitbox that causes the character to call this method.
        float targetHP = target.GetCurrentHealth();
        float targetMax = target.GetMaxHealth();

        if (current != null && target.gameObject.GetComponent<Boss>() == null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "LuckyStrike");
            if (current.GetAbilities().Contains(place))
            {
                float chance = weaponTypes[currentWeaponType].gameObject.GetComponents<Ability>()[place].GetModifier() * (1.5f - (targetHP / targetMax));
                if (Random.Range(0, 0.99f) < chance)
                    damage = (int)(target.GetMaxHealth() * 2);
            }
        }
        target.TakeDamage(damage, kbDir, triggerInvinc, kbMod, fixKB);

        if (current != null)
        {
            if (damage >= targetHP) //On defeating an enemy
            {
                int place = System.Array.IndexOf(Ability.GetNames(), "BurstStrength");
                if (current.GetAbilities().Contains(place))
                {
                    weaponTypes[currentWeaponType].gameObject.GetComponents<Ability>()[place].StartBuff();
                }

                place = System.Array.IndexOf(Ability.GetNames(), "BurstDefense");
                if (current.GetAbilities().Contains(place))
                {
                    weaponTypes[currentWeaponType].gameObject.GetComponents<Ability>()[place].StartBuff();
                }

                place = System.Array.IndexOf(Ability.GetNames(), "BurstSpeed");
                if (current.GetAbilities().Contains(place))
                {
                    weaponTypes[currentWeaponType].gameObject.GetComponents<Ability>()[place].StartBuff();
                }

                place = System.Array.IndexOf(Ability.GetNames(), "BurstSignatureGain");
                if (current.GetAbilities().Contains(place))
                {
                    weaponTypes[currentWeaponType].gameObject.GetComponents<Ability>()[place].StartBuff();
                }
            }
            else
            {
                int place = System.Array.IndexOf(Ability.GetNames(), "StrengthDebilitator");
                float bossMod = 1;
                if (target.gameObject.GetComponent<Boss>() != null)
                    bossMod = 20;

                if (current.GetAbilities().Contains(place))
                {
                    Ability a = weaponTypes[currentWeaponType].GetComponent<StrengthDebilitator>();
                    if (Random.Range(0, 0.9999f) < (2 * a.GetModifier() * damage / targetMax))
                    {
                        Debuff debuff = (Debuff)ScriptableObject.CreateInstance("Debuff");
                        debuff.SetBuff(-Random.Range(1, 4), 5);
                        target.AddDebuff(debuff, 1);
                    }
                }

                place = System.Array.IndexOf(Ability.GetNames(), "DefenseDebilitator");
                if (current.GetAbilities().Contains(place))
                {
                    Ability a = weaponTypes[currentWeaponType].GetComponent<DefenseDebilitator>();
                    if (Random.Range(0, 0.9999f) < (2 * a.GetModifier() * damage * bossMod / targetMax))
                    {
                        Debuff debuff = (Debuff)ScriptableObject.CreateInstance("Debuff");
                        debuff.SetBuff(-Random.Range(1, 4), 5);
                        target.AddDebuff(debuff, 2);
                    }
                }
            }

            int index = System.Array.IndexOf(Ability.GetNames(), "HealthDrain");
            if (current.GetAbilities().Contains(index))
            {
                Ability a = weaponTypes[currentWeaponType].GetComponent<HealthDrain>();
                TakeDamage((int)Mathf.Floor((damage * ((int)a.GetModifier() << 2) * .01f * -1) - Random.Range(0.001f, 1.000f) + 1.0f), Vector3.zero);
            }

            index = System.Array.IndexOf(Ability.GetNames(), "SignatureDrain");
            if (current.GetAbilities().Contains(index))
            {
                if (!signing)
                {
                    Ability a = weaponTypes[currentWeaponType].GetComponent<SignatureDrain>();
                    int percent = (int)(damage * 100 / targetMax) * (target.GetComponent<Boss>() != null ? 40 : 1);
                    //Debug.Log(percent);
                    inventory[equippedCustomWeapon].AddSignature(Mathf.Max(0, (int)(a.GetModifier() * 4 * (percent / 5) * -Random.Range(0.001f, 1.000f) + 1.0f)));
                    signatureBar.SetValue(inventory[equippedCustomWeapon].GetSignatureGauge());
                }
            }

            if (weaponTypes[current.GetWeaponType()].IsMelee() && !signing)
            {
                if (current.GetWeaponType() == 0)
                {
                    if (!((Sword)weaponTypes[0]).ChainHitList().Contains(target)) // Only decrement on first hit of chain
                    {
                        ((Sword)weaponTypes[0]).ChainHit(target);
                        if (current.GetMaxDurability() > 0f && current.DecrementDurability(1) <= 0)
                            BreakCustomWeapon(current);
                    }
                }
                else if (current.GetMaxDurability() > 0f && current.DecrementDurability(1) <= 0)
                {
                    BreakCustomWeapon(current);
                }

                if (inventory.Count > 0 && current.DecrementDurability(0) > 0)
                    miniDurabilityBar.SetValue(current.DecrementDurability(0));
            }
        }
    }

    public override void TakeDamage(int damage, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool fixKB = false)
    {
        if (damage > 0) // Taking damage (NOT healing)
        {
            if (invincibilityTime <= 0)
            {
                CustomWeapon current = GetCustomWeapon();
                if (current != null)
                {
                    int place = System.Array.IndexOf(Ability.GetNames(), "QuickDodge");
                    if (current.GetAbilities().Contains(place))
                    {
                        float chance = weaponTypes[currentWeaponType].gameObject.GetComponents<Ability>()[place].GetModifier() * (1.5f - (currentHealth / maxHealth));
                        if (Random.Range(0, 0.99f) < chance)
                        {
                            OverrideInvincibility(0.5f);
                            return;
                        }
                    }

                    place = System.Array.IndexOf(Ability.GetNames(), "PityCounter");
                    if (current.GetAbilities().Contains(place))
                    {
                        Ability a = weaponTypes[currentWeaponType].GetComponent<PityCounter>();
                        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();
                        foreach (Enemy e in allEnemies)
                        {
                            e.TakeDamage(Mathf.Max(1, (int)Mathf.Floor((0.4f * a.GetModifier() * damage) - Random.Range(0.001f, 1.000f) + 1.0f)), Vector3.zero);
                        }
                    }

                    place = System.Array.IndexOf(Ability.GetNames(), "PitySignature");
                    if (current.GetAbilities().Contains(place))
                    {
                        Ability a = weaponTypes[currentWeaponType].GetComponent<PitySignature>();
                        int perc = (int)(damage * 100 / maxHealth);
                        GetCustomWeapon().AddSignature((int)Mathf.Max(0, a.GetModifier() * 0.4f * perc - Random.Range(0.001f, 1.000f) + 1));
                        signatureBar.SetValue(inventory[equippedCustomWeapon].GetSignatureGauge());
                    }

                    place = System.Array.IndexOf(Ability.GetNames(), "AllOrNothingD");
                    if (current.GetAbilities().Contains(place))
                        BreakCustomWeapon(current);
                    else
                    {
                        place = System.Array.IndexOf(Ability.GetNames(), "AllOrNothingS");
                        if (current.GetAbilities().Contains(place))
                        {
                            GetCustomWeapon().ResetSignature();
                            aonSignatureTimer = 0;
                            signatureBar.SetValue(inventory[equippedCustomWeapon].GetSignatureGauge());
                        }
                    }
                }
                // The Quick Dodge failed

                if (playerDodge == DodgeState.Dodging)
                    playerDodge = DodgeState.Fail; //this means the coroutine is running, so it will set the dodge state back to cool or available when necessary.

                dodgeTrail.time = 0;
                dodgeTrail.emitting = false;
                currentHealth -= damage;
                roomManager.GetCurrent().AddDamageTaken(damage);

                if (triggerInvinc)
                    OverrideInvincibility(10 * (damage / maxHealth));

                if (kbMod != 0)
                {
                    if (fixKB) StartCoroutine(TakeKnockback(1, kbDir, kbMod));
                    else StartCoroutine(TakeKnockback(damage / maxHealth, kbDir, kbMod));
                }
            }
        }
        else
        {
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
            currentHealth = 0;
        else if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        miniHealthBar.SetValue(currentHealth);
        healthBar.SetValue(currentHealth);
        healthBar.UpdateAmountTxt(currentHealth + "/" + maxHealth);
    }

    public IEnumerator StunPlayer()
    {
        yield return new WaitUntil(() => !stunned);
        stunned = true;
        mobile = false;
        playerAttack = AttackState.NotAttacking;
        weaponTypes[currentWeaponType].Abort();

        float t = 0;
        while (t < stunTime)
        {
            if (GetMyFreezeTime() <= 0)
            {
                if (stunTime - t > stunCooldown / 2)
                    transform.Rotate(new Vector3(0, stunRotateSpeed * Time.deltaTime, 0));
                t += Time.deltaTime;
            }
            yield return null;
        }

        stunned = false;
        mobile = true;
        yield return null;
    }

    public override void StunMe(float t)
    {
        stunTime += t;
        initialStun = stunTime;
        StartCoroutine(StunPlayer());
    }

    public override IEnumerator TakeKnockback(float knockback, Vector3 kbDir, float kbMod = 0)
    {
        playerRb.velocity *= 0;
        playerRb.AddForce(kbDir.normalized * knockback * kbMod * 20, ForceMode.Impulse);
        stunTime = 0.5f;
        StartCoroutine(StunPlayer());
        yield return new WaitForSeconds(0.5f);
        playerRb.velocity *= 0;
    }

    public void SetMobile(bool b)
    {
        mobile = b;
    }

    public void SetControllable(bool b)
    {
        controllable = b;
    }

    /*******************************************
     * Custom Weapons
     *******************************************/
    public void GiveCustomWeapon(CustomWeapon cw)
    {
        inventory.Add(cw);
        currentMP += cw.GetMightPoints();
        inventoryBar.SetValue(currentMP);
        inventoryBar.UpdateAmountTxt(currentMP + "/" + maxMP);
    }

    //Returns the equipped custom weapon
    public CustomWeapon GetCustomWeapon()
    {
        if (!InventoryEmpty()) return inventory[equippedCustomWeapon];
        else return null;
    }

    //Returns the custom weapon of the specific index
    public CustomWeapon GetCustomWeapon(int i)
    {
        if (i >= 0 && i < inventory.Count) return inventory[i];
        else return null;
    }

    //Returns the index of the user's custom weapon
    public int GetCustomIndex()
    {
        return equippedCustomWeapon;
    }

    public void RemoveCustomWeapon()
    {
        RemoveCustomWeapon(equippedCustomWeapon);
    }

    public void RemoveCustomWeapon(CustomWeapon cw)
    {
        weaponTypes[cw.GetWeaponType()].StopCoroutine("Attack");
        weaponTypes[cw.GetWeaponType()].InitializeTransform();
        weaponTypes[cw.GetWeaponType()].SetState(0);
        currentMP -= cw.GetMightPoints();
        inventoryBar.SetValue(currentMP);
        inventoryBar.UpdateAmountTxt(currentMP + "/" + maxMP);

        //Disable abilities of current weapon
        Weapon cType;
        if (equippedCustomWeapon >= 0 && equippedCustomWeapon < inventory.Count)
        {
            cType = weaponTypes[currentWeaponType];
            for (int i = 0; i < Ability.GetNames().Length; i++)
            {
                Ability a = (Ability)cType.gameObject.GetComponent(System.Type.GetType(Ability.GetNames()[i]));
                if (cw.GetAbilities().Contains(i))
                {
                    a.Deactivate();
                }
            }
        }

        inventory.Remove(cw);
        invScrollPos -= 1;
        if (equippedCustomWeapon == inventory.Count) equippedCustomWeapon--;
        SetCustomWeapon(equippedCustomWeapon);
    }

    public void RemoveCustomWeapon(int i)
    {
        if (i >= 0 && i < inventory.Count)
            RemoveCustomWeapon(inventory[i]);
    }

    public void RemoveAllCustomWeapons()
    {
        for (int i = inventory.Count - 1; i >= 0; i--)
            RemoveCustomWeapon(i);
    }

    public IEnumerator DropCustomWeapon(CustomWeapon cw)
    {
        PickupCW drop = Instantiate(pickupPrefab, gameObject.transform.position + (transform.up * 0.5f) + (-transform.forward * 1.5f), Quaternion.Euler(0, 0, 0));
        drop.Initialize(cw.GetWeaponType(), cw.GetPower(), cw.DecrementDurability(0), cw.GetMaxDurability(), cw.GetSignatureGauge(), cw.GetAbilities(), cw.GetMods());
        drop.gameObject.transform.parent = roomManager.GetCurrent().gameObject.transform;

        RemoveCustomWeapon(cw);
        yield return null;
    }

    //Used when a weapon breaks
    public void BreakCustomWeapon(CustomWeapon cw)
    {
        RemoveCustomWeapon(cw);
        Destroy(cw);
        SetAttackState(0);
        SetMobile(true);
    }

    public void SetCustomWeapon(WeaponButton wb)
    {
        SetCustomWeapon(wb.GetWeaponNumber());
    }

    public void SetCustomWeapon(int num)
    {
        CustomWeapon current;
        Weapon cType;
        //Disable abilities of current weapon
        if (equippedCustomWeapon >= 0 && equippedCustomWeapon < inventory.Count)
        {
            current = inventory[equippedCustomWeapon];
            cType = weaponTypes[currentWeaponType];
            for (int i = 0; i < Ability.GetNames().Length; i++)
            {
                if (current.GetAbilities().Contains(i))
                {
                    ((Ability)cType.gameObject.GetComponent(System.Type.GetType(Ability.GetNames()[i]))).Deactivate();
                }
            }

            /********************************************
             * Sheathe Penalty:
             *   20% Signature Loss
             *  +30% if the weapon has "All Or Nothing D"
             *  +50% if the weapon has "All Or Nothing S"
             ********************************************/
            if (!IsPaused())
            {
                float sigLoss = 0.2f;
                int place = System.Array.IndexOf(Ability.GetNames(), "AllOrNothingD");
                if (current.GetAbilities().Contains(place))
                    sigLoss += 0.3f;

                place = System.Array.IndexOf(Ability.GetNames(), "AllOrNothingS");
                if (current.GetAbilities().Contains(place))
                    sigLoss += 0.5f;

                if (equippedCustomWeapon != num)
                    current.AddSignature((int)(-current.GetSignatureGauge() * sigLoss));
            }
        }
        if (inventory.Count == 0) //no weapons? *megamind face*
        {
            for (int i = 0; i < weaponTypes.Length; i++)
                weaponTypes[i].gameObject.SetActive(false);
            miniDurabilityBar.gameObject.SetActive(false);
            signatureBar.SetValue(0);
            equippedCustomWeapon = -1;
        }
        else if (num >= 0 && num < inventory.Count)
        {
            equippedCustomWeapon = num;
            if (inventory[equippedCustomWeapon].GetMaxDurability() > 0)
                miniDurabilityBar.gameObject.SetActive(true);
            SwitchWeaponType(inventory[equippedCustomWeapon].GetWeaponType());
            current = inventory[equippedCustomWeapon];
            cType = weaponTypes[currentWeaponType];
            cType.InitializeTransform();

            for (int i = 0; i < Ability.GetNames().Length; i++)
            {
                Ability a = (Ability)cType.gameObject.GetComponent(System.Type.GetType(Ability.GetNames()[i]));
                if (current.GetAbilities().Contains(i))
                {
                    a.enabled = true;
                    a.Initialize();
                    a.SetModifier(current.GetMods()[current.GetAbilities().IndexOf(i)]);
                }
                else
                {
                    a.enabled = false;
                }
            }
            miniDurabilityBar.SetMax(current.GetMaxDurability());
            miniDurabilityBar.SetValue(current.DecrementDurability(0));
            signatureBar.SetMax(inventory[equippedCustomWeapon].GetSignatureCap());
            signatureBar.SetValue(inventory[equippedCustomWeapon].GetSignatureGauge());
        }
        aonDurabilityTimer = 0;
        aonSignatureTimer = 0;

        if (equippedCustomWeapon % 2 == 0)
        {
            leftWeaponButtonUI.SetWeaponNumber(equippedCustomWeapon);
            rightWeaponButtonUI.SetWeaponNumber(equippedCustomWeapon + 1);
        }
        else
        {
            leftWeaponButtonUI.SetWeaponNumber(equippedCustomWeapon - 1);
            rightWeaponButtonUI.SetWeaponNumber(equippedCustomWeapon);
        }
    }

    private void SwitchWeaponType(int t)
    {
        for (int i = 0; i < weaponTypes.Length; i++)
        {
            if (i != t)
                weaponTypes[i].gameObject.SetActive(false);
        }
        weaponTypes[t].gameObject.SetActive(true);
        currentWeaponType = t;
    }

    public int InventoryCount()
    {
        return inventory.Count;
    }

    public int RemainingMP()
    {
        return maxMP - currentMP;
    }

    public int MaxMP()
    {
        return maxMP;
    }

    public float InventoryPercent()
    {
        if (maxMP > 0)
            return (float)currentMP / maxMP;
        return -1;
    }

    public void UpdateDPUI(float p)
    {
        miniDurabilityBar.SetMax(inventory[equippedCustomWeapon].GetMaxDurability());
        miniDurabilityBar.SetValue(inventory[equippedCustomWeapon].DecrementDurability(0));
    }

    public bool InventoryEmpty()
    {
        return inventory.Count == 0;
    }

    /*******************************************
     * Potion Management
     *******************************************/
    public bool GivePotion(int a)
    {
        if (potions.Count < potionCapacity && a >= 1 && a <= 6)
        {
            potions.Add(a);
            return true;
        }
        else
            return false;
    }

    public void TogglePotion(PotionButton pb)
    {
        int i = pb.GetNumber();
        if (i >= 0 && i < potions.Count)
            selectedPotions[i] = !selectedPotions[i];
    }

    public void UseAllPotions()
    {
        int potionsUsed = 0;
        for (int i = 0; i < potions.Count; i++)
            if (selectedPotions[i + potionsUsed])
            {
                if (UsePotion(i))
                {
                    i--;
                    roomManager.GetCurrent().IncrementPotionsUsed();
                }
                potionsUsed++;
            }
    }

    /*
     * Returns true on a successful potion use
     */
    protected bool UsePotion(int p)
    {
        if (p >= 0 && p < potions.Count)
        {
            Buff buff;
            switch (potions[p])
            {
                case 1: // Strike
                    buff = (Buff)ScriptableObject.CreateInstance("Buff");
                    buff.SetBuff(potionBuffInt, potionDuration);
                    AddBuff(buff, 1);
                    potions.RemoveAt(p);
                    return true;
                case 2: // Shield
                    buff = (Buff)ScriptableObject.CreateInstance("Buff");
                    buff.SetBuff(potionBuffInt, potionDuration);
                    AddBuff(buff, 2);
                    potions.RemoveAt(p);
                    return true;
                case 3: // Swift
                    buff = (Buff)ScriptableObject.CreateInstance("Buff");
                    buff.SetBuff(potionBuffSpeedSig, potionDuration);
                    AddBuff(buff, 3);
                    potions.RemoveAt(p);
                    return true;
                case 4: // Skill
                    buff = (Buff)ScriptableObject.CreateInstance("Buff");
                    buff.SetBuff(potionBuffSpeedSig, potionDuration);
                    AddBuff(buff, 4);
                    potions.RemoveAt(p);
                    return true;
                case 5: // Stun
                    // Find the list of all non-Boss enemies.
                    Enemy[] enemyArray = FindObjectsOfType<Enemy>();
                    List<Enemy> enemies = new List<Enemy>();
                    for (int i = 0; i < enemyArray.Length; i++)
                        if (enemyArray[i].GetComponent<Boss>() == null) //Cannot stun bosses
                            enemies.Add(enemyArray[i]);

                    if (enemies.Count == 0)
                        return false;

                    float stunTime = 40f / (4 * Mathf.Sqrt(enemies.Count));
                    for (int i = 0; i < enemies.Count; i++)
                        enemies[i].StunMe(stunTime);
                    potions.RemoveAt(p);
                    return true;
                case 6: // Saving
                    buff = (HealthRegen)ScriptableObject.CreateInstance("HealthRegen");
                    ((HealthRegen)buff).SetBuff(potionBuffRegen, potionDuration, potionRegenTick);
                    AddBuff(buff, 5);
                    potions.RemoveAt(p);
                    return true;
                default:
                    break;
            }
        }
        return false;
    }

    public List<int> GetPotions()
    {
        List<int> pList = new List<int>();
        foreach (int i in potions)
            pList.Add(i);
        return pList;
    }

    /*******************************************
     * State Management
     *******************************************/
    public void SetAttackState(int state)
    {
        if (state == 0)
            playerAttack = AttackState.NotAttacking;
        else if (state == 1)
            playerAttack = AttackState.Startup;
        else if (state == 2)
            playerAttack = AttackState.Active;
        else if (state == 3)
            playerAttack = AttackState.Endlag;
    }

    public int GetAttackState()
    {
        if (playerAttack == AttackState.NotAttacking)
            return 0;
        if (playerAttack == AttackState.Startup)
            return 1;
        if (playerAttack == AttackState.Active)
            return 2;
        //playerAttack == AttackState.Endlag
        return 3;
    }

    public int GetCurrentWeaponAttackState()
    {
        if (currentWeaponType < 0)
            return -1;

        if (weaponTypes[currentWeaponType].IsInactive())
            return 0;

        if (weaponTypes[currentWeaponType].IsStarting())
            return 1;

        if (weaponTypes[currentWeaponType].IsAttacking())
            return 2;

        // IsEnding()
        return 3;
    }

    //Unarmed strike
    public IEnumerator Attack()
    {
        SetAttackState(1);
        yield return new WaitForSeconds(startupTime);

        SetAttackState(2);
        unarmedStrike.gameObject.SetActive(true);
        yield return new WaitForSeconds(activeTime);

        SetAttackState(3);
        unarmedStrike.gameObject.SetActive(false);
        yield return new WaitForSeconds(cooldownTime);

        SetAttackState(0);
        yield return null;
    }

    /*******************************************
     * DODGE!!!!
     *******************************************/
    protected float CalculateDodgeCool()
    {
        CustomWeapon current = GetCustomWeapon();
        if (current != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "RollRecoveryUp");
            if (current.GetAbilities().Contains(place))
            {
                return 1 + weaponTypes[currentWeaponType].GetComponents<Ability>()[place].GetModifier();
            }
        }
        return 1;
    }

    protected float CalculateDodgeDistance()
    {
        CustomWeapon current = GetCustomWeapon();
        if (current != null)
        {
            int place = System.Array.IndexOf(Ability.GetNames(), "RollDistanceUp");
            if (current.GetAbilities().Contains(place))
                return 1 + weaponTypes[currentWeaponType].GetComponents<Ability>()[place].GetModifier();
        }
        return 1;
    }

    /*
     * Calculates the amount of signature meter gained, and adds it.
     * Returns the "bonus" that the dodge cooldown is divided by
     * (4 if an attack has been successfully dodged, 1 otherwise)
     */
    private float DodgeHelp(RaycastHit hit, List<Hitbox> dodged, float bonus)
    {
        Hitbox h = hit.collider.gameObject.GetComponent<Hitbox>();
        if (h != null && !dodged.Contains(h))
        {
            // TODO: Stop multihit dodges
            dodged.Add(h);

            int damage;
            if (h.gameObject.GetComponent<Explosive>() != null)
            {
                damage = (int)(h.GetSource().GetPower() * h.GetDamageMod() * 100 / 5);
                Debug.Log(damage);
                //damage = ((Enemy)h.GetSource()).SimulateDamage(((Explosive)h).GetDamageMod(), this);
            }
            else
            {
                damage = (int)(((Enemy)h.GetSource()).GetPower() * h.GetDamageMod() * 100 / 5);
            }
            int pts = (int)(damage * signatureMultiplier * (1 + SummationBuffs(4)) * (1 + SummationDebuffs(4)));
            if (equippedCustomWeapon > -1)
            {
                int pity = 1;
                int index1 = System.Array.IndexOf(Ability.GetNames(), "PityCounter");
                int index2 = System.Array.IndexOf(Ability.GetNames(), "PitySignature");

                CustomWeapon current = inventory[equippedCustomWeapon];
                if (current.GetAbilities().Contains(index1))
                    pity *= 2;
                if (current.GetAbilities().Contains(index2))
                    pity *= 2;

                current.AddSignature(pts / pity);
                signatureBar.SetValue(inventory[equippedCustomWeapon].GetSignatureGauge());
                roomManager.GetCurrent().AddSignaturePointsGained(pts / pity);
            }
            return 4 * CalculateDodgeCool();
        }
        return bonus;
    }

    public IEnumerator Dodge()
    {
        playerDodge = DodgeState.Dodging;
        float characterSpeed = speed * Mathf.Max(-0.5f, Mathf.Min((1 + SummationBuffs(3)) * (1 + SummationDebuffs(3)), 1.99f)) * directMults[2];
        playerRb.velocity = transform.forward * characterSpeed * 2.5f * (1 + (SummationBuffs(3) / 2)) * CalculateDodgeDistance();
        List<Hitbox> dodged = new List<Hitbox>();
        dodgeTrail.time = 0.5f;
        dodgeTrail.emitting = true;
        miniDodgeBar.gameObject.SetActive(true);
        miniDodgeBar.SetValue(1);

        float dt = 0;
        float bonus = 1 * CalculateDodgeCool();
        RaycastHit hit;
        while (dt < 0.4f)
        {
            //raycast back a certain distance based on speed, successful dodges reduce cooldown and fill signature gauge of current weapon
            //Debug.DrawRay(gameObject.transform.position, transform.forward * -1, Color.black, playerRb.velocity.magnitude * dt * 1.2f);
            if (playerDodge != DodgeState.Fail && Physics.SphereCast(new Ray(gameObject.transform.position, transform.forward * -1), 0.5f, out hit, playerRb.velocity.magnitude * dt * 1.2f))
            {
                bonus = DodgeHelp(hit, dodged, bonus);
            }
            dt += Time.deltaTime;
            yield return null;
        }

        while (dt < 0.5f)
        {
            dodgeTrail.time = 0.5f * (1 - ((dt - 0.4f) / 0.1f));
            if (playerDodge != DodgeState.Fail && Physics.SphereCast(new Ray(gameObject.transform.position, transform.forward * -1), 0.5f, out hit, playerRb.velocity.magnitude * dt))
            {
                bonus = DodgeHelp(hit, dodged, bonus);
            }
            dt += Time.deltaTime;
            yield return null;
        }
        dodgeTrail.time = 0;
        dodgeTrail.emitting = false;

        playerDodge = DodgeState.Cooldown;
        float t = 0;
        float trueCool = dodgeCool / bonus;
        while (t < trueCool)
        {
            t += Time.deltaTime;
            miniDodgeBar.SetValue(Mathf.Max(0, (trueCool - t) / trueCool));
            yield return null;
        }
        playerDodge = DodgeState.Available;
        miniDodgeBar.gameObject.SetActive(false);
        yield return null;
    }

    //public virtual void OnTriggerEnter(Collider targetCollider)
    //{
    //    if (targetCollider.gameObject.CompareTag("Wall"))
    //    {
    //        Vector3 dir = transform.position - targetCollider.gameObject.transform.position;
    //        dir = (new Vector3(dir.x, 0, dir.z)).normalized;
    //        transform.position += dir * playerRb.velocity.magnitude * 2 * Time.deltaTime;
    //        playerRb.velocity *= 0;
    //    }
    //}

    /***************
     * RANKS
     ***************/

    /***********************************************************************
     * Attempts to Rank Up either Health, Inventory, or Signature Fill Rate
     ***********************************************************************/
    public void RankUp(int attributeIndex)
    {        
        if (ranks[attributeIndex] < MAX_RANK)
        {
            ranks[attributeIndex]++;
            switch (attributeIndex)
            {
                case 0:
                    maxHealth = baseHealthCap * (1 + (rankGrowths[0] * ranks[attributeIndex]));
                    currentHealth = maxHealth;

                    miniHealthBar.SetMax(maxHealth);
                    miniHealthBar.SetValue(currentHealth);

                    healthBar.SetMax(maxHealth);
                    healthBar.SetValue(currentHealth, 0.5f, 12);
                    healthBar.UpdateAmountTxt(currentHealth + "/" + maxHealth);
                    healthBar.UpdateRankTxt(ranks[attributeIndex].ToString());
                    break;
                case 1:
                    maxMP = (int)(baseMPCap * (1 + (rankGrowths[1] * ranks[attributeIndex])));

                    miniDurabilityBar.SetMax(maxMP);
                    inventoryBar.SetMax(maxMP);

                    miniDurabilityBar.SetValue(currentMP);
                    inventoryBar.SetValue(currentMP);
                    inventoryBar.UpdateAmountTxt(currentMP + "/" + maxMP);
                    inventoryBar.UpdateRankTxt(ranks[attributeIndex].ToString());
                    break;
                case 2:
                default:
                    signatureMultiplier = (1 + (rankGrowths[2] * ranks[attributeIndex]));
                    signatureBar.UpdateAmountTxt("x" + (1 + (rankGrowths[2] * ranks[attributeIndex])).ToString("0.00"));
                    signatureBar.UpdateRankTxt(ranks[attributeIndex].ToString());
                    break;
            }
        }
    }

    public int GetRank(int attributeIndex)
    {
        if (attributeIndex < 0 || attributeIndex >= ranks.Length)
            return -1;
        return ranks[attributeIndex];
    }

    public bool IsAttributeMaxRank(int attributeIndex)
    {
        if (attributeIndex < 0 || attributeIndex >= ranks.Length)
            return false;
        return ranks[attributeIndex] == MAX_RANK;
    }

    public int GetCurrentTotalRank()
    {
        int total = 0;
        foreach (int rank in ranks)
            total += rank;
        return total;
    }

    public int GetMaxTotalRank()
    {
        return MAX_RANK * ranks.Length;
    }

    public bool IsPlayerMaxRank()
    {
        return GetCurrentTotalRank() == MAX_RANK * ranks.Length;
    }

    public void SetLifeState(bool s)
    {
        if (s) playerLife = LifeState.Alive;
        else playerLife = LifeState.Dead;
    }

    public bool IsAlive()
    {
        return currentHealth >= 0;
    }

    public override void FullyRestoreHealth()
    {
        base.FullyRestoreHealth();
        miniHealthBar.SetValue(currentHealth);
        healthBar.UpdateAmountTxt(currentHealth + "/" + maxHealth);
        healthBar.SetValue(currentHealth);
    }

    public void SetSigning(bool b)
    {
        signing = b;
    }

    public bool IsSigning()
    {
        return signing;
    }

    public void ResetCurrentWeapon()
    {
        if (currentWeaponType >= 0)
            weaponTypes[currentWeaponType].InitializeTransform();
    }

    /*******************************************
     * Controls
     *******************************************/
    //public List<KeyCode> GetAttackButtons()
    //{
    //    return myControls["Main Attack"];
    //}

    //public List<int> GetAttackMouseButtons()
    //{
    //    return mouseControls["Main Attack"];
    //}

    public bool SetControls(int control)
    {
        if (control < 0 || control >= controlSchemes.Count)
        {
            Debug.LogError("Control Preset " + control + " out of range!");
            return false;
        }

        inputActions.bindingMask = InputBinding.MaskByGroup(controlSchemes[control]);
        ControlPresetSettings preset = controlSettings[control];

        signatureCombo = preset.GetSignatureActivationMode() == ControlPresetSettings.SignatureActivation.Combo ? true : false;
        meleeAuto = preset.GetMeleeAim() == ControlPresetSettings.MeleeAim.Auto ? true : false;
        rangedAssist = preset.GetRangedAssist();
        scrollSensitivity = preset.GetScrollSensitivity();

        if (GetActionInputDevice("main attack") == Keyboard.current && (gameManager.IsPaused() || upgradeManager.IsUpgrading()))
        {
            Button[] buttons = FindObjectsOfType<Button>();
            int i = 0;
            while (i < buttons.Length)
            {
                if (buttons[i].enabled)
                {
                    eventSystem.SetSelectedGameObject(buttons[i].gameObject);
                    i = buttons.Length;
                }
                i++;
            }
        }

        Debug.Log("Control Preset " + control + " activated.");
        return true;
    }

    /**********************************************
     * Get the input device of the requested action
     **********************************************/
    public InputDevice GetActionInputDevice(string s)
    {
        InputAction act;
        s = s.ToLower();
        switch (s)
        {
            case "main attack":
                act = inputActions.Player.MainAttack;
                break;
            case "roll":
                act = inputActions.Player.Roll;
                break;
            case "signature attack":
                act = inputActions.Player.SignatureAttack;
                break;
            case "scroll inventory":
                act = inputActions.Player.ScrollInventory;
                break;
            case "drop weapon":
                act = inputActions.Player.DropWeapon;
                break;
            case "inventory":
                act = inputActions.Player.Inventory;
                break;
            case "pause":
                act = inputActions.Player.Pause;
                break;
            default:
                Debug.LogError("Please check the spelling for requested action \"" + s + "\".");
                return null;
        }
        return act.controls[0].device;
    }

    public bool GetMeleeAuto()
    {
        return meleeAuto;
    }

    public float GetRangedAssist()
    {
        return rangedAssist;
    }

    public bool IsPaused()
    {
        return gameManager.IsPaused();
    }

    private enum AttackState
    {
        NotAttacking,
        Startup,
        Active,
        Endlag,

        Attacking = Startup | Active | Endlag
    }

    private enum DodgeState
    {
        Available,
        Dodging,
        Cooldown,
        Fail
    }

    private enum LifeState
    {
        Alive,
        Dead
    }
}
