using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderCage : MonoBehaviour
{
    protected Hitbox boltObject;
    protected LinkedListNode<GameObject> startNode;
    protected LinkedList<GameObject> thunderClouds;
    protected LinkedList<GameObject> totalList;
    protected LinkedListNode<GameObject> currentNode;
    protected float boltDuration = 8f / 60;
    protected float boltTimer = 0;
    protected bool bolting = true;

    // This script works by having the bolt connect two nodes at a time,
    // changing nodes every interval of boltDuration (with half that time as a bit of windup)

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TotalListCyclic())
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
                    currentNode = currentNode.Next;
                    AimBolt();
                    boltTimer = 0;
                }
            }

            boltTimer += Time.deltaTime;
        }
    }

    public void AimBolt()
    {
        if (currentNode == null)
        {
            Debug.LogError("Total list is acyclic!");
            return;
        }
        Transform currentTransform = currentNode.Value.gameObject.transform;

        if (currentNode.Next == null)
        {
            Debug.LogError("Total list is acyclic!");
            return;
        }
        Transform nextTransform = currentNode.Next.Value.gameObject.transform;

        Vector3 avgLocation = Vector3.Lerp(currentTransform.position, nextTransform.position, 0.5f);
        Vector3 aim = currentNode.Next.Value.gameObject.transform.position
            - currentNode.Value.gameObject.transform.position;
        float dist = aim.magnitude;

        boltObject.gameObject.transform.position = avgLocation;
        boltObject.gameObject.transform.rotation = Quaternion.LookRotation(aim);
        boltObject.gameObject.transform.localScale = new Vector3(1, 1, dist / 2);
    }

    private bool TotalListCyclic()
    {
        if (totalList == null || totalList.Count < 3)
        {
            Debug.Log("Total list needs a start and at least two other nodes!");
            return false;
        }

        LinkedListNode<GameObject> tortoise = totalList.First;
        LinkedListNode<GameObject> hare = totalList.First;

        while (hare != null)
        {
            if (hare == tortoise)
                return true;

            tortoise = tortoise.Next;
            if (tortoise == null)
            {
                Debug.LogError("No cycle!");
                return false;
            }

            hare = hare.Next;
            if (hare == null)
            {
                Debug.LogError("No cycle!");
                return false;
            }

            hare = hare.Next;
        }

        return false;
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

        startNode = new LinkedListNode<GameObject>(start);
        thunderClouds = new LinkedList<GameObject>(gases);
        thunderClouds.AddLast(thunderClouds.First);

        totalList = new LinkedList<GameObject>(thunderClouds);
        totalList.AddFirst(startNode);
        currentNode = thunderClouds.First;
    }

    public void SetBoltObject(Hitbox bolt)
    {
        boltObject = bolt;
        boltObject.gameObject.SetActive(true);
    }
}
