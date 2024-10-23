using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderCage : ScriptableObject
{
    protected Hitbox boltObject;
    //protected List<GameObject> thunderClouds;
    protected List<GameObject> totalList;
    protected int currentNode = 0;
    protected float boltDuration = 4f / 60;
    protected float boltTimer = 0;
    protected bool bolting = true;

    // This script works by having the bolt connect two nodes at a time,
    // changing nodes every interval of boltDuration (with half that time as a bit of windup)

    // Start is called before the first frame update
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        if (bolting)
        {
            if (boltTimer >= boltDuration)
            {
                bolting = false;
                boltObject.gameObject.SetActive(false);
                boltTimer = 0;
            }
        }
        else
        {
            if (boltTimer >= boltDuration / 2)
            {
                bolting = true;
                boltObject.gameObject.SetActive(true);
                if (currentNode < totalList.Count - 1)
                    currentNode++;
                else
                    currentNode = 1;
                AimBolt();
                boltTimer = 0;
            }
        }

        boltTimer += Time.deltaTime;

    }

    public void EnableAll()
    {
        foreach (GameObject obj in totalList)
            obj.SetActive(true);
        ResetBolt();
    }

    public void ResetBolt()
    {
        currentNode = 0;
        bolting = true;
        boltObject.gameObject.SetActive(true);
        boltTimer = 0;
        AimBolt();
    }

    public void AimBolt()
    {
        Transform currentTransform = totalList[currentNode].gameObject.transform;
        Transform nextTransform;
        if (currentNode < totalList.Count - 1 && totalList[currentNode + 1].gameObject != null)
            nextTransform = totalList[currentNode + 1].gameObject.transform;
        else
            nextTransform = totalList[1].gameObject.transform;

        Vector3 avgLocation = Vector3.Lerp(currentTransform.position, nextTransform.position, 0.5f);
        Vector3 aim = nextTransform.position - currentTransform.position;
        float dist = aim.magnitude;

        boltObject.gameObject.transform.position = avgLocation;
        boltObject.gameObject.transform.rotation = Quaternion.LookRotation(aim);
        boltObject.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, dist / 2);
    }

    // Stop the bolting and return where the Twinotaur should be.
    private Transform EndBolt()
    {
        boltObject.gameObject.SetActive(false);
        if (currentNode < totalList.Count - 1)
            return totalList[currentNode + 1].transform;
        return totalList[1].transform;

        //if (bolting)
        //    return boltObject.gameObject.transform;
        //return totalList[currentNode].transform;
    }

    public Transform DisableAll()
    {
        foreach (GameObject obj in totalList)
            obj.SetActive(false);

        return EndBolt();
    }

    public void SetThunderLinks(GameObject start, List<GameObject> gases)
    {
        if (start == null)
        {
            Debug.LogError("Start must not be null!");
            return;
        }

        if (gases == null || gases.Count < 2)
        {
            Debug.LogError("Thunder Cage have at least two projectiles!");
            return;
        }

        totalList = new List<GameObject>();
        totalList.Add(start);
        foreach (GameObject gas in gases)
            totalList.Add(gas);
    }

    public void SetBoltObject(Hitbox bolt)
    {
        boltObject = bolt;
        boltObject.gameObject.SetActive(false);
    }

    public bool IsBolting()
    {
        return bolting;
    }

    public void DestroyBolt()
    {
        EndBolt();
        Destroy(boltObject);
    }

    public void DestroyAll()
    {
        for (int i = 0; i < totalList.Count; i++)
            Destroy(totalList[i]);
    }
}
