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
    protected int[] firstWaves = { 0, 1, 3, 5, 7, 9, 10, -1, 11, 15, -1, 16, 20, -1}; //-1 is an item room
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
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            for (int i = covers.Count - 1; i >= 0; i--)
            {
                Destroy(covers[i]);
                covers.Remove(covers[i]);
            }
            StartCoroutine("SpawnWave");
        }

        if (totalWaves == 0)
            allDefeated = true;
        //if (superWave >= 20 && allDefeated)
        //    for (int i = 0; i < messages.Count; i++)
        //    {
        //        messages[0].gameObject.SetActive(true);
        //        messages[1].gameObject.SetActive(true);
        //        messages[2].gameObject.SetActive(false);
        //        messages[3].gameObject.SetActive(false);
        //    }
    }

    public override IEnumerator SpawnWave()
    {
        spawned = true;
        currentWave++;
        superWave++;
        spawnPosList = new List<Vector3>();
        covers = new List<GameObject>();

        yield return new WaitForSeconds(waveDelay);
        switch (superWave)
        {
            //PART I
            case 1: //Tangerine Troll 1
                spawnPosList.Add(new Vector3(-4, 0.5f, 3));
                spawnPosList.Add(new Vector3(4, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[6], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 2: //Tangerine Troll 2
                spawnPosList.Add(new Vector3(-6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(0, 0.5f, 3f));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[6], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 3: //Pink Python 1
                spawnPosList.Add(new Vector3(-6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(0, 0.5f, 3f));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[7], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 4: //Pink Python 2
                spawnPosList.Add(new Vector3(-4.5f, 0.5f, 1));
                spawnPosList.Add(new Vector3(4.5f, 0.5f, 1));
                spawnPosList.Add(new Vector3(-7.5f, 0.5f, 3));
                spawnPosList.Add(new Vector3(7.5f, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[7], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 5: //Cerulean Satyr 1
                spawnPosList.Add(new Vector3(-6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(0, 0.5f, 3f));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[8], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 6: //Cerulean Satyr 2
                spawnPosList.Add(new Vector3(-4.5f, 0.5f, 1));
                spawnPosList.Add(new Vector3(4.5f, 0.5f, 1));
                spawnPosList.Add(new Vector3(-7.5f, 0.5f, 3));
                spawnPosList.Add(new Vector3(7.5f, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[8], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 7: //Turquoise Templar 1
                spawnPosList.Add(new Vector3(-6, 0.5f, 3));
                spawnPosList.Add(new Vector3(6, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[9], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 8: //Turquoise Templar 2
                spawnPosList.Add(new Vector3(-6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(0, 0.5f, 3f));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    Destroy(covers[i]);
                    spawnList.Add(Instantiate(enemyPrefabs[9], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                }
                break;
            case 9: //Wisteria Wizard + Green Snake + Violet Knight
                spawnPosList.Add(new Vector3(-4.5f, 0.5f, 1));
                spawnPosList.Add(new Vector3(4.5f, 0.5f, 1));
                spawnPosList.Add(new Vector3(0, 0.5f, 2));
                spawnPosList.Add(new Vector3(-7.5f, 0.5f, 3));
                spawnPosList.Add(new Vector3(7.5f, 0.5f, 3));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                spawnList.Add(Instantiate(enemyPrefabs[0], spawnPosList[0], gameObject.transform.rotation));
                spawnList[0].SetSpawnManager(this);
                spawnList[0].SetRoomManager(roomManager);
                spawnList[0].transform.parent = roomManager.GetCurrent().transform;

                spawnList.Add(Instantiate(enemyPrefabs[0], spawnPosList[1], gameObject.transform.rotation));
                spawnList[1].SetSpawnManager(this);
                spawnList[1].SetRoomManager(roomManager);
                spawnList[1].transform.parent = roomManager.GetCurrent().transform;

                spawnList.Add(Instantiate(enemyPrefabs[4], spawnPosList[2], gameObject.transform.rotation));
                spawnList[2].SetSpawnManager(this);
                spawnList[2].SetRoomManager(roomManager);
                spawnList[2].transform.parent = roomManager.GetCurrent().transform;

                for (int i = 3; i < 5; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[5], spawnPosList[i], gameObject.transform.rotation));
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
                boss.gameObject.transform.localPosition = new Vector3(0, 1.25f, 0);
                boss.gameObject.transform.localScale = Vector3.zero;
                boss.transform.parent = roomManager.GetCurrent().transform;
                float myScale = 0;

                yield return new WaitForSeconds(6);

                while (myScale < 1)
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
                spawnPosList.Add(new Vector3(-6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(0, 0.5f, 3f));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < 2; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[7], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(0, 3));
                    spawnList[i].ChangeDefense(Random.Range(0, 3));
                }

                spawnList.Add(Instantiate(enemyPrefabs[8], spawnPosList[2], gameObject.transform.rotation));
                spawnList[2].SetSpawnManager(this);
                spawnList[2].SetRoomManager(roomManager);
                spawnList[2].transform.parent = roomManager.GetCurrent().transform;
                spawnList[2].ChangeStrength(Random.Range(0, 3));
                spawnList[2].ChangeDefense(Random.Range(0, 3));

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;
            case 12:
                spawnPosList.Add(new Vector3(-6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(6, 0.5f, 1f));
                spawnPosList.Add(new Vector3(0, 0.5f, 3f));
                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < 2; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[6], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(0, 3));
                    spawnList[i].ChangeDefense(Random.Range(0, 3));
                }

                spawnList.Add(Instantiate(enemyPrefabs[9], spawnPosList[2], gameObject.transform.rotation));
                spawnList[2].SetSpawnManager(this);
                spawnList[2].SetRoomManager(roomManager);
                spawnList[2].transform.parent = roomManager.GetCurrent().transform;
                spawnList[2].ChangeStrength(Random.Range(0, 3));
                spawnList[2].ChangeDefense(Random.Range(0, 3));

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;
            case 13:
                spawnPosList.Add(new Vector3(0, 0.5f, 4));
                spawnPosList.Add(new Vector3(-2, 0.5f, 0));
                spawnPosList.Add(new Vector3(-4, 0.5f, 2));
                spawnPosList.Add(new Vector3(2, 0.5f, 0));
                spawnPosList.Add(new Vector3(4, 0.5f, 2));

                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < 2; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[8], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(0, 3));
                    spawnList[i].ChangeDefense(Random.Range(0, 3));
                }

                spawnList.Add(Instantiate(enemyPrefabs[6], spawnPosList[2], gameObject.transform.rotation));
                spawnList[2].SetSpawnManager(this);
                spawnList[2].SetRoomManager(roomManager);
                spawnList[2].transform.parent = roomManager.GetCurrent().transform;
                spawnList[2].ChangeStrength(Random.Range(0, 3));
                spawnList[2].ChangeDefense(Random.Range(0, 3));

                for (int i = 3; i < 5; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[5], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(0, 3));
                    spawnList[i].ChangeDefense(Random.Range(0, 3));
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;
            case 14:
                spawnPosList.Add(new Vector3(0, 0.5f, 4));
                spawnPosList.Add(new Vector3(-2, 0.5f, 0));
                spawnPosList.Add(new Vector3(-4, 0.5f, 2));
                spawnPosList.Add(new Vector3(2, 0.5f, 0));
                spawnPosList.Add(new Vector3(4, 0.5f, 2));
                spawnPosList.Add(new Vector3(0, 0.5f, -1));

                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < 5; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[5+i], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(0, 3));
                    spawnList[i].ChangeDefense(Random.Range(0, 3));
                }

                spawnList.Add(Instantiate(enemyPrefabs[Random.Range(5, 10)], spawnPosList[5], gameObject.transform.rotation));
                spawnList[5].SetSpawnManager(this);
                spawnList[5].SetRoomManager(roomManager);
                spawnList[5].transform.parent = roomManager.GetCurrent().transform;
                spawnList[5].ChangeStrength(Random.Range(0, 3));
                spawnList[5].ChangeDefense(Random.Range(0, 3));

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 15: // Ogrelord
                boss = Instantiate(bossPrefabs[1], Vector3.zero, Quaternion.Euler(Vector3.zero));
                boss.GetComponent<Boss>().enabled = false;
                boss.gameObject.transform.localScale = Vector3.zero;
                boss.transform.parent = roomManager.GetCurrent().transform;
                myScale = 0;

                yield return new WaitForSeconds(2);

                while (myScale < 1)
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
                spawnPosList.Add(new Vector3(-4.5f, 0.5f, 2));
                spawnPosList.Add(new Vector3(4.5f, 0.5f, 2));
                spawnPosList.Add(new Vector3(-7.5f, 0.5f, 4));
                spawnPosList.Add(new Vector3(7.5f, 0.5f, 4));

                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[SelectEnemy()], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(1, 4));
                    spawnList[i].ChangeDefense(Random.Range(1, 4));
                    spawnList[i].SetHasItem(true);
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 17:
            case 18:
                spawnPosList.Add(new Vector3(0, 0.5f, 3));
                spawnPosList.Add(new Vector3(-2, 0.5f, 0));
                spawnPosList.Add(new Vector3(-4, 0.5f, 1));
                spawnPosList.Add(new Vector3(2, 0.5f, 0));
                spawnPosList.Add(new Vector3(4, 0.5f, 1));

                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[SelectEnemy()], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(1, 4));
                    spawnList[i].ChangeDefense(Random.Range(1, 4));
                    spawnList[i].SetHasItem(true);
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 19:
                spawnPosList.Add(new Vector3(0, 0.5f, 3));
                spawnPosList.Add(new Vector3(-2, 0.5f, 0));
                spawnPosList.Add(new Vector3(-4, 0.5f, 1));
                spawnPosList.Add(new Vector3(2, 0.5f, 0));
                spawnPosList.Add(new Vector3(4, 0.5f, 1));

                for (int i = 0; i < spawnPosList.Count; i++)
                {
                    covers.Add(Instantiate(spawnCover, spawnPosList[i], gameObject.transform.rotation));
                    covers[i].transform.parent = roomManager.GetCurrent().transform;
                }
                yield return new WaitForSeconds(spawnDelay);

                for (int i = 0; i < covers.Count; i++)
                {
                    spawnList.Add(Instantiate(enemyPrefabs[SelectEnemy()], spawnPosList[i], gameObject.transform.rotation));
                    spawnList[i].SetSpawnManager(this);
                    spawnList[i].SetRoomManager(roomManager);
                    spawnList[i].transform.parent = roomManager.GetCurrent().transform;
                    spawnList[i].ChangeStrength(Random.Range(1, 4));
                    spawnList[i].ChangeDefense(Random.Range(1, 4));
                    spawnList[i].SetHasItem(true);
                }

                for (int i = 0; i < covers.Count; i++)
                    Destroy(covers[i]);
                break;

            case 20: // Twinotaurs
                boss = Instantiate(bossPrefabs[2], Vector3.zero, Quaternion.Euler(Vector3.zero));
                boss.transform.parent = roomManager.GetCurrent().transform;
                boss.GetComponent<Boss>().enabled = false;
                GameObject spark = ((Twinotaurs)boss).GetSpark();
                GameObject venom = ((Twinotaurs)boss).GetVenom();

                spark.transform.localScale = Vector3.zero;
                spark.GetComponent<Singletaur>().enabled = false;
                venom.transform.localScale = Vector3.zero;
                venom.GetComponent<Singletaur>().enabled = true;
                myScale = 0;

                yield return new WaitForSeconds(2);

                while (myScale < 1)
                {
                    myScale += Time.deltaTime;
                    ((Twinotaurs)boss).GetSpark().transform.localScale = Vector3.one * myScale;
                    ((Twinotaurs)boss).GetVenom().transform.localScale = Vector3.one * myScale;
                    yield return null;
                }
                spark.transform.localScale = Vector3.one;
                venom.transform.localScale = Vector3.one;
                boss.GetComponent<Boss>().enabled = true;
                spark.GetComponent<Singletaur>().enabled = true;
                venom.GetComponent<Singletaur>().enabled = true;
                break;

            default:
                break;
        }

        //When SpawnWave is called, set current waves and totalWaves to certain values
        //Based on level and who's calling it.
        yield return null;
    }

    public int GetFirstWave(int room)
    {
        if (room < 0 || room > firstWaves.Length)
            return -1;

        return firstWaves[room];
    }

    public void SetSuperWave(int s)
    {
        superWave = s;
    }
}
