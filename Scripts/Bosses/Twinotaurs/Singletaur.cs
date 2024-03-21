using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletaur : MonoBehaviour
{
    [SerializeField] protected Twinotaurs twins;
    Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Room room = twins.GetRoomManager().GetCurrent();
        if (transform.position.x > (room.GetLength() / 2))
        {
            transform.position = new Vector3(lastPos.x, transform.position.y, transform.position.z);
            //Debug.Log("Push left");
        }
        else if (transform.position.x < -(room.GetLength() / 2))
        {
            transform.position = new Vector3(lastPos.x, transform.position.y, transform.position.z);
            //Debug.Log("Push right");
        }

        if (transform.position.z > room.GetWidth() / 2)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, lastPos.z);
            //Debug.Log("Push down");
        }
        else if (transform.position.z < -(room.GetWidth() / 2))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, lastPos.z);
            //Debug.Log("Push up");
        }

        lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (twins.GetState() == 2)
        {
            if (twins.GetCurrentAttack() == 0 && (collider.gameObject.CompareTag("Wall") || collider.gameObject.CompareTag("Door"))) //Poison Cutoff
            {
                collider.isTrigger = false;
                twins.SetState(3);
                GetComponent<Rigidbody>().velocity *= 0;
                transform.position -= transform.forward * 0.5f;
            }
            else if (twins.GetCurrentAttack() == 4 && collider.gameObject.name == "Venom") //Syncrash
            {
                GetComponent<Collider>().isTrigger = false;
                collider.isTrigger = false;
                twins.SetState(3);
                GetComponent<Rigidbody>().velocity *= 0;
                collider.gameObject.GetComponent<Rigidbody>().velocity *= 0;
            }
        }
    }
}
