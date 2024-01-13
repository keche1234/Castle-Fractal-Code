using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ogrelord : Boss
{
    public GameObject ogre; //for pointing purposes
    public Vector3 clubPositionDefault;
    public Vector3 clubRotationDefault;

    [Header("Club Combo")]
    [SerializeField] protected GameObject club;
    [SerializeField] protected Hitbox swing;
    [SerializeField] protected Hitbox smack;
    [SerializeField] protected List<Hitbox> edges;
    [SerializeField] protected GameObject comboWarning;
    protected float clubStart = 1.75f;
    protected float clubDelay = 1f;
    protected float clubActive = 0.5f;
    protected float edgeActive = 2f;
    protected float clubEnd = 3.75f;

    [Header("Club Toss")]
    [SerializeField] protected Projectile flyingClubPrefab;
    [SerializeField] protected GameObject tossIndicator;
    [SerializeField] protected GameObject tossWarning; //parallel to tossIndicator
    protected int clubsToToss = 3;

    [Header("Rippling Geysers")]
    [SerializeField] protected Hitbox bodyBox;
    [SerializeField] protected Projectile geyserPrefab;
    protected bool landing = false;
    protected List<Projectile> geysers = new List<Projectile>();
    protected int jumps = 6;
    protected float airTime = 1f;
    protected float geyserStart = 1.5f;
    protected float geyserLand = 6f;
    protected float geyserEnd = 3f;

    [Header("Rallying Cry")]
    protected float cryDuration = 1.5f;

    [Header("Surging Spout")]
    [SerializeField] protected Hitbox spout;
    [SerializeField] protected Projectile rockPrefab;
    [SerializeField] protected GameObject spoutWarning;
    protected List<Projectile> rocks = new List<Projectile>();
    //protected float rockCharge = 0;
    //protected float rockDelay = 1;
    protected float spoutSpeed = 4.5f; // m/s
    protected float spoutDecel = 4.5f; // m/s^2
    protected float spoutStart = 2.5f;
    protected float spoutActive = 8f;
    protected float spoutEnd = 2f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        numAttacks = 5;
        currentAttack = 0;
        power = 20f;
        currentHealth = 1000;
        maxHealth = 1000;

        speed = 3f;
        rotateSpeed = Mathf.PI / 3;
        armored = true;

        miniHealthBar.SetMax(maxHealth);
        miniHealthBar.SetValue(currentHealth);

        miniHealthBar.gameObject.transform.parent.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        miniHealthBar.gameObject.transform.parent.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(0, -100));

        attributesUI.GetComponent<Billboard>().SetCamera(GameObject.Find("UI Camera").GetComponent<Camera>());
        attributesUI.GetComponent<UIAttach>().Setup(gameObject, GameObject.Find("UI Camera").GetComponent<Camera>(), new Vector2(5, -140));
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (freezeTime <= 0) //&& rockCharge < rockDelay)
        {
            //rockCharge += Time.deltaTime;
            //if (rockCharge > rockDelay)
            //    rockCharge = rockDelay;
        }

        for (int i = 0; i < rocks.Count; i++)
            if (rocks[i] == null)
                rocks.RemoveAt(i--);

        for (int i = 0; i < geysers.Count; i++)
            if (geysers[i] == null)
                geysers.RemoveAt(i--);
    }

    protected override void LookTowardsPlayer()
    {
        Vector3 lookVector = new Vector3(player.transform.position.x - ogre.transform.position.x, 0, player.transform.position.z - ogre.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, rotateSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected void SnapTowardsPlayer()
    {
        Vector3 lookVector = new Vector3(player.transform.position.x - ogre.transform.position.x, 0, player.transform.position.z - ogre.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, lookVector, Mathf.PI * 2, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    protected override IEnumerator Attack()
    {
        float t;
        switch (currentAttack)
        {
            case 0: //Summon
                if (summons.Count < 7)
                    StartCoroutine(Summon(1, 4, 2, 1.5f, 1, 2, 2));
                break;
            case 1: //Club Combo
                //startup
                state = ActionState.Startup;
                SnapTowardsPlayer();

                //Chase
                while ((transform.position - player.transform.position).magnitude > 3.5f)
                {
                    if (freezeTime <= 0)
                    {
                        LookTowardsPlayer();
                        charRb.velocity = transform.forward * speed;
                    }
                    yield return null;
                }
                charRb.velocity = Vector3.zero;

                //Gear up for swing
                comboWarning.transform.parent.gameObject.SetActive(true);
                comboWarning.transform.localScale = new Vector3(comboWarning.transform.localScale.x, 1, 0);
                t = 0;
                while (t < clubStart)
                {
                    if (freezeTime <= 0)
                    {
                        club.transform.Rotate(0, (195 / clubStart) * Time.deltaTime, 0);

                        t += Time.deltaTime;
                        comboWarning.transform.localScale = new Vector3(comboWarning.transform.localScale.x, 1, t / clubStart);
                    }
                    yield return null;
                }

                //Swing Out
                state = ActionState.Attacking;
                comboWarning.transform.parent.gameObject.SetActive(false);
                swing.gameObject.SetActive(true);
                t = 0;
                while (t < clubActive)
                {
                    if (freezeTime <= 0)
                    {
                        club.transform.Rotate(0, (-215 / clubActive) * Time.deltaTime, 0);
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                state = ActionState.Cooldown;
                swing.gameObject.SetActive(false);
                t = 0;
                while (t < clubDelay)
                {
                    if (freezeTime <= 0)
                    {
                        LookTowardsPlayer();
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                //Swing Up
                state = ActionState.Attacking;
                swing.gameObject.SetActive(true);
                t = 0;
                while (t < clubActive)
                {
                    if (freezeTime <= 0)
                    {
                        if (t < clubActive / 2)
                            club.transform.Rotate(0, (110 * 2 / clubActive) * Time.deltaTime, 0);
                        else
                            club.transform.Rotate(0, 0, (-115 * 2 / clubActive) * Time.deltaTime);
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                state = ActionState.Cooldown;
                swing.gameObject.SetActive(false);
                t = 0;
                while (t < clubDelay)
                {
                    if (freezeTime <= 0)
                    {
                        LookTowardsPlayer();
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                //Swing Down
                state = ActionState.Attacking;
                smack.gameObject.SetActive(true);
                t = 0;
                while (t < clubActive)
                {
                    if (freezeTime <= 0)
                    {
                        club.transform.Rotate(0, 0, (145 / clubActive) * Time.deltaTime);
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                foreach (Hitbox h in edges)
                    h.gameObject.SetActive(true);

                smack.gameObject.SetActive(false);
                while (t < edgeActive)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                state = ActionState.Cooldown;
                foreach (Hitbox h in edges)
                    h.gameObject.SetActive(false);
                t = 0;
                while (t < clubDelay)
                {
                    if (freezeTime <= 0)
                    {
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                //Return
                t = 0;
                while (t < clubEnd / 3)
                {
                    if (freezeTime <= 0)
                    {
                        club.transform.Rotate(0, (-270 / clubEnd) * Time.deltaTime, (-90 / clubEnd) * Time.deltaTime);
                        t += Time.deltaTime;
                    }
                    yield return null;
                }
                Reset(false);

                t = 0;
                while (t < clubEnd * 2f / 3)
                {
                    if (freezeTime <= 0)
                    {
                        LookTowardsPlayer();
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                currentAttack += 2 + Random.Range(0, 2);
                state = ActionState.Waiting;
                break;
            case 2: //Club Toss
                //startup
                state = ActionState.Startup;
                SnapTowardsPlayer();
                //Chase
                while ((transform.position - player.transform.position).magnitude > 3.5f)
                {
                    if (freezeTime <= 0)
                    {
                        LookTowardsPlayer();
                        charRb.velocity = transform.forward * speed;
                    }
                    yield return null;
                }
                charRb.velocity = Vector3.zero;

                for (int i = 0; i < clubsToToss; i++)
                {
                    state = ActionState.Startup;
                    tossIndicator.SetActive(true);
                    tossWarning.transform.localScale = new Vector3(tossWarning.transform.localScale.x, 1, 0);

                    float startup = clubStart * ((5 - i) / 5f);

                    //Gear up for throw
                    t = 0;
                    while (t < startup)
                    {
                        if (freezeTime <= 0)
                        {
                            if (t < startup / 2)
                                club.transform.Rotate((-30 / (startup / 2)) * Time.deltaTime, 0, -(90 / (startup / 2)) * Time.deltaTime);
                            LookTowardsPlayer();

                            // Fix Indicator
                            tossIndicator.transform.rotation = transform.rotation;

                            // Raycast towards player to find a wall, calculate distance to get scale for indicator
                            RaycastHit info;
                            if (Physics.Raycast(new Ray(transform.position, transform.forward), out info, Mathf.Infinity, LayerMask.GetMask("Wall")))
                                tossIndicator.transform.localScale = new Vector3(tossIndicator.transform.localScale.x, tossIndicator.transform.localScale.y, info.distance * 1.3f);

                            t += Time.deltaTime;
                            tossWarning.transform.localScale = new Vector3(tossWarning.transform.localScale.x, 1, t / startup);
                        }
                        yield return null;
                    }

                    //Toss
                    state = ActionState.Attacking;

                    t = 0;
                    while (t < clubActive)
                    {
                        if (freezeTime <= 0)
                        {
                            // Swing
                            club.transform.Rotate((75 / clubActive) * Time.deltaTime, 0, 0);
                            t += Time.deltaTime;
                        }
                        yield return null;
                    }
                    club.SetActive(false);
                    tossIndicator.SetActive(false);

                    Projectile flyingClub = Instantiate(flyingClubPrefab, transform.position + (transform.forward * 1.5f) + transform.up, transform.rotation, roomManager.GetCurrent().transform);
                    flyingClub.SetSource(this);
                    while (flyingClub != null)
                    {
                        LookTowardsPlayer();
                        yield return null; //flyingClub flies until it hits a wall
                    }

                    Reset(false);
                    club.SetActive(true);
                }

                // Cooldown
                t = 0;
                while (t < clubEnd / 2)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                currentAttack += 1 + Random.Range(0, 2);
                state = ActionState.Waiting;
                break;
            case 3: //Rippling Geyers
                //startup
                state = ActionState.Startup;
                t = 0;
                while (t < geyserStart)
                {
                    if (freezeTime <= 0)
                    {
                        LookTowardsPlayer();
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                //First Jump
                state = ActionState.Attacking;
                bodyBox.gameObject.SetActive(true);

                //Determine destination and set speed
                Vector3 xDir = (new Vector3(transform.position.x - player.transform.position.x, 0, 0)).normalized;
                if (xDir.x == 0)
                    xDir = Vector3.right;
                float xDist = ((((roomManager.GetCurrent().GetLength() * 0.5f)- 1) * xDir.x) - transform.position.x);
                charRb.AddForce(new Vector3(xDist / airTime, Physics.gravity.magnitude * airTime * 0.5f, (0 - transform.position.z) / airTime), ForceMode.VelocityChange);
                transform.rotation = Quaternion.LookRotation(-xDir);

                //move all summons inside, away from where the Ogrelord will be
                float length = roomManager.GetCurrent().GetLength() / 2;
                if (xDir.x < 0)
                {
                    foreach (Enemy e in summons)
                    {
                        if (e.transform.position.x < length + 1)
                            e.transform.position = new Vector3(0, e.transform.position.y, e.transform.position.z);
                    }
                }
                else
                {
                    foreach (Enemy e in summons)
                    {
                        if (e.transform.position.x > length - 1)
                            e.transform.position = new Vector3(0, e.transform.position.y, e.transform.position.z);
                    }
                }

                //Wait for state to be cooldown (I landed)
                t = 0;
                while (state == ActionState.Attacking)
                {
                    landing = t > (airTime / 2);
                    GetComponent<Collider>().isTrigger = t < (airTime / 2);
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }
                bodyBox.gameObject.SetActive(false);
                landing = false;
                charRb.velocity = Vector3.zero;

                //Wait for geyserLand
                t = 0;
                while (t < geyserLand)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                //Side Jumps
                int location = 0; //-1 is downstage, 0 is in the middle, 1 is upstage
                for (int i = 0; i < jumps - 1; i++)
                {
                    //Set location
                    int tempLoc;
                    do
                    {
                        tempLoc = Random.Range(-1, 2);
                    }
                    while (tempLoc == location);
                    location = tempLoc;

                    state = ActionState.Attacking;
                    bodyBox.gameObject.SetActive(true);

                    //Determine destination and set speed
                    float z = (location * ((roomManager.GetCurrent().GetWidth() / 2) - 2f));
                    charRb.AddForce(new Vector3(0, Physics.gravity.magnitude * airTime * 0.5f, (z - transform.position.z) / airTime), ForceMode.VelocityChange);

                    //Wait for state to be cooldown (I landed)
                    t = 0;
                    while (state == ActionState.Attacking)
                    {
                        landing = t > (airTime / 2);
                        if (freezeTime <= 0)
                            t += Time.deltaTime;
                        yield return null;
                    }
                    bodyBox.gameObject.SetActive(false);
                    charRb.velocity = Vector3.zero;

                    //Wait for geyserLand
                    t = 0;
                    while (t < geyserLand)
                    {
                        if (freezeTime <= 0)
                            t += Time.deltaTime;
                        yield return null;
                    }
                }

                state = ActionState.Cooldown;

                t = 0;
                while (t < geyserEnd)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }
                currentAttack += 2;
                state = ActionState.Waiting;
                break;
            case 4: //Super Stomp
                currentAttack++;
                state = ActionState.Waiting;
                break;
            case 5: //Rallying Cry
                if (summons.Count > 0)
                {
                    state = ActionState.Attacking;

                    //Startup
                    t = 0;
                    while (t < cryDuration / 2)
                    {
                        if (freezeTime <= 0)
                            t += Time.deltaTime;
                        yield return null;
                    }

                    //Buff the summons
                    for (int i = 0; i < summons.Count; i++)
                    {
                        Buff buff = (Buff)ScriptableObject.CreateInstance("Buff");
                        buff.SetBuff(0.5f, -1);
                        summons[i].AddBuff(buff, 3);
                    }

                    t = 0;
                    while (t < cryDuration / 2)
                    {
                        if (freezeTime <= 0)
                            t += Time.deltaTime;
                        yield return null;
                    }
                    LookTowardsPlayer();
                }
                currentAttack++;
                state = ActionState.Waiting;
                break;
            case 6: //Surging Spout
                state = ActionState.Startup;
                //Turn on hitbox and wait
                spout.gameObject.SetActive(true);
                club.gameObject.SetActive(false);
                t = 0;
                while (t < spoutStart)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }
                SnapTowardsPlayer();

                //Rush
                state = ActionState.Attacking;
                GetComponent<Collider>().isTrigger = true;
                charRb.useGravity = false;
                t = 0;
                while (t < spoutActive)
                {
                    if (freezeTime <= 0)
                    {
                        charRb.velocity = transform.forward * spoutSpeed;
                        t += Time.deltaTime;
                    }
                    yield return null;
                }

                //Slowdown
                state = ActionState.Cooldown;
                spout.gameObject.SetActive(false);
                GetComponent<Collider>().isTrigger = false;
                charRb.useGravity = true;
                t = 0;
                float total = spoutSpeed / spoutDecel;
                while (t < total)
                {
                    if (freezeTime <= 0)
                    {
                        charRb.velocity = transform.forward * (spoutSpeed - (spoutDecel * t));
                        t += Time.deltaTime;
                    }
                    yield return null;
                }
                charRb.velocity = Vector3.zero;

                //Wait
                t = 0;
                while (t < spoutEnd)
                {
                    if (freezeTime <= 0)
                        t += Time.deltaTime;
                    yield return null;
                }

                if ((transform.position - player.transform.position).magnitude < 3.5f) //jump back if too close!
                {
                    //Determine destination and set speed
                    charRb.AddForce(new Vector3(-transform.position.x * 2 / airTime, Physics.gravity.magnitude * airTime * 0.25f, -transform.position.z * 2 / airTime), ForceMode.VelocityChange);
                    t = 0;
                    while (t < airTime / 2)
                    {
                        if (freezeTime <= 0)
                            t += Time.deltaTime;
                        yield return null;
                    }
                }

                club.gameObject.SetActive(true);
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
        club.transform.localPosition = clubPositionDefault;
        club.transform.localRotation = Quaternion.Euler(clubRotationDefault);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        //Jump Class
        base.OnCollisionEnter(collision);
        if ((currentAttack == 3 || currentAttack == 4) && collision.collider.gameObject.CompareTag("Floor") && landing)
        {
            state = ActionState.Cooldown;
            //Spawn geysers
            for (int i = 0; i < 5; i++)
            {
                //Create geyser, set angle, set parent to room
                Projectile geyser = Instantiate(geyserPrefab, gameObject.transform.position, transform.rotation);
                geyser.gameObject.transform.Rotate(0, -60f + (i * 30f), 0);
                geyser.SetSource(this);
                geyser.transform.Translate(transform.up + (transform.forward * 2.5f), Space.World);
                geyser.transform.parent = roomManager.GetCurrent().transform;
                geysers.Add(geyser);
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        //Surging Spout
        if (currentAttack == 6 && (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Door")) && state == ActionState.Attacking)
        {
            //Turn towards player
            SnapTowardsPlayer();
            transform.Rotate(0, Random.Range(15, 30f) * (Random.Range(0, 1f) > 0.5f ? 1 : -1), 0);

            //Create rocks
            Vector3 norm = transform.position - collider.ClosestPoint(transform.position);
            //if (rockCharge >= rockDelay)
            //{
                for (int i = 0; i < 4; i++)
                {
                    //Create rock, set angle, set parent to room
                    Projectile rock = Instantiate(rockPrefab, collider.gameObject.transform.position, Quaternion.LookRotation(norm));
                    rock.gameObject.transform.Rotate(0, -45f + (i * 22.5f), 0);
                    rock.SetSource(this);
                    rock.transform.Translate(transform.forward * 2f, Space.World);
                    rock.transform.parent = roomManager.GetCurrent().transform;
                    rocks.Add(rock);
                    //rockCharge = 0;
                }
            //}
        }
    }
}
