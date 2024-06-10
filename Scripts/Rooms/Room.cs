using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Dimensions")]
    protected Vector3 topLeftCornerLocation; //(x, y, z) of top left corner
    [SerializeField] protected int xDimension; //x
    [SerializeField] protected int zDimension; //z

    [Header("Room Pieces")]
    [SerializeField] protected GameObject floorPrefab;
    [SerializeField] protected GameObject wallPrefab;
    [SerializeField] protected GameObject chestPrefab;
    [SerializeField] protected GameObject entrancePrefab;
    [SerializeField] protected ExitDoor exitPrefab;
    protected GameObject floor;
    protected List<GameObject> walls;
    protected List<GameObject> chests;
    protected GameObject entrance;
    protected GameObject exit;
    protected List<PickupCW> weapons;

    [Header("Door Materials")]
    [SerializeField] protected Material locked;
    [SerializeField] protected Material unlocked;

    [Header("Enemy Management")]
    [SerializeField] protected RoomManager roomManager;
    [SerializeField] protected SpawnManager spawnManager;

    [Header("Next Room")]
    [SerializeField] Room next;

    [Header("Boss Info")]
    [SerializeField] protected bool bossRoom = false;
    [SerializeField] protected int bossNum = 0;
    [SerializeField] protected Boss boss;

    //TODO: Update stats appropriately
    [Header("Room Stats")]
    [SerializeField] protected float roomTime;
    [SerializeField] protected int damageTaken;
    [SerializeField] protected int potionsUsed;
    [SerializeField] protected int signaturePtsGained;
    [SerializeField] protected int signatureMovesUsed;

    [Header("Booleans")]
    [SerializeField] protected bool breakRoom;
    [SerializeField] protected bool corrected = false;

    // Random Generation Algorithm Info
    protected int MAX_FENCES = 3;
    protected bool[,] grid; //tells me if something is occupied (Row, Col, or -World Z, World X) 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (!corrected) Correct();
    }

    public void Initialize(int x, int z, RoomManager rm, SpawnManager sm)
    {
        xDimension = x;
        zDimension = z;
        topLeftCornerLocation = new Vector3(((float)-x) / 2, 0, ((float)z) / 2);

        //borderPrefab = b;

        roomManager = rm;
        spawnManager = sm;

        grid = new bool[zDimension, xDimension];
        //borderPieces = new List<GameObject>();
        walls = new List<GameObject>();
        chests = new List<GameObject>();
    }

   /***************************
    * GRID OPERATIONS
    ***************************/
    protected Vector3 GridToLocalPosition(int row, int col)
    {
        if (row < 0 || row >= zDimension)
            Debug.LogWarning("Out of bounds x position " + row + "! Must be in range [0, " + zDimension + ")!");

        if (col < 0 || col >= xDimension)
            Debug.LogWarning("Out of bounds z position " + row + "! Must be in range [0, " + xDimension + ")!");

        Vector3 worldPosition = topLeftCornerLocation + new Vector3(0.5f, 0, -0.5f);

        // Add col to gridX and subtract row from gridY
        worldPosition += new Vector3(col, 0, -row);
        return worldPosition;
    }

    protected (int, int) WorldPositionToGrid(Vector3 worldPosition)
    {
        int row = (int)(worldPosition.z - (zDimension / 2));
        int col = (int)(worldPosition.x + (xDimension / 2));

        if (row < 0 || row >= zDimension)
            Debug.LogWarning("Out of bounds x position " + row + "! Must be in range [0, " + zDimension + ")!");

        if (col < 0 || col >= xDimension)
            Debug.LogWarning("Out of bounds z position " + row + "! Must be in range [0, " + xDimension + ")!");
        return (row, col);
    }

    /**************************************************************************
     * Returns the number of adjacent walls on the grid (left, right, up, down)
     **************************************************************************/
    protected int AdjacentWallCount(int row, int col, bool includeBorder)
    {
        if (row < 0 || row >= zDimension)
        {
            Debug.LogError("Out of bounds x position " + row + "! Must be in range [0, " + zDimension + ")!");
            return -1;
        }

        if (col < 0 || col >= xDimension)
        {
            Debug.LogError("Out of bounds z position " + row + "! Must be in range [0, " + xDimension + ")!");
            return -1;
        }

        int wallCount = 0;
        if ((includeBorder && row == 0) || grid[row - 1, col]) wallCount++; // Up
        if ((includeBorder && row == zDimension - 1) || grid[row + 1, col]) wallCount++; // Down
        if ((includeBorder && col == 0) || grid[row, col - 1]) wallCount++; // Left
        if ((includeBorder && col == xDimension - 1) || grid[row, col + 1]) wallCount++; // Right

        return wallCount;
    }

    protected int AdjacentWallCount(Vector3 worldPosition, bool includeBorder)
    {
        (int, int) gridPos = WorldPositionToGrid(worldPosition);
        return AdjacentWallCount(gridPos.Item1, gridPos.Item2, includeBorder);
    }

    
    /**********************************************************
     * Returns number of surrounding walls, including diagonals
     * cornerVector tells you which corners to include
     **********************************************************/
    protected int SurroundingWallCount(int row, int col, bool includeBorder)
    {
        int wallCount = AdjacentWallCount(row, col, includeBorder);
        if (wallCount <= 0) // OOB
            return wallCount;

        if ((includeBorder && row == 0 && col == 0)
            || (row != 0 && col != 0 && grid[row - 1, col - 1]))
            wallCount++; // Up-Left

        if ((includeBorder && row == 0 && col == xDimension - 1)
            || (row != 0 && col != xDimension - 1 && grid[row - 1, col + 1]))
            wallCount++; // Up-Right

        if ((includeBorder && row == zDimension - 1 && col == 0)
            || (row != zDimension - 1 && col != 0 && grid[row + 1, col - 1]))
            wallCount++; // Down-Left

        if ((includeBorder && row == zDimension - 1 && col == xDimension - 1)
            || (row != zDimension - 1 && col != xDimension - 1 && grid[row + 1, col + 1]))
            wallCount++; // Down-Right

        return wallCount;
    }

    protected int SurroundingWallCount(Vector3 worldPosition, bool includeBorder)
    {
        (int, int) gridPos = WorldPositionToGrid(worldPosition);
        return SurroundingWallCount(gridPos.Item1, gridPos.Item2, includeBorder);
    }

    protected bool PositionInBounds(int row, int col)
    {
        if (row < 0 || row >= zDimension)
        {
            Debug.Log("Out of bounds x position " + row + "; must be in range [0, " + zDimension + ")!");
            return false;
        }

        if (col < 0 || col >= xDimension)
        {
            Debug.Log("Out of bounds z position " + row + "; must be in range [0, " + xDimension + ")!");
            return false;
        }

        return true;
    }

    protected bool PositionInBounds(Vector3 worldPosition)
    {
        (int, int) gridPos = WorldPositionToGrid(worldPosition);
        return PositionInBounds(gridPos.Item1, gridPos.Item2);
    }

    /******************************
     * POSITION SPAWN VALIDITY
     * (assumes position in bounds)
     ******************************/
    public bool IsValidSpawnPosition(Vector3 worldPosition)
    {
        (int, int) gridPos = WorldPositionToGrid(worldPosition);
        return IsValidSpawnPosition(gridPos.Item1, gridPos.Item2);
    }

    protected bool IsValidSpawnPosition(int row, int col)
    {
        if (!PositionInBounds(row, col))
            return false;

        return !grid[row, col];
    }

    public List<Vector3> ValidSpawnPositions()
    {
        List<Vector3> validSpawnPositions = new List<Vector3>();
        for (int r = 0; r < zDimension; r++)
            for (int c = 0; c < xDimension; c++)
                if (IsValidSpawnPosition(r, c))
                    validSpawnPositions.Add(GridToLocalPosition(r, c));
        return validSpawnPositions;
    }

    /******************************************************************
     * The random generation algorithm.
     * It creates walls throughout the room, and returns how many
     * valid spawn positions there.
     * A spawn position is valid if you can reach the entrance from it
     ******************************************************************/
    public int GenerateRoom()
    {
        AddFloor(Instantiate(floorPrefab));
        AddEntrance(Instantiate(entrancePrefab));

        ExitDoor generatedExit = Instantiate(exitPrefab);
        generatedExit.SetRoomManager(roomManager);
        Debug.Log(roomManager);
        AddExit(generatedExit.gameObject);

        /***************************************************
         * There are four types of wall formations to spawn:
         * Single: One wall
         * Double: Two walls
         * Quad: Four walls in a square
         * Fence: Squares just out from one of four borders
         *        up to halfway + 1 through the room
         **************************************************/
        walls = new List<GameObject>();
        // FENCES
        GameObject[] fences = GenerateFences();
        foreach (GameObject fence in fences)
            walls.Add(fence);

        // TODO: QUADS

        // TODO: DOUBLES

        // TODO: SINGLES

        foreach (GameObject wall in walls)
        {
            wall.transform.parent = transform;
            wall.transform.localPosition = wall.transform.position;
        }
        return ValidSpawnPositions().Count;
    }

    /**************************************
     * 1) Select the number of fences
     * 2) For each fence:
     *    i) Select a border
     *    ii) Choose an unoccupied, non-corner space to start
     *    iii) Choose a random length (3-[half of length or width])
     *    iv) Instantiate walls, Put these walls in a game object, then put that game object in the room transform
     *    v) Mark the grid
     ***************************************/
    protected GameObject[] GenerateFences()
    {
        GameObject[] fences = new GameObject[Random.Range(0, MAX_FENCES + 1)]; // Fence Count
        List<(int, int)> startingLocations = new List<(int, int)>();
        for (int i = 0; i < fences.Length; i++)
            fences[i] = new GameObject("Fence");

        foreach (GameObject fence in fences)
        {
            (int, int) gridStart; // (row, col) == (-z, x)
            (int, int) gridDirection; // (row, col) == (-z, x)
            int length;
            do
            {
                // Choose a location and random length
                int border = Random.Range(0, 4);
                switch (border)
                {
                    case 0: // North Border (Fence goes south)
                        gridStart = (zDimension - 1, Random.Range(2, xDimension - 1));
                        gridDirection = (-1, 0);
                        length = Random.Range(3, (zDimension / 2) + 2);
                        break;
                    case 1: // South Border (Fence goes north)
                        gridStart = (0, Random.Range(2, xDimension - 1));
                        gridDirection = (1, 0);
                        length = Random.Range(3, (zDimension / 2) + 2);
                        break;
                    case 2: // West Border (Fence goes east)
                        gridStart = (Random.Range(2, zDimension - 1), xDimension - 1);
                        gridDirection = (0, -1);
                        length = Random.Range(3, (xDimension / 2) + 2);
                        break;
                    case 3: // East Border (Fence goes west)
                    default:
                        gridStart = (Random.Range(2, zDimension - 1), 0);
                        gridDirection = (0, 1);
                        length = Random.Range(3, (xDimension / 2) + 2);
                        break;
                }
            } while (startingLocations.Contains(gridStart));
            startingLocations.Add(gridStart);

            //Instantiate Each Wall and add to fence
            for (int i = 0; i < length; i++)
            {
                //Need to check if this position is adjacent to more than 1 wall,
                //as doing so could create a box (not desired)
                (int, int) wallGridPosition = (gridStart.Item1 + (gridDirection.Item1 * i), gridStart.Item2 + (gridDirection.Item2 * i));
                (int, int) nextGridPosition = (gridStart.Item1 + (gridDirection.Item1 * (i+1)), gridStart.Item2 + (gridDirection.Item2 * (i+1)));
                (int, int) nextSpaceLeft = (nextGridPosition.Item1 - gridDirection.Item2, nextGridPosition.Item2 - gridDirection.Item1);
                (int, int) nextSpaceRight = (nextGridPosition.Item1 + gridDirection.Item2, nextGridPosition.Item2 + gridDirection.Item1);

                // Space isn't occupied AND it doesn't make a diagonal with a square in the forward direction
                if (!grid[wallGridPosition.Item1, wallGridPosition.Item2] && AdjacentWallCount(nextGridPosition.Item1, nextGridPosition.Item2, true) <= 0
                    && !grid[nextSpaceLeft.Item1, nextSpaceLeft.Item2] && !grid[nextSpaceRight.Item1, nextSpaceRight.Item2])
                {
                    // Set position using gridStart and gridDirection
                    Vector3 wallPosition = GridToLocalPosition(wallGridPosition.Item1, wallGridPosition.Item2);

                    // Create wall
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.parent = fence.transform;
                    wall.transform.localPosition = wallPosition;
                    grid[gridStart.Item1 + (gridDirection.Item1 * i), gridStart.Item2 + (gridDirection.Item2 * i)] = true;
                }
                else
                    i = length;
            }
        }

        return fences;
    }

    /*****************************
     * BUILDING THE ROOM
     *****************************/
    //protected void CreateBorders()
    //{
    //    //Horizontal
    //    for (int i = 0; i <= xDimension; i++)
    //    {
    //        if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + i, topLeftCornerLocation.y, topLeftCornerLocation.z - 0.25f) &&
    //            exit.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + i, topLeftCornerLocation.y, topLeftCornerLocation.z - 0.25f)) //make sure there's not a door there
    //        {
    //            borderPieces.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + i, topLeftCornerLocation.y, topLeftCornerLocation.z - 0.25f),
    //                Quaternion.Euler(0, -90, 0)));
    //            borderPieces[borderPieces.Count - 1].transform.parent = gameObject.transform;
    //        }

    //        if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + i, topLeftCornerLocation.y, topLeftCornerLocation.z + zDimension + 0.25f) &&
    //            exit.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + i, topLeftCornerLocation.y, topLeftCornerLocation.z + zDimension + 0.25f))
    //        {
    //            borderPieces.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + i, topLeftCornerLocation.y, topLeftCornerLocation.z + zDimension + 0.25f),
    //                Quaternion.Euler(0, 90, 0)));
    //            borderPieces[borderPieces.Count - 1].transform.parent = this.gameObject.transform;
    //        }
    //    }
    //    //Vertical
    //    for (int i = 0; i <= zDimension; i++)
    //    {
    //        if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x - 0.25f, topLeftCornerLocation.y, topLeftCornerLocation.z + i) &&
    //            exit.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x - 0.25f, topLeftCornerLocation.y, topLeftCornerLocation.z + i))
    //        {
    //            borderPieces.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x - 0.25f, topLeftCornerLocation.y, topLeftCornerLocation.z + i),
    //                Quaternion.Euler(0, 0, 0)));
    //            borderPieces[borderPieces.Count - 1].transform.parent = this.gameObject.transform;
    //        }

    //        if (entrance.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + xDimension + 0.25f, topLeftCornerLocation.y, topLeftCornerLocation.z + i) &&
    //            exit.transform.position != borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + xDimension + 0.25f, topLeftCornerLocation.y, topLeftCornerLocation.z + i))
    //        {
    //            borderPieces.Add(Instantiate(borderPrefab, borderPrefab.transform.position + new Vector3(topLeftCornerLocation.x + xDimension + 0.25f, topLeftCornerLocation.y, topLeftCornerLocation.z + i),
    //                Quaternion.Euler(0, 180, 0)));
    //            borderPieces[borderPieces.Count - 1].transform.parent = this.gameObject.transform;
    //        }
    //    }
    //}

    //protected void CorrectBadPlacements()
    //{
    //    if (grid != null && borderPieces != null && walls != null && chests != null)
    //    {
    //        //Check that each wall is in a place on the grid
    //        for (int i = 0; i < walls.Count; i++)
    //        {
    //            int xPos = (int)Mathf.Round(walls[i].transform.position.x - topLeftCornerLocation.x);
    //            int zPos = (int)Mathf.Round(topLeftCornerLocation.z - walls[i].transform.position.z);

    //            if (!(xPos >= 0 && xPos < xDimension && zPos >= 0 && zPos < zDimension))
    //            {
    //                Destroy(walls[i]);
    //                walls.RemoveAt(i--);
    //            }
    //            else if (!grid[xPos, zPos])
    //                grid[xPos, zPos] = true;
    //        }

    //        //Check that each chest is in a place
    //        for (int i = 0; i < chests.Count; i++)
    //        {
    //            int xPos = (int)Mathf.Round(chests[i].transform.position.x - topLeftCornerLocation.x);
    //            int zPos = (int)Mathf.Round(topLeftCornerLocation.z - chests[i].transform.position.z);

    //            if (!(xPos >= 0 && xPos < xDimension && zPos >= 0 && zPos < zDimension))
    //            {
    //                Destroy(chests[i]);
    //                chests.RemoveAt(i--);
    //            }
    //            else if (!grid[xPos, zPos])
    //                grid[xPos, zPos] = true;
    //        }

    //        //Check that each weapon pickup is in a place
    //        for (int i = 0; i < weapons.Count; i++)
    //        {
    //            int xPos = (int)Mathf.Round(weapons[i].transform.position.x - topLeftCornerLocation.x);
    //            int zPos = (int)Mathf.Round(topLeftCornerLocation.z - weapons[i].transform.position.z);

    //            if (!(xPos >= 0 && xPos < xDimension && zPos >= 0 && zPos < zDimension))
    //            {
    //                Destroy(weapons[i]);
    //                weapons.RemoveAt(i--);
    //            }
    //            else if (!grid[xPos, zPos])
    //                grid[xPos, zPos] = true;
    //        }
    //    }
    //}

    /***************************************
     * Adding components to the floor object
     ***************************************/
    public void AddFloor(GameObject f)
    {
        if (f != null)
            floor = f;
        floor.transform.parent = transform;
        floor.transform.localPosition = Vector3.zero;
        //CorrectBadPlacements();
    }

    public void AddEntrance(GameObject e)
    {
        if (e != null)
            entrance = e;
        entrance.transform.parent = transform;
        entrance.transform.localPosition = new Vector3(-10, 0.75f, 0);

        //CorrectBadPlacements();
    }

    public GameObject GetEntrance()
    {
        return entrance;
    }

    public void AddExit(GameObject ed)
    {
        if (ed != null && ed.GetComponent<ExitDoor>() != null)
        {
            //TODO: Randomize Location
            exit = ed;
            exit.transform.parent = transform;
            exit.transform.localPosition = new Vector3(-10, 0.75f, 0);
            exit.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    //public void AddWall(GameObject w)
    //{
    //    if (w != null)
    //    {
    //        int xPos = (int)Mathf.Round(w.transform.position.x - topLeftCornerLocation.x);
    //        int zPos = (int)Mathf.Round(topLeftCornerLocation.z - w.transform.position.z);

    //        if (xPos >= 0 && xPos < xDimension && zPos >= 0 && zPos < zDimension && !grid[xPos, zPos])
    //        {
    //            grid[xPos, zPos] = true;
    //            walls.Add(w);
    //            walls[walls.Count - 1].transform.parent = gameObject.transform;
    //        }
    //    }
    //    CorrectBadPlacements();
    //}

    //public void AddChest(GameObject c)
    //{
    //    if (c != null)
    //    {
    //        int xPos = (int)Mathf.Round(c.transform.position.x - topLeftCornerLocation.x);
    //        int zPos = (int)Mathf.Round(topLeftCornerLocation.z - c.transform.position.z);

    //        if (xPos >= 0 && xPos < xDimension && zPos >= 0 && zPos < zDimension && !grid[xPos, zPos])
    //        {
    //            grid[xPos, zPos] = true;
    //            chests.Add(c);
    //            chests[chests.Count - 1].transform.parent = this.gameObject.transform;
    //        }
    //    }
    //    CorrectBadPlacements();
    //}

    //public void AddPickup(PickupCW pickup)
    //{
    //    weapons.Add(pickup);
    //    CorrectBadPlacements();
    //}

    public void RemoveWeapon(PickupCW pickup)
    {
        if (weapons != null) weapons.Remove(pickup);
    }

    public int GetXDimension()
    {
        return xDimension;
    }

    public int GetZDimension()
    {
        return zDimension;
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

    public bool IsBreakRoom()
    {
        return breakRoom;
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

    /*****************
     * Room Stats
     *****************/
    public float GetRoomTime()
    {
        return roomTime;
    }

    //Damage Taken Defeated
    public void AddDamageTaken(int pts)
    {
        damageTaken += pts;
    }
    public int GetDamageTaken()
    {
        return damageTaken;
    }

    // Potions Used
    public void IncrementPotionsUsed()
    {
        potionsUsed++;
    }
    public int GetPotionsUsed()
    {
        return potionsUsed;
    }

    // Signature Pts Gained
    public void AddSignaturePointsGained(int pts)
    {
        signaturePtsGained += pts;
    }
    public int GetSignaturePointsGained()
    {
        return signaturePtsGained;
    }

    // Signature Moves Used
    public void IncrementSignatureMovesUsed()
    {
        signatureMovesUsed++;
    }
    public int GetSignatureMovesUsed()
    {
        return signatureMovesUsed;
    }
}
