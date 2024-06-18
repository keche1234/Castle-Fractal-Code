using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour //Doubles as game manager
{
    [SerializeField] protected Room current; //the head of the linked list
    [SerializeField] protected PlayerController player;
    [SerializeField] protected int level; //offset of 0
    [SerializeField] protected float height;
    protected int bossInterval = 1;
    //[SerializeField] protected ExitDoor exit;

    //[Header("Prefabs")]
    [SerializeField] protected Room emptyRoomPrefab;
    //[SerializeField] protected GameObject floorPrefab;
    //[SerializeField] protected GameObject borderPrefab;
    //[SerializeField] protected GameObject wallPrefab;
    //[SerializeField] protected GameObject chestPrefab;
    //[SerializeField] protected GameObject doorPrefab;
    [SerializeField] protected List<Boss> bossPrefabs;

    [Header("Game Management")]
    [SerializeField] protected SpawnManager spawnManager;
    [SerializeField] protected PickupCW pickupPrefab;

    protected const float MERCY_WEAPON_TIME = 10f;
    protected float spawnWeaponTimer = 0;

    // Start is called before the first frame update
    protected void Start()
    {
        level = 0;
        CreateNext();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (player.InventoryCount() == 0 && FindObjectsOfType(System.Type.GetType("PickupCW")).Length == 0)
        {
            if (spawnWeaponTimer >= MERCY_WEAPON_TIME)
            {
                Room r = GetCurrent();
                Vector3 weaponPos = new Vector3(Random.Range((-r.GetXDimension() + 2) / 2, (r.GetXDimension() - 2) / 2), 1, Random.Range((-r.GetZDimension() + 2) / 2, (r.GetZDimension() - 2) / 2));
                PickupCW mercy = GenerateWeapon(Random.Range(0, 5), 4f / 5, 5f / 4, 2f / 3, 3f / 2, true);

                mercy.gameObject.transform.position = weaponPos;
                mercy.gameObject.transform.parent = r.gameObject.transform;
                spawnWeaponTimer = 0;
            }
            else
                spawnWeaponTimer += Time.deltaTime;
        }
    }

    public virtual void CreateNext()
    {
        Debug.Log(level);
        Room next = Instantiate(emptyRoomPrefab, new Vector3(0, height, 0), Quaternion.Euler(0, 0, 0));
        next.Initialize(20, 10, this, spawnManager);
        if ((level + 1) % bossInterval == 0) //Boss Room
        {
            //Select a random boss
            next.SetBossRoom(true);
            next.SetBossNumber(Random.Range(0, bossPrefabs.Count));
        }
        next.GenerateRoom();
        //Vector3 topL = new Vector3(-9.5f, height, -4.5f);
        //int length = 19;
        //int width = 9;

        //Set next
        //next.Initialize(topL, length, width, borderPrefab, spawnManager);
        //next.AddFloor(Instantiate(floorPrefab, new Vector3(0, height, 0), Quaternion.Euler(0, 0, 0)));
        //next.AddEntrance(Instantiate(doorPrefab, new Vector3(-0.5f, height + 0.75f, -4.75f), Quaternion.Euler(0, -90, 0)));

        // TODO: Add this to exit set up!
        //GameObject exit = Instantiate(doorPrefab, new Vector3(-0.5f, height + 0.75f, 4.75f), Quaternion.Euler(0, 90, 0));
        //exit.AddComponent<ExitDoor>();
        //exit.GetComponent<ExitDoor>().SetRoomManager(this);
        //exit.GetComponent<ExitDoor>().AddLocked(locked);
        //exit.GetComponent<ExitDoor>().AddUnlocked(unlocked);
        //next.AddExit(exit);

        //next.CreateBorders();

        //
        //TODO: Generate environmental components (Bosses have different requirements, like no internal walls)

        current.SetNext(next);
    }

    public Room GetCurrent()
    {
        return current;
    }

    public Room GetNext()
    {
        return current.GetNext();
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetSpawnManager(SpawnManager sm)
    {
        spawnManager = sm;
    }

    public virtual void Step()
    {
        //Push the next room down to current
        Room temp = current;
        current = temp.GetNext();
        Destroy(temp.gameObject);
        current.gameObject.transform.Translate(0, -height, 0);

        //Game Info
        level++;

        //Create the next room
        CreateNext();

        //Move the player
        player.transform.position = current.GetEntrance().transform.position + current.GetEntrance().transform.forward - new Vector3(0, 0.25f, 0);
        player.transform.rotation = current.GetEntrance().transform.rotation;
        StartCoroutine(DisablePlayer(0.5f));

        if (level % bossInterval == 0)
        {
            //spawn boss
            current.SetBoss(Instantiate(bossPrefabs[current.GetBossNumber()], bossPrefabs[current.GetBossNumber()].transform.position, Quaternion.Euler(0, 0, 0)));
            spawnManager.SetBoss(current.GetBoss());
            spawnManager.SetAllDefeated(false);
        }
        else
        {
            spawnManager.SetWaveInfo(0, 1);
            spawnManager.SetBoss(null);
        }
    }

    protected IEnumerator DisablePlayer(float wait)
    {
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Rigidbody>().velocity *= 0;
        yield return new WaitForSeconds(wait);
        player.GetComponent<PlayerController>().enabled = true;
        player.SetMobile(true);
        player.SetAttackState(0);
        player.OverrideInvincibility(0);
    }

    public PickupCW GenerateWeapon(int wType, float powMultFloor = 1, float powMultCeil = 1, float durMultFloor = 1, float durMultCeil = 1, bool randomMod = false)
    {
        if (wType < 0 || wType > 4)
            return null;

        PickupCW weapon = Instantiate(pickupPrefab);
        //List<int> possibilities = new List<int>(abilities[wType]);

        List<int> myAbilities = new List<int>();
        List<float> myMods = new List<float>();
        for (int i = 0; i < 2; i++)
        {
            int ability = Random.Range(0, Ability.GetGenericNames().Length);
            if (i > 0)
            {
                while (ability == myAbilities[0])
                    ability = Random.Range(0, Ability.GetGenericNames().Length);
            }
            myAbilities.Add(ability);

            switch (ability)
            {
                case 0:
                    StrengthUp.SetMinMaxMods();

                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt((int)StrengthUp.GetRandomMod()));
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
                case 10: //Blade Dull
                case 11: //Armor Pierce
                    myMods.Add(0);
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
                case 30:
                    HealthyLionheart.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt((int)HealthyLionheart.GetRandomMod()));
                    else
                        myMods.Add((int)HealthyLionheart.GetMeanMod());
                    break;
                case 31:
                    CrisisLionheart.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt((int)CrisisLionheart.GetRandomMod()));
                    else
                        myMods.Add((int)CrisisLionheart.GetMeanMod());
                    break;
                case 32:
                    HealthyWolfsoul.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt((int)HealthyWolfsoul.GetRandomMod()));
                    else
                        myMods.Add((int)HealthyWolfsoul.GetMeanMod());
                    break;
                case 33:
                    CrisisWolfsoul.SetMinMaxMods();
                    if (randomMod)
                        myMods.Add(Mathf.RoundToInt((int)CrisisWolfsoul.GetRandomMod()));
                    else
                        myMods.Add((int)CrisisWolfsoul.GetMeanMod());
                    break;
                case 34: //All or Nothing D
                case 35: //All or Nothing S
                    myMods.Add(0);
                    break;
                default:
                    break;
            }

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
}
