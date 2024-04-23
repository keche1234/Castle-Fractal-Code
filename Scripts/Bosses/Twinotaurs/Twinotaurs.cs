using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Twinotaurs : Boss
{
    public GameObject spark; //for pointing purposes
    public GameObject venom; //for pointing purposes
    private Rigidbody sparkRb;
    private Rigidbody venomRb;
    protected const float AIR_TIME = 0.8f; //jump towards a wall

    //[Header("Status Bars UI 2")] (attached to Venom)
    protected BarUI miniHealthBarVenom;
    protected Canvas attributesUIVenom;
    protected Text strengthUIVenom;
    protected Text defenseUIVenom;

    [Header("Cross Gas Zap")]
    [SerializeField] protected Hitbox venomCharge;
    [SerializeField] protected Hitbox sparkCharge;
    [SerializeField] protected Projectile poisonCloudPrefab;
    protected List<Projectile> poisonClouds = new List<Projectile>();
    protected const float DASH_START = 2f;
    protected const float DASH_COOL = 1f;
    protected const float CLOUD_DELAY = 0.5f;

    [Header("Golden Pounce")]
    [SerializeField] protected Hitbox pounceBoltPrefab;
    [SerializeField] protected List<GameObject> boltIndicators;
    [SerializeField] protected List<GameObject> boltWarnings;
    protected List<GameObject> pounceClouds;
    protected ThunderCage cage;
    protected const float POUNCE_GAS_SETUP = 4f; // Set up for the clouds
    //protected const float POUNCE_CLOUD_INITSPEED = 8f;
    protected const float POUNCE_BOLT_STARTUP = 0.5f; // Set up for spark to charge
    protected const float POUNCE_MAX_DIST = 6f;
    protected const float POUNCE_STARTUP = 0.4f;
    protected const float POUNCE_DURATION = 0.4f;
    protected const float POUNCE_ENDLAG = 1.2f;
    protected const int POUNCE_COUNT = 6;
    protected const float POUNCE_COOL = 2f;

    // use thunderGasPrefab from Summoners' Burst

    [Header("Perilous Partition")]
    [SerializeField] protected Projectile zapPathPrefab;
    [SerializeField] protected Projectile noxPathPrefab;
    protected List<Projectile> zapPath = new List<Projectile>();
    protected List<Projectile> noxPath = new List<Projectile>();
    protected const float PERILOUS_START = 1f;
    protected const float PERILOUS_ACTIVE = 6f;
    //protected const float PERILOUS_COOL = 3f;

    [Header("Syncrash")]
    [SerializeField] protected Hitbox crash;
    [SerializeField] protected GameObject wrathBoltPrefab;
    [SerializeField] protected GameObject sparkCrashIndicator;
    [SerializeField] protected GameObject sparkCrashWarning;
    [SerializeField] protected GameObject venomCrashIndicator;
    [SerializeField] protected GameObject venomCrashWarning;
    [SerializeField] protected GameObject centerCrashIndicator;
    [SerializeField] protected GameObject centerCrashWarning;
    protected const float CRASH_COOL = 4f;

    [Header("Summoners' Burst")]
    [SerializeField] protected Projectile thunderGasPrefab;
    [SerializeField] protected List<GameObject> summonKOIndicators;
    [SerializeField] protected List<GameObject> summonKOWarnings;
    // TODO: Warning list for Twinotaurs

    [Header("Summoners' Cyclone")]
    [SerializeField] protected Cyclone cyclonePrefab;

    protected List<Cyclone> cycloneList;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        charRb = null;
        sparkRb = spark.GetComponent<Rigidbody>();
        venomRb = venom.GetComponent<Rigidbody>();

        numAttacks = 7;
        currentAttack = Random.Range(0, 2);
        //currentAttack = 1;
        power = 10f;
        currentHealth = 600;
        maxHealth = 600;
        summonCount = 3;

        speed = 6f;
        rotateSpeed = Mathf.PI;
        armored = true;

        // Set up the thunder cage
        cage = (ThunderCage)ScriptableObject.CreateInstance("ThunderCage");
        Hitbox cageBolt = Instantiate(pounceBoltPrefab).GetComponent<Hitbox>();
        cageBolt.SetSource(this);
        cage.SetBoltObject(cageBolt);
        pounceClouds = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            Projectile cloud = Instantiate(thunderGasPrefab, transform.position, Quaternion.Euler(0, 0, 0));
            cloud.gameObject.transform.localScale = new Vector3(2, 2, 2);
            cloud.Setup(0, this, false, 0.01f, -1, 0, 0, true, true);
            pounceClouds.Add(cloud.gameObject);
            pounceClouds[i].gameObject.SetActive(false);
        }
        cage.SetThunderLinks(spark, pounceClouds);

        // Set up cyclones
        cycloneList = new List<Cyclone>();
        for (int i = 0; i < summonCount; i++)
        {
            Cyclone cyclone = Instantiate(cyclonePrefab, transform.position, Quaternion.Euler(0, 0, 0));
            cyclone.GetComponent<Hitbox>().SetSource(this);
            cyclone.transform.parent = transform;
            cyclone.gameObject.SetActive(false);
            cycloneList.Add(cyclone);
        }

        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        miniHealthBar.gameObject.transform.parent.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        miniHealthBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(spark, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -100));

        attributesUI.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        attributesUI.GetComponent<UIAttach>().Setup(spark, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -140));

        miniHealthBarVenom = Instantiate(miniHealthBar.gameObject.transform.parent).GetChild(0).gameObject.GetComponent<BarUI>();
        attributesUIVenom = Instantiate(attributesUI).GetComponent<Canvas>();
        strengthUIVenom = attributesUIVenom.transform.Find("Group").Find("S-Text").gameObject.GetComponent<Text>();
        defenseUIVenom = attributesUIVenom.transform.Find("Group").Find("D-Text").gameObject.GetComponent<Text>();

        miniHealthBarVenom.gameObject.transform.parent.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        miniHealthBarVenom.gameObject.transform.parent.GetComponent<UIAttach>().Setup(venom, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -100));

        attributesUIVenom.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        attributesUIVenom.GetComponent<UIAttach>().Setup(venom, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -140));

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        for (int i = 0; i < poisonClouds.Count; i++)
            if (poisonClouds[i] == null)
                poisonClouds.RemoveAt(i--);

        for (int i = 0; i < zapPath.Count; i++)
            if (zapPath[i] == null)
                zapPath.RemoveAt(i--);

        for (int i = 0; i < noxPath.Count; i++)
            if (noxPath[i] == null)
                noxPath.RemoveAt(i--);
    }

    //snap == true instantly turns obj to face the player
    //otherwise, use rotate speed
    protected void LookTowardsPlayer(GameObject obj, bool snap = false)
    {
        Vector3 lookVector = new Vector3(player.transform.position.x - obj.transform.position.x, 0, player.transform.position.z - obj.transform.position.z);

        float rotateVal;
        if (snap)
        {
            rotateVal = Mathf.PI * 2;
            obj.GetComponent<Rigidbody>().angularVelocity *= 0f;
        }
        else
            rotateVal = rotateSpeed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(obj.transform.forward, lookVector, rotateVal, 0.0f);
        obj.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected void LookTowardsPlayer(bool snap = false)
    {
        LookTowardsPlayer(spark, snap);
        LookTowardsPlayer(venom, snap);
    }

    /*
     * Used for the CROSS GAS ZAP attack
     * Has obj jump to the left side of the arena (side == -1),
     * to the right side of the arena (side == 1),
     * or strictly away from the player (other)
     */
    protected void CrossGasZapJump(GameObject obj, int side = 0)
    {
        if (side == -1 || side == 1)
        {
            Vector3 jump = new Vector3(0, Physics.gravity.magnitude * AIR_TIME * 0.5f, 0);
            Vector3 latDir = (new Vector3(side, 0, 0)).normalized;
            float xDist = (((0.5f * roomManager.GetCurrent().GetLength()) - 3) * latDir.x) - obj.transform.position.x;
            float zDist = -obj.transform.position.z;

            obj.GetComponent<Rigidbody>().AddForce(new Vector3(xDist / AIR_TIME, Physics.gravity.magnitude * AIR_TIME * 0.5f, zDist / AIR_TIME), ForceMode.VelocityChange);
            obj.GetComponent<Rigidbody>().useGravity = true;
            obj.transform.rotation = Quaternion.LookRotation(-latDir);
        }
        else
        {
            Vector3 jump = new Vector3(0, Physics.gravity.magnitude * AIR_TIME * 0.5f, 0);
            Vector3 latDir = (new Vector3(obj.transform.position.x - player.transform.position.x, 0, obj.transform.position.z - player.transform.position.z)).normalized;
            float xDist = (((0.5f * roomManager.GetCurrent().GetLength()) - 3) * latDir.x) - obj.transform.position.x;
            float zDist = (((0.5f * roomManager.GetCurrent().GetWidth()) - 3) * latDir.z) - obj.transform.position.z;

            obj.GetComponent<Rigidbody>().AddForce(new Vector3(xDist / AIR_TIME, Physics.gravity.magnitude * AIR_TIME * 0.5f, zDist / AIR_TIME), ForceMode.VelocityChange);
            obj.GetComponent<Rigidbody>().useGravity = true;
            obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            obj.transform.rotation = Quaternion.LookRotation(-latDir);
        }
    }

    /*
     * Used for the PERILOUS PARTITION attack
     * Has obj jump to the left side of the arena (side == -1),
     * to the right side of the arena (side == 1)
     */
    protected void PartitionJump(GameObject obj, int side)
    {
        if (side == -1 || side == 1)
        {
            Vector3 jump = new Vector3(0, Physics.gravity.magnitude * AIR_TIME * 0.5f, 0);
            Vector3 latDir = (new Vector3(side, 0, 0)).normalized;
            float xDist = (((0.2f * roomManager.GetCurrent().GetLength()) * latDir.x) - obj.transform.position.x);
            float zDist = -obj.transform.position.z;

            obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            obj.GetComponent<Rigidbody>().AddForce(new Vector3(xDist / AIR_TIME, Physics.gravity.magnitude * AIR_TIME * 0.5f, zDist / AIR_TIME), ForceMode.VelocityChange);
            obj.GetComponent<Rigidbody>().useGravity = true;
            obj.transform.rotation = Quaternion.LookRotation(transform.forward * side);
        }
    }

    /*
     * Used for the PERILOUS PARTITION attack
     * Spawns a piece of the path
     */
    protected void PartitionSpawn(Projectile prefab, List<Projectile> path, GameObject obj)
    {
        path.Add(Instantiate(prefab, new Vector3(obj.transform.position.x, 0.05f, obj.transform.position.z), Quaternion.Euler(0, 0, 0)));
        path[path.Count - 1].SetSource(this);
    }

    protected void Syncrash()
    {

    }

    protected override IEnumerator Attack()
    {
        float t;
        switch (currentAttack)
        {
            case 0: //Cross Gas Zap
                //startup
                state = ActionState.Startup;
                //Spark and Venom jump towards wall
                CrossGasZapJump(spark);
                CrossGasZapJump(venom);

                t = 0;
                while (t < AIR_TIME)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                sparkRb.GetComponent<Rigidbody>().useGravity = false;
                sparkRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                sparkRb.velocity *= 0f;
                venomRb.GetComponent<Rigidbody>().useGravity = false;
                venomRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                venomRb.velocity *= 0f;

                t = 0;
                while (t < DASH_START)
                {
                    if (freezeTime <= 0)
                    {
                        LookTowardsPlayer();
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                Vector3 mov;
                for (int i = 0; i < 3; i++)
                {
                    state = ActionState.Attacking;
                    LookTowardsPlayer(venom, true);

                    float poisonTimer = 0;
                    //Send Venom running towards player, with rotation. After certain intervals, enable a poison cloud
                    spark.GetComponent<Collider>().isTrigger = true;
                    venom.GetComponent<Collider>().isTrigger = true;
                    sparkRb.velocity *= 0f;
                    venomRb.velocity = venom.transform.forward * speed * (2f / 3);
                    mov = venomRb.velocity;

                    while (state == ActionState.Attacking)
                    {
                        if (!frozen)
                        {
                            venomRb.velocity = mov; //if being knocked back, don't try to apply current velocity
                            venomCharge.gameObject.SetActive(true);
                        }
                        else
                            venomCharge.gameObject.SetActive(false);

                        if (freezeTime <= 0)
                        {
                            if (poisonTimer > 0)
                                poisonTimer -= Time.deltaTime;
                            else
                            {
                                //spawn cloud
                                Projectile cloud = Instantiate(poisonCloudPrefab, venom.transform.position + Vector3.up, Quaternion.Euler(0, 0, 0)); //static projectile
                                cloud.Setup(0, this, false, 0, 6, .01f, 0, true, true, 0);
                                poisonClouds.Add(cloud);
                                poisonTimer = CLOUD_DELAY;
                            }
                            LookTowardsPlayer(spark);
                        }

                        yield return null;
                    }
                    venomCharge.gameObject.SetActive(false);

                    state = ActionState.Attacking;
                    LookTowardsPlayer(spark, true);
                    //Now send Spark running towards player, with rotation. After certain intervals, enable a poison cloud
                    spark.GetComponent<Collider>().isTrigger = true;
                    sparkRb.velocity = spark.transform.forward * speed * (4f / 3);
                    venomRb.velocity *= 0f;
                    mov = sparkRb.velocity;
                    while (state == ActionState.Attacking)
                    {
                        if (!frozen)
                        {
                            sparkRb.velocity = mov; //if being knocked back, don't try to apply current velocity
                            sparkCharge.gameObject.SetActive(true);
                        }
                        else
                            sparkCharge.gameObject.SetActive(false);
                        LookTowardsPlayer(venom);

                        yield return null;
                    }
                    sparkCharge.gameObject.SetActive(false);
                }

                spark.GetComponent<Collider>().isTrigger = false;
                venom.GetComponent<Collider>().isTrigger = false;

                sparkRb.velocity *= 0f;
                venomRb.velocity *= 0f;

                t = 0;
                while (t < DASH_COOL)
                {
                    if (freezeTime <= 0)
                    {
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                currentAttack = 2;
                state = ActionState.Waiting;
                break;
            case 1: // Golden Pounce
                state = ActionState.Startup;

                List<Vector3> gasGoals = new List<Vector3>();
                List<Vector3> gasAims = new List<Vector3>();
                List<float> gasDecels = new List<float>();

                for (int i = 0; i < 4; i++)
                {
                    // Set start and goals
                    float xPos = Random.Range((room.GetLength() / 2) * 0.5f, (room.GetLength() / 2) * 0.8f);
                    float zPos = Random.Range((room.GetWidth() / 2) * 0.5f, (room.GetWidth() / 2) * 0.8f);
                    pounceClouds[i].transform.position = venom.transform.position;

                    switch (i)
                    {
                        case 0:
                            xPos *= -1;
                            pounceClouds[i].transform.position += new Vector3(-1, 1, 1);
                            break;
                        case 1:
                            pounceClouds[i].transform.position += new Vector3(1, 1, 1);
                            break;
                        case 2:
                            zPos *= -1;
                            pounceClouds[i].transform.position += new Vector3(1, 1, -1);
                            break;
                        case 3:
                            xPos *= -1;
                            zPos *= -1;
                            pounceClouds[i].transform.position += new Vector3(-1, 1, -1);
                            break;
                        default:
                            break;
                    }
                    Vector3 goalPos = new Vector3(xPos, 1, zPos);
                    gasGoals.Add(goalPos);
                    //Debug.Log("Goal " + i + ": " + goalPos);

                    //Determine Aim
                    gasAims.Add(gasGoals[i] - pounceClouds[i].transform.position);
                    pounceClouds[i].gameObject.transform.rotation = Quaternion.LookRotation(gasAims[i]);
                    pounceClouds[i].gameObject.SetActive(true);

                    // Determine decel based on POUNCE_CLOUD_INITSPEED
                    gasDecels.Add(-2 * (goalPos - pounceClouds[i].transform.position).magnitude / Mathf.Pow(POUNCE_GAS_SETUP, 2));
                }

                // Spread out the poison with time
                t = 0;
                foreach (GameObject indicator in boltIndicators)
                    indicator.gameObject.SetActive(true);

                while (t < POUNCE_GAS_SETUP)
                {
                    for (int i = 0; i < pounceClouds.Count; i++)
                    {
                        Vector3 nextCloud;
                        if (i < pounceClouds.Count - 1)
                            nextCloud = pounceClouds[i + 1].transform.position;
                        else
                            nextCloud = pounceClouds[0].transform.position;

                        // Determine direction
                        Vector3 aim = nextCloud - pounceClouds[i].transform.position;
                        boltIndicators[i].transform.rotation = Quaternion.LookRotation(aim.normalized);

                        // Determine center
                        Vector3 centerLoc = (nextCloud + pounceClouds[i].transform.position) / 2;
                        boltIndicators[i].transform.position = new Vector3(centerLoc.x, 0.1f, centerLoc.z);

                        // Determine scale
                        boltIndicators[i].transform.localScale = new Vector3(boltIndicators[i].transform.localScale.x, boltIndicators[i].transform.localScale.y, aim.magnitude + 0.5f);
                    }

                    if (freezeTime <= 0)
                    {
                        // Move each gas cloud to its target
                        for (int i = 0; i < pounceClouds.Count; i++)
                            pounceClouds[i].GetComponent<Projectile>().SetSpeed(Mathf.Max(gasDecels[i] * (t - POUNCE_GAS_SETUP), 0));
                        LookTowardsPlayer(venom, false);
                        t += Time.deltaTime;

                        for (int i = 0; i < pounceClouds.Count; i++)
                            boltWarnings[i].transform.localScale = new Vector3(boltWarnings[i].transform.localScale.x, boltWarnings[i].transform.localScale.y, t / POUNCE_GAS_SETUP);
                    }
                    else
                    {
                        for (int i = 0; i < pounceClouds.Count; i++)
                            pounceClouds[i].GetComponent<Projectile>().SetSpeed(0);
                    }
                    yield return null;
                }
                foreach (GameObject indicator in boltIndicators)
                    indicator.gameObject.SetActive(false);

                // Enable Thunder Cage and Reset Bolt
                cage.EnableAll();
                spark.SetActive(false);
                miniHealthBar.gameObject.SetActive(false);
                attributesUI.gameObject.SetActive(false);

                state = ActionState.Attacking;
                venom.GetComponent<Collider>().isTrigger = true;
                Vector3 lookVector;
                for (int p = 0; p < POUNCE_COUNT; p++)
                {
                    lookVector = new Vector3(player.transform.position.x - venom.transform.position.x, 0, player.transform.position.z - venom.transform.position.z);
                    t = 0;
                    while (t < POUNCE_STARTUP)
                    {
                        if (freezeTime <= 0)
                        {
                            if (t < POUNCE_STARTUP * 0.75f)
                                lookVector = new Vector3(player.transform.position.x - venom.transform.position.x, 0, player.transform.position.z - venom.transform.position.z);
                            cage.Update();
                            t += Time.deltaTime;
                        }
                        yield return null;
                    }

                    // POUNCE!
                    Vector3 aim = lookVector;
                    aim = new Vector3(aim.x, 0, aim.z);
                    float pounceSpeed = Mathf.Min(aim.magnitude, POUNCE_MAX_DIST) / POUNCE_DURATION;
                    venom.transform.rotation = Quaternion.LookRotation(lookVector);

                    venomCharge.gameObject.SetActive(true);
                    t = 0;
                    while (t < POUNCE_DURATION)
                    {
                        if (freezeTime <= 0)
                        {
                            venomRb.velocity = aim.normalized * pounceSpeed;
                            cage.Update();
                            t += Time.deltaTime;
                        }
                        else
                            venomRb.velocity *= 0;
                        yield return null;
                    }

                    venomCharge.gameObject.SetActive(false);

                    // Chill
                    while (t < POUNCE_ENDLAG)
                    {
                        if (freezeTime <= 0)
                        {
                            venomRb.velocity = aim.normalized * (pounceSpeed * (1 - (t / POUNCE_ENDLAG)));
                            cage.Update();
                            t += Time.deltaTime;
                        }
                        yield return null;
                    }
                    venomRb.velocity *= 0;
                }

                state = ActionState.Cooldown;
                venom.GetComponent<Collider>().isTrigger = false;
                venomRb.velocity *= 0;
                t = 0;
                while (t < POUNCE_COOL)
                {
                    if (freezeTime <= 0)
                    {
                        if (cage.IsBolting())
                            cage.Update();
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                Transform endLoc = cage.DisableAll();
                spark.SetActive(true);
                miniHealthBar.gameObject.SetActive(true);
                attributesUI.gameObject.SetActive(true);
                spark.transform.position = new Vector3(endLoc.position.x, 0, endLoc.position.z);
                spark.transform.rotation = endLoc.rotation;

                yield return null;
                currentAttack = 2;
                state = ActionState.Waiting;
                break;
            case 2: //Summon
                state = ActionState.Startup;
                foreach (Cyclone c in cycloneList)
                    c.gameObject.SetActive(false);

                StartCoroutine(Summon(summonCount, 3, 3, 1.5f, 2, 1, 1));
                while (state != ActionState.Waiting) yield return null;
                break;
            case 3: //Perilous Partition
                //startup
                state = ActionState.Startup;

                //Spark and venom jump to halfway points of the arena.
                PartitionJump(spark, -1);
                PartitionJump(venom, 1);
                spark.GetComponent<Collider>().isTrigger = true;
                venom.GetComponent<Collider>().isTrigger = true;
                t = 0;
                while (t < AIR_TIME)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }
                spark.GetComponent<Collider>().isTrigger = false;
                sparkRb.useGravity = false;
                sparkRb.constraints = ~RigidbodyConstraints.FreezePositionZ;
                sparkRb.velocity *= 0;
                venom.GetComponent<Collider>().isTrigger = false;
                venomRb.useGravity = false;
                venomRb.constraints = ~RigidbodyConstraints.FreezePositionZ;
                venomRb.velocity *= 0;

                //Wait for PERILOUS_START
                t = 0;
                while (t < PERILOUS_START)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                state = ActionState.Attacking;
                //For PERILOUS_ACTIVE
                //Run up and down
                //Spawn path pieces intermitently

                sparkCharge.gameObject.SetActive(true);
                venomCharge.gameObject.SetActive(true);
                sparkRb.velocity = spark.transform.forward * speed;
                venomRb.velocity = venom.transform.forward * speed;
                Vector3 sparkMov = sparkRb.velocity;
                Vector3 venomMov = venomRb.velocity;

                t = 0;
                float spawnTimer = 2f / speed;
                PartitionSpawn(zapPathPrefab, zapPath, spark);
                PartitionSpawn(noxPathPrefab, noxPath, venom);
                while (t < PERILOUS_ACTIVE)
                {
                    int wallMask = 1 << 7;
                    //int perilMask = 1 << 8;
                    if (freezeTime <= 0)
                    {
                        sparkRb.velocity = sparkMov;
                        venomRb.velocity = venomMov;
                        sparkCharge.gameObject.SetActive(true);
                        venomCharge.gameObject.SetActive(true);

                        //Turn if you hit a wall, a spawn part of the path
                        RaycastHit sparkHit;
                        if (Physics.Raycast(spark.transform.position + spark.transform.up, spark.transform.forward, out sparkHit, 1, wallMask))
                        {
                            PartitionSpawn(zapPathPrefab, zapPath, spark);
                            spark.transform.Rotate(0, 180, 0);
                            sparkRb.velocity *= -1;
                            sparkMov = sparkRb.velocity;
                        }

                        RaycastHit venomHit;
                        if (Physics.Raycast(venom.transform.position + venom.transform.up, venom.transform.forward, out venomHit, 1, wallMask))
                        {
                            venom.transform.Rotate(0, 180, 0);
                            venomRb.velocity *= -1;
                            venomMov = venomRb.velocity;
                            PartitionSpawn(noxPathPrefab, noxPath, venom);
                        }

                        if (spawnTimer <= 0)
                        {
                            PartitionSpawn(zapPathPrefab, zapPath, spark);
                            PartitionSpawn(noxPathPrefab, noxPath, venom);
                            spawnTimer = 2f / speed;
                        }
                        else
                            spawnTimer -= Time.deltaTime;
                        t += Time.deltaTime;
                    }
                    else
                    {
                        sparkCharge.gameObject.SetActive(false);
                        venomCharge.gameObject.SetActive(false);
                    }
                    yield return null;
                }
                sparkCharge.gameObject.SetActive(false);
                venomCharge.gameObject.SetActive(false);

                //Wait for noxPath to be empty
                state = ActionState.Cooldown;
                sparkRb.velocity *= 0f;
                venomRb.velocity *= 0f;
                while (zapPath.Count > 0 || noxPath.Count > 0)
                {
                    yield return null;
                }

                sparkRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                venomRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                yield return null;
                currentAttack++;
                state = ActionState.Waiting;
                break;
            case 4: //Syncrash
                int nextMove = 1 + Random.Range(0, 2);
                state = ActionState.Startup;

                //Spark and venom charge
                sparkCrashIndicator.SetActive(true);
                venomCrashIndicator.SetActive(true);
                centerCrashIndicator.SetActive(true);

                for (int i = 0; i < summonKOIndicators.Count; i++)
                    summonKOIndicators[i].SetActive(false);

                t = 0;
                while (t < DASH_START)
                {
                    if (freezeTime <= 0)
                    {
                        Vector3 crashPoint = (spark.transform.position + venom.transform.position) / 2;
                        lookVector = new Vector3(venom.transform.position.x - spark.transform.position.x, 0, venom.transform.position.z - spark.transform.position.z);

                        Vector3 sparkDirection = Vector3.RotateTowards(spark.transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
                        Vector3 venomDirection = Vector3.RotateTowards(venom.transform.forward, -lookVector, rotateSpeed * Time.deltaTime, 0.0f);

                        spark.transform.rotation = Quaternion.LookRotation(sparkDirection);
                        venom.transform.rotation = Quaternion.LookRotation(venomDirection);

                        sparkCrashIndicator.transform.rotation = Quaternion.LookRotation(crashPoint - spark.transform.position);
                        sparkCrashIndicator.transform.position = (Vector3.up * 0.5f) + (crashPoint + spark.transform.position) / 2;
                        sparkCrashIndicator.transform.localScale = new Vector3(sparkCrashIndicator.transform.localScale.x, sparkCrashIndicator.transform.localScale.y, (lookVector.magnitude / 2) + 2);

                        venomCrashIndicator.transform.rotation = Quaternion.LookRotation(crashPoint - venom.transform.position);
                        venomCrashIndicator.transform.position = (Vector3.up * 0.5f) + (crashPoint + venom.transform.position) / 2;
                        venomCrashIndicator.transform.localScale = new Vector3(venomCrashIndicator.transform.localScale.x, venomCrashIndicator.transform.localScale.y, (lookVector.magnitude / 2) + 2);

                        centerCrashIndicator.transform.position = crashPoint;

                        t += Time.deltaTime;
                        // Dash
                        sparkCrashWarning.transform.localScale = new Vector3(sparkCrashWarning.transform.localScale.y, sparkCrashWarning.transform.localScale.y, t / DASH_START);
                        venomCrashWarning.transform.localScale = new Vector3(venomCrashWarning.transform.localScale.y, venomCrashWarning.transform.localScale.y, t / DASH_START);
                        centerCrashWarning.transform.localScale = new Vector3(t / DASH_START, centerCrashWarning.transform.localScale.y, t / DASH_START);

                        // Burst or Cyclone
                        for (int i = 0; i < summonKOIndicators.Count; i++)
                        {
                            if (i < summons.Count)
                            {
                                summonKOIndicators[i].SetActive(true);
                                summonKOIndicators[i].transform.position = summons[i].transform.position;
                                summonKOWarnings[i].transform.localScale = new Vector3(t / DASH_START, summonKOWarnings[i].transform.localScale.y, t / DASH_START);
                            }
                            else
                                summonKOIndicators[i].SetActive(false);
                        }
                    }
                    yield return null;
                }

                sparkCharge.gameObject.SetActive(true);
                venomCharge.gameObject.SetActive(true);
                spark.GetComponent<Collider>().isTrigger = true;
                venom.GetComponent<Collider>().isTrigger = true;
                sparkRb.velocity = (venom.transform.position - spark.transform.position).normalized * speed;
                venomRb.velocity = -sparkRb.velocity;
                sparkMov = sparkRb.velocity;
                venomMov = venomRb.velocity;

                sparkCrashIndicator.SetActive(false);
                venomCrashIndicator.SetActive(false);
                centerCrashIndicator.SetActive(false);
                for (int i = 0; i < summonKOIndicators.Count; i++)
                    summonKOIndicators[i].SetActive(false);

                state = ActionState.Attacking;
                while (state == ActionState.Attacking)
                {
                    if (!frozen)
                    {
                        sparkRb.velocity = sparkMov;
                        venomRb.velocity = venomMov;
                        sparkCharge.gameObject.SetActive(true);
                        venomCharge.gameObject.SetActive(true);
                    }
                    else
                    {
                        sparkCharge.gameObject.SetActive(false);
                        venomCharge.gameObject.SetActive(false);
                    }
                    yield return null;
                }

                //On hit, CRASH! and strike summons
                crash.gameObject.SetActive(true);
                //find surviving summons
                //show bolt
                List<GameObject> bolts = new List<GameObject>();

                for (int i = 0; i < summons.Count; i++)
                {
                    bolts.Add(Instantiate(wrathBoltPrefab, summons[i].gameObject.transform.position, Quaternion.Euler(0, 0, 0)));
                    yield return null;
                }
                sparkCharge.gameObject.SetActive(false);
                venomCharge.gameObject.SetActive(false);

                //Strike time
                t = 0;
                while (t < 1f / 10)
                {
                    if (!frozen)
                        t += Time.deltaTime;
                    yield return null;
                }

                crash.gameObject.SetActive(false);
                for (int i = 0; i < bolts.Count; i++)
                {
                    Destroy(bolts[i]);
                    bolts.RemoveAt(i);
                    i--;
                }

                currentAttack += nextMove;
                state = ActionState.Waiting;
                break;
            case 5: //Summoners' Burst
                state = ActionState.Attacking;
                List<Projectile> thunderGases = new List<Projectile>();

                // Need to destroy each object through a separate list from the summon list
                // Due to Boss's update method altering lists
                List<Enemy> destroyList = new List<Enemy>();
                for (int i = 0; i < summons.Count; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Vector3 direction = player.transform.position - summons[i].gameObject.transform.position;
                        thunderGases.Add(Instantiate(thunderGasPrefab,
                            summons[i].gameObject.transform.position,
                            Quaternion.LookRotation(direction) * Quaternion.Euler(0, (i * 15f) + 60 * j, 0)));
                        thunderGases[i * 6 + j].Setup(6, this, false, 0, -1, 0.03f, 0, true, true, 1);
                        thunderGases[i * 6 + j].SetInvincTrigger(false);
                    }
                    destroyList.Add(summons[i]);
                }

                foreach (Enemy s in destroyList)
                    Destroy(s.gameObject);

                state = ActionState.Cooldown;

                t = 0;
                while (t < CRASH_COOL)
                {
                    if (!frozen)
                        t += Time.deltaTime;
                    yield return null;
                }

                currentAttack = 0 + Random.Range(0, 2);
                state = ActionState.Waiting;
                break;
            case 6: //Summoners' Cyclone
                state = ActionState.Attacking;
                // Need to destroy each object through a separate list from the summon list
                // Due to Boss's update method altering lists
                destroyList = new List<Enemy>();
                for (int i = 0; i < summons.Count; i++)
                {
                    // Activate cyclones
                    Vector3 summonPos = summons[i].gameObject.transform.position;
                    cycloneList[i].transform.position = new Vector3(summonPos.x, 0, summonPos.z);
                    cycloneList[i].gameObject.SetActive(true);
                    destroyList.Add(summons[i]);
                }

                foreach (Enemy s in destroyList)
                    Destroy(s.gameObject);

                state = ActionState.Cooldown;

                t = 0;
                while (t < CRASH_COOL)
                {
                    if (!frozen)
                        t += Time.deltaTime;
                    yield return null;
                }

                currentAttack = 0 + Random.Range(0, 2);
                state = ActionState.Waiting;
                break;
            default:
                break;
        }
        yield return null;
    }

    public override void TakeDamage(int damage, Vector3 kbDir, bool triggerInvinc = true, float kbMod = 0, bool fixKB = false)
    {
        base.TakeDamage(damage, kbDir, triggerInvinc, kbMod, fixKB);
        miniHealthBarVenom.SetMax(maxHealth);
        miniHealthBarVenom.SetValue(currentHealth);
    }

    //TODO: Override UI Updaters
    public override void UpdateAttributeUI()
    {
        base.UpdateAttributeUI();
        strengthUIVenom.text = strengthUI.text;
        strengthUIVenom.color = strengthUI.color;
        defenseUIVenom.text = defenseUI.text;
        defenseUIVenom.color = defenseUI.color;

        //float s = strength + SummationBuffs(1) + SummationDebuffs(1);
        //float d = defense + SummationBuffs(2) + SummationDebuffs(2);
        //strengthUI.text = "" + Mathf.Min(Mathf.Max(s, -9), 9);
        //defenseUI.text = "" + Mathf.Min(Mathf.Max(d, -9), 9);
        //strengthUIVenom.text = "" + Mathf.Min(Mathf.Max(s, -9), 9);
        //defenseUIVenom.text = "" + Mathf.Min(Mathf.Max(d, -9), 9);

        //if (s < 0)
        //{
        //    strengthUI.color = Color.red;
        //    strengthUIVenom.color = Color.red;
        //}
        //else
        //{
        //    strengthUI.color = Color.white;
        //    strengthUIVenom.color = Color.white;
        //}

        //if (d < 0)
        //{
        //    defenseUI.color = Color.red;
        //    defenseUIVenom.color = Color.red;
        //}
        //else
        //{
        //    defenseUI.color = Color.white;
        //    defenseUIVenom.color = Color.white;
        //}
    }

    public override void Reset(bool zeroSpeed)
    {

    }

    public void OnDestroy()
    {
        foreach (Enemy e in summons)
        {
            Destroy(e.gameObject);
            summons.Remove(e);
        }

        foreach (GameObject c in pounceClouds)
        {
            Destroy(c);
            pounceClouds.Remove(c);
        }
    }
}
