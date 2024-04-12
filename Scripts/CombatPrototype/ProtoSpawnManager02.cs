using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProtoSpawnManager02 : SpawnManager
{
    // SpawnList Order:
    // 0) Green Snake
    // 1) Blue Ogre
    // 2) Yellow Minotaur
    // 3) Red Mage
    // 4) Violet Knight
    // 5) Wisteria Wizard
    // 6) Tangerine Troll
    // 7) Pink Python
    // 8) Cerulean Satyr
    // 9) Turquoise Templar

    [SerializeField] protected int superWave = 0;
    [SerializeField] protected List<Text> messages;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        spawnList = new List<Enemy>();
        spawnPos = new List<Vector3>();
    }

    // Update is called once per frame
    new void Update()
    {
        if (!spawned && spawnList.Count == 0 && currentWave < totalWaves)
        {
            StartCoroutine("SpawnWave");
        }
        if (superWave >= 20 && allDefeated)
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
        spawnPos = new List<Vector3>();
        List<GameObject> covers = new List<GameObject>();
        yield return new WaitForSeconds(waveDelay);

        switch(superWave)
        {
            case 1: //Tangering Troll
                spawnPos.Add(new Vector3(-6, 0.5f, 3));
                spawnPos.Add(new Vector3(6, 0.5f, 3));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add((Enemy)Instantiate(enemyPrefabs[6], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            //Up to 20
        }

        //switch (superWave)
        //{
        //    case 1:
        //        spawnPos.Add(new Vector3(-6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(0, 0.5f, 3));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 2:
        //        spawnPos.Add(new Vector3(-5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.5f, 2));
        //        spawnPos.Add(new Vector3(5, 0.5f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 3:
        //        spawnPos.Add(new Vector3(-6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(0, 0.5f, 3));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 4:
        //        spawnPos.Add(new Vector3(-5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.5f, 2));
        //        spawnPos.Add(new Vector3(5, 0.5f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 5:
        //        spawnPos.Add(new Vector3(-6, 0.75f, -3));
        //        spawnPos.Add(new Vector3(6, 0.75f, -3));
        //        spawnPos.Add(new Vector3(0, 0.75f, 3));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 6:
        //        spawnPos.Add(new Vector3(-5, 0.75f, -2));
        //        spawnPos.Add(new Vector3(5, 0.75f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.75f, 2));
        //        spawnPos.Add(new Vector3(5, 0.75f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 7:
        //        spawnPos.Add(new Vector3(-6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(0, 0.5f, 3));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 8:
        //        spawnPos.Add(new Vector3(-5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.5f, 2));
        //        spawnPos.Add(new Vector3(5, 0.5f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 9:
        //        spawnPos.Add(new Vector3(-6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(6, 0.5f, -3));
        //        spawnPos.Add(new Vector3(0, 0.5f, 3));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 10:
        //        spawnPos.Add(new Vector3(-5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.5f, 2));
        //        spawnPos.Add(new Vector3(5, 0.5f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            Destroy(covers[i]);
        //            spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPos[i], gameObject.transform.rotation));
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 11:
        //        spawnPos.Add(new Vector3(-4, 0.5f, 0));
        //        spawnPos.Add(new Vector3(4, 0.5f, 0));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //            Destroy(covers[i]);

        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[0], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPos[1], gameObject.transform.rotation));

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 12:
        //        spawnPos.Add(new Vector3(-4, 0.5f, 2));
        //        spawnPos.Add(new Vector3(4, 0.75f, 2));
        //        spawnPos.Add(new Vector3(0, 0.5f, -2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //            Destroy(covers[i]);

        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[0], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPos[1], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPos[2], gameObject.transform.rotation));

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 13:
        //        spawnPos.Add(new Vector3(-5, 0.75f, -2));
        //        spawnPos.Add(new Vector3(5, 0.5f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.5f, 2));
        //        spawnPos.Add(new Vector3(5, 0.5f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //            Destroy(covers[i]);

        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPos[0], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPos[1], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPos[2], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPos[3], gameObject.transform.rotation));

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 14:
        //        spawnPos.Add(new Vector3(-3, 0.5f, -2));
        //        spawnPos.Add(new Vector3(3, 0.5f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.5f, 0));
        //        spawnPos.Add(new Vector3(5, 0.5f, 0));
        //        spawnPos.Add(new Vector3(0, 0.5f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //            Destroy(covers[i]);

        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPos[0], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPos[1], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[2], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[3], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPos[4], gameObject.transform.rotation));

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    case 15:
        //        spawnPos.Add(new Vector3(0, 0.5f, 0));
        //        spawnPos.Add(new Vector3(-3, 0.5f, -2));
        //        spawnPos.Add(new Vector3(3, 0.5f, -2));
        //        spawnPos.Add(new Vector3(-5, 0.5f, 0));
        //        spawnPos.Add(new Vector3(5, 0.75f, 0));
        //        spawnPos.Add(new Vector3(0, 0.5f, 2));
        //        for (int i = 0; i < spawnPos.Count; i++)
        //        {
        //            covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
        //            covers[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        yield return new WaitForSeconds(spawnDelay);

        //        for (int i = 0; i < covers.Count; i++)
        //            Destroy(covers[i]);

        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[4], spawnPos[0], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[2], spawnPos[1], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[1], spawnPos[2], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[3], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[0], spawnPos[4], gameObject.transform.rotation));
        //        spawnList.Add((Enemy)Instantiate(enemyPrefabs[3], spawnPos[5], gameObject.transform.rotation));

        //        for (int i = 0; i < covers.Count; i++)
        //        {
        //            spawnList[i].SetSpawnManager(this);
        //            spawnList[i].SetRoomManager(roomManager);
        //            spawnList[i].transform.parent = roomManager.GetCurrent().transform;
        //        }
        //        break;
        //    default:
        //        break;
        //}

        //When SpawnWave is called, set current waves and totalWaves to certain values
        //Based on level and who's calling it.
        yield return null;
    }
}
