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
    protected float roomTime = 0;

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
        {
            //roomTime += Time.deltaTime;
            //int minutes = (int)(roomTime / 60);
            //int seconds = (int)(roomTime % 60);
            //timeText.text = "<mark=#" + timeTextColor + ">Time\n" + minutes + ":" + $"{seconds:D2}";
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Step();
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            while (level < 7)
                Step();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            while (level < 10)
                Step();
        }
    }

    /*
     * Generates a weapon of type `wType`, with two abilities from `abilities`
     */
    public PickupCW GenerateWeapon(int wType, float powMultFloor = 1, float powMultCeil = 1, float durMultFloor = 1, float durMultCeil = 1)
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
                    myMods.Add((int)StrengthUp.GetMeanMod());
                    break;
                case 1:
                    DefenseUp.SetMinMaxMods();
                    myMods.Add((int)DefenseUp.GetMeanMod());
                    break;
                case 2:
                    StrengthDebilitator.SetMinMaxMods();
                    myMods.Add(StrengthDebilitator.GetMeanMod());
                    break;
                case 3:
                    DefenseDebilitator.SetMinMaxMods();
                    myMods.Add(DefenseDebilitator.GetMeanMod());
                    break;
                case 4:
                    AttackRateUp.SetMinMaxMods();
                    myMods.Add(AttackRateUp.GetMeanMod());
                    break;
                case 5:
                    DodgeRecoveryUp.SetMinMaxMods();
                    myMods.Add(DodgeRecoveryUp.GetMeanMod());
                    break;
                case 6:
                    HealthyStrength.SetMinMaxMods();
                    myMods.Add((int)HealthyStrength.GetMeanMod());
                    break;
                case 7:
                    HealthyDefense.SetMinMaxMods();
                    myMods.Add((int)HealthyDefense.GetMeanMod());
                    break;
                case 8:
                    HealthySpeed.SetMinMaxMods();
                    myMods.Add(HealthySpeed.GetMeanMod());
                    break;
                case 9:
                    HealthySignatureGain.SetMinMaxMods();
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
                    myMods.Add(AttackRangeUp.GetMeanMod());
                    break;
                case 13:
                    DodgeDistanceUp.SetMinMaxMods();
                    myMods.Add(DodgeDistanceUp.GetMeanMod());
                    break;
                case 14:
                    BurstStrength.SetMinMaxMods();
                    myMods.Add((int)BurstStrength.GetMeanMod());
                    break;
                case 15:
                    BurstDefense.SetMinMaxMods();
                    myMods.Add((int)BurstDefense.GetMeanMod());
                    break;
                case 16:
                    BurstSpeed.SetMinMaxMods();
                    myMods.Add(BurstSpeed.GetMeanMod());
                    break;
                case 17:
                    BurstSignatureGain.SetMinMaxMods();
                    myMods.Add(BurstSignatureGain.GetMeanMod());
                    break;
                case 18:
                    LuckyStrike.SetMinMaxMods();
                    myMods.Add(LuckyStrike.GetMeanMod());
                    break;
                case 19:
                    QuickDodge.SetMinMaxMods();
                    myMods.Add(QuickDodge.GetMeanMod());
                    break;
                case 20:
                    SignatureDamageUp.SetMinMaxMods();
                    myMods.Add(SignatureDamageUp.GetMeanMod());
                    break;
                case 21:
                    SignatureDurationUp.SetMinMaxMods();
                    myMods.Add(SignatureDurationUp.GetMeanMod());
                    break;
                case 22:
                    CrisisStrength.SetMinMaxMods();
                    myMods.Add((int)CrisisStrength.GetMeanMod());
                    break;
                case 23:
                    CrisisDefense.SetMinMaxMods();
                    myMods.Add((int)CrisisDefense.GetMeanMod());
                    break;
                case 24:
                    CrisisSpeed.SetMinMaxMods();
                    myMods.Add(CrisisSpeed.GetMeanMod());
                    break;
                case 25:
                    CrisisSignatureGain.SetMinMaxMods();
                    myMods.Add(CrisisSignatureGain.GetMeanMod());
                    break;
                case 26:
                    HealthDrain.SetMinMaxMods();
                    myMods.Add(HealthDrain.GetMeanMod());
                    break;
                case 27:
                    SignatureDrain.SetMinMaxMods();
                    myMods.Add(SignatureDrain.GetMeanMod());
                    break;
                case 28:
                    PityCounter.SetMinMaxMods();
                    myMods.Add(PityCounter.GetMeanMod());
                    break;
                case 29:
                    PitySignature.SetMinMaxMods();
                    myMods.Add(PitySignature.GetMeanMod());
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
        if (level < 12)
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
                    messages.Add("Tip: Mash or hold Left Click to perform up to three Sword swipes!");
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
                case 4: //Tome vs. Turquoise Templar
                    player.RemoveCustomWeapon();
                    PickupCW tome = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    tome.Initialize(4, Tome.GetBasePower(), 0, 0, 0, null, null);
                    tome.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);

                    messages = new List<string>();
                    messages.Add("Tip: When you’re in Crisis (≤25% Health), the Tome deals slightly more damage!");
                    tipRotation.SetMessageList(new List<string>(messages));
                    break;
                case 5: //Spear vs. Wisteria Wizard
                    player.RemoveCustomWeapon();
                    PickupCW spear = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    spear.Initialize(2, Spear.GetBasePower(), 0, 0, 0, null, null);
                    spear.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);

                    messages = new List<string>();
                    messages.Add("Tip: The Spear deals the most damage at the tip!");
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
                    messages.Add("Tip: Use the Scroll Wheel or (Comma and Period) to choose different weapons!");
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
                    messages.Add("Tip: Press Q to see your inventory, and E to drop your equipped weapon!");
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
                    for (int i = 0; i < 3; i++)
                    {
                        pickups.Add(GenerateWeapon(Random.Range(0, 5)));
                        pickups[i].gameObject.transform.position = new Vector3(-2.5f + (2.5f * i), 1, -2f);
                        pickups[i].transform.parent = GetCurrent().transform;
                    }
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
                        pickups.Add(GenerateWeapon(i % 5, 4f / 5, 5f / 4, 2f / 3, 3f / 2));
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

                    messages = new List<string>();
                    messages.Add("Tip: Enemies can drop weapons or potions! Access potions in your inventory (Q)!");
                    tipRotation.SetMessageList(messages);
                    break;
                case 12:
                    pickups = new List<PickupCW>();
                    for (int i = 0; i < 3; i++)
                    {
                        pickups.Add(GenerateWeapon(Random.Range(0, 5), 4f / 5, 5f / 4, 2f / 3, 3f / 2));
                        pickups[i].gameObject.transform.position = new Vector3(-2.5f + (2.5f * i), 1, -2f);
                        pickups[i].transform.parent = GetCurrent().transform;
                    }
                    spawnManager.SetWaveInfo(0, 1);
                    finalFloor = true;

                    messages = new List<string>();
                    messages.Add("Tip: The Twinotaurs move and attack fast, but don’t hit too hard!");
                    tipRotation.SetMessageList(messages);
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
}
