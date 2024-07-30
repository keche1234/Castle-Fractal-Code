using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magestic : Boss
{
    public GameObject mage;
    public GameObject pointer;

    [Header("Pyrostorm")]
    [SerializeField] protected Projectile pyroPrefab;
    protected float pyroStart = 3f;
    protected float pyroEnd = 5f;
    protected float pyroStormCount = 2;

    [Header("Pyrospiral")]
    protected float pyroRotateSpeed = -1440; //degrees per second
    protected float pyroSpiralShots = 10f;
    protected float pyroDegreesToShoot; //rotate this much before shooting
    protected float pyroActive;
    protected float pyroSpiralCount = 10;

    [Header("Frostflash")]
    [SerializeField] protected Hitbox bang;
    [SerializeField] protected List<Hitbox> surroundBeams;
    [SerializeField] protected List<Hitbox> focusBeams;
    [SerializeField] protected GameObject bangWarning;
    [SerializeField] protected List<GameObject> surroundWarning;
    [SerializeField] protected List<GameObject> focusWarning;
    protected float frostStart = 1.5f;
    protected float frostActive = 1f;
    protected float frostEnd = 3f;

    [Header("Glacier Lasers")]
    [SerializeField] protected List<Hitbox> laserLateralBeams;
    [SerializeField] protected List<Hitbox> laserLongBeamsX; // Rotate on the x-axis
    [SerializeField] protected List<Hitbox> laserLongBeamsZ; // Rotate on the z-axis
    [SerializeField] protected List<GameObject> laserWarning;
    protected float laserRotateSpeedLateralMin = 30f; //degrees per second
    protected float laserRotateSpeedLateralMax = 45f; //degrees per second
    protected float laserRotateSpeedLongMin = 60f; //degrees per second
    protected float laserRotateSpeedLongMax = 90f; //degrees per second
    protected float laserActive;
    protected float laserLinger = 5f;

    [Header("Claim Summons")]
    protected float claimDuration = 1f;

    [Header("Vinestrike")]
    [SerializeField] protected Projectile wandPrefab;
    protected List<Projectile> myWands = new List<Projectile>();
    [SerializeField] protected List<GameObject> bushes;
    [SerializeField] protected List<Hitbox> vines;
    [SerializeField] protected List<GameObject> vineIndicators;
    [SerializeField] protected List<GameObject> vineWarnings; // Parallel to indicators
    protected float wandSpawn = 0.5f;
    protected float wandStart = 0.5f;
    protected float wandEnd = 1f;
    protected float vineStart = 1.5f;
    protected float vineActive = 0.2f;
    protected float vineEnd = 2f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        numAttacks = 7;
        currentAttack = 0;
        summonCount = 5;

        rotateSpeed = 15f;
        armored = true;

        for (int i = 0; i < bushes.Count; i++)
        {
            bushes[i].transform.parent = transform.parent;
            vines[i].transform.parent = transform.parent;
        }

        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        miniHealthBar.gameObject.transform.parent.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        miniHealthBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -100));

        attributesUI.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        attributesUI.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -140));
        menuManager = GameObject.Find("MenuManager").GetComponent<GameMenuManager>();

        pyroDegreesToShoot = Mathf.Abs(pyroRotateSpeed / 2) + Random.Range(99f, 144f);
        pyroActive = pyroDegreesToShoot * pyroSpiralShots / Mathf.Abs(pyroRotateSpeed);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        charRb.velocity = Vector3.zero;
        charRb.angularVelocity = Vector3.zero;
        if (state == ActionState.Waiting)
            transform.position = new Vector3(0, mage.transform.position.y, 0);

        if (currentAttack == 4)
            for (int i = 0; i < myWands.Count; i++)
                if (myWands[i] == null)
                    myWands.RemoveAt(i--);
    }

    protected override void LookTowardsPlayer()
    {
        Vector3 lookVector = new Vector3(player.transform.position.x - mage.transform.position.x, 0, player.transform.position.z - mage.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected void SnapTowardsPlayer()
    {
        Vector3 lookVector = new Vector3(player.transform.position.x - mage.transform.position.x, 0, player.transform.position.z - mage.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, Mathf.PI * 2, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected override IEnumerator Attack()
    {
        float t;
        switch (currentAttack)
        {
            case 0: //Summon
                state = ActionState.Startup;
                StartCoroutine(Summon(summonCount, 2, 4, 1, 1.5f, 1.25f, 2));
                break;
            case 1: //Pyrostorm
                //startup
                state = ActionState.Startup;
                for (int i = 0; i < pyroStormCount; i++)
                {
                    t = 0;
                    while (t < (pyroStart / (pyroStormCount - i)))
                    {
                        if (GetMyFreezeTime() <= 0)
                        {
                            if (t / pyroStart < 0.75f)
                                LookTowardsPlayer();
                            t += Time.deltaTime;
                        }
                        yield return null;
                    }
                    //create the fireballs
                    state = ActionState.Attacking;

                    for (int j = 0; j < 2; j++)
                    {
                        Projectile p = Instantiate(pyroPrefab, transform.position, Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, -45 + (90 * j), 0));
                        p.transform.parent = roomManager.GetCurrent().transform;
                        p.transform.position += p.transform.forward;
                        p.SetSource(this);
                        p.gameObject.GetComponent<Splitting>().SetSource(this);
                    }
                    yield return null;

                    //wait
                    state = ActionState.Cooldown;
                    if (i >= pyroStormCount - 1)
                    {
                        while (GameObject.Find("Pyroball(Clone)") != null || GameObject.Find("Pyroball(Clone)(Clone)") != null || GameObject.Find("Pyroball(Clone)(Clone)(Clone)") != null)
                        {
                            if (GetMyFreezeTime() <= 0)
                                LookTowardsPlayer();
                            yield return null;
                        }
                    }
                    else
                    {
                        t = 0;
                        while (t < (pyroEnd / (pyroStormCount - 1 - i)))
                        {
                            if (GetMyFreezeTime() <= 0)
                            {
                                LookTowardsPlayer();
                                t += Time.deltaTime;
                            }
                            yield return null;
                        }
                    }
                }
                currentAttack += 2 + Random.Range(0, 2);
                state = ActionState.Waiting;
                break;
            case 2: //Pyrospiral
                state = ActionState.Startup;

                while (!IsFacingPlayer())
                {
                    if (GetMyFreezeTime() <= 0)
                        LookTowardsPlayer();
                    yield return null;
                }

                float rotSpeed = 0;
                t = 0;
                while (Mathf.Abs(rotSpeed) < Mathf.Abs(pyroRotateSpeed))
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        if (t > pyroStart / 2) rotSpeed += pyroRotateSpeed * Time.deltaTime / (pyroStart / 2);
                        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                state = ActionState.Attacking;
                float degreesRotated = 0;
                t = 0;
                while (t < pyroActive)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        while (degreesRotated >= pyroDegreesToShoot)
                        {
                            for (int i = 0; i < pyroSpiralCount; i++)
                            {
                                //create the fireball
                                Projectile p = Instantiate(pyroPrefab, pointer.transform.position, Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, (360 / pyroSpiralCount) * i, 0));
                                p.transform.parent = roomManager.GetCurrent().transform;
                                p.gameObject.transform.position += p.gameObject.transform.forward;
                                p.gameObject.transform.localScale *= 0.4f;
                                p.SetSource(this);
                                ((Explosive)p).SetExplosionMod(0.1f);
                                p.SetSpeed(4.5f);
                                p.gameObject.GetComponent<Splitting>().enabled = false;
                            }
                            degreesRotated -= pyroDegreesToShoot;
                        }

                        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
                        degreesRotated += Mathf.Abs(rotSpeed * Time.deltaTime);
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                state = ActionState.Cooldown;
                t = 0;
                while (t < pyroEnd)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        if (t < pyroEnd / 2) rotSpeed -= pyroRotateSpeed * Time.deltaTime / (pyroEnd / 2);
                        else rotSpeed = 0;

                        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                currentAttack += 1 + Random.Range(0, 2);
                state = ActionState.Waiting;
                break;
            case 3: //Frostflash
                //Startup
                state = ActionState.Startup;
                bangWarning.transform.parent.gameObject.SetActive(true);
                bangWarning.transform.localScale = new Vector3(0, 1, 0);
                foreach (GameObject w in surroundWarning)
                {
                    w.transform.parent.gameObject.SetActive(true);
                    w.transform.localScale = new Vector3(w.transform.localScale.x, 1, 0);
                }

                t = 0;
                while (t < frostStart)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        t += Time.deltaTime;

                        float progress = t / frostStart;
                        bangWarning.transform.localScale = new Vector3(progress, 1, progress);
                        foreach (GameObject w in surroundWarning)
                            w.transform.localScale = new Vector3(w.transform.localScale.x, 1, progress);
                    }
                    yield return null;
                }

                //Fire! (Surround)
                state = ActionState.Attacking;
                bangWarning.transform.parent.gameObject.SetActive(false);
                foreach (GameObject w in surroundWarning)
                    w.transform.parent.gameObject.SetActive(false);

                foreach (Hitbox h in surroundBeams)
                    h.gameObject.SetActive(true);
                bang.gameObject.SetActive(true);

                t = 0;
                while (t < frostActive)
                {
                    if (GetMyFreezeTime() <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                //Cooldown
                state = ActionState.Cooldown;
                foreach (Hitbox h in surroundBeams)
                    h.gameObject.SetActive(false);
                bang.gameObject.SetActive(false);

                t = 0;
                while (t < frostEnd)
                {
                    if (GetMyFreezeTime() <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                //Loop through corners: Lower left, top left, bottom right, top right
                for (int i = 0; i < 4; i++)
                {
                    state = ActionState.Startup;
                    bangWarning.transform.parent.gameObject.SetActive(true);
                    foreach (GameObject w in focusWarning)
                        w.transform.parent.gameObject.SetActive(true);

                    //Teleport
                    Room r = roomManager.GetCurrent();
                    transform.position = new Vector3(((r.GetXDimension() / 2) - 2f) * Mathf.Pow(-1, (int)(i / 2) + 1), transform.position.y, ((r.GetZDimension() / 2) - 2f) * Mathf.Pow(-1, (i + 1) % 2));
                    Vector3 otherCorner = new Vector3(-transform.position.x, transform.position.y, -transform.position.z);
                    transform.rotation = Quaternion.LookRotation(otherCorner - transform.position);

                    //Startup
                    t = 0;
                    while (t < frostStart)
                    {
                        if (GetMyFreezeTime() <= 0)
                        {
                            t += Time.deltaTime;
                            float progress = t / frostStart;
                            bangWarning.transform.localScale = new Vector3(progress, 1, progress);
                            foreach (GameObject w in focusWarning)
                                w.transform.localScale = new Vector3(w.transform.localScale.x, 1, progress);
                        }
                        yield return null;
                    }

                    //Fire!
                    state = ActionState.Attacking;
                    bangWarning.transform.parent.gameObject.SetActive(false);
                    foreach (GameObject w in focusWarning)
                        w.transform.parent.gameObject.SetActive(false);

                    foreach (Hitbox h in focusBeams)
                        h.gameObject.SetActive(true);
                    bang.gameObject.SetActive(true);

                    t = 0;
                    while (t < frostActive)
                    {
                        if (GetMyFreezeTime() <= 0)
                            t += Time.deltaTime;
                        yield return null;
                    }

                    //Cooldown
                    state = ActionState.Cooldown;
                    foreach (Hitbox h in focusBeams)
                        h.gameObject.SetActive(false);
                    bang.gameObject.SetActive(false);

                    t = 0;
                    while (t < frostEnd)
                    {
                        if (GetMyFreezeTime() <= 0)
                            t += Time.deltaTime;
                        yield return null;
                    }
                    yield return null;
                }

                //Startup
                state = ActionState.Startup;
                bangWarning.transform.parent.gameObject.SetActive(true);
                foreach (GameObject w in surroundWarning)
                    w.transform.parent.gameObject.SetActive(true);

                transform.position = new Vector3(0, transform.position.y, 0);
                transform.rotation = Quaternion.Euler(0, 180, 0);
                t = 0;
                while (t < frostStart)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        t += Time.deltaTime;
                        float progress = t / frostStart;
                        bangWarning.transform.localScale = new Vector3(progress, 1, progress);
                        foreach (GameObject w in surroundWarning)
                            w.transform.localScale = new Vector3(w.transform.localScale.x, 1, progress);
                    }
                    yield return null;
                }

                state = ActionState.Attacking;
                bangWarning.transform.parent.gameObject.SetActive(false);
                foreach (GameObject w in surroundWarning)
                    w.transform.parent.gameObject.SetActive(false);

                //Final Flash! (Surround)
                foreach (Hitbox h in surroundBeams)
                    h.gameObject.SetActive(true);
                bang.gameObject.SetActive(true);

                t = 0;
                while (t < frostActive)
                {
                    if (GetMyFreezeTime() <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                //Cooldown
                state = ActionState.Cooldown;
                foreach (Hitbox h in surroundBeams)
                    h.gameObject.SetActive(false);
                bang.gameObject.SetActive(false);

                t = 0;
                while (t < frostEnd)
                {
                    if (GetMyFreezeTime() <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }
                currentAttack += 2;
                if (summons.Count == 0)
                    currentAttack++;
                state = ActionState.Waiting;
                break;
            case 4: //Glacier Lasers
                //Startup
                state = ActionState.Startup;
                bangWarning.transform.parent.gameObject.SetActive(true);
                bangWarning.transform.localScale = new Vector3(0, 1, 0);
                foreach (GameObject w in laserWarning)
                {
                    w.transform.parent.gameObject.SetActive(true);
                    w.transform.localScale = new Vector3(w.transform.localScale.x, 1, 0);
                }

                t = 0;
                while (t < frostStart * 2)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        // Rotate towards facing straight down.
                        if (Vector3.Angle(transform.forward, -Vector3.forward) != 0)
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(-Vector3.forward), 120 * Time.deltaTime);

                        t += Time.deltaTime;
                        float progress = t / (frostStart * 2);
                        bangWarning.transform.localScale = new Vector3(progress, 1, progress);
                        foreach (GameObject w in laserWarning)
                            w.transform.localScale = new Vector3(w.transform.localScale.x, 1, progress);
                    }
                    yield return null;
                }

                //Fire!
                state = ActionState.Attacking;
                bangWarning.transform.parent.gameObject.SetActive(false);
                foreach (GameObject w in laserWarning)
                    w.transform.parent.gameObject.SetActive(false);

                bang.gameObject.SetActive(true);
                for (int i = 0; i < laserLateralBeams.Count; i++)
                {
                    laserLateralBeams[i].gameObject.SetActive(true);
                    laserLateralBeams[i].gameObject.transform.rotation = transform.rotation * Quaternion.Euler(0, 90 - (180 * i), 0);
                }

                for (int i = 0; i < laserLongBeamsX.Count; i++)
                {
                    laserLongBeamsX[i].gameObject.SetActive(true);
                    laserLongBeamsX[i].gameObject.transform.rotation = transform.rotation * Quaternion.Euler(-90 + (180 * i), 0, 0);
                }

                for (int i = 0; i < laserLongBeamsZ.Count; i++)
                {
                    laserLongBeamsZ[i].gameObject.SetActive(true);
                    laserLongBeamsZ[i].gameObject.transform.rotation = transform.rotation * Quaternion.Euler(-90 + (180 * i), 0, 0);
                }

                float laserSpeedLateralCap = Random.Range(laserRotateSpeedLateralMin, laserRotateSpeedLateralMax);
                float laserSpeedLongXCap = Random.Range(laserRotateSpeedLongMin, laserRotateSpeedLongMax);
                float laserSpeedLongZCap = Random.Range(laserRotateSpeedLongMin, laserRotateSpeedLongMax);

                float laserSpeedLat = 0;
                float laserSpeedLongX = 0;
                float laserSpeedLongZ = 0;

                float laserAccelerationLat = 2 * laserSpeedLateralCap / laserLinger;
                float laserAccelerationLongX = 2 * laserSpeedLongXCap / laserLinger;
                float laserAccelerationLongZ = 2 * laserSpeedLongZCap / laserLinger;

                laserActive = (540 - (0.5f * laserAccelerationLat * Mathf.Pow(laserLinger / 2, 2))
                                    - ((laserSpeedLateralCap * (laserLinger / 2)) + (0.5f * laserAccelerationLat * Mathf.Pow(laserLinger / 2, 2)))) / laserSpeedLateralCap;

                // Speed up the rotation
                t = 0;
                while (t < laserLinger)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        // Rotate the hitboxes
                        for (int i = 0; i < laserLateralBeams.Count; i++)
                            laserLateralBeams[i].gameObject.transform.Rotate(new Vector3(0, laserSpeedLat * Time.deltaTime, 0));
                        for (int i = 0; i < laserLongBeamsX.Count; i++)
                            laserLongBeamsX[i].gameObject.transform.Rotate(new Vector3(laserSpeedLongX * Time.deltaTime, 0, 0));
                        for (int i = 0; i < laserLongBeamsZ.Count; i++)
                            laserLongBeamsZ[i].gameObject.transform.Rotate(new Vector3(0, (i == 0 ? 1 : -1) * laserSpeedLongZ * Time.deltaTime, 0));

                        if (t < laserLinger / 2)
                        {
                            // Speed up the rotation
                            laserSpeedLat += laserAccelerationLat * Time.deltaTime;
                            laserSpeedLongX += laserAccelerationLongX * Time.deltaTime;
                            laserSpeedLongZ += laserAccelerationLongZ * Time.deltaTime;
                        }
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                t = 0;
                while (t < laserActive)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        // Rotate the hitboxes
                        for (int i = 0; i < laserLateralBeams.Count; i++)
                            laserLateralBeams[i].gameObject.transform.Rotate(new Vector3(0, laserSpeedLat * Time.deltaTime, 0));
                        for (int i = 0; i < laserLongBeamsX.Count; i++)
                            laserLongBeamsX[i].gameObject.transform.Rotate(new Vector3(laserSpeedLongX * Time.deltaTime, 0, 0));
                        for (int i = 0; i < laserLongBeamsZ.Count; i++)
                            laserLongBeamsZ[i].gameObject.transform.Rotate(new Vector3(0, (i == 0 ? 1 : -1) * laserSpeedLongZ * Time.deltaTime, 0));

                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                t = 0;
                while (t < laserLinger)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        if (t < laserLinger / 2)
                        {
                            // Rotate the hitboxes
                            for (int i = 0; i < laserLateralBeams.Count; i++)
                                laserLateralBeams[i].gameObject.transform.Rotate(new Vector3(0, laserSpeedLat * Time.deltaTime, 0));
                            for (int i = 0; i < laserLongBeamsX.Count; i++)
                                laserLongBeamsX[i].gameObject.transform.Rotate(new Vector3(laserSpeedLongX * Time.deltaTime, 0, 0));
                            for (int i = 0; i < laserLongBeamsZ.Count; i++)
                                laserLongBeamsZ[i].gameObject.transform.Rotate(new Vector3(0, (i == 0 ? 1 : -1) * laserSpeedLongZ * Time.deltaTime, 0));

                            // Slow down the rotation
                            laserSpeedLat -= laserAccelerationLat * Time.deltaTime;
                            laserSpeedLongX -= laserAccelerationLongX * Time.deltaTime;
                            laserSpeedLongZ -= laserAccelerationLongZ * Time.deltaTime;
                        }

                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                state = ActionState.Cooldown;
                foreach (Hitbox h in laserLateralBeams)
                    h.gameObject.SetActive(false);
                foreach (Hitbox h in laserLongBeamsX)
                    h.gameObject.SetActive(false);
                foreach (Hitbox h in laserLongBeamsZ)
                    h.gameObject.SetActive(false);
                bang.gameObject.SetActive(false);

                t = 0;
                while (t < frostEnd)
                {
                    if (GetMyFreezeTime() <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                currentAttack += 1;
                state = ActionState.Waiting;
                break;
            case 5: //Claim Summons
                state = ActionState.Attacking;
                //Destroy mages and determine buffs
                List<int> buffAttributes = new List<int>();
                while (summons.Count > 0)
                {
                    if (summons[0].GetComponent<RedMage>() != null) //is Red Mage, strength buff
                    {
                        buffAttributes.Add(1);
                    }
                    else //is Wisteria Wizard, defense buff
                    {
                        buffAttributes.Add(2);
                    }

                    Destroy(summons[0].gameObject);
                    summons.RemoveAt(0);
                    yield return null;
                }

                //Delay before adding buff
                t = 0;
                while (t < claimDuration)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        t += Time.deltaTime;
                        LookTowardsPlayer();
                    }
                    yield return null;
                }

                //Add buffs
                for (int i = 0; i < buffAttributes.Count; i++)
                {
                    Buff buff = (Buff)ScriptableObject.CreateInstance("Buff");
                    buff.SetBuff(2, 30);
                    AddBuff(buff, buffAttributes[i]);
                }
                LookTowardsPlayer();
                currentAttack++;
                state = ActionState.Waiting;
                break;
            case 6: //Vinestrike
                /*
                 * Part 1: Wands
                 */
                //Summons wands
                state = ActionState.Startup;
                List<Projectile> wands = new List<Projectile>();
                SnapTowardsPlayer();
                for (int i = 0; i < 5; i++)
                {
                    if (i != 2)
                    {
                        Projectile wand = Instantiate(wandPrefab, transform.position, transform.rotation);
                        wand.GetComponent<Projectile>().enabled = false;
                        wand.gameObject.transform.Translate(transform.right * (-3 + (i * 1.5f)), Space.World);
                        wand.gameObject.transform.Rotate(0, -90 - (i * 45), 0, Space.World);
                        wand.SetSource(this);

                        if (i == 1 || i == 3)
                            wand.gameObject.transform.Translate(transform.forward * -2, Space.World);

                        wand.transform.parent = transform.parent;
                        wands.Add(wand);
                        myWands.Add(wand);
                    }
                }

                t = 0;
                while (t < wandSpawn)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        LookTowardsPlayer();
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                //Wand Loop
                for (int i = 0; i < 4; i++)
                {
                    //Throw wand
                    state = ActionState.Startup;
                    t = 0;
                    while (t < wandStart)
                    {
                        if (GetMyFreezeTime() <= 0)
                        {
                            LookTowardsPlayer();
                            t += Time.deltaTime;
                        }
                        yield return null;
                    }
                    wands[i].GetComponent<Projectile>().enabled = true;
                    wands[i].transform.parent = roomManager.GetCurrent().transform.parent;
                    state = ActionState.Attacking;
                    yield return null;

                    //(Inner Loop) Wait for it to hit wall, then throw again
                    state = ActionState.Cooldown;
                    Vector3 wandPos = wands[i].transform.position;
                    while (wands[i] != null)
                    {
                        wandPos = wands[i].transform.position;
                        yield return null;
                    }
                    bushes[i].SetActive(true);
                    bushes[i].transform.position = wandPos + new Vector3(0, -0.5f, 0);
                    yield return null;
                }

                //Wait for wand end
                t = 0;
                while (t < wandEnd)
                {
                    if (GetMyFreezeTime() <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                /*
                 * Part 2: Vines
                 */
                //(Outer Loop)
                //Disappear
                mage.gameObject.SetActive(false);
                pointer.gameObject.SetActive(false);
                miniHealthBar.gameObject.SetActive(false);
                attributesUI.gameObject.SetActive(false);
                GetComponent<Collider>().enabled = false;
                for (int i = 0; i < 4; i++)
                {
                    //(Inner Loop) Wait for vineStart
                    state = ActionState.Startup;
                    vines[i].gameObject.SetActive(true);
                    vines[i].transform.position = bushes[i].transform.position;
                    vines[i].SetKB(0, 0, 0, 4);

                    vineIndicators[i].gameObject.SetActive(true);
                    vineIndicators[i].transform.position = new Vector3(bushes[i].transform.position.x, 0, bushes[i].transform.position.z);
                    vineWarnings[i].transform.localScale = new Vector3(vineWarnings[i].transform.localScale.x, vineWarnings[i].transform.localScale.y, 0);

                    t = 0;
                    while (t < vineStart)
                    {
                        if (GetMyFreezeTime() <= 0)
                        {
                            if (t < vineStart / 2)
                            {
                                vines[i].transform.rotation = Quaternion.LookRotation(new Vector3(player.transform.position.x - vines[i].transform.position.x, 0,
                                                                                                player.transform.position.z - vines[i].transform.position.z));

                                // Fix Indicator
                                vineIndicators[i].transform.rotation = vines[i].transform.rotation;

                                // Raycast towards player to find a wall, calculate distance to get scale for indicator
                                RaycastHit info;
                                if (Physics.Raycast(new Ray(bushes[i].transform.position, player.transform.position - bushes[i].transform.position), out info, Mathf.Infinity, LayerMask.GetMask("Wall")))
                                    vineIndicators[i].transform.localScale = new Vector3(vineIndicators[i].transform.localScale.x, vineIndicators[i].transform.localScale.y, info.distance * 1.3f);
                            }
                            t += Time.deltaTime;
                            vineWarnings[i].transform.localScale = new Vector3(vineWarnings[i].transform.localScale.x, vineWarnings[i].transform.localScale.y, t / vineStart);
                        }
                        yield return null;
                    }

                    //Stretch across vineActive seconds
                    state = ActionState.Attacking;
                    t = 0;
                    while (t < vineActive)
                    {
                        if (GetMyFreezeTime() <= 0)
                        {
                            vines[i].transform.localScale = new Vector3(1, 1, 75 * t);
                            t += Time.deltaTime;
                        }
                        yield return null;
                    }
                }

                state = ActionState.Cooldown;
                for (int i = 0; i < 4; i++)
                    vines[i].SetKB(0, 0, 0, 1);

                t = 0;
                while (t < vineEnd)
                {
                    if (GetMyFreezeTime() <= 0)
                    {
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                for (int i = 0; i < 4; i++)
                {
                    vines[i].transform.localScale = new Vector3(1, 1, 0);
                    vines[i].gameObject.SetActive(false);
                    bushes[i].gameObject.SetActive(false);
                    vineIndicators[i].gameObject.SetActive(false);
                }
                mage.SetActive(true);
                pointer.SetActive(true);
                miniHealthBar.gameObject.SetActive(true);
                attributesUI.gameObject.SetActive(true);
                GetComponent<Collider>().enabled = true;

                currentAttack = 0;
                state = ActionState.Waiting;
                break;
            default:
                break;
        }
        yield return null;
    }

    public override void Reset(bool zeroSpeed)
    {

    }

    public override void OnDestroy()
    {
        //Debug.Log(myWands.Count + " wand(s)");
        for (int i = 0; i < myWands.Count; i++)
        {
            if (myWands[i] && myWands[i].gameObject)
                Destroy(myWands[i].gameObject);
        }
        base.OnDestroy();
    }
}
