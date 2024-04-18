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
    //[SerializeField] protected ExitDoor exit;

    [Header("Prefabs")]
    [SerializeField] protected Room emptyRoomPrefab;
    [SerializeField] protected GameObject floorPrefab;
    [SerializeField] protected GameObject borderPrefab;
    [SerializeField] protected GameObject wallPrefab;
    [SerializeField] protected GameObject chestPrefab;
    [SerializeField] protected GameObject doorPrefab;
    [SerializeField] protected List<Boss> bossPrefabs;

    [Header("Materials")]
    [SerializeField] protected Material locked;
    [SerializeField] protected Material unlocked;

    [Header("Game Management")]
    [SerializeField] protected SpawnManager spawnManager;
    [SerializeField] protected PickupCW pickupPrefab;

    // Start is called before the first frame update
    protected void Start()
    {
        level = 0;
        CreateNext();
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public virtual void CreateNext()
    {
        Room next = Instantiate(emptyRoomPrefab, new Vector3(0, height, 0), Quaternion.Euler(0, 0, 0));
        Vector3 topL = new Vector3(-9.5f, height, -4.5f);
        int length = 19;
        int width = 9;

        //Set next
        next.Initialize(topL, length, width, borderPrefab, spawnManager);
        next.AddFloor(Instantiate(floorPrefab, new Vector3(0, height, 0), Quaternion.Euler(0, 0, 0)));
        next.AddEntrance(Instantiate(doorPrefab, new Vector3(-0.5f, height + 0.75f, -4.75f), Quaternion.Euler(0, -90, 0)));

        GameObject exit = Instantiate(doorPrefab, new Vector3(-0.5f, height + 0.75f, 4.75f), Quaternion.Euler(0, 90, 0));
        exit.AddComponent<ExitDoor>();
        exit.GetComponent<ExitDoor>().SetRoomManager(this);
        exit.GetComponent<ExitDoor>().AddLocked(locked);
        exit.GetComponent<ExitDoor>().AddUnlocked(unlocked);
        next.AddExit(exit);

        next.CreateBorders();

        if ((level + 1) % 5 == 0) //Boss Room
        {
            //Select a random boss
            next.SetBossRoom(true);
            next.SetBossNumber(Random.Range(0, bossPrefabs.Count));
        }

        //TODO: Generate environmental components (Bosses have different requirements, like no internal walls)

        current.SetNext(next);
    }

    //public void SetCurrent(Room r)
    //{
    //    current = r;
    //}

    //public void SetNext(Room r)
    //{
    //    next = r;
    //}

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

        //Create the next room
        CreateNext();

        //Move the player
        player.transform.position = current.GetEntrance().transform.position + (current.GetEntrance().transform.right) - new Vector3(0, 0.25f, 0);
        player.transform.rotation = current.GetEntrance().transform.rotation;
        player.transform.Rotate(0, 90, 0);
        StartCoroutine(DisablePlayer(0.5f));

        //Game Info
        level++;

        //if (level % 5 == 0)
        //{
        //    //spawn boss
        //    current.SetBoss(Instantiate(bossPrefabs[current.GetBossNumber()], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)));
        //}
        //else
        //{
        //    spawnManager.SetWaveInfo(0, 1);
        //}
    }

    protected IEnumerator DisablePlayer(float wait)
    {
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Rigidbody>().velocity *= 0;
        yield return new WaitForSeconds(wait);
        player.GetComponent<PlayerController>().enabled = true;

    }
}
