using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoPlayerRevive : MonoBehaviour
{
    [SerializeField] protected PlayerController player;
    [SerializeField] protected ProtoRoomManager protoRoomManager;
    [SerializeField] protected float delay;
    [SerializeField] protected bool reviving;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetCurrentHealth() <= 0 && !reviving)
            StartCoroutine("Revive");
    }

    protected IEnumerator Revive()
    {
        //reviving = true;
        //yield return new WaitForSeconds(delay);
        //player.TakeDamage(((int) player.GetCurrentHealth()) - 30);
        //player.GetComponent<PlayerController>().enabled = false;
        //player.transform.position = protoRoomManager.GetCurrent().GetEntrance().transform.position + (protoRoomManager.GetCurrent().GetEntrance().transform.right) - new Vector3(0, 0.25f, 0);
        //player.transform.rotation = protoRoomManager.GetCurrent().GetEntrance().transform.rotation;
        //player.transform.Rotate(0, 90, 0);
        //player.gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

        //while (player.gameObject.transform.localScale.x <= 1 && player.gameObject.transform.localScale.y <= 1 && player.gameObject.transform.localScale.z <= 1 )
        //{
        //    player.gameObject.transform.localScale += new Vector3(2 * Time.deltaTime / delay, 2 * Time.deltaTime / delay, 2 * Time.deltaTime / delay);
        //    yield return null;
        //}

        //player.gameObject.transform.localScale = new Vector3(1, 1, 1);
        //player.GetComponent<PlayerController>().enabled = true;
        //player.SetLifeState(true);
        //StartCoroutine(player.GrantInvincibility(2.5f));
        //player.SetCustomWeapon(0);
        //reviving = false;
        yield return null;
    }
}
