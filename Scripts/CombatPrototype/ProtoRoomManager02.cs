using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class ProtoRoomManager02 : RoomManager
{
    //[SerializeField] protected TextMeshProUGUI timeText;
    //[SerializeField] protected string timeTextColor;
    [SerializeField] protected Text tipText;
    protected MessageRotate tipRotation;
    protected bool finalFloor = false;
    protected const float MERCY_WEAPON_TIME = 10f;
    protected float spawnWeaponTimer = 0;

    [SerializeField] protected GameManager gameManager; //to pause the game and display full stats

    [Header("Room Stats")]
    [SerializeField] protected List<float> roomTimes;
    [SerializeField] protected List<int> damageTaken;
    [SerializeField] protected List<int> falls;
    [SerializeField] protected List<int> potionsUsed;
    [SerializeField] protected List<int> signaturePtsGained;
    [SerializeField] protected List<int> signatureMovesUsed;
    

    protected List<List<int>> abilities;
    protected List<List<float>> mods;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        abilities = new List<List<int>>();
        mods = new List<List<float>>();
        for (int i = 0; i < 5; i++)
        {
            abilities.Add(new List<int>());
            mods.Add(new List<float>());
        }

        tipRotation = tipText.GetComponent<MessageRotate>();
        if (tipRotation == null)
            Debug.LogError("Missing text rotation!");

        roomTimes = new List<float>();
        for (int i = 0; i < 13; i++)
            roomTimes.Add(0);

        damageTaken = new List<int>();
        for (int i = 0; i < 13; i++)
            damageTaken.Add(0);

        falls = new List<int>();
        for (int i = 0; i < 13; i++)
            falls.Add(0);

        potionsUsed = new List<int>();
        for (int i = 0; i < 13; i++)
            potionsUsed.Add(0);

        signaturePtsGained = new List<int>();
        for (int i = 0; i < 13; i++)
            signaturePtsGained.Add(0);

        signatureMovesUsed = new List<int>();
        for (int i = 0; i < 13; i++)
            signatureMovesUsed.Add(0);

        // Sword abilities
        abilities[0].Add(0);
        abilities[0].Add(25);
        abilities[0].Add(20);
        abilities[0].Add(15);
        abilities[0].Add(10);
        abilities[0].Add(5);

        // Axe abilities
        abilities[1].Add(24);
        abilities[1].Add(19);
        abilities[1].Add(14);
        abilities[1].Add(9);
        abilities[1].Add(4);
        abilities[1].Add(29);

        // Spear abilities
        abilities[2].Add(18);
        abilities[2].Add(13);
        abilities[2].Add(8);
        abilities[2].Add(3);
        abilities[2].Add(28);
        abilities[2].Add(23);

        // Crossbow abilities
        abilities[3].Add(12);
        abilities[3].Add(7);
        abilities[3].Add(2);
        abilities[3].Add(27);
        abilities[3].Add(22);
        abilities[3].Add(17);

        // Tome abilities
        abilities[4].Add(6);
        abilities[4].Add(1);
        abilities[4].Add(26);
        abilities[4].Add(21);
        abilities[4].Add(16);
        abilities[4].Add(11);
    }

    // Update is called once per frame
    new void Update()
    {
        if (!current.RoomCleared())
            roomTimes[level] += Time.deltaTime;

        // TODO: Bring this mercy weapon code from CP02 to main pipeline
        if (level > 0 && player.InventoryCount() == 0 && FindObjectsOfType(System.Type.GetType("PickupCW")).Length == 0)
        {
            if (spawnWeaponTimer >= MERCY_WEAPON_TIME)
            {
                Room r = GetCurrent();
                Vector3 weaponPos = new Vector3(Random.Range((-r.GetLength() + 2) / 2, (r.GetLength() - 2)/ 2), 1, Random.Range((-r.GetWidth() + 2) / 2, (r.GetWidth() - 2)/ 2));
                PickupCW mercy;
                if (level < 7) // in part 1
                {
                    int wType = Random.Range(0, 5);
                    mercy = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0)).GetComponent<PickupCW>();
                    switch (wType)
                    {
                        case 0:
                            mercy.Initialize(0, Sword.GetBasePower(), 0, 0, 0, null, null);
                            break;
                        case 1:
                            mercy.Initialize(0, Axe.GetBasePower(), 0, 0, 0, null, null);
                            break;
                        case 2:
                            mercy.Initialize(0, Spear.GetBasePower(), 0, 0, 0, null, null);
                            break;
                        case 3:
                            mercy.Initialize(0, Crossbow.GetBasePower(), 0, 0, 0, null, null);
                            break;
                        case 4:
                            mercy.Initialize(0, Tome.GetBasePower(), 0, 0, 0, null, null);
                            break;
                    }
                }
                else if (level < 10) // in part 2
                    mercy = GenerateWeapon(Random.Range(0, 5));
                else
                    mercy = GenerateWeapon(Random.Range(0, 5), 4f / 5, 5f / 4, 2f / 3, 3f / 2, true);

                mercy.gameObject.transform.position = weaponPos;
                mercy.gameObject.transform.parent = r.gameObject.transform;
                spawnWeaponTimer = 0;
            }
            spawnWeaponTimer += Time.deltaTime;
        }

        ////TODO: Erase Debug Code!
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    Step();
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    while (level < 7)
        //        Step();
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    while (level < 10)
        //        Step();
        //}
    }

    /*
     * Generates a weapon of type `wType`, with two abilities from `abilities`
     */
    public PickupCW GenerateWeapon(int wType, float powMultFloor = 1, float powMultCeil = 1, float durMultFloor = 1, float durMultCeil = 1, bool randomMod = false)
    {
        if (wType < 0 || wType > 4)
            return null;

        PickupCW weapon = Instantiate(pickupPrefab);
        List<int> possibilities = new List<int>(abilities[wType]);

        List<int> myAbilities = new List<int>();
        List<float> myMods = new List<float>();
        for (int i = 0; i < 2; i++)
        {
            int j = Random.Range(0, possibilities.Count);
            myAbilities.Add(possibilities[j]);

            switch (myAbilities[i])
            {
                case 0:
                    StrengthUp.SetMinMaxMods();

                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(StrengthUp.GetRandomMod()));
                    else
                        myMods.Add((int)StrengthUp.GetMeanMod());
                    break;
                case 1:
                    DefenseUp.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(DefenseUp.GetRandomMod()));
                    else
                        myMods.Add((int)DefenseUp.GetMeanMod());
                    break;
                case 2:
                    StrengthDebilitator.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(StrengthDebilitator.GetRandomMod());
                    else
                        myMods.Add(StrengthDebilitator.GetMeanMod());
                    break;
                case 3:
                    DefenseDebilitator.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(DefenseDebilitator.GetRandomMod());
                    else
                        myMods.Add(DefenseDebilitator.GetMeanMod());
                    break;
                case 4:
                    AttackRateUp.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(AttackRateUp.GetRandomMod());
                    else
                        myMods.Add(AttackRateUp.GetMeanMod());
                    break;
                case 5:
                    RollRecoveryUp.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(RollRecoveryUp.GetRandomMod());
                    else
                        myMods.Add(RollRecoveryUp.GetMeanMod());
                    break;
                case 6:
                    HealthyStrength.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(HealthyStrength.GetRandomMod()));
                    else
                        myMods.Add((int)HealthyStrength.GetMeanMod());
                    break;
                case 7:
                    HealthyDefense.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(HealthyDefense.GetRandomMod()));
                    else
                        myMods.Add((int)HealthyDefense.GetMeanMod());
                    break;
                case 8:
                    HealthySpeed.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(HealthySpeed.GetRandomMod());
                    else
                        myMods.Add(HealthySpeed.GetMeanMod());
                    break;
                case 9:
                    HealthySignatureGain.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(HealthySignatureGain.GetRandomMod());
                    else
                        myMods.Add(HealthySignatureGain.GetMeanMod());
                    break;
                case 10:
                    myMods.Add(BladeDull.GetMeanMod());
                    break;
                case 11:
                    myMods.Add(ArmorPierce.GetMeanMod());
                    break;
                case 12:
                    AttackRangeUp.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(AttackRangeUp.GetRandomMod());
                    else
                        myMods.Add(AttackRangeUp.GetMeanMod());
                    break;
                case 13:
                    RollDistanceUp.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(RollDistanceUp.GetRandomMod());
                    else
                        myMods.Add(RollDistanceUp.GetMeanMod());
                    break;
                case 14:
                    BurstStrength.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(BurstStrength.GetRandomMod()));
                    else
                        myMods.Add((int)BurstStrength.GetMeanMod());
                    break;
                case 15:
                    BurstDefense.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(BurstDefense.GetRandomMod()));
                    else
                        myMods.Add((int)BurstDefense.GetMeanMod());
                    break;
                case 16:
                    BurstSpeed.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(BurstSpeed.GetRandomMod());
                    else
                        myMods.Add(BurstSpeed.GetMeanMod());
                    break;
                case 17:
                    BurstSignatureGain.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(BurstSignatureGain.GetRandomMod());
                    else
                        myMods.Add(BurstSignatureGain.GetMeanMod());
                    break;
                case 18:
                    LuckyStrike.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(LuckyStrike.GetRandomMod());
                    else
                        myMods.Add(LuckyStrike.GetMeanMod());
                    break;
                case 19:
                    QuickDodge.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(QuickDodge.GetRandomMod());
                    else
                        myMods.Add(QuickDodge.GetMeanMod());
                    break;
                case 20:
                    SignatureDamageUp.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(SignatureDamageUp.GetRandomMod());
                    else
                        myMods.Add(SignatureDamageUp.GetMeanMod());
                    break;
                case 21:
                    SignatureDurationUp.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(SignatureDurationUp.GetRandomMod());
                    else
                        myMods.Add(SignatureDurationUp.GetMeanMod());
                    break;
                case 22:
                    CrisisStrength.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(CrisisStrength.GetRandomMod()));
                    else
                        myMods.Add((int)CrisisStrength.GetMeanMod());
                    break;
                case 23:
                    CrisisDefense.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(CrisisDefense.GetRandomMod()));
                    else
                        myMods.Add((int)CrisisDefense.GetMeanMod());
                    break;
                case 24:
                    CrisisSpeed.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(CrisisSpeed.GetRandomMod());
                    else
                        myMods.Add(CrisisSpeed.GetMeanMod());
                    break;
                case 25:
                    CrisisSignatureGain.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(CrisisSignatureGain.GetRandomMod());
                    else
                        myMods.Add(CrisisSignatureGain.GetMeanMod());
                    break;
                case 26:
                    HealthDrain.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(HealthDrain.GetRandomMod()));
                    else
                        myMods.Add((int)HealthDrain.GetMeanMod());
                    break;
                case 27:
                    SignatureDrain.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(SignatureDrain.GetRandomMod()));
                    else
                        myMods.Add((int)SignatureDrain.GetMeanMod());
                    break;
                case 28:
                    PityCounter.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(PityCounter.GetRandomMod()));
                    else
                        myMods.Add((int)PityCounter.GetMeanMod());
                    break;
                case 29:
                    PitySignature.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt(PitySignature.GetRandomMod()));
                    else
                        myMods.Add((int)PitySignature.GetMeanMod());
                    break;
                default:
                    break;
            }
            possibilities.RemoveAt(j);

            //GameObject dummy = Instantiate((GameObject)null);
            //dummy.AddComponent(System.Type.GetType(Ability.GetNames()[myAbilities[i]]));
            //myMods.Add(((Ability)dummy.GetComponent(System.Type.GetType(Ability.GetNames()[myAbilities[i]]))).GetRandomMod());
            //myAbilities.RemoveAt(j);
            //Destroy(dummy);
        }

        float pow;
        float dur;

        switch (wType)
        {
            case 0:
                pow = Sword.GetBasePower();
                dur = Sword.GetBaseDurability();
                break;
            case 1:
                pow = Axe.GetBasePower();
                dur = Axe.GetBaseDurability();
                break;
            case 2:
                pow = Spear.GetBasePower();
                dur = Spear.GetBaseDurability();
                break;
            case 3:
                pow = Crossbow.GetBasePower();
                dur = Crossbow.GetBaseDurability();
                break;
            case 4:
                pow = Tome.GetBasePower();
                dur = Tome.GetBaseDurability();
                break;
            default:
                pow = 0;
                dur = 0;
                break;
        }

        if (powMultFloor <= 0 || durMultFloor <= 0 || powMultFloor > powMultCeil || durMultFloor > durMultCeil)
            Debug.LogError("Invalid Power " + "([" + powMultFloor + ", " + powMultCeil + "]) or Durability " + "([" + durMultFloor + ", " + durMultCeil + "]) Range!");
        else
        {
            pow = Mathf.Round(pow * Random.Range(powMultFloor, powMultCeil) * 10.0f) * 0.1f;
            dur = (int)(Mathf.Round(dur * Random.Range(durMultFloor, durMultCeil) * 10.0f) * 0.1f);
        }

        weapon.Initialize(wType, pow, dur, dur, 0, myAbilities, myMods);

        return weapon;
    }

    public override void Step()
    {
        if (level < 13)
        {
            base.Step();
            spawnManager.SetWaveInfo(0, 0);
            spawnManager.DestroyAllEnemies();
            player.TakeDamage(((int)player.GetCurrentHealth()) - 30, Vector3.zero);

            int wave = ((ProtoSpawnManager02)spawnManager).GetFirstWave(level);
            if (wave > -1)
            {
                ((ProtoSpawnManager02)spawnManager).SetSuperWave(wave - 1);
            }

            switch (level)
            {
                // Set Boss Wave to true when summon boss, false otherwise
                //Part I
                case 1: //Sword vs. Tangerine Troll
                    PickupCW sword = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    sword.Initialize(0, Sword.GetBasePower(), 0, 0, 0, null, null);
                    sword.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);

                    tipText.gameObject.SetActive(true);
                    List<string> messages = new List<string>();
                    messages.Add("Tip: Mash or hold Left Click to perform up to three Sword slices!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 2: //Axe vs. Pink Python
                    player.RemoveCustomWeapon();
                    PickupCW axe = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    axe.Initialize(1, Axe.GetBasePower(), 0, 0, 0, null, null);
                    axe.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);

                    messages = new List<string>();
                    messages.Add("Tip: Right click to roll forward! Use this to reposition your Axe swing.");
                    tipRotation.SetMessageList(messages);
                    break;
                case 3: //Crossbow vs. Cerulean Satyr
                    player.RemoveCustomWeapon();
                    PickupCW crossbow = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    crossbow.Initialize(3, Crossbow.GetBasePower(), 0, 0, 0, null, null);
                    crossbow.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);

                    messages = new List<string>();
                    messages.Add("Tip: Mash or hold Left Click to perform up to five shots with the Crossbow!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 4: //Spear vs. Turquoise Templar
                    player.RemoveCustomWeapon();
                    PickupCW spear = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    spear.Initialize(2, Spear.GetBasePower(), 0, 0, 0, null, null);
                    spear.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);

                    messages = new List<string>();
                    messages.Add("Tip: The Spear deals the most damage at the tip!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 5: //Tome vs. Wisteria Wizard
                    player.RemoveCustomWeapon();
                    PickupCW tome = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    tome.Initialize(4, Tome.GetBasePower(), 0, 0, 0, null, null);
                    tome.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 1);

                    messages = new List<string>();
                    messages.Add("Tip: When you’re in Crisis (≤25% Health), the Tome deals slightly more damage!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 6: //Magestic
                    player.RemoveCustomWeapon();
                    List<PickupCW> pickups = new List<PickupCW>();
                    for (int i = 0; i < 5; i++)
                    {
                        pickups.Add(GenerateWeapon(i));
                        pickups[i].gameObject.transform.position = new Vector3(-5f + (2.5f * i), 1, -2f);
                        pickups[i].transform.parent = GetCurrent().transform;
                    }

                    spawnManager.SetWaveInfo(0, 1);
                    spawnManager.SetBossInfo(true);

                    messages = new List<string>();
                    messages.Add("Tip: Use the Scroll Wheel or (',' and '.') to choose different weapons!");
                    messages.Add("Tip: To defeat Magestic, stay calm and find openings in its attacks!");
                    tipRotation.SetMessageList(messages);
                    break;

                // PART II
                case 7: //Weapon Room
                    player.RemoveAllCustomWeapons();
                    pickups = new List<PickupCW>();
                    for (int i = 0; i < 10; i++)
                    {
                        pickups.Add(GenerateWeapon(i % 5));
                        pickups[i].gameObject.transform.position = new Vector3(-5f + (2.5f * (i % 5)), 1, -2f + (4f * (i / 5)));
                        pickups[i].transform.parent = GetCurrent().transform;
                    }
                    spawnManager.SetWaveInfo(0, 0);
                    spawnManager.SetBossInfo(false);

                    messages = new List<string>();
                    messages.Add("Tip: Press Q or Middle Click to see your full inventory, and E to drop your equipped weapon!");
                    messages.Add("Tip: The best way to learn how a Weapon Ability works is by using it!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 8: //All New Enemies
                    spawnManager.SetWaveInfo(0, 4);
                    spawnManager.SetSpawned(false);

                    messages = new List<string>();
                    messages.Add("Tip: If you roll away from an attack just before getting hit, you’ll fill up your weapon’s Signature Gauge!");
                    messages.Add("Tip: Hold Shift and Left Click when the Gauge is full to perform a Signature Attack!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 9: //Ogrelord
                    pickups = new List<PickupCW>();
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    pickups.Add(GenerateWeapon(Random.Range(0, 5)));
                    //    pickups[i].gameObject.transform.position = new Vector3(-2.5f + (2.5f * i), 1, -2f);
                    //    pickups[i].transform.parent = GetCurrent().transform;
                    //}
                    spawnManager.SetWaveInfo(0, 1);
                    spawnManager.SetBossInfo(true);

                    messages = new List<string>();
                    messages.Add("Tip: Focus on gaining and using Signature Moves (Shift + L-Click) to make quick work of the Ogrelord!");
                    tipRotation.SetMessageList(messages);
                    break;

                // PART III
                case 10: //Weapon Room
                    player.RemoveAllCustomWeapons();
                    pickups = new List<PickupCW>();
                    for (int i = 0; i < 10; i++)
                    {
                        pickups.Add(GenerateWeapon(i % 5, 4f / 5, 5f / 4, 2f / 3, 3f / 2, true));
                        pickups[i].gameObject.transform.position = new Vector3(-5f + (2.5f * (i % 5)), 1, -2f + (4f * (i / 5)));
                        pickups[i].transform.parent = GetCurrent().transform;
                    }
                    spawnManager.SetWaveInfo(0, 0);
                    spawnManager.SetBossInfo(false);

                    messages = new List<string>();
                    messages.Add("Tip: You’ve got one more Boss to fight!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 11: //All Enemies
                    spawnManager.SetWaveInfo(0, 4);
                    spawnManager.SetSpawned(false);

                    messages = new List<string>();
                    messages.Add("Tip: Enemies can drop weapons or potions! Access potions in your inventory (Q/Middle Click).");
                    tipRotation.SetMessageList(messages);
                    break;
                case 12:
                    //pickups = new List<PickupCW>();
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    pickups.Add(GenerateWeapon(Random.Range(0, 5), 4f / 5, 5f / 4, 2f / 3, 3f / 2));
                    //    pickups[i].gameObject.transform.position = new Vector3(-2.5f + (2.5f * i), 1, -2f);
                    //    pickups[i].transform.parent = GetCurrent().transform;
                    //}
                    spawnManager.SetWaveInfo(0, 1);
                    finalFloor = true;

                    messages = new List<string>();
                    messages.Add("Tip: The Twinotaurs don’t hit too hard, but move and attack fast! Stay calm and agile!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 13:
                    spawnManager.SetWaveInfo(0, 0);
                    tipRotation.gameObject.SetActive(false);
                    gameManager.Pause(2);
                    break;
                default:
                    break;
            }
        }
    }

    public bool IsFinalFloor()
    {
        return finalFloor;
    }

    /*************************
     * Room Stats
     *************************/
    public float GetRoomTime(int lev)
    {
        if (lev < 0 || lev > 12)
            return -1;
        return roomTimes[lev];
    }

    //Damage Taken Defeated
    public void AddDamageTaken(int pts)
    {
        damageTaken[level] += pts;
    }
    public int GetDamageTaken(int lev)
    {
        if (lev < 0 || lev > 12)
            return -1;
        return damageTaken[lev];
    }

    // Falls
    public void IncrementFalls()
    {
        falls[level]++;
    }
    public int GetFalls(int lev)
    {
        if (lev < 0 || lev > 12)
            return -1;
        return falls[lev];
    }

    // Potions Used
    public void IncrementPotionsUsed()
    {
        potionsUsed[level]++;
    }
    public int GetPotionsUsed(int lev)
    {
        if (lev < 0 || lev > 12)
            return -1;
        return potionsUsed[lev];
    }

    // Signature Pts Gained
    public void AddSignaturePointsGained(int pts)
    {
        signaturePtsGained[level] += pts;
    }
    public int GetSignaturePointsGained(int lev)
    {
        if (lev < 0 || lev > 12)
            return -1;
        return signaturePtsGained[lev];
    }

    // Signature Moves Used
    public void IncrementSignatureMovesUsed()
    {
        signatureMovesUsed[level]++;
    }
    public int GetSignatureMovesUsed(int lev)
    {
        if (lev < 0 || lev > 12)
            return -1;
        return signatureMovesUsed[lev];
    }
}
