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
            //PART I
            case 1: //Tangerine Troll 1
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
                    spawnList.Add(Instantiate(enemyPrefabs[6], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 2: //Tangerine Troll 2
                spawnPos.Add(new Vector3(-6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(0, 0.5f, 4.5f));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[6], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 3: //Pink Python 1
                spawnPos.Add(new Vector3(-6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(0, 0.5f, 4.5f));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[7], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 4: //Pink Python 2
                spawnPos.Add(new Vector3(-4.5f, 0.5f, 2));
                spawnPos.Add(new Vector3(4.5f, 0.5f, 2));
                spawnPos.Add(new Vector3(-7.5f, 0.5f, 4));
                spawnPos.Add(new Vector3(7.5f, 0.5f, 4));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[7], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 5: //Cerulean Satyr 1
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
                    spawnList.Add(Instantiate(enemyPrefabs[8], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 6: //Cerulean Satyr 2
                spawnPos.Add(new Vector3(-6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(0, 0.5f, 4.5f));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[8], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 7: //Turquoise Templar 1
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
                    spawnList.Add(Instantiate(enemyPrefabs[9], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 8: //Turquoise Templar 2
                spawnPos.Add(new Vector3(-6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(0, 0.5f, 4.5f));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[9], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 9: //Wisteria Wizard + Green Snake + Violet Knight
                spawnPos.Add(new Vector3(-4.5f, 0.5f, 2));
                spawnPos.Add(new Vector3(4.5f, 0.5f, 2));
                spawnPos.Add(new Vector3(-7.5f, 0.5f, 4));
                spawnPos.Add(new Vector3(7.5f, 0.5f, 4));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                spawnList.Add(Instantiate(enemyPrefabs[0], spawnPos[0], gameObject.transform.rotation));
                spawnList[0].SetSpawnManager(this);
                spawnList[0].SetRoomManager(roomManager);
                spawnList[0].transform.parent = roomManager.GetCurrent().transform;

                spawnList.Add(Instantiate(enemyPrefabs[4], spawnPos[1], gameObject.transform.rotation));
                spawnList[0].SetSpawnManager(this);
                spawnList[0].SetRoomManager(roomManager);
                spawnList[0].transform.parent = roomManager.GetCurrent().transform;

                for (int i = 2; i < 4; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[5], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);

                break;
            case 10: // Magestic
                Boss boss = Instantiate(bossPrefabs[0], Vector3.zero, Quaternion.Euler(Vector3.zero));
                boss.GetComponent<Boss>().enabled = false;
                boss.gameObject.transform.localScale = Vector3.zero;
                float myScale = 0;

                yield return new WaitForSeconds(10);

                while (myScale <= 1)
                {
                    myScale += Time.deltaTime;
                    boss.gameObject.transform.localScale = Vector3.one * myScale;
                    yield return null;
                }
                boss.gameObject.transform.localScale = Vector3.one;
                boss.GetComponent<Boss>().enabled = true;
                break;

            //PART II
            case 11:
                spawnPos.Add(new Vector3(-6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(6, 0.5f, 1.5f));
                spawnPos.Add(new Vector3(0, 0.5f, 4.5f));
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                spawnList.Add(Instantiate(enemyPrefabs[0], spawnPos[0], gameObject.transform.rotation));
                spawnList[0].SetSpawnManager(this);
                spawnList[0].SetRoomManager(roomManager);
                spawnList[0].transform.parent = roomManager.GetCurrent().transform;

                for (int i = 0; i < 2; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[7], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                spawnList.Add(Instantiate(enemyPrefabs[8], spawnPos[2], gameObject.transform.rotation));
                spawnList[2].SetSpawnManager(this);
                spawnList[2].SetRoomManager(roomManager);
                spawnList[2].transform.parent = roomManager.GetCurrent().transform;

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;
            case 13:
                spawnPos.Add(new Vector3(0, 0.5f, 4));
                spawnPos.Add(new Vector3(-2, 0.5f, 0));
                spawnPos.Add(new Vector3(-4, 0.5f, 2));
                spawnPos.Add(new Vector3(2, 0.5f, 0));
                spawnPos.Add(new Vector3(4, 0.5f, 2));

                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < 2; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[8], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                spawnList.Add(Instantiate(enemyPrefabs[6], spawnPos[2], gameObject.transform.rotation));
                spawnList[2].SetSpawnManager(this);
                spawnList[2].SetRoomManager(roomManager);
                spawnList[2].transform.parent = roomManager.GetCurrent().transform;

                for (int i = 3; i < 5; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[5], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;
            case 14:
                spawnPos.Add(new Vector3(0, 0.5f, 4));
                spawnPos.Add(new Vector3(-2, 0.5f, 0));
                spawnPos.Add(new Vector3(-4, 0.5f, 2));
                spawnPos.Add(new Vector3(2, 0.5f, 0));
                spawnPos.Add(new Vector3(4, 0.5f, 2));
                spawnPos.Add(new Vector3(0, 0.5f, 3));

                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < 5; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[5+i], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                spawnList.Add(Instantiate(enemyPrefabs[Random.Range(5, 10)], spawnPos[5], gameObject.transform.rotation));
                spawnList[5].SetSpawnManager(this);
                spawnList[5].SetRoomManager(roomManager);
                spawnList[5].transform.parent = roomManager.GetCurrent().transform;

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 15: // Ogrelord
                boss = Instantiate(bossPrefabs[1], Vector3.zero, Quaternion.Euler(Vector3.zero));
                boss.GetComponent<Boss>().enabled = false;
                boss.gameObject.transform.localScale = Vector3.zero;
                myScale = 0;

                yield return new WaitForSeconds(10);

                while (myScale <= 1)
                {
                    myScale += Time.deltaTime;
                    boss.gameObject.transform.localScale = Vector3.one * myScale;
                    yield return null;
                }
                boss.gameObject.transform.localScale = Vector3.one;
                boss.GetComponent<Boss>().enabled = true;
                break;

            //PART III
            case 16:
                spawnPos.Add(new Vector3(-4.5f, 0.5f, 2));
                spawnPos.Add(new Vector3(4.5f, 0.5f, 2));
                spawnPos.Add(new Vector3(-7.5f, 0.5f, 4));
                spawnPos.Add(new Vector3(7.5f, 0.5f, 4));

                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[SelectEnemy()], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 17:
            case 18:
                spawnPos.Add(new Vector3(0, 0.5f, 4));
                spawnPos.Add(new Vector3(-2, 0.5f, 0));
                spawnPos.Add(new Vector3(-4, 0.5f, 2));
                spawnPos.Add(new Vector3(2, 0.5f, 0));
                spawnPos.Add(new Vector3(4, 0.5f, 2));

                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[SelectEnemy()], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 19:
                spawnPos.Add(new Vector3(0, 0.5f, 4));
                spawnPos.Add(new Vector3(-2, 0.5f, 0));
                spawnPos.Add(new Vector3(-4, 0.5f, 2));
                spawnPos.Add(new Vector3(2, 0.5f, 0));
                spawnPos.Add(new Vector3(4, 0.5f, 2));

                for (int i = 0; i < spawnPos.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPos[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[SelectEnemy()], spawnPos[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 20: // Twinotaurs
                boss = Instantiate(bossPrefabs[1], Vector3.zero, Quaternion.Euler(Vector3.zero));
                boss.GetComponent<Boss>().enabled = false;
                boss.gameObject.transform.localScale = Vector3.zero;
                myScale = 0;

                yield return new WaitForSeconds(10);

                while (myScale <= 1)
                {
                    myScale += Time.deltaTime;
                    boss.gameObject.transform.localScale = Vector3.one * myScale;
                    yield return null;
                }
                boss.gameObject.transform.localScale = Vector3.one;
                boss.GetComponent<Boss>().enabled = true;
                break;

            default:
                break;
        }

        //When SpawnWave is called, set current waves and totalWaves to certain values
        //Based on level and who's calling it.
        yield return null;
    }
}
