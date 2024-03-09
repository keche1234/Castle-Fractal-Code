using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletaur : MonoBehaviour
{
    [SerializeField] protected Twinotaurs twins;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
                transform.position -= transform.forward;
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
