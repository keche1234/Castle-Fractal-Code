using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] protected RoomManager roomManager;
    [SerializeField] protected Material lockedMat;
    [SerializeField] protected Material unlockedMat;
    [SerializeField] protected bool locked;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (roomManager && roomManager.GetCurrent())
        {
            if (roomManager.GetCurrent().RoomCleared())
            {
                gameObject.GetComponent<MeshRenderer>().material = unlockedMat;
                locked = false;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = lockedMat;
                locked = true;
            }
        }
    }

    public void SetRoomManager(RoomManager rm)
    {
        roomManager = rm;
    }

    public void AddLocked(Material m)
    {
        lockedMat = m;
    }

    public void AddUnlocked(Material m)
    {
        unlockedMat = m;
    }

    ///*
    // * b = true => locked
    // * b = false => unlocked
    // */
    //public void SetLock(bool b)
    //{
    //    locked = b;
    //    if (locked) gameObject.GetComponent<MeshRenderer>().material = lockedMat;
    //    else gameObject.GetComponent<MeshRenderer>().material = unlockedMat;
    //}

    public void OnCollisionEnter(Collision targetCollider)
    {
        if (targetCollider.gameObject.CompareTag("Player") && !locked)
        {
            roomManager.Step();
        }
    }
}
