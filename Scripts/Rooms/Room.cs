using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] protected Vector3 topL; //(x, y, z) of top corner
    [SerializeField] protected int length; //x
    [SerializeField] protected int width; //z

    [Header("Room Pieces")]
    [SerializeField] protected GameObject floor;
    [SerializeField] protected GameObject borderPrefab;
    [SerializeField] protected GameObject entrance;
    [SerializeField] protected GameObject exit;
    protected List<GameObject> borders;
    [SerializeField] protected List<GameObject> walls;
    [SerializeField] protected List<GameObject> chests;
    [SerializeField] protected List<PickupCW> weapons;

    [Header("Enemy Management")]
    [SerializeField] protected SpawnManager spawnManager;

    [Header("Next Room")]
    [SerializeField] Room next;

    [Header("Boss Info")]
    [SerializeField] protected bool bossRoom = false;
    [SerializeField] protected int bossNum = 0;
    [SerializeField] protected Boss boss;

    [Header("Booleans")]
    [SerializeField] protected bool breakRoom;
    [SerializeField] protected bool corrected = false;

    protected bool[,] grid; //tells me if something is occupied
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (!corrected) Correct();
    }

    public void Initialize(Vector3 corner, int l, int w, GameObject b, SpawnManager sm)
    {
        topL = corner;
        length = l;
        width = w;
        borderPrefab = b;
        spawnManager = sm;

        grid = new bool[(int)topL.x + length, (int)topL.z + width];
        borders = new List<GameObject>();
        walls = new List<GameObject>();
        chests = new List<GameObject>();
    }

    /*****************************
     * Building the room
     *****************************/
    public void CreateBorders()
    {
        //Horizontal
        for (int i = 0; i <= length; i++)
        {
            if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topL.x + i, topL.y, topL.z - 0.25f) &&
                exit.transform.position != borderPrefab.transform.position + new Vector3(topL.x + i, topL.y, topL.z - 0.25f)) //make sure there's not a door there
            {
                borders.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topL.x + i, topL.y, topL.z - 0.25f),
                    Quaternion.Euler(0, -90, 0)));
                borders[borders.Count - 1].transform.parent = this.gameObject.transform;
            }

            if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topL.x + i, topL.y, topL.z + width + 0.25f) &&
                exit.transform.position != borderPrefab.transform.position + new Vector3(topL.x + i, topL.y, topL.z + width + 0.25f))
            {
                borders.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topL.x + i, topL.y, topL.z + width + 0.25f),
                    Quaternion.Euler(0, 90, 0)));
                borders[borders.Count - 1].transform.parent = this.gameObject.transform;
            }
        }
        //Vertical
        for (int i = 0; i <= width; i++)
        {
            if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topL.x - 0.25f, topL.y, topL.z + i) &&
                exit.transform.position != borderPrefab.transform.position + new Vector3(topL.x - 0.25f, topL.y, topL.z + i))
            {
                borders.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topL.x - 0.25f, topL.y, topL.z + i),
                    Quaternion.Euler(0, 0, 0)));
                borders[borders.Count - 1].transform.parent = this.gameObject.transform;
            }

            if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topL.x + length + 0.25f, topL.y, topL.z + i) &&
                exit.transform.position != borderPrefab.transform.position + new Vector3(topL.x + length + 0.25f, topL.y, topL.z + i))
            {
                borders.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topL.x + length + 0.25f, topL.y, topL.z + i),
                    Quaternion.Euler(0, 180, 0)));
                borders[borders.Count - 1].transform.parent = this.gameObject.transform;
            }
        }
    }

    public void Correct()
    {
        if (grid != null && borders != null && walls != null && chests != null)
        {
            //Check that each wall is in a place
            for (int i = 0; i < walls.Count; i++)
            {
                int xPos = (int)Mathf.Round(walls[i].transform.position.x - topL.x);
                int zPos = (int)Mathf.Round(topL.z - walls[i].transform.position.z);

                if (!(xPos >= 0 && xPos < length && zPos >= 0 && zPos < width))
                {
                    Destroy(walls[i]);
                    walls.RemoveAt(i--);
                }
                else if (!grid[xPos, zPos])
                    grid[xPos, zPos] = true;
            }

            //Check that each chest is in a place
            for (int i = 0; i < chests.Count; i++)
            {
                int xPos = (int)Mathf.Round(chests[i].transform.position.x - topL.x);
                int zPos = (int)Mathf.Round(topL.z - chests[i].transform.position.z);

                if (!(xPos >= 0 && xPos < length && zPos >= 0 && zPos < width))
                {
                    Destroy(chests[i]);
                    chests.RemoveAt(i--);
                }
                else if (!grid[xPos, zPos])
                    grid[xPos, zPos] = true;
            }

            //Check that each weapon pickup is in a place
            for (int i = 0; i < weapons.Count; i++)
            {
                int xPos = (int)Mathf.Round(weapons[i].transform.position.x - topL.x);
                int zPos = (int)Mathf.Round(topL.z - weapons[i].transform.position.z);

                if (!(xPos >= 0 && xPos < length && zPos >= 0 && zPos < width))
                {
                    Destroy(weapons[i]);
                    weapons.RemoveAt(i--);
                }
                else if (!grid[xPos, zPos])
                    grid[xPos, zPos] = true;
            }
        }
        corrected = true;
    }

    //Be sure to check the dimensions
    public void AddFloor(GameObject f)
    {
        if (f != null)
            floor = f;
        floor.transform.parent = this.gameObject.transform;
        Correct();
    }

    public void AddEntrance(GameObject e)
    {
        if (e != null)
            entrance = e;
        entrance.transform.parent = this.gameObject.transform;
        Correct();
    }

    public GameObject GetEntrance()
    {
        return entrance;
    }

    public void AddWall(GameObject w)
    {
        if (w != null)
        {
            int xPos = (int)Mathf.Round(w.transform.position.x - topL.x);
            int zPos = (int)Mathf.Round(topL.z - w.transform.position.z);

            if (xPos >= 0 && xPos < length && zPos >= 0 && zPos < width && !grid[xPos, zPos])
            {
                grid[xPos, zPos] = true;
                walls.Add(w);
                walls[walls.Count - 1].transform.parent = this.gameObject.transform;
            }
        }
        Correct();
    }

    public void AddChest(GameObject c)
    {
        if (c != null)
        {
            int xPos = (int)Mathf.Round(c.transform.position.x - topL.x);
            int zPos = (int)Mathf.Round(topL.z - c.transform.position.z);

            if (xPos >= 0 && xPos < length && zPos >= 0 && zPos < width && !grid[xPos, zPos])
            {
                grid[xPos, zPos] = true;
                chests.Add(c);
                chests[chests.Count - 1].transform.parent = this.gameObject.transform;
            }
        }
        Correct();
    }

    public void AddPickup(PickupCW pickup)
    {
        weapons.Add(pickup);
        Correct();
    }

    public void AddExit(GameObject ed)
    {
        if (ed != null && ed.GetComponent<ExitDoor>() != null)
            exit = ed;
        exit.transform.parent = this.gameObject.transform;
        Correct();
    }

    public void RemoveWeapon(PickupCW pickup)
    {
        if (weapons != null) weapons.Remove(pickup);
    }

    public int GetLength()
    {
        return length;
    }

    public int GetWidth()
    {
        return width;
    }

    /************************
     * Room Manager business
     ************************/
    public void SetNext(Room n)
    {
        next = n;
    }

    public Room GetNext()
    {
        return next;
    }

    /******************
     * Boss Management
     ******************/
    public void SetBoss(Boss b)
    {
        Destroy(boss);
        boss = b;
        boss.transform.parent = transform;
        boss.SetRoom(this);
    }

    public Boss GetBoss()
    {
        return boss;
    }

    public void SetBossRoom(bool b)
    {
        bossRoom = b;
    }

    public bool IsBossRoom()
    {
        return bossRoom;
    }

    public void SetBossNumber(int i)
    {
        bossNum = i;
    }

    public int GetBossNumber()
    {
        return bossNum;
    }

    public bool RoomCleared()
    {
        return spawnManager.AllDefeated();
    }
}
