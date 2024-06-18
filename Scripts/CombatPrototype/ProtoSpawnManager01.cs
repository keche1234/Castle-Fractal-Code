using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProtoSpawnManager01 : SpawnManager
{
    [SerializeField] protected int superWave = 0;
    [SerializeField] protected List<TextMeshProUGUI> messages;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        spawnList = new List<Enemy>();
        spawnPosList = new List<Vector3>();
    }

    // Update is called once per frame
    new void Update()
    {
        if (!spawned && spawnList.Count == 0 && currentWave < totalWaves)
        {
            StartCoroutine("SpawnWave");
        }
        if (superWave >= 15 && allDefeated)
            for (int i = 0; i < messages.Count; i++)
            {
                messages[0].gameObject.SetActive(true);
                messages[1].gameObject.SetActive(true);
                messages[2].gameObject.SetActive(false);
                messages[3].gameObject.SetActive(false);
            }
    }

    public override IEnumerator SpawnWave()
    {
        spawned = true;
        currentWave++;
        superWave++;
        spawnPosList = new List<Vector3>();
        List<GameObject> covers = new List<GameObject>();
        yield return new WaitForSeconds(waveDelay);

        switch(superWave)
        {
            case 1:
                spawnPosList.Add(new Vector3(-6, 0.5f, -3));
                spawnPosList.Add(new Vector3(6, 0.5f, -3));
                spawnPosList.Add(new Vector3(0, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 2:
                spawnPosList.Add(new Vector3(-5, 0.5f, -2));
                spawnPosList.Add(new Vector3(5, 0.5f, -2));
                spawnPosList.Add(new Vector3(-5, 0.5f, 2));
                spawnPosList.Add(new Vector3(5, 0.5f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 3:
                spawnPosList.Add(new Vector3(-6, 0.5f, -3));
                spawnPosList.Add(new Vector3(6, 0.5f, -3));
                spawnPosList.Add(new Vector3(0, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 4:
                spawnPosList.Add(new Vector3(-5, 0.5f, -2));
                spawnPosList.Add(new Vector3(5, 0.5f, -2));
                spawnPosList.Add(new Vector3(-5, 0.5f, 2));
                spawnPosList.Add(new Vector3(5, 0.5f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 5:
                spawnPosList.Add(new Vector3(-6, 0.75f, -3));
                spawnPosList.Add(new Vector3(6, 0.75f, -3));
                spawnPosList.Add(new Vector3(0, 0.75f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 6:
                spawnPosList.Add(new Vector3(-5, 0.75f, -2));
                spawnPosList.Add(new Vector3(5, 0.75f, -2));
                spawnPosList.Add(new Vector3(-5, 0.75f, 2));
                spawnPosList.Add(new Vector3(5, 0.75f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 7:
                spawnPosList.Add(new Vector3(-6, 0.5f, -3));
                spawnPosList.Add(new Vector3(6, 0.5f, -3));
                spawnPosList.Add(new Vector3(0, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 8:
                spawnPosList.Add(new Vector3(-5, 0.5f, -2));
                spawnPosList.Add(new Vector3(5, 0.5f, -2));
                spawnPosList.Add(new Vector3(-5, 0.5f, 2));
                spawnPosList.Add(new Vector3(5, 0.5f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 9:
                spawnPosList.Add(new Vector3(-6, 0.5f, -3));
                spawnPosList.Add(new Vector3(6, 0.5f, -3));
                spawnPosList.Add(new Vector3(0, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 10:
                spawnPosList.Add(new Vector3(-5, 0.5f, -2));
                spawnPosList.Add(new Vector3(5, 0.5f, -2));
                spawnPosList.Add(new Vector3(-5, 0.5f, 2));
                spawnPosList.Add(new Vector3(5, 0.5f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 11:
                spawnPosList.Add(new Vector3(-4, 0.5f, 0));
                spawnPosList.Add(new Vector3(4, 0.5f, 0));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);

                spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[0], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPosList[1], gameObject.transform.rotation));

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 12:
                spawnPosList.Add(new Vector3(-4, 0.5f, 2));
                spawnPosList.Add(new Vector3(4, 0.75f, 2));
                spawnPosList.Add(new Vector3(0, 0.5f, -2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);

                spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[0], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPosList[1], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPosList[2], gameObject.transform.rotation));

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 13:
                spawnPosList.Add(new Vector3(-5, 0.75f, -2));
                spawnPosList.Add(new Vector3(5, 0.5f, -2));
                spawnPosList.Add(new Vector3(-5, 0.5f, 2));
                spawnPosList.Add(new Vector3(5, 0.5f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);

                spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPosList[0], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPosList[1], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPosList[2], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPosList[3], gameObject.transform.rotation));

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 14:
                spawnPosList.Add(new Vector3(-3, 0.5f, -2));
                spawnPosList.Add(new Vector3(3, 0.5f, -2));
                spawnPosList.Add(new Vector3(-5, 0.5f, 0));
                spawnPosList.Add(new Vector3(5, 0.5f, 0));
                spawnPosList.Add(new Vector3(0, 0.5f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);

                spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPosList[0], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPosList[1], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[2], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[3], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPosList[4], gameObject.transform.rotation));

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 15:
                spawnPosList.Add(new Vector3(0, 0.5f, 0));
                spawnPosList.Add(new Vector3(-3, 0.5f, -2));
                spawnPosList.Add(new Vector3(3, 0.5f, -2));
                spawnPosList.Add(new Vector3(-5, 0.5f, 0));
                spawnPosList.Add(new Vector3(5, 0.75f, 0));
                spawnPosList.Add(new Vector3(0, 0.5f, 2));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);

                spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPosList[0], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPosList[1], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPosList[2], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[3], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPosList[4], gameObject.transform.rotation));
                spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPosList[5], gameObject.transform.rotation));

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            default:
                break;
        }

        //When SpawnWave is called, set current waves and totalWaves to certain values
        //Based on level and who's calling it.
        yield return null;
    }
}
