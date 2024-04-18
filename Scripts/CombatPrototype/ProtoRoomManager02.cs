using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProtoRoomManager02 : RoomManager
{
    [SerializeField] protected TextMeshProUGUI timeText;
    [SerializeField] protected string timeTextColor;
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

        // Sword abilities
        for (int j = 0; j < 6; j++)
            abilities[0].Add((0 - (j * 6) + j) % 30);

        // Axe abilities
        for (int j = 0; j < 6; j++)
            abilities[0].Add((24 - (j * 6) + j) % 30);

        // Spear abilities
        for (int j = 0; j < 6; j++)
            abilities[0].Add((18 - (j * 6) + j) % 30);

        // Crossbow abilities
        for (int j = 0; j < 6; j++)
            abilities[0].Add((12 - (j * 6) + j) % 30);

        // Tome abilities
        for (int j = 0; j < 6; j++)
            abilities[0].Add((6 - (j * 6) + j) % 30);
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
    public PickupCW GenerateWeapon(int wType)
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

            GameObject dummy = Instantiate((GameObject)null);
            dummy.AddComponent(System.Type.GetType(Ability.GetNames()[myAbilities[i]]));
            myMods.Add(((Ability)dummy.GetComponent(System.Type.GetType(Ability.GetNames()[myAbilities[i]]))).GetRandomMod());
            myAbilities.RemoveAt(j);
            Destroy(dummy);
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

        weapon.Initialize(wType, pow, dur, dur, 0, myAbilities, myMods);

        return weapon;
    }

    public override void Step()
    {
        if (level < 12)
        {
            base.Step();
            spawnManager.SetSpawned(false);
            player.TakeDamage(((int)player.GetCurrentHealth()) - 30, Vector3.zero);
            switch (level)
            {
                // TODO Set Boss Wave to true when summon boss, false otherwise
                //Part I
                case 1: //Tangerine Troll
                    PickupCW sword = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    sword.Initialize(0, Sword.GetBasePower(), 0, 0, 0, null, null);
                    sword.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);
                    break;
                case 2: //Pink Python
                    player.RemoveCustomWeapon();
                    PickupCW axe = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    axe.Initialize(1, Axe.GetBasePower(), 0, 0, 0, null, null);
                    axe.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);
                    break;
                case 3: //Cerulean Satyr
                    player.RemoveCustomWeapon();
                    PickupCW tome = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    tome.Initialize(4, Tome.GetBasePower(), 0, 0, 0, null, null);
                    tome.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);
                    break;
                case 4: //Turquoise Templar
                    player.RemoveCustomWeapon();
                    PickupCW crossbow = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    crossbow.Initialize(3, Crossbow.GetBasePower(), 0, 0, 0, null, null);
                    crossbow.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);
                    break;
                case 5: //Wisteria Wizard
                    player.RemoveCustomWeapon();
                    PickupCW spear = Instantiate(pickupPrefab, new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    spear.Initialize(2, Spear.GetBasePower(), 0, 0, 0, null, null);
                    spear.transform.parent = GetCurrent().transform;
                    spawnManager.SetWaveInfo(0, 2);
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
                    break;

                // PART II
                // TODO: Make room weapon parent
                case 7: //Weapon Room
                    player.RemoveAllCustomWeapons();
                    pickups = new List<PickupCW>();
                    for (int i = 0; i < 10; i++)
                    {
                        pickups.Add(GenerateWeapon(i));
                        pickups[i].gameObject.transform.position = new Vector3(-5f + (2.5f * i), 1, -2f + (4f * (i / 5)));
                    }
                    spawnManager.SetWaveInfo(0, 0);
                    spawnManager.SetBossInfo(false);
                    break;
                case 8: //All New Enemies
                    spawnManager.SetWaveInfo(0, 4);
                    break;
                case 9: //Ogrelord
                    pickups = new List<PickupCW>();
                    for (int i = 0; i < 5; i++)
                    {
                        pickups.Add(GenerateWeapon(i));
                        pickups[i].gameObject.transform.position = new Vector3(-5f + (2.5f * i), 1, -2f);
                    }
                    spawnManager.SetWaveInfo(0, 1);
                    spawnManager.SetBossInfo(true);
                    break;

                // PART III
                case 10: //Weapon Room
                    pickups = new List<PickupCW>();
                    for (int i = 0; i < 10; i++)
                    {
                        pickups.Add(GenerateWeapon(i));
                        pickups[i].gameObject.transform.position = new Vector3(-5f + (2.5f * i), 1, -2f + (4f * (i / 5)));
                    }
                    spawnManager.SetWaveInfo(0, 0);
                    spawnManager.SetBossInfo(false);
                    break;
                case 11: //All Enemies
                    spawnManager.SetWaveInfo(0, 4);
                    break;
                case 12:
                    spawnManager.SetWaveInfo(0, 1);
                    break;
                default:
                    break;
            }
        }

        //if (level < 6)
        //{
        //    base.Step();
        //    player.TakeDamage(((int)player.GetCurrentHealth()) - 30);
        //    switch (level)
        //    {
        //        case 1:
        //            //Add appropriate powerup
        //            PickupCW sword = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            sword.Initialize(0, 9, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 2:
        //            //Remove weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW crossbow = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            crossbow.Initialize(3, 3, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 3:
        //            //Break weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW spear = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            spear.Initialize(2, 6, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 4:
        //            //Remove weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW axe = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            axe.Initialize(1, 12, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 5:
        //            //Remove weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW tome = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            tome.Initialize(4, 15, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 6:
        //            finalFloor = true;
        //            player.RemoveCustomWeapon();
        //            PickupCW[] pickups = new PickupCW[5];
        //            for (int i = 0; i < pickups.Length; i++)
        //                pickups[i] = Instantiate(pickupPrefab, new Vector3(-4 + (2 * i), 0.75f, -2), Quaternion.Euler(0, 0, 0));

        //            pickups[0].Initialize(0, 9, 20, 20, null, null);
        //            pickups[1].Initialize(1, 12, 25, 25, null, null);
        //            pickups[2].Initialize(2, 6, 15, 15, null, null);
        //            pickups[3].Initialize(3, 3, 30, 30, null, null);
        //            pickups[4].Initialize(4, 15, 10, 10, null, null);
        //            spawnManager.SetWaveInfo(0, 5);
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }

    public bool IsFinalFloor()
    {
        return finalFloor;
    }
}
